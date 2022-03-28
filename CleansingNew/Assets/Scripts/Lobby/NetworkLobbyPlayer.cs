using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheCleansing.Lobby                   //a room player stores the user's name and if they are ready or not
{
    public class NetworkLobbyPlayer : NetworkBehaviour              //script sits on the player when they join and destroyed when they leave, each player has this script
    {
        [Header("UI")]
        [SerializeField] private GameObject lobbyUI = null;             //turns the lobby on or off for the player
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];          //used for show player name
        [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[2];         //used to show if players are ready to start
        [SerializeField] private TMP_Text[] playerClassTexts = new TMP_Text[2];         //text that stores the classes of players
        [SerializeField] private Button startGameButton = null;                 //shows this button only to the leader, allows them to start game when everyone is ready
        [SerializeField] private Button readyButton = null;

        [SyncVar(hook = nameof(HandleDisplayNameChanged))]              //variables that can only be changed on the server and is updated everywhere once updated
        public string DisplayName = "Loading...";                   //server changes the names and the logic, it notifies all the other cilents
        [SyncVar(hook = nameof(HandleReadyStatusChanged))]          //hook is the name of the method that is called when function is executed, e.g. when ready is called, this method is called, updated the ui only when the variables are changed
        public bool IsReady = false;
        [SyncVar(hook = nameof(HandleClassChanged))]
        public string CharacterClass;                               //the character class the user has chosen
        [SyncVar]
        public int ConnectionId;
        [SyncVar]
        public int PlayerNumber;


        private bool isLeader;
        public bool IsLeader                //sets the leader boolean
        {
            get
            {
                return isLeader;
            }
            set
            {
                isLeader = value;
                startGameButton.gameObject.SetActive(value);            //activates the button if the leader is true
            }
        }

        private NetworkManagerTC room;
        private NetworkManagerTC Room        //a way to reference room easliy
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerTC;          //casts the networkManager as a networkManagerLobby
            }
        }

        public override void OnStartAuthority()         //called on the object that belongs to the user
        {
            CmdSetDisplayName(PlayerNameInput.DisplayName);         //a function called by a client that runs on a server, gets player name from the player's input and sets it on the server and is validated
            Debug.Log("Room Lobby player name: " + this.DisplayName);
            lobbyUI.SetActive(true);            //activates ui because its ourselves and not others
        }

        public override void OnStartClient()            //called on every network behaviour when active on a client
        {
            Room.RoomPlayers.Add(this);             //adds to list for room players, this instance added to list
            UpdateDisplay();            //ui updated, e.g. whenever something starts or something is destoryed
            Debug.Log("Adding lobby player name: " + this.DisplayName);
        }

        public override void OnStopClient()             //called whenever anyone disconnects
        {
            Room.RoomPlayers.Remove(this);          //removes whoever disconnected
            UpdateDisplay();
            Debug.Log("Removing lobby player name: " + this.DisplayName);
        }

        public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
        public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
        public void HandleClassChanged(string oldValue, string newValue) => UpdateDisplay();

        private void UpdateDisplay()
        {
            if (!hasAuthority)                  //looks for the player that has authority and updates their display
            {
                foreach (var player in Room.RoomPlayers)            //checks through objects and sees if the client has authority over it
                {
                    if (player.hasAuthority)
                    {
                        player.UpdateDisplay();
                        break;
                    }
                }

                return;
            }

            //better opitmised code would be to check which values need to be updated and only update the specified text, rather than clearing everying and setting values again

            for (int i = 0; i < playerNameTexts.Length; i++)                //if player object belongs to us, goes through the name text and sets string
            {
                playerNameTexts[i].text = "Waiting For Player...";          //clears all the text and sets to empty
                playerReadyTexts[i].text = string.Empty;
            }

            for (int i = 0; i < Room.RoomPlayers.Count; i++)
            {
                playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;          //sets player name to ui
                playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?            //sets player if they are ready or not
                    "<color=green>Ready</color>" :                              //changes color depending if they are ready
                    "<color=red>Not Ready</color>";
                playerClassTexts[i].text = "(" + Room.RoomPlayers[i].CharacterClass + ")";          //sets character class
            }

            if (IsReady)
            {
                readyButton.GetComponentInChildren<Text>().text = "Unready";
            }
            else
            {
                readyButton.GetComponentInChildren<Text>().text = "Ready";
            }
        }

        public void HandleReadyToStart(bool readyToStart)              //updates ready status
        {
            if (!isLeader) { return; }          //method only applicable to leader

            startGameButton.interactable = readyToStart;            //button only interacatble if all players are ready
        }

        public void QuitLobby()
        {
            //checks if leader or not, then does appropriate dissconnect
            if (IsLeader)
            {
                Room.StopHost();
            }
            else
            {
                Room.StopClient();
            }
        }

        [Command]
        private void CmdSetDisplayName(string displayName)              //when name recived by server, sets name of player
        {
            DisplayName = displayName;
        }

        [Command]
        public void CmdSetTank()                            //sets the class of the players, tank, healer, medic
        {
            CharacterClass = "Tank";
        }

        [Command]
        public void CmdSetSoldier()
        {
            CharacterClass = "Soldier";
        }

        [Command]
        public void CmdSetMedic()
        {
            CharacterClass = "Medic";
        }

        [Command]
        public void CmdReadyUp()                            //command run on server
        {
            Debug.Log("Ready");
            IsReady = !IsReady;                             //server toggles if players have ready'd up 

            Room.NotifyPlayersOfReadyState();               //notifies room status of player's ready up
        }
        
        [Command]
        public void CmdStartGame()
        {
            Debug.Log("Start");
            if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }               //checks if the first person in the room is the leader

            Room.StartGame();
        }
    }
}