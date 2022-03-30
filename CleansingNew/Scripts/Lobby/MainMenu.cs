using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheCleansing.Lobby
{
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu instance;

        [SerializeField] private NetworkManagerTC networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;        //ui that turns on and off
        [SerializeField] private GameObject namePanel = null;
        [SerializeField] private GameObject IpPanel = null;
        [SerializeField] private GameObject panelInfoPage = null;

        public void HostLobby()
        {
            networkManager.StartHost();             //when host button pressed, sets player as host

            landingPagePanel.SetActive(false);      //disables landing page panel
        }

        public void ReturnToMainMenu()
        {
            Debug.Log("MainMenu displayed");
            landingPagePanel.SetActive(true);
            namePanel.SetActive(false);
            IpPanel.SetActive(false);
            panelInfoPage.SetActive(false);
        }


        public void Exit()
        {
            /**
            if (UnityEditor.EditorApplication.isPlaying){
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                Debug.Log("quit");
                Application.Quit();
            }**/
            Debug.Log("quit");
            Application.Quit();
        }
    }
}