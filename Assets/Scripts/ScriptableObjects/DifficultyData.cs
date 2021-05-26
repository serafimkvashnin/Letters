using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "New DifficultyData", menuName = "Difficulty Data", order = 9)]
    public class DifficultyData : ScriptableObject
    {
        [SerializeField]
        private string difficultyName;

        [SerializeField]
        private int cardsToSpawn;

        public string DifficultyName => difficultyName;
        
        public int CardsToSpawn => cardsToSpawn;  
    }
}
