using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Game.Hotfix
{
    [CustomEditor(typeof(EmojiText))]
    public class EmojiTextLis : UnityEditor.UI.TextEditor
    {
        EmojiText m_info;
        protected override void OnEnable()
        {
            base.OnEnable();
        }
 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            serializedObject.Update();
            m_info = (EmojiText)target;
            m_info.m_atlasCfgFile = EditorGUILayout.ObjectField( "EmojiText", m_info.m_atlasCfgFile, typeof(TextAsset)) as TextAsset;
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
