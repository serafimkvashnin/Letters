using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Cards
{
    [Serializable]
    public class CardData
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private Sprite sprite;

        [SerializeField]
        private Vector3 rotation;

        public string Name => name;

        public Sprite Sprite => sprite; 

        public Vector3 Rotation => rotation;
    }
}
