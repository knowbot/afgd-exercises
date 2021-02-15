using System;
using UnityEngine;

namespace AfGD.Execise3
{
    //A Node representation
    [Serializable]
    public class Node
    {
        // The Position of this Node
        public Vector3 Position => m_Position;

        // A label for this Node
        public string Name => m_Name;

        // Feel free to add any useful information
        // associated with a Node.

        // Disable field is never assigned warnings
#pragma warning disable 649
        [SerializeField] string m_Name;
        [SerializeField] Vector3 m_Position;
#pragma warning restore 649
    }
}