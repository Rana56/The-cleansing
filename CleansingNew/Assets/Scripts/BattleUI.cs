using Mirror;
using System;
using TMPro;
using UnityEngine;

//script attached to player, wiil be spawned when player spawned

namespace TheCleansing.Lobby
{
    public class BattleUI : NetworkBehaviour
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

        
        private void Awake()
        {
            Debug.Log("Testing");
            SetUpBattle();
            updateDisplay();
        }

        /**
        public override void OnStartAuthority()
        {
            Debug.Log("Testing");
            SetUpBattle();
            updateDisplay();
        }**/


        public void SetUpBattle()
        {
            if (hasAuthority)
            {
                for (int i = 0; i < playerNameTexts.Length; i++)
                {
                    playerNameTexts[i].text = Game.GamePlayers[i].PlayerName;
                }
            }
        }
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
        }
    }
}
