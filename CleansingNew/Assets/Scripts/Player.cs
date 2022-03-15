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

        public override void OnStartLocalPlayer()
        {
            Debug.Log("UI instantiate");
            GameObject pUI = Instantiate(battleUI);
            //camera rotation
            Debug.Log("GamePlayer List: " + Game.GamePlayers.Count);
            Camera.main.transform.SetParent(transform);
            //Camera.main.transform.localPosition = new Vector3(GameObject.Find("LocalGamePlayer").GetComponent<Transform>().transform.position);
        }

    }
}
