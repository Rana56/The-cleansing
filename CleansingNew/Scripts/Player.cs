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
        private BattleUI localUI;

        //[SyncVar(hook = nameof(HandleOwnerSet))]
        private uint ownerID;

        public override void OnStartClient()
        {
            Debug.Log("UI instantiate");
            GameObject playerUI = Instantiate(battleUI);

            Debug.Log("UI activate");                   
            if (hasAuthority)                                       //Turns on UI only for local player
            {
                Debug.Log("Local Player");
                gameObject.name = "LocalPlayer";
                localUI = playerUI.GetComponent<BattleUI>();
                localUI.GetComponent<BattleUI>().activateUI();
                localUI.SetUpUINames();
            }
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

    }
}
