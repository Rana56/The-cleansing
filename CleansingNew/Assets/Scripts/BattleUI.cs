using Mirror;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Random = System.Random;

//script attached to player, wiil be spawned when player spawned

namespace TheCleansing.Lobby
{
    public class BattleUI : NetworkBehaviour
    {
        [Header("UIs")]
        [SerializeField] private GameObject battleUI = null;
        [SerializeField] private GameObject movesUI = null;
        [SerializeField] private GameObject attackUI = null;

        //local player
        [Header("Local Player")]
        [SerializeField] private TMP_Text localName = null;          //used for show player name
        [SerializeField] private TMP_Text localHealth = null;          //used for show player health
        private GameObject playerScriptObject;

        //other player
        [Header("Other Player")]
        [SerializeField] private TMP_Text playerName = null;          //used for show player name
        [SerializeField] private TMP_Text playerHealth = null;          //used for show player health

        //player information
        [SerializeField] private TMP_Text info_text = null;
        public uint PlayerNetId { get; private set; }           //stores player net ID
        NetworkGamePlayer otherPlayer = null;                           //connected players are stored in variables
        NetworkGamePlayer localPlayer = null;

        //Class Guns information
        private string[] TankGuns = { "Ak47", "Shotgun", "RPG" };
        private string[] SoldierGuns = { "M4", "PumpShotgun", "Sniper" };
        private string[] MedicGuns = { "Pistol", "Smg", "MGrand" };
        private Dictionary<string, float> gunDamgage = new Dictionary<string, float>() {
            {"Ak47", 25},
            {"Shotgun", 45},
            {"RPG", 70},
            {"M4", 25},
            {"PumpShotgun", 40},
            {"Sniper", 60},
            {"Pistol", 15},
            {"Smg", 20},
            {"MGrand", 55},
        };

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

        public void deactivateMovesUI()               //deactivates the moves panel
        {
            Debug.Log("Deactivate buttons");
            Button[] buttons = movesUI.GetComponentsInChildren<Button>();           //gets all the buttons in panel 
            foreach(Button button in buttons)                                       //loops through the buttons and makes it not interatable
            {
                button.interactable = false;                        //makes button not interactable
            }
        }

        public void activateMovesUI()               //deactivates the moves panel
        {
            Debug.Log("Activate buttons");
            Button[] buttons = movesUI.GetComponentsInChildren<Button>();           //gets all the buttons in panel 
            foreach (Button button in buttons)                                       //loops through the buttons and makes it not interatable
            {
                if (button.GetComponentInChildren<Text>().text == "Special")
                {
                    button.interactable = false;
                }
                else
                {
                    button.interactable = true;                        //makes button not interactable
                }
            }
        }

        public void setUpAttackButtons()                                                //changes the button names based on the character class
        {
            Debug.Log("Chaging button names");
            Button[] buttons = attackUI.GetComponentsInChildren<Button>();
            if(localPlayer.CharacterClass == "Tank")
            {
                for(int i = 0; i < TankGuns.Length; i++)                                      
                {
                    buttons[i].GetComponentInChildren<Text>().text = TankGuns[i];               //gets the text of the buttons and assigns to text of class gun names
                }
            }
            else if (localPlayer.CharacterClass == "Soldier")
            {
                for (int i = 0; i < TankGuns.Length; i++)                                       
                {
                    buttons[i].GetComponentInChildren<Text>().text = SoldierGuns[i];
                }
            }
            else if (localPlayer.CharacterClass == "Medic")
            {
                for (int i = 0; i < TankGuns.Length; i++)                                       
                {
                    buttons[i].GetComponentInChildren<Text>().text = MedicGuns[i];
                }
            }
            else
            {
                Debug.Log("Error setupAttackButtons: " + localPlayer.CharacterClass);
            }
        }

        public void SetUpUI(Player player)          //sets the name of the players to UI
        {
            Debug.Log("Setup UI");
            playerScriptObject = player.gameObject;                                     //UI stores the game object that the player script is attached to
            PlayerNetId = player.netId;         //stores the net id of player
            Debug.Log("NetID: " + PlayerNetId);

            for (int i = 0; i < Game.GamePlayers.Count; i++)                        //loops over the list of game players (the connected players), then checks if they are a local player.
            {
                Health health = Game.GamePlayers[i].GetComponentInParent<Health>();
                health.OnHealthChanged += updateHealthDisplay;                                      //subscribes to event, when event invoked, method run
                Game.GamePlayers[i].UpdateReady += readyCheck;

                if (Game.GamePlayers[i].name.Equals("LocalGamePlayer"))
                {
                    localName.text = Game.GamePlayers[i].PlayerName;                //If it's true, the player name is assigned to appropriate text
                    localPlayer = Game.GamePlayers[i];
                }
                else
                {
                    playerName.text = Game.GamePlayers[i].PlayerName;
                    otherPlayer = Game.GamePlayers[i];
                }
            }
            updateHealthDisplay();
            setUpAttackButtons();
        }

        public void updateHealthDisplay()                //updates the display of health of players
        {
            Debug.Log("Updating Health");            
            for (int i = 0; i < Game.GamePlayers.Count; i++)
            {
                Health health = Game.GamePlayers[i].GetComponentInParent<Health>();                     //gets health script attched to NetworkGamePlayer
                Debug.Log("Updating Health: " + health + ", " + Game.GamePlayers[i].name);

                if (Game.GamePlayers[i].name.Equals("LocalGamePlayer"))
                {
                    localHealth.text = health.toString();                //If it's true, the player name is assigned to appropriate text
                }
                else
                {
                    playerHealth.text = health.toString();
                }

            }
        }

        public void Attack()            //attacks opponent
        {
            Debug.Log("Attack health");                                                     //gets health script attched to NetworkGamePlayer
            Health health = localPlayer.GetComponentInParent<Health>();                     //changes player health, doesn't change own health
            String buttonName = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;         //gets the name of the button pressed

            float value;
            bool hasValue = gunDamgage.TryGetValue(buttonName, out value);                  //searches dictionary for gun and returns the damage
            if (hasValue)
            {
                health.AttackPlayer(value, otherPlayer);                                           //local payer calls command
            }
            else
            {
                Debug.Log(buttonName + " - not in Gun dictionary");
            }

            localPlayer.CmdReadyUp();                                                       //readies local player
            deactivateMovesUI();
        }

        public void Heal()              //heals player
        {
            Debug.Log("Heal");
            Health health = localPlayer.GetComponentInParent<Health>();                     //gets health script attched to NetworkGamePlayer
            
            if(localPlayer.CharacterClass == "Tank")                                        //changes heal based on class
            {
                health.HealPlayer(5);                                                          //changes player health, doesn't change own health
            }
            else if(localPlayer.CharacterClass == "Soldier")
            {
                health.HealPlayer(10);
            } 
            else if(localPlayer.CharacterClass == "Medic")
            {
                health.HealPlayer(20);
            }
            else
            {
                Debug.Log("Error heal: " + localPlayer.CharacterClass);
            }
            
            localPlayer.CmdReadyUp();                                                       //readies local player
            deactivateMovesUI();
        }

        public void Special()
        {
            Debug.Log("Special");

            Random random = new Random();
            Health health = localPlayer.GetComponentInParent<Health>();                     //gets health script attched to NetworkGamePlayer

            if (localPlayer.CharacterClass == "Tank")                                        //special of tank, gets random gun and multiplies damage
            {
                Debug.Log("Special Tank");
                int index = random.Next(gunDamgage.Count);                                 //gets index based on size of gun dictionary

                List<float> damage = new List<float>(gunDamgage.Values);                    //creates a list based on key set

                health.AttackPlayer(damage[index] * 2, otherPlayer);
                Debug.Log(damage[index]);
            }
            else if (localPlayer.CharacterClass == "Soldier")
            {
                Debug.Log("Special Soldier");
                health.AttackPlayer(70, otherPlayer);                                                       //special of soldier, does 
            }
            else if (localPlayer.CharacterClass == "Medic")
            {
                Debug.Log("Special Medic");
                health.HealPlayer(localPlayer.GetComponentInParent<Health>().getMaxHealth() / 2);               //gets the max health of medic and heals themselves
            }
            else
            {
                Debug.Log("Error special: " + localPlayer.CharacterClass);
            }

            localPlayer.CmdReadyUp();                                                       //readies local player
            deactivateMovesUI();
        }

        public void readyCheck()
        {
            if (localPlayer.IsReady && !otherPlayer.IsReady)                            //if local player is ready and other player is not ready, it will change the notification           
            {
                Debug.Log("Local player ready and other player not ready");
                info_text.text = "Wating for other Player...";                      //changes notificiation

            } else if (localPlayer.IsReady && otherPlayer.IsReady)              //if both players are ready, it will make the notification blank
            {
                Debug.Log("Both player ready");
                info_text.text = "";

            } 
            else if (!localPlayer.IsReady && !otherPlayer.IsReady){
                info_text.text = "Select your move";
                activateMovesUI();
            }
            else
            {
                info_text.text = "Select your move";                                       
                activateMovesUI();
                /*
                BattleUI[] moveUIs = FindObjectsOfType<BattleUI>();        //gets all the battle UI and activates it again
                foreach (BattleUI ui in moveUIs)
                {
                    ui.activateMovesUI();                   //TODO Fix doesn't work for other clients
                }*/
                                                             //activates move ui, i.e. both players' ready is false            
            }
        }

        public override void OnStopClient()                                                     //unsubscribes from event when client disconnects
        {
            for(int i = 0; i < Game.GamePlayers.Count; i++)
            {
                Health health = Game.GamePlayers[i].GetComponentInParent<Health>();
                health.OnHealthChanged -= updateHealthDisplay;
                Game.GamePlayers[i].UpdateReady -= readyCheck;
            }
        }

        #region Old Code
        /*
        [SerializeField] private TMP_Text[] playerHealths = new TMP_Text[2];

        //localName.text = GameObject.Find("LocalGamePlayer").GetComponent<NetworkGamePlayer>().PlayerName;       //sets name by finding the gameobject for NetworkGamePlayer, then getting the script of it and then retrives the name of player
        //playerName.text = GameObject.Find("LocalGamePlayer").GetComponent<NetworkGamePlayer>().PlayerName;

        localHealth.text = player.GetComponent<Health>().toString();

        else if (!localPlayer.IsReady && !otherPlayer.IsReady)            //if boths players are not ready, it will activate the move ui
            {
                Debug.Log("Acctiviating move UI");
                activateMovesUI();
            }
        */
        #endregion
    }
}
