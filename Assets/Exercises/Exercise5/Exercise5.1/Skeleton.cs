using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Exercise5
{

    [System.Serializable]
    public struct SkeletonJoint
    {
        public Matrix4x4 m_invBindPose;
        public string m_name;
        public byte m_iParent;
    }

    [System.Serializable]
    public class Skeleton 
    {
        public SkeletonJoint[] m_aJoint;

        /// <summary>
        /// Compute joint poses in model space given the skeleton hierarchy and 
        /// an animation pose (local space poses)
        /// </summary>
        /// <param name="animPose">animation pose with local space transforms</param>
        /// <param name="poseGlobal">output transformations in model space</param>
        public void GetPoseInGlobalSpace(AnimationPose animPose, ref Matrix4x4[] globalPose)
        {            
            // KEEP THIS check validity of the arguments
            int jointCount = m_aJoint.Length;
            if (animPose.m_aLocalPose.Length != jointCount)
                Debug.LogError("[Skeleton] the AnimationPose and Skeleton don't match");
            if (globalPose.Length != jointCount) globalPose = new Matrix4x4[jointCount];

            // TODO exercise 5.1
            // You should 
            // 1. iterate through all the joints, 
            // 2. use the hierarchy of joints to retrieve the local transformation of each joint in the animPose,
            // 3. concatenate the local transformations until you reach the root joint of the skeleton (m_iParent == 255)
            // 4. store the resultint matrix on poseGlobal, at the same index as the joint
            // REPLACE with the correct code (slide 31)
            for (int i = 0; i < jointCount; i++)
            {
                globalPose[i] = animPose.m_aLocalPose[i].Matrix();
            }

        }
    }
}
