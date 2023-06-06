using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Game.Hotfix
{
    [CustomEditor(typeof(EmojiInputField))]
    public class EmojiInputFieldLis : UnityEditor.UI.InputFieldEditor
    {
        EmojiInputField m_info;
        protected override void OnEnable()
        {
            base.OnEnable();
        }
 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            serializedObject.Update();
            m_info = (EmojiInputField)target;
            m_info.m_emojiText = EditorGUILayout.ObjectField( "EmojiText", m_info.m_emojiText, typeof(EmojiText)) as EmojiText;
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
