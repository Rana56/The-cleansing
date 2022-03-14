using Mirror;
using UnityEngine;

namespace TheCleansing.Lobby
{
    public class Health : NetworkBehaviour
    {
        public string unitName;                 //script stores basic info of characters
        //public int unitLevel;

        public int damage;

        [SerializeField] private int maxHP = 200;

        [SyncVar]
        [SerializeField] private int currentHP;             //sync the health

        public bool TakeDamge(int dmg)          //does damage and then checks if the user is dead
        {
            currentHP -= dmg;

            if (currentHP <= 0)
            {
                currentHP = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        public int getHp()
        {
            return currentHP;
        }

        public void resetHealth()
        {
            currentHP = maxHP;
        }

        public string toString()
        {
            return currentHP + "/" + maxHP;
        }
    }
}