using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheCleansing.Lobby
{
    public class NetworkManagerTC : NetworkManager
    {
        [SerializeField] private int minPlayers = 2;                        //minimum players needed to start game
        [Scene] [SerializeField] private string menuScene = string.Empty;       //scene reference

        [Header("Room")]
        [SerializeField] private NetworkLobbyPlayer roomPlayerPrefab = null;             //reference to network room player
                                                                                             //[SerializeField] string menuScene;

        [Header("Game")]
        [SerializeField] private NetworkGamePlayer gamePlayerPrefab = null;
        [SerializeField] private GameObject playerSpawnSystem = null;                   //gameobject with the player spawn system
        [SerializeField] private GameObject gameManager = null;

        public string CurrentGamePhase;                                             //game phase that checks if its attacking or animations time 
        
        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<NetworkConnection> OnServerReadied;          //used to know if everyone has connected to the game and is ready to start on the server, include a timeout if someone disconnects
        public static event Action OnServerStopped;                             //called when server stopped

        public List<NetworkLobbyPlayer> RoomPlayers { get; } = new List<NetworkLobbyPlayer>();          //stores all the joined player in a list, so they can all be accessed for functions
        public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();          //stores all the players in the game
        public List<Player> SpawnedGamePlayers { get; } = new List<Player>();                 //stores all the physical game players, the gameobejects on screen
        
        //loads all game objects from resources, under the spawnable prefabs. spawnable prefabs are objects the will spawn on the network
        public override void OnStartServer()
        {
            spawnPrefabs.Clear();
            spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
        }

        public override void OnStartClient()
        {
            spawnPrefabs.Clear();
            var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

            NetworkClient.ClearSpawners();

            foreach (var prefab in spawnablePrefabs)
            {
                NetworkClient.RegisterPrefab(prefab);
            }
        }

        public override void OnClientConnect()                //does base logic and raises the event
        {
            base.OnClientConnect();                                     //ClientConnect(NetworkConnection conn) deprecated - use NetworkClient.connection instead
            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();

            OnClientDisconnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (numPlayers >= maxConnections)                       //doesn't let player join if max players reached
            {
                conn.Disconnect();
                return;
            }

            if (SceneManager.GetActiveScene().path != menuScene)            //doesn't let player join if game already started
            {
                conn.Disconnect();
                return;
            }
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                bool isLeader = RoomPlayers.Count == 0;           //first person in the lobby is assgined the leader

                NetworkLobbyPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);              //spawns prefab

                roomPlayerInstance.IsLeader = isLeader;             //sets person as leader and so will get leader privalages
                roomPlayerInstance.ConnectionId = conn.connectionId;
                roomPlayerInstance.PlayerNumber = RoomPlayers.Count + 1;

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);              //adds player for connection, assigns network connection to player

                Debug.Log("Player name and Id: " + roomPlayerInstance.DisplayName + ", " + roomPlayerInstance.ConnectionId.ToString());
            }
        }

        
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkLobbyPlayer>();              //gets players room player script and removes it from the list on the server side

                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();               //notifies other players on the client side
            }

            base.OnServerDisconnect(conn);          //destorys the player for the connection, does original method before overrriding
        }
        
        public void NotifyPlayersOfReadyState()
        {
            foreach (var player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());                //loops through all the players to see if they are ready to start
            }
        }

        
        public bool IsReadyToStart()                                //loops through lobby player to check if they are ready
        {
            if (numPlayers < minPlayers) { return false; }          //if min players not reached, game won't start

            foreach (var player in RoomPlayers)                 //if all the players are not ready, game won't start
            {
                if (!player.IsReady)
                {
                    return false;
                }
            }

            return true;        //returns true, if it doens't meet the previous if requirements
        }

        public bool IsReadyToAction()                               //loops through lobby player to check if they are ready
        {
            if (numPlayers < minPlayers) { return false; }          //if min players not reached, game won't start

            foreach (var player in GamePlayers)                 //if all the players are not ready, game won't start
            {
                if (!player.IsReady)
                {
                    return false;
                }
            }

            return true;        //returns true, if it doens't meet the previous if requirements
        }

        public void SetGameReadyFalse()                                 //sets the ready of game players back to false, so they can continue with their moves
        {
            foreach (NetworkGamePlayer player in GamePlayers)
            {
                Debug.Log("Setting Players to False");
                player.IsReady = false;
                Debug.Log(player.IsReady);
            }
        }

        public void ChangeGamePhase(string newGamePhase)                    //changes the game phase between "Move selection" and "Animation"
        {
            CurrentGamePhase = newGamePhase;
            if (newGamePhase == "Move Selection")
            {
                Debug.Log("Moves Selection");
                //end move selection
                SetGameReadyFalse();                                        //gameplayers set to false

                /*
                BattleUI[] moveUIs = FindObjectsOfType<BattleUI>();        //gets all the battle UI and activates it again
                foreach (BattleUI ui in moveUIs)
                {
                    ui.activateMovesUI();                   //TODO Fix doesn't work for other clients
                }*/

            }
            else if (newGamePhase == "Animation")
            {
                Debug.Log("Animations...");
                //Animation function
                //TODO timer - waits for a few seconds
                ChangeGamePhase("Move Selection");                   //changes game phase back to move 
            }
            else
            {
                Debug.Log("Error Game Phase: " + newGamePhase);
            }
        }

        public void NotifyGamePlayerReady()             //checks if all game player are ready
        {
            if (IsReadyToAction())                      //loops through all the players to see if they are ready to start
            {
                Debug.Log("Change to animation phase");             //does animations one all players are ready
                ChangeGamePhase("Animation");
            }
        }

        public override void OnStopServer()         //called when server is stopped, called for every client - clears list and list is empty when starting new game
        {
            OnServerStopped?.Invoke();          //checks server is stopped before clearing lists

            RoomPlayers.Clear();
            GamePlayers.Clear();
            SpawnedGamePlayers.Clear();
        }

        
        public void StartGame()                     //when start button is pressed, this function is run
        {
            if (SceneManager.GetActiveScene().path == menuScene)         //checks is current scene is the menu
            {
                if (!IsReadyToStart()) { return; }                  //checks if everyone is ready

                ServerChangeScene("Map_1");                 //changes scene
            }
        }

        public override void ServerChangeScene(string newSceneName)                 //method handles scene change, e.g. going to a game, going out of game
        {
            // From menu to game
            if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Map_"))            //checks if current scene is the menu and if the new scene matches the string - going from the main menu to the game
            {
                for (int i = RoomPlayers.Count - 1; i >= 0; i--)                    //when going from menu to game, goes trough all the room players
                {
                    var conn = RoomPlayers[i].connectionToClient;                   //gets their connection
                    var gameplayerInstance = Instantiate(gamePlayerPrefab);             //spawns in their game version of the prefab
                    Debug.Log("Destorying lobby room object: " + gameplayerInstance);

                    gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);      //sets the display name, transfers the name from the room to the game player
                    gameplayerInstance.SetConnectionId(RoomPlayers[i].ConnectionId);
                    gameplayerInstance.SetPlayerNumber(RoomPlayers[i].PlayerNumber);

                    Debug.Log("Destorying lobby room object: " + conn.identity.gameObject);
                    NetworkServer.Destroy(conn.identity.gameObject);        //destorys their game object for thier current identity, i.e. gets rid of their room player object
                    
                    //playerPrefab.GetComponent<NetworkGamePlayerLobby>().CmdSetDisplayName(conn.identity.GetComponent<NetworkGamePlayerLobby>().GetDisplayName());
                    
                    //the connection to the client is now not the object that was destroyed, it is the object that was spawned in
                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);          //assigns the new game player to connection player insted of the old one
                    
                    Debug.Log("Replace player connection ");
                }

                CurrentGamePhase = "Move Selection";                        
            }

            base.ServerChangeScene(newSceneName);       //does the base logic for chaging the scene
        }

        public override void OnServerReady(NetworkConnection conn)      //listens into whether the client is ready on the server
        {
            base.OnServerReady(conn);

            OnServerReadied?.Invoke(conn);              //onserverReadied event raised
        }
        
        public override void OnServerSceneChanged(string newSceneName)               //called on the server, when a scene is completed
        {                                                                           //when the scene is loaded, actions can start on the scene
            if (newSceneName.StartsWith("Map_"))
            {                           //checks if it is one of the levels   
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);              //spwans the player spawn system, connection not passed as parameter, so the server owns it
                NetworkServer.Spawn(playerSpawnSystemInstance);                 //all clients has a spawn system and is owned by the server

                GameObject gameManagerInstance = Instantiate(gameManager);
                NetworkServer.Spawn(gameManagerInstance);
            }
        }
        
    }
}


