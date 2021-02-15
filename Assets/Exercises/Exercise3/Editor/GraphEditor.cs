using UnityEditor;
using UnityEngine;

namespace AfGD.Execise3
{
    [CustomEditor(typeof(Graph))]
    public class GraphEditor : Editor
    {
        SerializedProperty m_NodesProp;
        SerializedProperty m_EdgesProp;

        void OnEnable()
        {
            m_NodesProp = serializedObject.FindProperty("m_Nodes");
            m_EdgesProp = serializedObject.FindProperty("m_Edges");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display header that allows us to resize  
            // the number of nodes in this graph.
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Nodes");
            m_NodesProp.arraySize = EditorGUILayout.IntField(m_NodesProp.arraySize);
            EditorGUILayout.EndHorizontal();

            // Display the drawer for each node in this graph
            EditorGUI.indentLevel++;
            for (int i = 0; i < m_NodesProp.arraySize; i++)
                EditorGUILayout.PropertyField(m_NodesProp.GetArrayElementAtIndex(i));
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            // Display header that allows us to resize
            // the number of edges in this graph.
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Edges");
            m_EdgesProp.arraySize = EditorGUILayout.IntField(m_EdgesProp.arraySize);
            EditorGUILayout.EndHorizontal();

            // Display the drawer for each edge in this graph
            EditorGUI.indentLevel++;
            for (int i = 0; i < m_EdgesProp.arraySize; i++)
                EditorGUILayout.PropertyField(m_EdgesProp.GetArrayElementAtIndex(i));
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(Node))]
    public class NodeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30f;

            var nameProp = property.FindPropertyRelative("m_Name");
            var positionProp = property.FindPropertyRelative("m_Position");

            EditorGUILayout.BeginHorizontal();

            nameProp.stringValue = EditorGUILayout.TextField(nameProp.stringValue);

            var positionXProp = positionProp.FindPropertyRelative("x");
            positionXProp.floatValue = EditorGUILayout.FloatField(new GUIContent("X"), positionXProp.floatValue);

            var positionZProp = positionProp.FindPropertyRelative("z");
            positionZProp.floatValue = EditorGUILayout.FloatField(new GUIContent("Y"), positionZProp.floatValue);

            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = labelWidth;
        }
    }

    [CustomPropertyDrawer(typeof(Edge))]
    public class EdgeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50f;

            var costProp = property.FindPropertyRelative("Cost");
            var fromProp = property.FindPropertyRelative("From");
            var toProp = property.FindPropertyRelative("To");

            EditorGUILayout.BeginHorizontal();

            fromProp.intValue = EditorGUILayout.IntField(new GUIContent("From"), fromProp.intValue);
            toProp.intValue = EditorGUILayout.IntField(new GUIContent("To"), toProp.intValue);
            costProp.floatValue = EditorGUILayout.FloatField(new GUIContent("Cost"), costProp.floatValue);

            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = labelWidth;
        }
    }
}