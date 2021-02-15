using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace AfGD.Execise3
{
    // This attribute adds this ScriptableObject to the right click Create menu in the Project Window
    [CreateAssetMenu(fileName = "Graph", menuName = "Exercice 3/GraphScriptableObject", order = 1)]
    public class Graph : ScriptableObject
    {
        // Disable field is never assigned warnings
#pragma warning disable 649
        // An array that stores the Nodes in our graphs
        [SerializeField] Node[] m_Nodes;

        // An array that stores the connections between
        // the nodes in our graph.
        [SerializeField] Edge[] m_Edges;
#pragma warning restore 649

        // A list to keep track of explored nodes for debug 
        // purposes. This list is cleared every execution.
        readonly List<Node> m_ExploredNodes = new List<Node>();

        // Returns the closest node to a point
        public Node GetClosestNodeToPoint(Vector3 point)
        {
            var minSqrDist = float.MaxValue;
            Node closestNode = null;

            // Loop over all our nodes
            for (int i = 0; i < m_Nodes.Length; i++)
            {
                var node = m_Nodes[i];
                var pointToNode = node.Position - point;

                // If the distance between the point and this node
                // is smaller than the smallest distance encountered
                // thus far we keep this node. 
                if (pointToNode.sqrMagnitude < minSqrDist)
                {
                    closestNode = node;
                    minSqrDist = pointToNode.sqrMagnitude;
                }
            }

            Assert.IsNotNull(closestNode);

            // Return the closets node we have found
            return closestNode;
        }

        // Returns whether a node is part of this graph.
        public bool Contains(Node node)
        {
            // null is not part of our graph
            if (node == null)
                return false;

            // Loop over the nodes and return true 
            // if we encounter the node
            for (int i = 0; i < m_Nodes.Length; i++)
            {
                if (m_Nodes[i] == node)
                    return true;
            }

            // Return false if we have not found anything
            return false;
        }

        // Returns the cost to go from a node to another node.
        // The nodes need to be neighbours.
        public float GetCost(Node from, Node to)
        {
            // It is free to travel from a node to itself
            if (from == to)
                return 0f;

            // Loop over the edges and find the edge
            // that connect from and to
            for (int i = 0; i < m_Edges.Length; i++)
            {
                var edge = m_Edges[i];
                if (m_Nodes[edge.From] == from && m_Nodes[edge.To] == to)
                    return edge.Cost;
            }

            // Return a really high cost if we have not 
            // found and edge that connects from and to
            return float.MaxValue;
        }

        // Returns the neigbours that are connected 
        // to a node by an edge.
        public void GetNeighbours(Node node, List<Node> neighours)
        {
            Assert.IsNotNull(neighours, "Cannot return the neighbours for this node. The List<T> in which the neighbours will be put is null!");
            Assert.IsTrue(Contains(node), "Cannot return neighbours for this node. The node is not part of this Graph!");

            // Clear any potential previous results
            neighours.Clear();

            // If the graph does not have any edges
            // This node does not have any either
            if (m_Edges == null)
                return;

            // Loop over edges and any nodes to 
            // the result where from is equal to 
            // the index of the node passed in
            var index = GetIndex(node);
            foreach (var edge in m_Edges)
            {
                if (edge.From == index)
                {
                    var neighour = m_Nodes[edge.To];

                    // Add the neighbour to the result
                    if (!neighours.Contains(node))
                        neighours.Add(neighour);

                    // Add the neighbour to the encountered nodes
                    if (!m_ExploredNodes.Contains(neighour))
                        m_ExploredNodes.Add(neighour);
                }
            }
        }

        // Returns the index of a node
        int GetIndex(Node node)
        {
            Assert.IsNotNull(node);

            // Loop over the nodes until we have found
            // the node we are looking for
            for (int i = 0; i < m_Nodes.Length; i++)
            {
                if (m_Nodes[i] == node)
                    return i;
            }

            // Return -1 if we have not found anything
            return -1;
        }

        void OnValidate()
        {
            Clear();

            if (m_Nodes == null || m_Edges == null)
                return;

            // Make sure that all the nodes our edges connect
            // are part of nodes contained in this graph
            foreach (var edge in m_Edges)
            {
                Assert.IsTrue(edge.IsValid(), "Graph contains an invalid edge!");
                Assert.IsTrue(edge.From >= 0 && edge.From < m_Nodes.Length, "Graph contains an edge from a node that does not exist!");
                Assert.IsTrue(edge.To >= 0 && edge.To < m_Nodes.Length, "Graph contains an edge to a node that does not exist!");
                Assert.IsFalse(edge.From == edge.To, "Graph contains an edge that loops to itself!");
            }
        }

        // Draws all the debug visuals for both 
        // nodes and edges inside the SceneView
        public void DebugDraw()
        {
            if (m_Nodes == null || m_Edges == null)
                return;

            // Draw nodes
            foreach (var node in m_Nodes)
            {
                if (node == null)
                    continue;

                var color = Handles.color;
                Handles.color = Color.white;
                Handles.DrawWireDisc(node.Position, Vector3.up, .3f);
                Handles.color = color;

                Handles.Label(node.Position, new GUIContent(node.Name));
            }

            // Draw explored Nodes
            foreach (var node in m_ExploredNodes)
            {
                if (node == null)
                    continue;

                var color = Handles.color;
                Handles.color = Color.blue;
                Handles.DrawWireDisc(node.Position, Vector3.up, .33f);
                Handles.color = color;
            }

            // Draw edges
            foreach (var edge in m_Edges)
            {
                // Skip the node if it is not valid.
                if (!edge.IsValid())
                    continue;

                var fromPos = m_Nodes[edge.From].Position;
                var toPos = m_Nodes[edge.To].Position;
                var fromToDir = (toPos - fromPos).normalized;

                // Draw a line from start node to destination node.
                Debug.DrawLine(fromPos + .3f * fromToDir, toPos - .3f * fromToDir);

                var offset = Vector3.Cross(fromToDir, Vector3.up);

                // Draw the cost of the connection in the middle between start and destination.
                Handles.Label(0.5f * (fromPos + toPos) + offset * .1f, new GUIContent(edge.Cost.ToString()));
            }
        }

        // Clears the explored nodes list 
        public void Clear()
        {
            m_ExploredNodes.Clear();
        }
    }
}