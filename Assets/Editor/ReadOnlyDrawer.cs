using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Tắt khả năng chỉnh sửa trên UI
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true; // Bật lại cho các thành phần khác
    }

    // Hiển thị đúng độ dài của danh sách (List)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}