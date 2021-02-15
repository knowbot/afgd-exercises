using System;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Execise3
{
    public static class BreadthFirstSearch
    {
        // Exercise 3.1 - Implement Breadth first search. 
        // Explore the graph and fill the _cameFrom_ dictionary with data using breadth first search.
        // PathFinding.ReconstructPath() will use the data in cameFrom to reconstruct 
        // a path between the start node and end node. 
        //
        // Notes:      
        //  To get the neighbours (connected nodes) of a specific node use:
        //      Graph.GetNeighbours(Node node, List<Node> neighbours)
        //      This function will fill the passed in List<Node> with the neighbours of node.
        //
        //  Consider using the following data structures for your implementation:
        //      List<T>, https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=net-5.0
        //      Queue<T>, https://docs.microsoft.com/en-us/dotnet/api/system.collections.queue?view=net-5.0
        //
        public static void Execute(Graph graph, Node start, Node goal, Dictionary<Node, Node> cameFrom)
        {
            Queue<Node> frontier = new Queue<Node>();
            frontier.Enqueue(start);
            while (frontier.Count != 0)
            {
                Node curr = frontier.Dequeue();
                if (curr == goal)
                    break;
                List<Node> neighbours = new List<Node>();
                graph.GetNeighbours(curr, neighbours);
                foreach (var next in neighbours)
                {
                    if (!cameFrom.ContainsKey(next))
                    {
                        cameFrom[next] = curr;
                        frontier.Enqueue(next);
                    }
                }
            }
            
        }
    }
}