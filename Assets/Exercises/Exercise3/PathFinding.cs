using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

namespace AfGD.Execise3
{
    public class PathFinding : MonoBehaviour
    {
        // Various path finding algorithms we can use.
        enum PathFindingAlgorithm
        {
            BreadFirstSearch,
            UniformCostSearch,
            AStar,
        }

        // Disable field is never assigned warnings
#pragma warning disable 649
        // The graph we execute our algorithm on.
        [SerializeField] Graph m_Graph;

        // The path finding algorithm we have selected.
        [SerializeField] PathFindingAlgorithm m_Algorithm;

        // The position where we start.
        [SerializeField] Vector3 m_Start = Vector3.zero;

        // The desired position where end.
        [SerializeField] Vector3 m_End = new Vector3(9f, 0f, 9f);
#pragma warning restore 649

        // List of positions that store the result
        // of the path we have found.
        List<Vector3> m_Path = new List<Vector3>();

        public void Run()
        {
            if (m_Graph == null)
            {
                Debug.LogError("The assigned graph is either not assigned or invalid.");
                return;
            }

            // Clear state of previous run
            m_Path.Clear();
            m_Graph.Clear();

            // Convert start and end locations to nodes in the graph
            var start = m_Graph.GetClosestNodeToPoint(m_Start);
            var goal = m_Graph.GetClosestNodeToPoint(m_End);

            if(start == goal)
            {
                Debug.LogError("Start and goal are the same node. No path was found.");
                return;
            }    

            // Create a dictionairy to keep track of how we 
            // discovered a node.
            var cameFrom = new Dictionary<Node, Node>();

            // Execute the algorithm we have selected.
            switch (m_Algorithm)
            {
                case PathFindingAlgorithm.AStar:
                    AStarSearch.Execute(m_Graph, start, goal, cameFrom);
                    break;
                case PathFindingAlgorithm.BreadFirstSearch:
                    BreadthFirstSearch.Execute(m_Graph, start, goal, cameFrom);
                    break;
                case PathFindingAlgorithm.UniformCostSearch:
                    UniformCostSearch.Execute(m_Graph, start, goal, cameFrom);
                    break;
            }

            // Reconstruct Path
            ReconstructPath(cameFrom, start, goal, m_Path);
        }

        // This function reconstructs a path from StartPoint to EndPoint
        // using the cameFrom dictionairy. The cameFrom dictionairy should
        // be filled with the optimal path to go from a node to another. 
        static void ReconstructPath(Dictionary<Node, Node> cameFrom, Node startPoint, Node endPoint, List<Vector3> path)
        {
            // Start at our goal
            var current = endPoint;

            // Keep backtracking until we have found the start
            while (current != startPoint)
            {
                // Add the current node to our path.
                path.Add(current.Position);

                // Validate that we know how we got to the current Node
                Assert.IsTrue(cameFrom.ContainsKey(current), $"Path cannot be reconstructed as the Node {current.Name} cannot be found in the cameFrom dictionairy.");

                // Backtrack to the Node we used to get to the 
                // current node.
                current = cameFrom[current];
            }

            // Add the start point to the path
            path.Add(startPoint.Position);

            // Flip the List so that the first element 
            // is start and the last element is the goal
            path.Reverse();
        }

        void OnDrawGizmos()
        {
            // Draw the graph visualizaiton.
            if (m_Graph != null)
                m_Graph.DebugDraw();

            // Draw the path if we have any.
            if (m_Path != null && m_Path.Count > 1)
            {
                for (int i = 1; i < m_Path.Count; i++)
                    Debug.DrawLine(m_Path[i - 1], m_Path[i], Color.yellow);
            }

            var handleColor = Handles.color;

            // Draw Start in Green
            Handles.color = Color.green;
            Handles.DrawWireDisc(m_Start, Vector3.up, .25f);
            Handles.DrawWireDisc(m_Start, Vector3.up, .35f);
            Handles.Label(m_Start, new GUIContent("Start"));

            // Draw goal in Red
            Handles.color = Color.red;
            Handles.DrawWireDisc(m_End, Vector3.up, .25f);
            Handles.DrawWireDisc(m_End, Vector3.up, .35f);
            Handles.Label(m_End, new GUIContent("End"));

            Handles.color = handleColor;
        }

        void OnValidate()
        {
            if (m_Path != null)
                m_Path.Clear();

            if (m_Graph != null)
                m_Graph.Clear();
        }
    }
}