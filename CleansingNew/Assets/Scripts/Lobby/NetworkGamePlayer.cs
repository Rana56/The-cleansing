using Mirror;
using TMPro;
using UnityEngine;

namespace TheCleansing.Lobby                   //a room player stores the user's name and if they are ready or not
{
    public class NetworkGamePlayer : NetworkBehaviour              //script sits on the player when they join and destroyed when they leave, each player has this script
    {
        [SyncVar]              //sync the vairable across all the clients so that all players knows the correct names in case someone leaves or joins
        public string PlayerName = "Loading...";               //server changes the names and the logic, it notifies all the other cilents
        [SyncVar]
        public int ConnectionId;
        [SyncVar]
        public int PlayerNumber;                    //used to track who is player 1 and 2

        private NetworkManagerTC game;
        private NetworkManagerTC Game        //a way to reference room easliy
        {
            get
            {
                if (game != null) { return game; }
                return game = NetworkManager.singleton as NetworkManagerTC;          //casts the networkManager as a networkManagerLobby
            }
        }

        public override void OnStartAuthority()
        {
            gameObject.name = "LocalGamePlayer";            //this names the local player
            Debug.Log("Local player name: " + this.PlayerName);                 //the game player that the client has authority over will be renamed to LocalGamePlayer
        }

        public override void OnStartClient()            //called on every network behaviour when active on a client
        {
            DontDestroyOnLoad(gameObject);          //this means that when the scene is changed the object won't be destoryed
            Game.GamePlayers.Add(this);             //adds to list for players, this instance added to list
        }

        public override void OnStopClient()             //called whenever anyone disconnects
        {
            Game.GamePlayers.Remove(this);          //removes whoever disconnected

        }
        
        [Server]                    //ensures the logic only run on the server
        public void SetDisplayName(string displayName)                      //sets dislpay name
        {
            this.PlayerName = displayName;
            Debug.Log("Local player name: " + this.PlayerName);
        }
        [Server]
        public void SetConnectionId(int connID)
        {
            this.ConnectionId = connID;
        }
        [Server]
        public void SetPlayerNumber(int playerNum)
        {
            this.PlayerNumber = playerNum;
        }
    }
}