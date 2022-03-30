using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace TheCleansing.Lobby                                                        //script used to store and access maps
{
    [CreateAssetMenu(fileName = "New Map Set", menuName = "Rounds/Map Set")]            //define names when doing Create -> Rounds -> MapSet
    public class MapSet : ScriptableObject
    {
        [Scene]             //scene tag represents item as scenes
        [SerializeField] private List<string> maps = new List<string>();            //list that stores the maps 

        public IReadOnlyCollection<string> Maps => maps.AsReadOnly();               //read only collection of strings, so users can't modify maps
    }
}