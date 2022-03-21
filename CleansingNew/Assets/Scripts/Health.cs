using Mirror;
using System;
using UnityEngine;

namespace TheCleansing.Lobby
{
    public class Health : NetworkBehaviour
    {
        public int damage;

        [SerializeField] private float MaxHP = 200;

        [SyncVar(hook = nameof(HandleHelathUpdated))]          //synced across the network and calls method whenever health is updated
        [SerializeField] private float health = 0;             //sync the health

        public static event EventHandler<DeathEventArgs> OnDeath;
        public event EventHandler<HealthChangedEventArgs> OnHealthChanged;

        public bool IsDead => health == 0;              //function that checks if health is euqal to 0, returns true if check is correct

        public override void OnStartServer()            //when server starts, sets the health of players
        {
            health = MaxHP;
        }

        private void HandleHelathUpdated(float oldValue, float newValue)                //this method run whenever the health is changed
        {
            OnHealthChanged?.Invoke(this, new HealthChangedEventArgs
            {
                CurrentHealth = health,
                MaxHealth = MaxHP
            });
        }

        [Server]
        public void Add(float value)                    //add health to player
        {
            value = Mathf.Max(value, 0);                //sets value to the largest parameter value, i.e. can't have negatives

            health = Mathf.Min(health + value, MaxHP);          //sets health to the smallest value of passed paramter, i.e. can't have helath more than max health    
        }

        [Server]
        public void Remove(float value)                 //removes helath from player
        {
            value = Mathf.Max(value, 0);

            health = Mathf.Min(health - value, 0);              //sets health to 0 if, final health goes past zero. Otherwise, sets it to value after taking damage

            if (health == 0)
            {
                RpcHandleDeath();
            }
        }


        [ClientRpc]                                         //when player dies, it will turn off its game object
        private void RpcHandleDeath()                       //method called on server and run on clients
        {
            //gameObject.SetActive(false);  
            GameObject.Find("LocalPlayer").SetActive(false);            //turns off the player game object
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