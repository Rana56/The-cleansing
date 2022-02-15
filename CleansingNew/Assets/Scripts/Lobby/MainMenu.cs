using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheCleansing.Lobby
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerCleansingLobby networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;        //ui that turns on and off

        public void HostLobby()
        {
            networkManager.StartHost();             //when host button pressed, sets player as host

            landingPagePanel.SetActive(false);      //disables landing page panel
        }
    }
}