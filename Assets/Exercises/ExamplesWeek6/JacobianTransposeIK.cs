using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Examples6
{
    public class JacobianTransposeIK : MonoBehaviour
    {
        [Tooltip("the joints that we are controlling")]
        public Transform joint1, joint2, joint3, joint4, jointEnd;
        [Tooltip("target that our end effector is trying to reach")]
        public Transform target;

        [Tooltip("scalar used to scale down the update rate")]
        [Range(0.01f, 0.2f)]
        public float alpha = .05f;

        [Tooltip("error tolerance, will stop updating after distance between end effector and target is smaller than tolerance.")]
        [Range(0.01f, 0.5f)]
        public float tolerance = .05f;

        [Tooltip("maximum number of iterations before we follow to the next frame")]
        [Range(1, 100)]
        public int maxIterations = 20;

        [Range(0, 180)]
        public float rotationLimit = 50f;

        void Solve()
        {
            float error = (target.position - jointEnd.position).magnitude;
            int iterations = 0;
            // every frame, iterate until we reach a good approximation or exceed the
            // maximum number of iterations
            while (error > tolerance && iterations < maxIterations)
            {
                // CLASS CODE HERE


                error = (target.position - jointEnd.position).magnitude;
                iterations++;
            }
        }

        // Update is called once per frame
        void Update()
        {
            Solve();
        }


    }

}