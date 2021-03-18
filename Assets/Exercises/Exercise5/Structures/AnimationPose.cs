using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Exercise5
{
    [System.Serializable]
    public struct JointPose
    {
        public Quaternion m_rot;
        public Vector3 m_pos;
        public float m_scale;

        public Matrix4x4 Matrix()
        {
            return Matrix4x4.TRS(m_pos, m_rot, Vector3.one * m_scale);
        }
    }

    [System.Serializable]
    public struct AnimationPose
    {
        public JointPose[] m_aLocalPose;
    }
}