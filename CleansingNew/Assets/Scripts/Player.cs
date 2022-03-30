using Mirror;
using System;
using TMPro;
using UnityEngine;


//script attached to player, wiil be spawned when player spawned

namespace TheCleansing.Lobby
{
    public class Player : NetworkBehaviour                //physical player in game
    {
        [SerializeField] private GameObject battleUI = null;            //UI Object
        [SerializeField] private Animator animator = null;                      //the animator for the characters

        private BattleUI localUI;

        //[SyncVar(hook = nameof(HandleOwnerSet))]
        private uint ownerID;

        private NetworkManagerTC game;
        private NetworkManagerTC Game        //a way to reference game easliy
        {
            get
            {
                if (game != null) { return game; }
                return game = NetworkManager.singleton as NetworkManagerTC;          //casts the networkManager as a networkManagerLobby
            }
        }

        public override void OnStartClient()
        {
            Debug.Log("UI instantiate");
            GameObject playerUI = Instantiate(battleUI);

            Debug.Log("UI activate");                   
            if (hasAuthority)                                       //Turns on UI only for local player
            {
                Debug.Log("Local Player");
                gameObject.name = "LocalPlayer";                        //changes game object's name of physical player

                localUI = playerUI.GetComponent<BattleUI>();
                localUI.activateUI();
                localUI.SetUpUI(this, animator);
                Debug.Log("Animator");
                animator.SetBool("IsShooting", true);
            }

            Game.SpawnedGamePlayers.Add(this);                      //adds this player instance to list so all players can be referenced
            Debug.Log("Local Player spawned: " + Game.SpawnedGamePlayers.Count);
        }

        public uint OwnerID => ownerID;             //this method returns the owner id of player

        public override void OnStartAuthority()
        {
            //camera position
            Transform cameraTransform = Camera.main.gameObject.transform;           //finds the main camera part of the scene
            //cameraTransform.parent = this.GetComponent<Transform>().transform;          //makes the camera a child of the player
            //cameraTransform.position = this.GetComponent<Transform>().position;         //sets position/rotation same as player
            //cameraTransform.rotation = this.GetComponent<Transform>().rotation;

            //Camera.main.transform.localPosition = new Vector3(GameObject.Find("LocalGamePlayer").GetComponent<Transform>().transform.position);
        }

        public override void OnStartLocalPlayer()
        {
            //Camera.main.transform.SetParent(transform);
            //Camera.main.transform.localPosition = new Vector3(0,0,0);
        }

    }
}
