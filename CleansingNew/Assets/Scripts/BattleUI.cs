using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//script attached to player, wiil be spawned when player spawned

namespace TheCleansing.Lobby
{
    public class BattleUI : NetworkBehaviour
    {
        [Header("UIs")]
        [SerializeField] private GameObject battleUI = null;
        [SerializeField] private GameObject movesUI = null;

        //local player
        [Header("Local Player")]
        [SerializeField] private TMP_Text localName = null;          //used for show player name
        [SerializeField] private TMP_Text localHealth = null;          //used for show player health
        private GameObject playerScriptObject;

        //other player
        [Header("Other Player")]
        [SerializeField] private TMP_Text playerName = null;          //used for show player name
        [SerializeField] private TMP_Text playerHealth = null;          //used for show player health

        //player information
        [SerializeField] private TMP_Text info_text = null;
        public uint PlayerNetId { get; private set; }           //stores player net ID
        NetworkGamePlayer otherPlayer = null;                           //connected players are stored in variables
        NetworkGamePlayer localPlayer = null;

        private NetworkManagerTC game;
        private NetworkManagerTC Game        //a way to reference game easliy
        {
            get
            {
                if (game != null) { return game; }
                return game = NetworkManager.singleton as NetworkManagerTC;          //casts the networkManager as a networkManagerLobby
            }
        }

        public void activateUI()
        {
            battleUI.SetActive(true);         //turns on UI
            gameObject.name = "LocalBattleUI";
        }

        public void deactivateMovesUI()               //deactivates the moves panel
        {
            Debug.Log("Deactivate buttons");
            Button[] buttons = movesUI.GetComponentsInChildren<Button>();           //gets all the buttons in panel 
            foreach(Button button in buttons)                                       //loops through the buttons and makes it not interatable
            {
                button.interactable = false;                        //makes button not interactable
            }
        }

        public void activateMovesUI()               //deactivates the moves panel
        {
            Debug.Log("Activate buttons");
            Button[] buttons = movesUI.GetComponentsInChildren<Button>();           //gets all the buttons in panel 
            foreach (Button button in buttons)                                       //loops through the buttons and makes it not interatable
            {
                button.interactable = true;                        //makes button not interactable
            }
        }

        public void SetUpUI(Player player)          //sets the name of the players to UI
        {
            Debug.Log("Setup UI");
            playerScriptObject = player.gameObject;                                     //UI stores the game object that the player script is attached to
            PlayerNetId = player.netId;         //stores the net id of player
            Debug.Log("NetID: " + PlayerNetId);

            for (int i = 0; i < Game.GamePlayers.Count; i++)                        //loops over the list of game players (the connected players), then checks if they are a local player.
            {
                Health health = Game.GamePlayers[i].GetComponentInParent<Health>();
                health.OnHealthChanged += updateHealthDisplay;                                      //subscribes to event, when event invoked, method run
                Game.GamePlayers[i].UpdateReady += readyCheck;

                if (Game.GamePlayers[i].name.Equals("LocalGamePlayer"))
                {
                    localName.text = Game.GamePlayers[i].PlayerName;                //If it's true, the player name is assigned to appropriate text
                    localPlayer = Game.GamePlayers[i];
                }
                else
                {
                    playerName.text = Game.GamePlayers[i].PlayerName;
                    otherPlayer = Game.GamePlayers[i];
                }
            } 
            updateHealthDisplay();
        }

        public void updateHealthDisplay()                //updates the display of health of players
        {
            Debug.Log("Updating Health");            
            for (int i = 0; i < Game.GamePlayers.Count; i++)
            {
                Health health = Game.GamePlayers[i].GetComponentInParent<Health>();                     //gets health script attched to NetworkGamePlayer
                Debug.Log("Updating Health: " + health + ", " + Game.GamePlayers[i].name);

                if (Game.GamePlayers[i].name.Equals("LocalGamePlayer"))
                {
                    localHealth.text = health.toString();                //If it's true, the player name is assigned to appropriate text
                }
                else
                {
                    playerHealth.text = health.toString();
                }

            }
        }

        public void Attack()            //attacks opponent
        {
            Debug.Log("Attack health");                                                     //gets health script attched to NetworkGamePlayer
            Health health = localPlayer.GetComponentInParent<Health>();                     //changes player health, doesn't change own health
            health.AttackPlayer(10, otherPlayer);                                           //local payer calls command

            localPlayer.CmdReadyUp();                                                       //readies local player
            deactivateMovesUI();
        }

        public void Heal()              //heals player
        {
            Debug.Log("Heal");
            Health health = localPlayer.GetComponentInParent<Health>();                     //gets health script attched to NetworkGamePlayer
            health.HealPlayer(5);                                                          //changes player health, doesn't change own health

            localPlayer.CmdReadyUp();                                                       //readies local player
            deactivateMovesUI();
        }

        public void readyCheck()
        {
            if (localPlayer.IsReady && !otherPlayer.IsReady)                         //changes notificiation
            {
                info_text.text = "Wating for other Player...";
            } else if (localPlayer.IsReady && otherPlayer.IsReady)
            {
                info_text.text = "";
            }
            else
            {
                info_text.text = "Select your move";                                        //TODO: Fix this so when other player selects it updates localuser too
            }
        }

        public override void OnStopClient()                                                     //unsubscribes from event when client disconnects
        {
            for(int i = 0; i < Game.GamePlayers.Count; i++)
            {
                Health health = Game.GamePlayers[i].GetComponentInParent<Health>();
                health.OnHealthChanged -= updateHealthDisplay;
                Game.GamePlayers[i].UpdateReady -= readyCheck;
            }
        }

        #region Old Code
        /*
        [SerializeField] private TMP_Text[] playerHealths = new TMP_Text[2];

        //localName.text = GameObject.Find("LocalGamePlayer").GetComponent<NetworkGamePlayer>().PlayerName;       //sets name by finding the gameobject for NetworkGamePlayer, then getting the script of it and then retrives the name of player
        //playerName.text = GameObject.Find("LocalGamePlayer").GetComponent<NetworkGamePlayer>().PlayerName;

        localHealth.text = player.GetComponent<Health>().toString();
        */
        #endregion
    }
}
