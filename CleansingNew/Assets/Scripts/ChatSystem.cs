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

        public override void OnStartAuthority()
        {
            chatUI.SetActive(true);                             //activates UI

            OnMessage += HandleNewMessage;                  //subsribe to event, handleNewMessage called every time onMessage invoked
        }

        [ClientCallback]
        private void OnDestroy()
        {
            if (!hasAuthority) { return; }

            OnMessage -= HandleNewMessage;              //unsubscribes from event
        }

        private void HandleNewMessage(string message)                   //appends messages
        {
            chatText.text += message;
        }

        [Client]
        public void Send(string message)
        {
            if (!Input.GetKeyDown(KeyCode.Return)) { return; }              //checks if enter is pressed

            if (string.IsNullOrWhiteSpace(message)) { return; }             //checks if empty message

            CmdSendMessage(message);                                        //calls command 

            userInput.text = string.Empty;                                  //resets inputfield to empty
        }

        [Command]
        private void CmdSendMessage(string message)                                     //sends message to server, called by client, run on server
        {
            RpcHandleMessage($"[{connectionToClient.connectionId}]: {message}");
        }

        [ClientRpc]                                                 //called on server, run on clients
        private void RpcHandleMessage(string message)
        {
            OnMessage?.Invoke($"\n{message}");
        }
    }
}

