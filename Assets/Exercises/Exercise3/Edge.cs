using System;

namespace AfGD.Execise3
{
    // Struct representing a connection
    // between two nodes in a graph.
    [Serializable]
    public struct Edge
    {
        // Index into the Node array of the graph.
        public int From;

        // Index into the Node array of the graph.
        public int To;

        // The cost associated with this Edge.
        public float Cost;

        // Returns whether this Edge 
        // is valid to be used
        public bool IsValid()
        {
            return From >= 0
                && To >= 0
                && Cost >= 0f;
        }
    }
}