/*
MIT License

Copyright (c) 2016 xiaobin83

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;
using UnityEditor;

namespace ui
{
	[CustomEditor(typeof(EmojiText), true)]
	public class EmojiTextEditor : UnityEditor.UI.TextEditor
	{
		GUIContent lbEmoji;

		SerializedProperty propConfig;
		SerializedProperty propHrefColor;
		SerializedProperty propHrefOnClickedEventName;
		SerializedProperty propHrefOnClickedEvent;
		SerializedProperty propEscapeUnicodeCharacter;
		SerializedProperty propAltPredefinedStringColor;
		SerializedProperty propPredefinedStringColor;
		SerializedProperty propShowRawText;


		protected override void OnEnable()
		{
			base.OnEnable();
			lbEmoji = new GUIContent("Emoji");

			propConfig = serializedObject.FindProperty("m_Config");
			propHrefColor = serializedObject.FindProperty("hrefColor");
			propHrefOnClickedEventName = serializedObject.FindProperty("hrefOnClickedEventName");
			propHrefOnClickedEvent = serializedObject.FindProperty("hrefOnClickedEvent");
			propEscapeUnicodeCharacter = serializedObject.FindProperty("escapeUnicodeCharacter");
			propAltPredefinedStringColor = serializedObject.FindProperty("altPredefinedStringColor");
			propPredefinedStringColor = serializedObject.FindProperty("predefinedStringColor");
			propShowRawText = serializedObject.FindProperty("showRawText");
        }

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.LabelField(lbEmoji, EditorStyles.boldLabel);
			++EditorGUI.indentLevel;
			EditorGUILayout.PropertyField(propConfig);
			if (propConfig.objectReferenceValue == null)
			{
				if (GUILayout.Button("Create Config"))
				{
					var objConfig = ScriptableObject.CreateInstance<EmojiConfig>();
					var configPath = AssetDatabase.GenerateUniqueAssetPath("Assets/EmojiConfig.asset");
					AssetDatabase.CreateAsset(objConfig, configPath);
					AssetDatabase.SaveAssets();
					propConfig.objectReferenceValue = objConfig;
				}
			}
			EditorGUILayout.PropertyField(propHrefColor);
			EditorGUILayout.PropertyField(propHrefOnClickedEventName);
			EditorGUILayout.PropertyField(propHrefOnClickedEvent);
			EditorGUILayout.PropertyField(propEscapeUnicodeCharacter);
			EditorGUILayout.PropertyField(propAltPredefinedStringColor);
			EditorGUILayout.PropertyField(propPredefinedStringColor);
			EditorGUILayout.PropertyField(propShowRawText);
			serializedObject.ApplyModifiedProperties();
			--EditorGUI.indentLevel;
		}

	}
}
