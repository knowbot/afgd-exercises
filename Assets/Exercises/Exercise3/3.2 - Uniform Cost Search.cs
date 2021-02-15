using System;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Execise3
{
    public static class UniformCostSearch
    {
        // Exercise 3.2 - Implement uniform cost search (or Dijkstra's algorithm)
        // Explore the graph and fill the _cameFrom_ dictionairy with data using uniform cost search.
        // Similar to Exercise 3.1 PathFinding.ReconstructPath() will use the data in cameFrom  
        // to reconstruct a path between the start node and end node. 
        //
        // Notes:
        //  To get the cost of visiting a certain neighbour use: 
        //      Graph.GetCost(Node from, Node to)
        //      This function will search for the corresponding edge between form and to and return its cost.
        //
        //  Consider using the following data structures next to those from Exercise 3.1:
        //  - Dictionary<TKey,TValue>, https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-5.0
        //  - The PriorityQueue implemenetation in PriorityQueue.cs:
        //   
        //      A new PriorityQueue can be created as follows:
        //          var priorityQueue = new PriorityQueue<Node>()
        //      
        //      Add an entry to the PriorityQueue as follows:
        //          priorityQueue.Enqueue(node, priority); // This will add the entry node to the queue accordition to the priority value (float)
        //          
        //      Subsequently entries can be popped from the PriorityQueue as follows:
        //          var node = priorityQueue.Dequeue(); // This will take the next node of the queue with the *lowest* priority value
        //
        public static void Execute(Graph graph, Node startPoint, Node endPoint, Dictionary<Node, Node> cameFrom)
        {
            throw new NotImplementedException("Implement UniformCostSearch search algorithm here.");
        }
    }
}