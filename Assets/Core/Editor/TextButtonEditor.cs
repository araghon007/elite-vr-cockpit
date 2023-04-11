﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace EVRC.UI
{
    [CustomEditor(typeof(TextButton), true)]
    public class TextButtonEditor : ButtonEditor
    {
        SerializedProperty m_textProp;
        SerializedProperty m_textColorProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_textProp = serializedObject.FindProperty("text");
            m_textColorProp = serializedObject.FindProperty("textColors");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TextButton textButton = (TextButton)target;

            EditorGUILayout.PropertyField(m_textProp);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_textColorProp);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
