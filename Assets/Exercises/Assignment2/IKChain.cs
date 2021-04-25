using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Exercises.Assignment2
{
    public class IKChain
    {
        // chains and joints
        public string Name => BaseJoint.gameObject.name + "-" + EndJoint.gameObject.name;
        public IKChain Parent { get; set; } = null;
        public List<IKChain> Children { get; } = new List<IKChain>();
        public List<IKJoint> Joints { get; } = new List<IKJoint>();
        
        // chain data
        public float Length { get; set; } = 0f;
        public List<float> Distances { get; set; } = new List<float>();
        public int BranchLevel { get; private set; } = 0;
        
        // useful accessors
        public IKJoint BaseJoint => Joints.First();
        public IKJoint EndJoint => Joints.Last();
        public Vector3 Base { get; set; } = Vector3.zero;
        public Vector3 Target { get; set; } = Vector3.zero;

        public static IKChain ParseChain(GameObject baseChain, IKChain parent = null)
        {
            var chain = new IKChain();
            if (parent != null)
            {
                chain.Parent = parent;
                chain.BranchLevel = parent.BranchLevel + 1;
                chain.Joints.Add(parent.EndJoint);
            }
            foreach (IKJoint joint in baseChain.GetComponentsInChildren<IKJoint>())
            {
                IKJoint prevJoint = null;
                if(chain.Joints.Count > 0)
                    prevJoint = chain.Joints.Last();
                chain.Joints.Add(joint);
                if(prevJoint) // calc distances for every joint after the root
                    chain.Distances.Add(Vector3.Distance(joint.Position, prevJoint.Position));
                if (joint.transform.childCount <= 2) continue; // keep going if there are no subchains
                foreach (IKJoint branch in joint.transform.Cast<Transform>().SelectMany(t => t.GetComponents<IKJoint>()))
                    chain.Children.Add(ParseChain(branch.gameObject, chain));
                break;
            }
            // calc total length of chain (recursively)
            chain.Length = chain.Distances.Sum();
            chain.Base = chain.BaseJoint.Position;
            Debug.Log("Parsed chain with root " + chain.BaseJoint.name + ", containing " + chain.Joints.Count + " joints.");
            if(chain.IsEndChain())
                Debug.Log("This is an end chain!");
            return chain;
        }

        public void Unreachable()
        {
            // first, we set the targets for each subchain
            if (!IsEndChain()) // our target will be the centroid of all the target positions in its children
                Target = Children.Aggregate(new Vector3(0, 0, 0), (v, c) => v + c.Target) / Children.Count;
            for (int i = 0; i < Joints.Count - 1; i++)
            {
                float unrDistance = Vector3.Distance(Joints[i].Position, Target);
                float unrDelta = Distances[i] / unrDistance;
                Joints[i + 1].Position = unrDelta * Target + (1 - unrDelta) * Joints[i].Position;
            }
        }

        public void Forward()
        {
            Vector3 subBasePos = BaseJoint.Position;
            if (IsOutOfReach())
                Unreachable();
            else
            {
                if (!IsEndChain())
                    Target /= Children.Count; // for a non end-chain, we take the averaged position of the "target" aka the new position of the sub-bases after the Forward pass
                EndJoint.Position = Target;
                for (int i = Joints.Count - 2; i == 0; i++)
                {
                    float fwdDistance = Vector3.Distance(Joints[i + 1].Position, Joints[i].Position);
                    float fwdDelta = fwdDistance / Distances[i];
                    Joints[i].Position = fwdDelta * Joints[i].Position + (1 - fwdDelta) * Joints[i + 1].Position;
                }  
            }
            // first we save the original BaseJoint position, since it will be changed to eventually calculate the centroid
            
            if (Parent != null) // add new sub-base position to the "target" of the parent chain
                Parent.Target += BaseJoint.Position;
            BaseJoint.Position = subBasePos; // restore the sub-base position
        }

        public void Backward()
        {
            if (!IsOutOfReach())
            {
                BaseJoint.Position = Base;
                if (!IsEndChain())
                    Target = Vector3.zero; // if sub-chain, reset target
                for (int i = 0; i < Joints.Count - 1; i++)
                {
                    float bwdDistance = Vector3.Distance(Joints[i + 1].Position, Joints[i].Position);
                    float bwdDelta = Distances[i] / bwdDistance;
                    Joints[i + 1].Position = bwdDelta * Joints[i + 1].Position + (1 - bwdDelta) * Joints[i].Position;
                }
            }
            Joints.ForEach(j => j.UpdateTransform()); // update transforms
            Children.ForEach(c => c.Backward());
        }

        public bool IsEndChain()
        {
            return Children.Count == 0;
        }

        public bool IsOutOfReach()
        {
            return Vector3.Distance(BaseJoint.Position, Target) > Length;
        }
    }
}