using UnityEditor;
using UnityEngine;

namespace Dialogue.Editor
{
    [CustomEditor(typeof(DialogueNode))]
    public class DialogueNodeEditor : UnityEditor.Editor
    {
        private SerializedProperty textProperty;

        private void OnEnable()
        {
            // Find the serialized property for the 'text' field in DialogueNode
            textProperty = serializedObject.FindProperty("text");
        }

        public override void OnInspectorGUI()
        {
            // Update the serialized object
            serializedObject.Update();

            // Create a GUIStyle with word wrapping enabled
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true // Enable word wrapping
            };

            // Draw the text field with word wrapping
            EditorGUILayout.LabelField("Dialogue Text", EditorStyles.boldLabel);
            if (textProperty != null)
            {
                textProperty.stringValue = EditorGUILayout.TextArea(
                    textProperty.stringValue,
                    textAreaStyle, // Use the custom style
                    GUILayout.MinHeight(100), // Minimum height for the text area
                    GUILayout.MaxHeight(300)  // Optional: Maximum height for the text area
                );
            }
            else
            {
                EditorGUILayout.HelpBox("Text property not found!", MessageType.Error);
            }

            // Apply changes to the serialized object
            serializedObject.ApplyModifiedProperties();

            // Draw the default inspector for other fields
            DrawDefaultInspector();
        }
    }
}