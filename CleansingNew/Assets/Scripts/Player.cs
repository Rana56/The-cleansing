using Mirror;
using System;
using TMPro;
using UnityEngine;


//script attached to player, wiil be spawned when player spawned

namespace TheCleansing.Lobby
{
    public class Player : NetworkBehaviour                //physical player in game
    {
        [SerializeField] private GameObject battleUI = null;

        private NetworkManagerTC game;
        private NetworkManagerTC Game        //a way to reference room easliy
        {
            get
            {
                if (game != null) { return game; }
                return game = NetworkManager.singleton as NetworkManagerTC;          //casts the networkManager as a networkManagerLobby
            }
        }

        private void Awake()
        {
            Debug.Log("Reacing instantiate");
            GameObject pUI = Instantiate(battleUI);
            //pUI.GetComponent;
        }

    }
}
