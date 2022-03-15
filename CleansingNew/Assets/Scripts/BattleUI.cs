using Mirror;
using System;
using TMPro;
using UnityEngine;

//script attached to player, wiil be spawned when player spawned

namespace TheCleansing.Lobby
{
    public class BattleUI : MonoBehaviour
    {
        //[SerializeField] private GameObject battleUI = null;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];          //used for show player name
        [SerializeField] private TMP_Text[] playerHealths = new TMP_Text[2];          //used for show player health

        private NetworkManagerTC game;
        private NetworkManagerTC Game        //a way to reference room easliy
        {
            get
            {
                if (game != null) { return game; }
                return game = NetworkManager.singleton as NetworkManagerTC;          //casts the networkManager as a networkManagerLobby
            }
        }

        /**
        public void Start()
        {
            SetUpBattle()
        }

        
        public override void OnStartAuthority()
        {
            Debug.Log("Testing2");
            SetUpBattle();
            //updateDisplay();
        }

        public override void OnStartClient()
        {
            Debug.Log("Testing3");
            SetUpBattle();
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("Testing4");
            SetUpBattle();
        }**/


        public void SetUpBattle()
        {
            Debug.Log("Setup Test");

            Debug.Log("Setup authority");
            /**
            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                Debug.Log("Setup loop");
                playerNameTexts[i].text = Game.GamePlayers[i].PlayerName;
            }**/
            //playerNameTexts[0].text = Game.GamePlayers[0].PlayerName;
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
    }
}
