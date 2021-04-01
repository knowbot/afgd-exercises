// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// namespace Exercises.Assignment2
// {
//     public class MultiFABRIK : MonoBehaviour
//     {
//         [Tooltip("GameObject that is parent of all joint chains, contains the root joint")]
//         public GameObject rootObject;
//
//         private JointChain _jointChain;
//         
//         [Tooltip("The jointws that we are controlling")]
//         public List<Transform> joints;
//
//         [Tooltip("Targets that our end effectors are trying to reach")]
//         public List<Transform> targets;
//
//         [Tooltip("Error tolerance, will stop updating after distance between end effector and target is smaller than tolerance.")]
//         [Range(.01f, .2f)]
//         public float tolerance = 0.05f;
//
//         [Tooltip("Maximum number of iterations before we follow to the next frame")]
//         [Range(1, 100)]
//         public int maxIterations = 20;
//         //
//         // // [Tooltip("rotation constraint. " +
//         // // 	"Instead of an elipse with 4 rotation limits, " +
//         // // 	"we use a circle with a single rotation limit. " +
//         // // 	"Implementation will be a lot simpler than in the paper.")]
//         // [Range(0f, 180f)]
//         // public float rotationLimit = 45f;
//
//         // distances/lengths between joints.
//         private float[] _distances;
//         // total length of the system
//         private float _chainLength;
//         
//         private class JointChain
//         {
//             private int _endJointCount = 0;
//             private float _chainLength = 0f;
//             public List<float> Distances { get; set; } = new List<float>();
//             public List<Vector3> StartDirections { get; set; } = new List<Vector3>();
//             public List<Quaternion> StartRotations { get; set; } = new List<Quaternion>();
//             public static Transform Target { get; set; }
//             public Quaternion TargetStartRotation { get; set; } = new Quaternion();
//             public List<Transform> Joints { get; set; } =  new List<Transform>();
//             public JointChain Parent { get; set; }
//             public List<JointChain> Children { get; set; } = new List<JointChain>();
//
//             public Transform BaseJoint => Joints.First();
//             public Transform EndJoint => Joints.Last();
//
//             public Boolean IsEndChain => Joints.Last()?.childCount == 0;
//             public JointChain(JointChain parent = null)
//             {
//                 Parent = parent;
//             }
//             // parse the chain into the appropriate data structure
//             public JointChain ParseChain(GameObject chainObject, List<Transform> targets, JointChain parent = null)
//             { 
//                 Parent = parent;
//                 foreach (Transform child in chainObject.transform)
//                     if(child.name.Contains("Joint"))
//                         Joints.Add(child);
//                 for(int i = 0; i < Joints.Count; i++)
//                 {
//                     if (Joints[i] != EndJoint)
//                     {
//                         Vector3 vector = Joints[i + 1].position - Joints[i].position; 
//                         Distances[i] = vector.magnitude;
//                         StartDirections[i] = vector.normalized;
//                     }
//                     StartRotations[i] = Joints[i].rotation;
//                     // we only want to add branching chains to the children!
//                     // if child count = 0: just a part of the chain (if it's the last joint and has 
//                     // if child count > 0: new sub-base
//                     if(Joints[i].GetComponentsInChildren<Transform>().Count(c => c.name.Contains("Joint")) > 0) 
//                         Children.Add(ParseChain(Joints[i].gameObject, targets, this));
//                 }
//                 if (Children.Count != 0) return this;
//                 Target = targets[_endJointCount];
//                 TargetStartRotation = Target.rotation; 
//                 _endJointCount++;
//                 return this;
//             }
//         }
//         
//         // Start is called before the first frame update
//         void Awake()
//         {
//             // create empty chain and populate by parsing the root gameObject
//             _jointChain = new JointChain().ParseChain(rootObject, targets);
//         }
//
//         void Update()
//         {
//             Solve();
//             // for (int i = 1; i < joints.Length - 1; i++)
//             // {
//             //     DebugJointLimit(joints[i], joints[i - 1], rotationLimit, 2);
//             // }
//         }
//
//         // this implementation follows as closely as possible the pseudocode implementation described on page 245
//         private void Solve()
//         {
//             // get distance from root to target
//             float rootToTarget = Vector3.Distance(rootPosition, targetPosition);
//             if (rootToTarget > _distances.Sum()) // if target unreachable
//                 Unreachable(targetPosition, ref newJointPositions);
//             else
//             { // if target is reachable
//                 // check distance between end effector and target
//                 float endToTarget = Vector3.Distance(joints[endIndex].position, targetPosition);
//                 int iterations = 0;
//                 while (endToTarget > tolerance && iterations < maxIterations) // while distance is over the threshold
//                 {
//                     Forward(targetPosition, ref newJointPositions);
//                     Backward(rootPosition, ref newJointPositions);
//                     endToTarget = Vector3.Distance(newJointPositions[joints.Length - 1], targetPosition);
//                     iterations++;
//                 }
//             }
//             ApplyResults(newJointPositions);
//         }
//
//         private void Unreachable(Vector3 targetPosition, ref Vector3[] jointPositions)
//         {
//             for (int i = 0; i < jointPositions.Length - 1; i++) 
//             {
//                 // get the distance from joint to target
//                 float jointToTarget = Vector3.Distance(jointPositions[i], targetPosition);
//                 // calculate delta
//                 float delta = _distances[i] / jointToTarget;
//                 // compute new joint position for next joint
//                 jointPositions[i + 1] = (1 - delta) * jointPositions[i] + delta * targetPosition;
//             }
//         }
//
//         private void Forward(Vector3 targetPosition, ref Vector3[] jointPositions)
//         {
//             // FORWARD PASS
//             jointPositions[jointPositions.Length - 1] = targetPosition;
//             for (int i = jointPositions.Length - 2; i > 0; i--)
//             {
//                 float forwDist = Vector3.Distance(jointPositions[i + 1], jointPositions[i]);
//                 float delta = _distances[i] / forwDist;
//                 jointPositions[i] = (1 - delta) * jointPositions[i+1] + delta * jointPositions[i];
//             }
//         }
//
//         private void Backward(Vector3 rootPosition, ref Vector3[] jointPositions)
//         {
//             jointPositions[0] = rootPosition;
//             for (int i = 0; i < jointPositions.Length - 1; i++)
//             {
//                 float backDist = Vector3.Distance(jointPositions[i + 1], jointPositions[i]);
//                 float delta = _distances[i] / backDist;
//                 jointPositions[i+1] = (1 - delta) * jointPositions[i] + delta * jointPositions[i+1];
//             }
//         }
//
//         private void ApplyResults(IReadOnlyList<Vector3> jointPositions)
//         {
//             for (int i = 0; i < joints.Length; i++)
//             {
//                 joints[i].position = jointPositions[i];
//                 if (i == joints.Length - 1)
//                 {
//                     // rotate the last join according to target
//                     Quaternion offsetRotation = joints[i].rotation * Quaternion.Inverse(_targetStartRotation);
//                     joints[i].rotation = _targetStartRotation * offsetRotation;
//                     break;
//                 }
//                 // rotate other joints according to directions
//                 Vector3 newDirection = (jointPositions[i + 1] - jointPositions[i]).normalized;
//                 joints[i].rotation = _startRotations[i] * Quaternion.FromToRotation(_startDirections[i], newDirection);
//             }
//         }
//         
//         /// <summary>
//         /// Helper function to draw the joint limit in the editor
//         /// The drawing migh not make sense if you did not complete the 
//         /// second task in the assignment (joint rotations)
//         /// </summary>
//         /// <param name="tr">current joint</param>
//         /// <param name="trPrev">previous joint</param>
//         /// <param name="angle">angle limit in degrees</param>
//         /// <param name="scale"></param>
//         // void DebugJointLimit(Transform tr, Transform trPrev, float angle, float scale = 1)
//         // {
//         //     float angleRad = Mathf.Deg2Rad * angle;
//         //     float cosAngle = Mathf.Cos(angleRad);
//         //     float sinAngle = Mathf.Sin(angleRad);
//         //     int steps = 36;
//         //     float stepSize = 360f / steps;
//         //     // steps is the number of line segments used to draw the cone
//         //     for (int i = 0; i < steps; i++)
//         //     {
//         //         float twistRad = Mathf.Deg2Rad * i * stepSize;
//         //         Vector3 vec = new Vector3(cosAngle, 0, 0);
//         //         vec.y = Mathf.Cos(twistRad) * sinAngle;
//         //         vec.z = Mathf.Sin(twistRad) * sinAngle;
//         //         vec = trPrev.rotation * vec;
//         //         
//         //         twistRad = Mathf.Deg2Rad * (i+1) * stepSize;
//         //         Vector3 vec2 = new Vector3(cosAngle, 0, 0);
//         //         vec2.y = Mathf.Cos(twistRad) * sinAngle;
//         //         vec2.z = Mathf.Sin(twistRad) * sinAngle;
//         //         vec2 = trPrev.rotation * vec2;
//         //
//         //         Debug.DrawLine(tr.position, tr.position + vec * scale, Color.white);
//         //         Debug.DrawLine(tr.position + vec * scale, tr.position + vec2 * scale, Color.white);
//         //     }
//         // }
//     }
//
// }