using System.Collections.Generic;
using System.Linq;

namespace TheCleansing.Lobby
{
    public class MapSystem
    {
        private readonly IReadOnlyCollection<string> maps;              //variables to store maps
        private readonly int numRounds;                                 //stores the number of rounds

        private int currentRound;                                       //current round
        private List<string> mapsLeft;                             //list that's a copy of maps and decreases as rounds are complete as maps are removed

        public MapSystem(MapSet mapSet, int numberOfRounds)                //constructor
        {
            maps = mapSet.Maps;
            this.numRounds = numberOfRounds;                                //sets variables to parameters when initalised

            ResetMaps();                                                    //resets maps
        }

        public bool IsComplete => currentRound == numRounds;                //function checks if all rounds are complete

        public string NextMap                                               //gets the next map
        {
            get
            {
                if (IsComplete) { return null; }                            //check if game is complete, if not complete get maps 

                currentRound++;                                             //increase current round

                if (mapsLeft.Count == 0) { ResetMaps(); }              //if maps list empy, resets list

                string map = mapsLeft[UnityEngine.Random.Range(0, mapsLeft.Count)];       //gets a radom map between 0 and count

                mapsLeft.Remove(map);                                  //removes selected map

                return map;
            }
        }

        private void ResetMaps() => mapsLeft = maps.ToList();                  //resets maps
    }
}