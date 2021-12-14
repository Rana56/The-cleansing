using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TheCleansing;
using UnityEngine;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }          //Uses enum to define the game states the game can be in, allows to create varibales that can only be one the sates

namespace TheCleansing.Combat
{
    public class BattleSystem : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject ememyPrefab;
        [SerializeField] private Transform enemySpawnPoint;           //location of characters

        [SerializeField] private GameObject enemyDeadPopup = null;      //popup that shows when user kills enemy
        [SerializeField] private GameObject PlayerDeadPopup = null;
        [SerializeField] private GameObject battleInterface = null;

        private GameObject enemy;
        private Health playerUnit;
        private Health enemyUnit;
        private BattleUI battleUI;

        public BattleState state;
        private bool enemyTurn = false;

        //Start is called before the first frame update
        public void Start()
        {
            state = BattleState.START;
            SetupBattle();
        }


        public override void OnStartClient()
        {
            EnemyServerSpawn();
        }

        private void SetupBattle()
        {
            //GameObject playerGo = Instantiate(playerPrefab, playerBattleStation);             //spawns a playerPrefab a child of and on top of the playerBattleStation
            //playerUnit = playerGo.GetComponent<Unit>();          //gets the unit component attached to the player, allows to get health information and other stuff

        }

        public void OnPlayerAttackButton()
        {
            //Enemy take damge
            bool isDead = enemyUnit.TakeDamge(10);
            enemyTurn = true;

            if (isDead)
            {
                EnemyKilledPopup();
                NetworkServer.Destroy(enemy);
            } else
            {
                if (enemyTurn)
                {
                    //AttackPlayer();
                    enemyTurn = false;
                    Thread.Sleep(2);
                }
            }
        }

        public Health getEnemyUnit()
        {
            return enemyUnit;
        }

        /**
        public void AttackPlayer()
        {
            Debug.Log("Test");
            if (isClient)
            {
                GameObject[] Users = GameObject.FindGameObjectsWithTag("Player");
                var playernum = UnityEngine.Random.Range(0, NetworkServer.connections.Count);     //generates a radom number to select player and attack them
                var atatckdmg = UnityEngine.Random.Range(0, 10);
                Debug.Log("Test2");
                Users[playernum].GetComponent<Health>().TakeDamge(atatckdmg);               //attacks a random player
            }
            Debug.Log("Test3");

        }**/

        [Server]
        public void EnemyKilledPopup()
        {
            enemyDeadPopup.SetActive(true);
            battleInterface.SetActive(false);
        }

        [Server]
        public void PlayerKilledPopup()
        {
            PlayerDeadPopup.SetActive(true);
            battleInterface.SetActive(false);
        }

        [Server]
        public void EnemyServerSpawn()
        {
            enemy = Instantiate(ememyPrefab, enemySpawnPoint.position, Quaternion.Euler(0,180,0));                //server spawns enemy, quanterion.identitiy means there is no rotaion, eurler means it rotates 180
            enemyUnit = enemy.GetComponent<Health>();
            NetworkServer.Spawn(enemy);
            //battleUI.setEnemyText($"Enemy Health: {enemyUnit.getHp()}/200");
        }
    }
}
