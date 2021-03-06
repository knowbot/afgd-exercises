using System;
using System.Collections.Generic;
using UnityEngine;
using AfGD.Execise3;

namespace AfGD.Assignment1
{
    public static class AStarSearch
    {
        // Exercise 3.3 - Implement A* search
        // Explore the graph and fill the _cameFrom_ dictionairy with data using uniform cost search.
        // Similar to Exercise 3.1 PathFinding.ReconstructPath() will use the data in cameFrom  
        // to reconstruct a path between the start node and end node. 
        //
        // Notes:
        //      Use the data structures used in Exercise 3.1 and 3.2
        //
        
        private static float HeuristicDistance(Node startPoint, Node endPoint)
        {
            // Heuristic is adjusted to the order of magnitude of the costs
            return (Math.Abs(startPoint.Position.x - endPoint.Position.x) + Math.Abs(startPoint.Position.z - endPoint.Position.z)) * 100;
        }
        
        public static void Execute(Graph graph, Node startPoint, Node endPoint, Dictionary<Node, Node> cameFrom)
        {
            PriorityQueue<Node> frontier = new PriorityQueue<Node>();
            Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
            frontier.Enqueue(startPoint, 0);
            costSoFar[startPoint] = 0;
            while (frontier.Count > 0)
            {
                Node curr = frontier.Dequeue();
                if (curr == endPoint)
                    break;
                Debug.Log("Visiting " + curr.Name);
                List<Node> neighbours = new List<Node>();
                graph.GetNeighbours(curr, neighbours);
                foreach (Node next in neighbours)
                {
                    float newCost = graph.GetCost(curr, next) + costSoFar[curr];
                    if (costSoFar.ContainsKey(next) && !(costSoFar[next] > newCost)) continue;
                    costSoFar[next] = newCost;
                    cameFrom[next] = curr;
                    float priority = newCost + HeuristicDistance(next, endPoint);
                    frontier.Enqueue(next, priority);
                    
                    Debug.Log("Added " + next.Name + " with priority " + priority);
                }
            }
        }

    }
}