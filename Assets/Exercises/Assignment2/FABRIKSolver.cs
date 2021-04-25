using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Exercises.Assignment2
{
    [Serializable]
    public class EndChainAndTargetPair
    {
        [HideInInspector] public string name;
        public Transform target;

        public EndChainAndTargetPair(IKChain chain, Transform target)
        {
            this.name = chain.Name;
            this.target = target;
        }
    }
    
    public class FABRIKSolver : MonoBehaviour
    {
        [Tooltip("GameObject that is parent of all joint chains, contains the root joint")]
        public GameObject rootObject;

        private IKChain _rootChain;
        private List<IKChain> _chains = new List<IKChain>();
        private List<IKChain> _endChains = new List<IKChain>();

        [Tooltip("Targets that our end effectors are trying to reach")]
        public List<EndChainAndTargetPair> endChainAndTargetPairs = new List<EndChainAndTargetPair>();

        [Tooltip("Error tolerance, will stop updating after distance between end effector and target is smaller than tolerance.")]
        [Range(.01f, .2f)]
        public float tolerance = 0.05f;

        [Tooltip("Maximum number of iterations before we follow to the next frame")]
        [Range(1, 100)]
        public int maxIterations = 20;
        //
        // // [Tooltip("rotation constraint. " +
        // // 	"Instead of an elipse with 4 rotation limits, " +
        // // 	"we use a circle with a single rotation limit. " +
        // // 	"Implementation will be a lot simpler than in the paper.")]
        // [Range(0f, 180f)]
        // public float rotationLimit = 45f;
        
        // Start is called before the first frame update
        private void Start()
        {
            // LoadChain();
            // LoadTargets();
        }

        [ContextMenu("Load Chain")] 
        private void LoadChain()
        {
            // clear out lists
            _chains.Clear();
            _endChains.Clear();
            endChainAndTargetPairs.Clear();
            
            _rootChain = IKChain.ParseChain(rootObject);
            Unravel(_rootChain);
            _chains.Sort((x, y) => y.BranchLevel.CompareTo(x.BranchLevel));
            _endChains.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
            foreach (IKChain endChain in _endChains)
            {
                endChainAndTargetPairs.Add(new EndChainAndTargetPair(endChain, null));
            }
        }

        [ContextMenu("Make Target Pairs")]
        private void LoadTargets() 
        {
            foreach (EndChainAndTargetPair pair in endChainAndTargetPairs)
            {
                IKChain e = _endChains.Find(c => c.Name == pair.name);
                if (e != null)
                    e.Target = pair.target.position;
            }
        }
        
        
        
        private void Unravel(IKChain chain)
        {
            _chains.Add(chain);
            if(chain.IsEndChain())
                _endChains.Add(chain);
            chain.Children.ForEach(Unravel);
        }

        void Update()
        {
            // Solve();
            // for (int i = 1; i < joints.Length - 1; i++)
            // {
            //     DebugJointLimit(joints[i], joints[i - 1], rotationLimit, 2);
            // }
        }
        
        [ContextMenu("Solve")] 
        private void Solve()
        {
            int iterations = 0;
            while (iterations < maxIterations) // while distance is over the threshold
            {
               foreach(IKChain chain in _chains)
                   chain.Forward();
               _rootChain.Backward();
               iterations++;
            }
            
        }
    }

}