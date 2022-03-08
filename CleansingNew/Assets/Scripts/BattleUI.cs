using Mirror;
using System;
using TMPro;
using UnityEngine;


namespace TheCleansing.Lobby
{
    public class BattleUI : NetworkBehaviour
    {
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];          //used for show player name
        [SerializeField] private TMP_Text[] playerHealths = new TMP_Text[2];          //used for show player health

        [SyncVar(hook = nameof(OnNameChanged))]
        private string displayName = "Loading...";

        private NetworkManagerCleansingLobby room;
        private NetworkManagerCleansingLobby Room        //a way to reference room easliy
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerCleansingLobby;          //casts the networkManager as a networkManagerLobby
            }
        }

        private void OnNameChanged(String _Old, String _New) => UpdateDisplay();

        private void UpdateDisplay()
        {
            /**
            Debug.Log("Test2");
            if (isLocalPlayer)
            {
                for (int i = 0; i < playerNameTexts.Length; i++)
                {
                    playerNameTexts[i].text = Room.GamePlayers[i].displayName;
                }
            }

            if (!hasAuthority)                  //looks for the player that has authority and updates their display
            {
                foreach (var player in Room.GamePlayers)
                {
                    if (player.hasAuthority)
                    {
                        player.UpdateDisplay();
                        break;
                    }
                }

                return;
            }**/

            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = Room.GamePlayers[i].displayName;
            }
        }

        public override void OnStartAuthority()
        {
            Debug.Log("Test1");
            CmdSetDisplayName(PlayerNameInput.DisplayName);
        }

        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }
    }
}
