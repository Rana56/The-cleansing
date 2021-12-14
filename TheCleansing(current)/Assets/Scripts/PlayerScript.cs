using Mirror;
using TheCleansing.Combat;
using UnityEngine;

namespace TheCleansing
{
    public class PlayerScript : NetworkBehaviour
    {
        public TextMesh playerNameText;
        public GameObject floatingInfo;
        private Material playerMaterialClone;
        private BattleUI battleUI;
        private BattleSystem battle;
        private int health = 200;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;
        [SyncVar(hook = nameof(OnColorChanged))]
        public Color playerColor = Color.white;

        void OnNameChanged(string _Old, string _New)
        {
            playerNameText.text = playerName;
        }
        void OnColorChanged(Color _Old, Color _New)
        {
            playerNameText.color = _New;
            playerMaterialClone = new Material(GetComponent<Renderer>().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer>().material = playerMaterialClone;
        }
        public override void OnStartLocalPlayer()
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 2, -10);

            floatingInfo.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
            floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            string name = "Player" + Random.Range(100, 999);
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            CmdSetupPlayer(name, color);

            battleUI.playerScript = this;
        }
        void Awake()
        {
            //allow all players to run this
            battleUI = GameObject.FindObjectOfType<BattleUI>();
            battle = GameObject.FindObjectOfType<BattleSystem>();
        }

        public void TakeDamge()
        {
            var atatckdmg = Random.Range(0, 10);
            Debug.Log(atatckdmg);
            var isDead = this.GetComponent<Health>().TakeDamge(atatckdmg);               //attacks a random player
            if (isDead)
            {
                battle.PlayerKilledPopup();
            }
        }

        [Command]
        public void CmdSendPlayerMessage()
        {
            if (battleUI)
                //sceneScript.statusText = $"{playerName} says hello {Random.Range(10, 99)}";
                health -= 10;
                battleUI.setEnemyText($"Enemy Health: {health}/200");
                //battleUI.enemyStatusText = $"Enemy Health: {health}/200";
        }
        
        [Command]
        public void CmdUpdateEnemyHealth()
        {
            battle.OnPlayerAttackButton();
            battleUI.setEnemyText($"Enemy Health: {battle.getEnemyUnit().getHp()}/200");
        }

        
        [Command]
        public void CmdUpdatePlayerHealth()
        {
            TakeDamge();
            battleUI.setPlayerText($"Player Health: {this.GetComponent<Health>().getHp()}/200");
        }

        [Command]
        public void CmdSetupPlayer(string _name, Color _col)
        {
            // player info sent to server, then server updates sync vars which handles it on all clients
            playerName = _name;
            playerColor = _col;

            battleUI.setPlayerText($"Player Health: {this.GetComponent<Health>().getHp()}/200");
            /**
            if (isLocalPlayer)
            {
                battleUI.setPlayerText($"Player Health: {this.GetComponent<Health>().getHp()}/200");
            }**/
            //battleUI.setEnemyText($"{playerName} joined.");
        }

        void Update()
        {
            if (!isLocalPlayer) {
                // make non-local players run this
                floatingInfo.transform.LookAt(Camera.main.transform);
                return; 
            }
        }
    }
}