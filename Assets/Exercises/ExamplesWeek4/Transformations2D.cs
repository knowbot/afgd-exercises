using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AfGD
{

    // In this example we will see how a translation in 2D is a shear operation in 3D
    public class Transformations2D : MonoBehaviour
    {
        // variables to store the transformations
        public float angle = 0;

        public float scaleX = 1;
        public float scaleY = 1;

        public float translationX = 0;
        public float translationY = 0;

        Matrix4x4 transformation = Matrix4x4.identity;

        public bool drawCubeInHomogeneousSpace = true;

        // Called when script is loaded or a value is changed in the inspector 
        private void OnValidate()
        {
            // create a rotation, translation and scale transformation
            Matrix4x4 R = Matrix4x4.identity;
            float angleRad = Mathf.Deg2Rad * angle;
            R.m00 = Mathf.Cos(angleRad);
            R.m10 = Mathf.Sin(angleRad);
            R.m01 = -Mathf.Sin(angleRad);
            R.m11 = Mathf.Cos(angleRad);

            // we will see how a translation in 2D is just a shear operation in 3D
            Matrix4x4 T = Matrix4x4.identity;
            T.m02 = translationX;
            T.m12 = translationY;

            Matrix4x4 S = Matrix4x4.identity;
            S.m00 = scaleX;
            S.m11 = scaleY;

            // we concatenate the transformations into a single matrix
            // notices that, since we use column vectors, 
            // transformations are applied from right to left
            // that means first Scale -> then Rotation -> and Translation last
            transformation = T * R * S;
        }


        void OnDrawGizmos()
        {
            // draw part of a grid to indicate the plane with no shear (z==0)
            Handles.color = Color.red;
            Handles.DrawDottedLine(new Vector3(-2f, -.5f, 0f), new Vector3(2f, -.5f, 0f), 2f);
            Handles.DrawDottedLine(new Vector3(-2f, .5f, 0f), new Vector3(2f, .5f, 0f), 2f);
            Handles.DrawDottedLine(new Vector3(-.5f, -2f, 0f), new Vector3(-.5f, 2f, 0f), 2f);
            Handles.DrawDottedLine(new Vector3(.5f, -2f, 0f), new Vector3(.5f, 2f, 0f), 2f);

            // draw part of a grid to indicate the euclidian plane (z==1)
            Handles.color = Color.green;
            Handles.DrawDottedLine(new Vector3(-2f, -.5f, 1f), new Vector3(2f, -.5f, 1f), 1f);
            Handles.DrawDottedLine(new Vector3(-2f, .5f, 1f), new Vector3(2f, .5f, 1f), 1f);
            Handles.DrawDottedLine(new Vector3(-.5f, -2f, 1f), new Vector3(-.5f, 2f, 1f), 1f);
            Handles.DrawDottedLine(new Vector3(.5f, -2f, 1f), new Vector3(.5f, 2f, 1f), 1f);

            // we apply the transformation, now all our Gizmos.Draw... calls will use this matrix
            Gizmos.matrix = transformation;

            // Draw a cube to show the effect of shearing in the range [0, 1] of values of 'z'
            if (drawCubeInHomogeneousSpace)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(new Vector3(0, 0, .5f), Vector3.one * 1.005f);
                Gizmos.color = Color.grey * 0.25f;
                Gizmos.DrawCube(new Vector3(0, 0, .5f), Vector3.one);
            }
            // draw a squezed cube to show how shearing in 3D can be seen as a translation
            // as long as we only look at the plane defined by z==1
            Gizmos.color = new Color(1, 1, 0, .5f);
            Gizmos.DrawCube(new Vector3(0, 0, 1f), new Vector3(1, 1, .001f));

            // we pop the transformation matrix and replace it with the identity matrix
            // that is the matrix that will not affect other matrices or points/vectors
            // we don't need it in this example, but it is a good idea to avoid hard to detect
            // bugs if we modify the scene later
            Gizmos.matrix = Matrix4x4.identity;

        }
    }
}