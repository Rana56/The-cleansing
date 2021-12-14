using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheCleansing.Lobby
{
    public class JoinLobbyMenu : MonoBehaviour              //script allows user to join with IP
    {
        [SerializeField] private NetworkManagerCleansingLobby networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;
        [SerializeField] private TMP_InputField ipAddressInputField = null;                 //IP input field
        [SerializeField] private Button joinButton = null;

        private void OnEnable()             //checks if client is connected or disconnected
        {
            NetworkManagerCleansingLobby.OnClientConnected += HandleClientConnected;
            NetworkManagerCleansingLobby.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            NetworkManagerCleansingLobby.OnClientConnected -= HandleClientConnected;
            NetworkManagerCleansingLobby.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void JoinLobby()
        {
            string ipAddress = ipAddressInputField.text;

            networkManager.networkAddress = ipAddress;
            networkManager.StartClient();               //when client is started, ipaddress used as the address to connect to

            joinButton.interactable = false;            //disables button so player can't spam the button
        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;             //re-enables button incase the player goes back to the menu

            gameObject.SetActive(false);                //disables this game object as it's a popup
            landingPagePanel.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            joinButton.interactable = true;             //if user fails to connect, enables the join button so they can try again
        }
    }
}
