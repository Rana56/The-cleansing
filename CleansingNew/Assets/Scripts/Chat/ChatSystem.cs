using Mirror;
using System;
using TMPro;
using UnityEngine;

namespace TheCleansing.Lobby
{
    public class ChatSystem : NetworkBehaviour
    {
        [SerializeField] private GameObject chatUI = null;                  //gameobjects and fields for UI
        [SerializeField] private TMP_Text chatText = null;
        [SerializeField] private TMP_InputField userInput = null;

        private static event Action<string> OnMessage;                      //even raised when user starts writing
        private String localPlayer = "";

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
            Debug.Log("Activating Chat");
            chatUI.SetActive(true);                             //activates UI

            OnMessage += HandleNewMessage;                  //subsribe to event, handleNewMessage called every time onMessage invoked
        }

        public override void OnStartClient()
        {
            Debug.Log("Chat Client Started");
            
            
            for (int i = 0; i < Game.GamePlayers.Count; i++)                        //loops over the list of game players (the connected players), then checks if they are a local player.
            {
                Debug.Log(Game.GamePlayers[i]);
                if (connectionToClient.connectionId == Game.GamePlayers[i].ConnectionId)                       //checks if connection to client matches network game player
                {
                    this.localPlayer = Game.GamePlayers[i].PlayerName;           //sets the name of the local player
                    Debug.Log(this.localPlayer);
                }
            }
        }

        [ClientCallback]
        private void OnDestroy()                        //called when object destroyed
        {
            if (!hasAuthority) { return; }

            OnMessage -= HandleNewMessage;              //unsubscribes from event
        }

        private void HandleNewMessage(string message)                   //appends messages to text box
        {
            chatText.text += message;
        }

        [Client]
        public void Send(string message)                                    //method called on EndEdit of the inputField
        {
            if (!Input.GetKeyDown(KeyCode.Return)) { return; }              //checks if enter is pressed

            if (string.IsNullOrWhiteSpace(message)) { return; }             //checks if empty message

            CmdSendMessage(message);                                        //calls command 

            userInput.text = string.Empty;                                  //resets inputfield to empty
        }

        [Command]
        private void CmdSendMessage(string message)                                     //sends message to server, called by client, run on server
        {
            RpcHandleMessage($"[{localPlayer}]: {message}");            //formats message, connectionToClient.connectionId
        }

        [ClientRpc]                                                 //called on server, run on clients
        private void RpcHandleMessage(string message)               //message sent from local player shown to other client
        {
            OnMessage?.Invoke($"\n{message}");
        }
    }
}

