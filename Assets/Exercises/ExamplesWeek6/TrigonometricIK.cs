using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Examples6
{
    // this implementation only works in the x/y plane
    // it assumes that a segment with angle 0 points to +x-axis, and +90 points to +y-axis
    public class TrigonometricIK : MonoBehaviour
    {
        [Tooltip("the joints that we are controlling")]
        public Transform topJoint, midJoint, endJoint;
        [Tooltip("target that our endJoint is trying to reach")]
        public Transform target;
        [Tooltip("there are two possible solutions to the 3 joint IK problem on the plane")]
        public bool alternateSolution = false;

        // rotation angle to be used for rotating top and middle joints
        private float topAngle = 0, midAngle = 0;

        public void Solve()
        {
            // TODO: CLASS CODE HERE            

        }

        void Update()
        {
            Solve();
        }
    }

}