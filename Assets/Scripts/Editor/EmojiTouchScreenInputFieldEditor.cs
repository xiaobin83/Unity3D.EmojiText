using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ui
{
	[CustomEditor(typeof(EmojiTouchScreenInputField))]
	public class EmojiTouchScreenInputFieldEditor : Editor
	{
		GUIContent lbTextComponent;
		SerializedProperty propTextComponent;

		GUIContent lbPlaceholder;
		SerializedProperty propPlaceholder;

		GUIContent lbText;
		SerializedProperty propText;

		GUIContent lbKeyboardType;
		SerializedProperty propKeyboardType;

		SerializedProperty propOnEndEdit;

		SerializedProperty propExcludeEmojiCharacters;
		SerializedProperty propEmojiConfig;
		SerializedProperty propEmojiReplaceChar;

		void OnEnable()
		{
			lbTextComponent = new GUIContent("Text Component");
			propTextComponent = serializedObject.FindProperty("m_TextComponent");

			lbPlaceholder = new GUIContent("Placeholder");
			propPlaceholder = serializedObject.FindProperty("m_Placeholder");

			lbText = new GUIContent("Text");
			propText = serializedObject.FindProperty("m_Text");

			lbKeyboardType = new GUIContent("Keyboard Type");
			propKeyboardType = serializedObject.FindProperty("m_KeyboardType");

			propOnEndEdit = serializedObject.FindProperty("onEndEdit");


			propExcludeEmojiCharacters = serializedObject.FindProperty("m_ExcludeEmojiCharaceters");
			propEmojiConfig = serializedObject.FindProperty("m_Config");

			propEmojiReplaceChar = serializedObject.FindProperty("m_EmojiReplaceChar");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(propTextComponent, lbTextComponent);
			EditorGUILayout.PropertyField(propPlaceholder, lbPlaceholder);
			EditorGUILayout.PropertyField(propKeyboardType, lbKeyboardType);
			EditorGUILayout.PropertyField(propOnEndEdit);
			EditorGUILayout.PropertyField(propExcludeEmojiCharacters);
			if (propExcludeEmojiCharacters.boolValue)
			{
				EditorGUILayout.PropertyField(propEmojiConfig);
				EditorGUILayout.PropertyField(propEmojiReplaceChar);
			}
			serializedObject.ApplyModifiedProperties();
		}

	}


}
