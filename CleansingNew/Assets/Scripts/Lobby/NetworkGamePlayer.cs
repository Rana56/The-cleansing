using Mirror;
using System;
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
        [SyncVar (hook = nameof(HandleReadyChange))]
        public bool IsReady = false;                        //checks if the player is ready
        [SyncVar]
        public int score;

        public event Action UpdateReady;

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
            Debug.Log("Adding game player name: " + this.PlayerName);
        }

        public override void OnStopClient()             //called whenever anyone disconnects
        {
            Game.GamePlayers.Remove(this);          //removes whoever disconnected
            Debug.Log("Removing game player name: " + this.PlayerName);

            /*
            if(Game.GamePlayers.Count < 2)
            {
                Game.StopHost();
            }*/
        }

        private void HandleReadyChange(bool oldValue, bool newValue)
        {
            Debug.Log("Invoke ready up");
            UpdateReady?.Invoke();
        }

        [Command]
        public void CmdReadyUp()                            //command run on server
        {
            Debug.Log("Ready");
            IsReady = !IsReady;                             //server toggles if players have ready'd up 

            Game.NotifyGamePlayerReady();                   //tells server player is ready
        }

        [Server]                    //ensures the logic only run on the server
        public void SetDisplayName(string displayName)                      //sets dislpay name
        {
            this.PlayerName = displayName;
            Debug.Log("Local player name: " + this.PlayerName);
        }

        [Server]
        public void SetConnectionId(int connID)                         //set connection id
        {
            this.ConnectionId = connID;
            Debug.Log("Local connId: " + this.ConnectionId);
        }

        [Server]
        public void SetPlayerNumber(int playerNum)                      //set playerNum
        {
            this.PlayerNumber = playerNum;
            Debug.Log("Local player number: " + this.PlayerNumber);
        }

        [Server]
        public void IncrementScore()            //incements score - score is the amount the player has won
        {
            score++;
        }

    }
}