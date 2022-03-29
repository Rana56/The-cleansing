using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

namespace TheCleansing.Lobby
{
	public class GameplayManager : NetworkBehaviour				//Round system
	{
		[Header("GamePlayers")]
		[SerializeField] private GameObject LocalGamePlayer;
		//[SerializeField] private NetworkGamePlayer LocalGamePlayerScript;

        private NetworkManagerTC game;
        private NetworkManagerTC Game        //a way to reference room easliy
        {
            get
            {
                if (game != null) { return game; }
                return game = NetworkManager.singleton as NetworkManagerTC;          //casts the networkManager as a networkManagerLobby
            }
        }

        public override void OnStartServer()
        {
            NetworkManagerTC.OnServerStopped += CleanServer;                //subsribes method to event, method called when server stopped
            NetworkManagerTC.OnServerReadied += CheckStartRound;
        }

        [ServerCallback]
        private void OnDestroy()                    //when monobehaviours are destroyed, this method called and unsubcribes from events. It's a server call back, so it only called on server
        {
            CleanServer();
        }

        [Server]
        private void CheckStartRound(NetworkConnection obj)
        {
            Debug.Log("Start round check");
            if (Game.GamePlayers.Count(player => player.connectionToClient.isReady) != Game.GamePlayers.Count) { return; }                      //This function gets the number of gamePlayers that are ready, if it not equal to number of people in the list, don't do anything
        }

        [ServerCallback]                            //this tag doesn't give errors if client calls the method, client calls is ignored 
        public void StartRound()                    //method done if server
        {
            RpcStartRound();
        }

        private void CleanServer()
        {
            NetworkManagerTC.OnServerStopped -= CleanServer;                //unsubscribes from events
            NetworkManagerTC.OnServerReadied -= CheckStartRound;
        }

        private void RpcStartRound()
        {
            Debug.Log("Start Round");
        }

        public void GetLocalGamePlayer()
		{
			LocalGamePlayer = GameObject.Find("LocalGamePlayer");
			//LocalGamePlayerScript = LocalGamePlayer.GetComponent<NetworkGamePlayer>();
		}
	}
}
