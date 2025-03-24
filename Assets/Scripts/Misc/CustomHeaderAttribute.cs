using UnityEngine;
using UnityEditor;
using System.Reflection;

public class CustomHeaderAttribute : PropertyAttribute
{
    public string headerText;
    public Color color;

    public CustomHeaderAttribute(string headerText)
    {
        this.headerText = headerText;
        this.color = new Color(1f, .5f, 0f);
    }
}

[CustomEditor(typeof(MonoBehaviour), true)]
public class CustomHeaderDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Получаем все поля объекта
        var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            // Проверяем наличие атрибута CustomHeader
            var attributes = field.GetCustomAttributes(typeof(CustomHeaderAttribute), false);
            foreach (CustomHeaderAttribute header in attributes)
            {
                // Настраиваем стиль заголовка
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
                {
                    normal = { textColor = header.color },
                    fontSize = 14
                };

                // Рисуем заголовок
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField(header.headerText, style);
                EditorGUILayout.Space(5);
            }

            // Отображаем поле в инспекторе
            var property = serializedObject.FindProperty(field.Name);
            if (property != null)
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}