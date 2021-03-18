using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AfGD.Exercise5
{
    [CustomEditor(typeof(SkeletonController))]
    public class SkeletonControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var skelCtrl = (SkeletonController)target;

            if (GUILayout.Button("Save current pose"))
            {
                skelCtrl.SaveCurrentPose();
            }
            
            if (GUILayout.Button("Reinitialize object"))
            {
                skelCtrl.Init();
            }
        }
    

        private void OnSceneGUI()
        {
            float handleSize = 0.07f;
            var skelCtrl = (SkeletonController)target;
            for (int i = 0; i < skelCtrl.currentPose.m_aLocalPose.Length; i++)
            {
                Matrix4x4 m = skelCtrl.transform.localToWorldMatrix * skelCtrl.modelSpacePose[i];
                Quaternion rot = m.rotation;
                EditorGUI.BeginChangeCheck();

                // three discs handle to control rotation around individual axis
                Handles.color = Color.red;
                Quaternion r = Handles.Disc(rot, m.GetColumn(3), m.GetColumn(0), handleSize, false, 0.1f);
                Handles.color = Color.green;
                r = Handles.Disc(r, m.GetColumn(3), m.GetColumn(1), handleSize, false, 0.1f);
                Handles.color = Color.blue;
                r = Handles.Disc(r, m.GetColumn(3), m.GetColumn(2), handleSize, false, 0.1f);

                if (EditorGUI.EndChangeCheck())
                {
                    skelCtrl.currentPose.m_aLocalPose[i].m_rot *= Quaternion.Inverse(rot) * r;
                    skelCtrl.currentPose.m_aLocalPose[i].m_rot.Normalize();
                }
            }
        }

    }
}
