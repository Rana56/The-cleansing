using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private GameObject battleInterface = null;

        //private GameObject enemy;
        private Health playerUnit;
        private Health enemyUnit;
        private BattleUI battleUI;

        public BattleState state;

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

            //CmdUpdateEnemyHealth();

            if (isDead)
            {
                enemyDeadPopup.SetActive(true);
                battleInterface.SetActive(false);
            } else
            {

            }
        }

        public Health getEnemyUnit()
        {
            return enemyUnit;
        }

        /**
        [Command]
        public void CmdUpdateEnemyHealth()
        {
            battleUI.setEnemyText($"Enemy Health: {enemyUnit.getHp()}/200");
        }
        
        [Command]
        public void CmdUpdatePlayerHealth()
        {
            battleUI.setPlayerText($"Enemy Health: {playerUnit.getHp()}/200");
        }**/


        [Server]
        public void EnemyServerSpawn()
        {
            var enemy = Instantiate(ememyPrefab, enemySpawnPoint.position, Quaternion.Euler(0,180,0));                //server spawns enemy, quanterion.identitiy means there is no rotaion, eurler means it rotates 180
            enemyUnit = enemy.GetComponent<Health>();
            NetworkServer.Spawn(enemy);
            //battleUI.setEnemyText($"Enemy Health: {enemyUnit.getHp()}/200");
            //CmdUpdateEnemyHealth();
        }
    }
}
