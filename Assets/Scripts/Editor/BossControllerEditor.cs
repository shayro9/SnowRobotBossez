using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossController))]
[CanEditMultipleObjects]
public class BossControllerEditor : Editor
{
    private SerializedObject m_Object;
    private SerializedProperty groups;
    private SerializedProperty attack_manager;
    private SerializedProperty health;
    private SerializedProperty type;
    private SerializedProperty pattern;

    void OnEnable()
    {
        m_Object = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        BossController item = (BossController)target;
        attack_manager = m_Object.FindProperty("attack_manager");
        groups = m_Object.FindProperty("groups");
        health = m_Object.FindProperty("health");
        type = m_Object.FindProperty("type");
        pattern = m_Object.FindProperty("pattern");

        EditorGUI.BeginDisabledGroup(true);
        item.health = EditorGUILayout.FloatField("Max Health", item.GetMaxHealth());
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(groups, new GUIContent("Groups"), true);
    
        EditorGUILayout.PropertyField(attack_manager, new GUIContent("Manager"), true);

        m_Object.ApplyModifiedProperties();
    }
}
