using UnityEditor;
using UnityEngine;

namespace AfGD.Assignment1
{
    [CustomEditor(typeof(PathFinding))]
    public class PathFindingEditor : Editor
    {
        SerializedProperty m_GraphProperty;
        SerializedProperty m_StartProperty;
        SerializedProperty m_EndProperty;

        void OnEnable()
        {
            m_GraphProperty = serializedObject.FindProperty("m_Graph");
            m_StartProperty = serializedObject.FindProperty("m_Start");
            m_EndProperty = serializedObject.FindProperty("m_End");
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty so that we have access to the lastest data.
            serializedObject.Update();

            // Display property fields for the Graph.
            // Property automatically figures out what kind of field it should display.
            EditorGUILayout.PropertyField(m_GraphProperty);

            // Put some space before our next field.
            EditorGUILayout.Space();

            // Display the drawer for our start position
            Vector3Sliders("Start Position (Green)", m_StartProperty);

            // Put some space before our next field.
            EditorGUILayout.Space();

            // Display the drawer for our end position
            Vector3Sliders("End Position (Red)", m_EndProperty);

            // Put some space before our next field.
            EditorGUILayout.Space();

            if (GUILayout.Button("Run"))
            {
                // Grab the pathfinding monobehaviour and
                // execute the pathfinding algorithm.
                var pathFindingMonobehaviour = (PathFinding)target;
                pathFindingMonobehaviour.Run();

                // Repaint SceneView to show the result
                SceneView.RepaintAll();
            }

            // Apply changes to the serializedProperty 
            serializedObject.ApplyModifiedProperties();
        }

        void Vector3Sliders(string label, SerializedProperty property)
        {
            // Display the label.
            EditorGUILayout.LabelField(label);

            // Increase indentation.
            EditorGUI.indentLevel++;

            // Get the current value.
            var vec = property.vector3Value;

            // Display sliders for the x and z components.
            vec.x = EditorGUILayout.Slider(new GUIContent("x"), vec.x, -1f, 31f);
            vec.z = EditorGUILayout.Slider(new GUIContent("z"), vec.z, -1f, 31f);

            // Write the result back to the serialized property.
            property.vector3Value = vec;

            // Restore indentation.
            EditorGUI.indentLevel--;
        }
    }
}