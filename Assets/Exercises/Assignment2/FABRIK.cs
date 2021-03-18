using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Assignment2
{
    public class FABRIK : MonoBehaviour
    {
        [Tooltip("the joints that we are controlling")]
        public Transform[] joints;

        [Tooltip("target that our end effector is trying to reach")]
        public Transform target;

        [Tooltip("error tolerance, will stop updating after distance between end effector and target is smaller than tolerance.")]
        [Range(.01f, .2f)]
        public float tolerance = 0.05f;

        [Tooltip("maximum number of iterations before we follow to the next frame")]
        [Range(1, 100)]
        public int maxIterations = 20;

        [Tooltip("rotation constraint. " +
        	"Instead of an elipse with 4 rotation limits, " +
        	"we use a circle with a single rotation limit. " +
        	"Implementation will be a lot simpler than in the paper.")]
        [Range(0f, 180f)]
        public float rotationLimit = 45f;

        // distances/lengths between joints.
        private float[] distances;
        // total length of the system
        private float chainLength;


        private void Solve()
        {
            // TODO: YOUR IMPLEMENTATION HERE
            // FEEL FREE TO CREATE HELPER FUNCTIONS

        }

        // Start is called before the first frame update
        void Start()
        {
            // pre-compute segment lenghts and total length of the chain
            // we assume that the segment/bone length is constant during execution
            distances = new float[joints.Length-1];
            chainLength = 0;
            // If we have N joints, then there are N-1 segment/bone lengths connecting these joints
            for (int i = 0; i < joints.Length - 1; i++)
            {
                distances[i] = (joints[i + 1].position - joints[i].position).magnitude;
                chainLength += distances[i];
            }
        }

        void Update()
        {
            Solve();
            for (int i = 1; i < joints.Length - 1; i++)
            {
                DebugJointLimit(joints[i], joints[i - 1], rotationLimit, 2);
            }
        }

        /// <summary>
        /// Helper function to draw the joint limit in the editor
        /// The drawing migh not make sense if you did not complete the 
        /// second task in the assignment (joint rotations)
        /// </summary>
        /// <param name="tr">current joint</param>
        /// <param name="trPrev">previous joint</param>
        /// <param name="angle">angle limit in degrees</param>
        /// <param name="scale"></param>
        void DebugJointLimit(Transform tr, Transform trPrev, float angle, float scale = 1)
        {
            float angleRad = Mathf.Deg2Rad * angle;
            float cosAngle = Mathf.Cos(angleRad);
            float sinAngle = Mathf.Sin(angleRad);
            int steps = 36;
            float stepSize = 360f / steps;
            // steps is the number of line segments used to draw the cone
            for (int i = 0; i < steps; i++)
            {
                float twistRad = Mathf.Deg2Rad * i * stepSize;
                Vector3 vec = new Vector3(cosAngle, 0, 0);
                vec.y = Mathf.Cos(twistRad) * sinAngle;
                vec.z = Mathf.Sin(twistRad) * sinAngle;
                vec = trPrev.rotation * vec;
                
                twistRad = Mathf.Deg2Rad * (i+1) * stepSize;
                Vector3 vec2 = new Vector3(cosAngle, 0, 0);
                vec2.y = Mathf.Cos(twistRad) * sinAngle;
                vec2.z = Mathf.Sin(twistRad) * sinAngle;
                vec2 = trPrev.rotation * vec2;

                Debug.DrawLine(tr.position, tr.position + vec * scale, Color.white);
                Debug.DrawLine(tr.position + vec * scale, tr.position + vec2 * scale, Color.white);
            }
        }
    }

}