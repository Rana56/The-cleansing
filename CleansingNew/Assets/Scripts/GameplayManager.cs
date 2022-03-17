using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
		[SerializeField] private NetworkGamePlayer LocalGamePlayerScript;


		public void GetLocalGamePlayer()
		{
			LocalGamePlayer = GameObject.Find("LocalGamePlayer");
			LocalGamePlayerScript = LocalGamePlayer.GetComponent<NetworkGamePlayer>();
		}
	}
}
