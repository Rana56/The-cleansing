using Mirror;
using System;
using UnityEngine;

namespace TheCleansing.Lobby
{
    public class Health : NetworkBehaviour
    {
        public int damage;

        [SerializeField] private float MaxHP = 200;

        [SyncVar(hook = nameof(HandleHealthUpdated))]          //synced across the network and calls method whenever health is updated
        [SerializeField] private float health = 0;             //sync the health

        public static event EventHandler<DeathEventArgs> OnDeath;
        public event Action OnHealthChanged;

        public bool IsDead => health == 0;              //function that checks if health is euqal to 0, returns true if check is correct

        public override void OnStartServer()            //when server starts, sets the health of players
        {
            health = MaxHP;
            Debug.Log("Setting health");
        }

        private void HandleHealthUpdated(float oldValue, float newValue)                //this method run whenever the health is changed
        {
            Debug.Log("Invoking health");
            OnHealthChanged?.Invoke();
        }

        [Server]
        public void Add(float value)                    //add health to player
        {
            value = Mathf.Max(value, 0);                //sets value to the largest parameter value, i.e. can't have negatives

            health = Mathf.Min(health + value, MaxHP);          //sets health to the smallest value of passed paramter, i.e. can't have helath more than max health    
        }

        [Server]
        public void Remove(float value)                 //removes health from player
        {
            value = Mathf.Max(value, 0);

            health = Mathf.Max(health - value, 0);              //sets health to 0 if, final health goes past zero. Otherwise, sets it to value after taking damage

            if (health == 0)
            {
                RpcHandleDeath();
            }
        }

        [Command]                                                                       //comand called by local player, this command is called on client and run on server
        public void AttackPlayer(float value, NetworkGamePlayer player)
        {
            Health otherPlayerHealth = player.GetComponent<Health>();                   //calls server command on other player health 
            otherPlayerHealth.Remove(value);
        }

        [Command]                                                                       //comand called by local player, this command is called on client and run on server
        public void HealPlayer(float value)
        {                                                                   //calls server command on other player health 
            Add(value);
        }

        [ClientRpc]                                         //when player dies, it will turn off its game object
        private void RpcHandleDeath()                       //method called on server and run on clients
        {
            //gameObject.SetActive(false);  
            //GameObject.Find("LocalPlayer").SetActive(false);            //turns off the player game object
            //TODO despawn player object
            Debug.Log("Despawn Player object");

        }

        public float getHp()
        {
            return health;
        }

        public void resetHealth()
        {
            health = MaxHP;
        }

        public string toString()
        {
            return health + "/" + MaxHP;
        }
    }
}