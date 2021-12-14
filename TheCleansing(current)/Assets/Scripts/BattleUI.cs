using Mirror;
using TheCleansing.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace TheCleansing
{
    public class BattleUI : NetworkBehaviour
    {
        public Text playerInfoText;
        public Text enemyInfoText;
        public PlayerScript playerScript;
        public BattleSystem battle;

        [SyncVar(hook = nameof(OnPlayerStatusChange))]
        private string playerStatusText;

        [SyncVar(hook = nameof(OnEnemyStatusChange))]
        private string enemyStatusText;

        void Awake()
        {
            //allow all players to run this
            battle = GameObject.FindObjectOfType<BattleSystem>();
        }

        //player status change
        void OnPlayerStatusChange(string _Old, string _New)
        {
            //called from sync var hook, to update info on screen for all players
            playerInfoText.text = playerStatusText;
        }

        //Enemy satus change
        void OnEnemyStatusChange(string _Old, string _New)
        {
            enemyInfoText.text = enemyStatusText;
        }

        public void AttackEnemy()           //attack button
        {
            if (playerScript != null)
                playerScript.CmdUpdateEnemyHealth();
                playerScript.CmdUpdatePlayerHealth();
        }

        public void PassTurn()          //pass button
        {
            //pass turn for player
            //set enemy turn to true and so it will be enemy turn
        }

        public void setEnemyText(string text)
        {
            enemyStatusText = text;
        }

        public void setPlayerText(string text)
        {
            playerStatusText = text;
        }
    }
}

//TODO have a battle HUD to handle all text changes