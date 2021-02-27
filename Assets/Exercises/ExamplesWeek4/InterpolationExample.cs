using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AfGD
{
    // This example provides a comparison between linear and spherical/circular
    // interpolation, and tries to demonstratewhat the consequence is of
    // using over the other to interpolate rotations
    public class InterpolationExample : MonoBehaviour
    {
        [Range(0, 360)]
        [Tooltip("rotation angle for demonstration")]
        public float angle = 90;
        // the vector we will be rotating
        Vector2 startVec = new Vector2(1, 0);
        // the rotated vector
        Vector2 endVec;

        // variables used for animation
        [Range(5, 90)]
        [Tooltip("rotation speed")]
        public float degreesPerSecond = 20;
        float currentAngle = 0;

        public Transform circularInterpExample;
        public Transform linearInterpExample;

        // called when script is loaded or a value is changed in the inspector 
        private void OnValidate()
        {
            // find the rotated vector
            float angleRad = Mathf.Deg2Rad * angle;
            endVec = new Vector2(Mathf.Cos(angleRad) * startVec.x - Mathf.Sin(angleRad) * startVec.y,
                          Mathf.Sin(angleRad) * startVec.x + Mathf.Cos(angleRad) * startVec.y);
        }

        private void OnDrawGizmos()
        {
            // draw the unit circle
            Handles.color = Color.white;
            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 1f, 2f);

            // draw vectors marking the start and end of the interpolation
            Handles.color = Color.yellow;
            Handles.DrawLine(Vector2.zero, startVec, 2f);
            Handles.DrawLine(Vector2.zero, endVec, 2f);

            // draw the interpolation trajectory for linear and circular/spherical interpolation
            Handles.color = Color.red;
            Handles.DrawLine(startVec, endVec, 2f);
            Handles.color = Color.green;
            Handles.DrawWireArc(Vector3.zero, Vector3.forward, startVec, Vector2.SignedAngle(startVec, endVec), 1f, 2f);

            GraphRotationSpeedFunctions();

            // we only animate if application is running
            if (!Application.isPlaying)
                return;

            // interpolation parameter in the range [0, 1]
            float parameter = currentAngle / angle;
            // compute linear and circular/spherical interpolation 
            // (we use Vector3 because Vector2 has no Slerp implementation)
            Vector2 lerpRot = Vector2.Lerp(startVec, endVec, parameter);
            Vector2 slerpRot = Vector3.Slerp(startVec, endVec, parameter);

            // build and apply rotation quaternions to represent the 2D rotations
            float lerpAngle = Vector2.SignedAngle(startVec, lerpRot) / 2f * Mathf.Deg2Rad;
            linearInterpExample.rotation = new Quaternion(0, 0, Mathf.Sin(lerpAngle), Mathf.Cos(lerpAngle));
            float slerpAngle = Vector2.SignedAngle(startVec, slerpRot) / 2f * Mathf.Deg2Rad;
            circularInterpExample.rotation = new Quaternion(0, 0, Mathf.Sin(slerpAngle), Mathf.Cos(slerpAngle));

            // draw vectors indicating the current state of the interpolation strategies
            Handles.color = Color.red;
            Handles.DrawLine(Vector3.zero, lerpRot.normalized, 2f);
            Handles.color = Color.green;
            Handles.DrawLine(Vector3.zero, slerpRot, 2f);
            // draw points indicating the current state of the interpolation strategies
            Handles.color = Color.red;
            Handles.DrawSolidDisc(lerpRot, Vector3.forward, 0.05f);
            Handles.DrawSolidDisc(lerpRot.normalized, Vector3.forward, 0.05f);
            Handles.color = Color.green;
            Handles.DrawSolidDisc(slerpRot, Vector3.forward, 0.05f);

            // draw local x, y and z axis on the interpolation examples
            DebugDraw.DrawFrame(circularInterpExample.localToWorldMatrix);
            DebugDraw.DrawFrame(linearInterpExample.localToWorldMatrix);

            // step animation
            currentAngle += degreesPerSecond * Time.deltaTime;

            // restart animation
            if (currentAngle > angle)
                currentAngle = 0;
        }

        /// <summary>
        /// this function draws a graph in the top right corner
        /// the graph represents a transfer function, exactly like 
        /// the ones we see when dealing with easing functions
        /// </summary>
        void GraphRotationSpeedFunctions()
        {
            float lastSlerpAngle = 0;
            float lastLerpAngle = 0;

            Handles.matrix = Matrix4x4.Translate(new Vector3(1.1f, 0f, 0));
            Handles.color = Color.white;
            Handles.DrawLine(Vector2.zero, Vector2.right, 2);
            Handles.DrawLine(Vector2.zero, Vector2.up, 2);

            for (int i = 1; i <= 50; i++)
            {
                // interpolation parameter in the range [0, 1]
                float parameter = (i / 50f);
                // compute linear and circular/spherical interpolation 
                // (we use Vector3 because Vector2 has no Slerp implementation)
                Vector2 lerpRot = Vector2.Lerp(startVec, endVec, parameter);
                Vector2 slerpRot = Vector3.Slerp(startVec, endVec, parameter);

                // compute absolute rotation angles resulting from the interpolation
                // and normalize it to 1 for drawing
                float lerpAngle = Vector2.Angle(startVec, lerpRot) / Vector2.Angle(startVec, endVec);
                float slerpAngle = Vector2.Angle(startVec, slerpRot) / Vector2.Angle(startVec, endVec);

                Handles.color = Color.red;
                Handles.DrawLine(new Vector2((i - 1) / 50f, lastLerpAngle), new Vector2(i / 50f, lerpAngle), 2);
                Handles.color = Color.green;
                Handles.DrawLine(new Vector2((i - 1) / 50f, lastSlerpAngle), new Vector2(i / 50f, slerpAngle), 2);

                lastLerpAngle = lerpAngle;
                lastSlerpAngle = slerpAngle;
            }
            Handles.matrix = Matrix4x4.identity;

        }

    }
}