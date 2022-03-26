using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

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

        private void CheckStartRound(NetworkConnection obj)
        {
            Debug.Log("Start round");
        }

        private void CleanServer()
        {
            NetworkManagerTC.OnServerStopped -= CleanServer;                //unsubscribes from events
            NetworkManagerTC.OnServerReadied -= CheckStartRound;
        }

        public void GetLocalGamePlayer()
		{
			LocalGamePlayer = GameObject.Find("LocalGamePlayer");
			//LocalGamePlayerScript = LocalGamePlayer.GetComponent<NetworkGamePlayer>();
		}
	}
}
