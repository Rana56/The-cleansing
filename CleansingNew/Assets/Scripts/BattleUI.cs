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
        [SerializeField] private GameObject battleUI = null;

        //local player
        [Header("Local Player")]
        [SerializeField] private TMP_Text localName = null;          //used for show player name
        [SerializeField] private TMP_Text localhealth = null;          //used for show player health

        //other player
        [Header("Other Player")]
        [SerializeField] private TMP_Text playerName = null;          //used for show player name
        [SerializeField] private TMP_Text playerHealth = null;          //used for show player health

        public uint PlayerNetId { get; private set; }           //stores player net ID

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

        public void SetUpUI(Player player)          //sets the name of the players to UI
        {
            PlayerNetId = player.netId;         //stores the net id of player

            Debug.Log("Setup UI");
            
            for (int i = 0; i < Game.GamePlayers.Count; i++)                        //loops over the list of game players (the connected players), then checks if they are a local player.
            {
                if (Game.GamePlayers[i].name.Equals("LocalGamePlayer"))
                {
                    localName.text = Game.GamePlayers[i].PlayerName;                //If it's true, the player name is assigned to appropriate text
                }
                else
                {
                    playerName.text = Game.GamePlayers[i].PlayerName;
                }
            }
        }

        /**
        public void updateDisplay()                //updates the display of health of players
        {
            if (hasAuthority && isLocalPlayer)
            {
                for (int i = 0; i < playerNameTexts.Length; i++)
                {
                    Health playerHealth = Game.GamePlayers[i].GetComponent<Health>();
                    playerHealths[i].text = playerHealth.toString();                        //displays the health of player

                }
            }
        }**/


        #region Old Code
        /*
        [SerializeField] private TMP_Text[] playerHealths = new TMP_Text[2];

        //localName.text = GameObject.Find("LocalGamePlayer").GetComponent<NetworkGamePlayer>().PlayerName;       //sets name by finding the gameobject for NetworkGamePlayer, then getting the script of it and then retrives the name of player
        //playerName.text = GameObject.Find("LocalGamePlayer").GetComponent<NetworkGamePlayer>().PlayerName;
        */
        #endregion
    }
}
