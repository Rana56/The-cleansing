using Mirror;

namespace TheCleansing.Lobby                   //a room player stores the user's name and if they are ready or not
{
    public class NetworkGamePlayerLobby : NetworkBehaviour              //script sits on the player when they join and destroyed when they leave, each player has this script
    {

        [SyncVar]              //sync the vairable across all the clients so that all players knows the correct names in case someone leaves or joins
        private string displayName = "Loading...";                   //server changes the names and the logic, it notifies all the other cilents

        private NetworkManagerCleansingLobby room;
        private NetworkManagerCleansingLobby Room        //a way to reference room easliy
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerCleansingLobby;          //casts the networkManager as a networkManagerLobby
            }
        }


        public override void OnStartClient()            //called on every network behaviour when active on a client
        {
            DontDestroyOnLoad(gameObject);          //this means that when the scene is changed the object won't be destoryed
            Room.GamePlayers.Add(this);             //adds to list for players, this instance added to list
        }

        public override void OnStopClient()             //called whenever anyone disconnects
        {
            Room.GamePlayers.Remove(this);          //removes whoever disconnected

        }

        [Server]                    //ensures the logic only run on the server
        public void SetDisplayName(string displayName)                      //sets dislpay name
        {
            this.displayName = displayName;
        }
    }
}