using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MonoBehaviour), true)]
public class CustomHeaderDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            var attributes = field.GetCustomAttributes(typeof(CustomHeaderAttribute), false);
            foreach (CustomHeaderAttribute header in attributes)
            {
                GUIStyle style = new(EditorStyles.boldLabel)
                {
                    normal = { textColor = header.color },
                    fontSize = 14
                };

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField(header.headerText, style);
                EditorGUILayout.Space(5);
            }

            var property = serializedObject.FindProperty(field.Name);
            if (property != null)
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
