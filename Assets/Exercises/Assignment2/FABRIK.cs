using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Exercises.Assignment2
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
        private float[] _distances;
        // total length of the system
        private float _chainLength;

        private Vector3[] _startDirections;
        private Quaternion[] _startRotations;
        private Quaternion _targetStartRotation;
        
        // this implementation follows as closely as possible the pseudocode implementation described on page 245
        private void Solve()
        {
            int endIndex = joints.Length - 1;
            // extract positions to array to store them
            Vector3[] newJointPositions = joints.Select(j => j.position).ToArray();
            Vector3 targetPosition = target.position;
            Vector3 rootPosition = joints[0].position;
            // get distance from root to target
            float rootToTarget = Vector3.Distance(rootPosition, targetPosition);
            if (rootToTarget > _distances.Sum()) // if target unreachable
                Unreachable(targetPosition, ref newJointPositions);
            else
            { // if target is reachable
                // check distance between end effector and target
                float endToTarget = Vector3.Distance(joints[endIndex].position, targetPosition);
                int iterations = 0;
                while (endToTarget > tolerance && iterations < maxIterations) // while distance is over the threshold
                {
                    Forward(targetPosition, ref newJointPositions);
                    Backward(rootPosition, ref newJointPositions);
                    endToTarget = Vector3.Distance(newJointPositions[joints.Length - 1], targetPosition);
                    iterations++;
                }
            }
            ApplyResults(newJointPositions);
        }

        private void Unreachable(Vector3 targetPosition, ref Vector3[] jointPositions)
        {
            for (int i = 0; i < jointPositions.Length - 1; i++) 
            {
                // get the distance from joint to target
                float jointToTarget = Vector3.Distance(jointPositions[i], targetPosition);
                // calculate delta
                float delta = _distances[i] / jointToTarget;
                // compute new joint position for next joint
                jointPositions[i + 1] = (1 - delta) * jointPositions[i] + delta * targetPosition;
            }
        }

        private void Forward(Vector3 targetPosition, ref Vector3[] jointPositions)
        {
            // FORWARD PASS
            jointPositions[jointPositions.Length - 1] = targetPosition;
            for (int i = jointPositions.Length - 2; i > 0; i--)
            {
                // ApplyConstraints(i, ref jointPositions);
                float forwDist = Vector3.Distance(jointPositions[i + 1], jointPositions[i]);
                float delta = _distances[i] / forwDist;
                jointPositions[i] = (1 - delta) * jointPositions[i+1] + delta * jointPositions[i];
            }
        }

        private void Backward(Vector3 rootPosition, ref Vector3[] jointPositions)
        {
            jointPositions[0] = rootPosition;
            for (int i = 0; i < jointPositions.Length - 1; i++)
            {
                if(i != 0)
                    ApplyConstraints(i, ref jointPositions);
                float backDist = Vector3.Distance(jointPositions[i + 1], jointPositions[i]);
                float delta = _distances[i] / backDist;
                jointPositions[i+1] = (1 - delta) * jointPositions[i] + delta * jointPositions[i+1];
            }
        }

        private void ApplyConstraints(int i, ref Vector3[] jointPositions)
        {
            // l
            Vector3 bonePrev = jointPositions[i] - jointPositions[i - 1];
            //ln
            Vector3 boneNext = jointPositions[i + 1] - jointPositions[i];
            float angle = Mathf.Acos(Vector3.Dot(boneNext, bonePrev) / (boneNext.magnitude * bonePrev.magnitude));
            if (angle * Mathf.Rad2Deg < rotationLimit)
                return;
            //o
            Vector3 projection = Vector3.Project(boneNext, bonePrev);
            // check if angle > 90
            if (Vector3.Dot(bonePrev, projection) < 0f && rotationLimit <= 90)
                projection *= -1;

            //Po
            Vector3 projectedP = jointPositions[i] + projection;
            //d
            Vector3 direction = (jointPositions[i + 1] - projectedP).normalized;
            //r
            float constraintRadius = Mathf.Abs(projection.magnitude * Mathf.Tan(Mathf.Deg2Rad * rotationLimit));
            //Pn'
            jointPositions[i + 1] = projectedP + direction * constraintRadius;
        }

        private void ApplyResults(IReadOnlyList<Vector3> jointPositions)
        {
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].position = jointPositions[i];  
                if (i == joints.Length - 1)
                {
                    // rotate the last join according to target
                    Quaternion offsetRotation = joints[i].rotation * Quaternion.Inverse(_targetStartRotation);
                    joints[i].rotation = _targetStartRotation * offsetRotation;
                    break;
                }
                // rotate other joints according to directions
                Vector3 newDirection = (jointPositions[i + 1] - jointPositions[i]).normalized;
                joints[i].rotation = _startRotations[i] * Quaternion.FromToRotation(_startDirections[i], newDirection);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // pre-compute segment lenghts and total length of the chain
            // we assume that the segment/bone length is constant during execution
            _distances = new float[joints.Length-1];
            _chainLength = 0;
            // If we have N joints, then there are N-1 segment/bone lengths connecting these joints
            for (int i = 0; i < joints.Length - 1; i++)
            {
                _distances[i] = (joints[i + 1].position - joints[i].position).magnitude;
                _chainLength += _distances[i];
            }
            
            _targetStartRotation = target.rotation;
            _startDirections = new Vector3[joints.Length - 1];
            for (int i = 0; i < _startDirections.Length; i++)
                _startDirections[i] = (joints[i + 1].position - joints[i].position).normalized;
            _startRotations = joints.Select(j => j.rotation).ToArray();
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