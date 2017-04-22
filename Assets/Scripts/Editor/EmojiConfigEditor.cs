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

using UnityEditor;
using UnityEngine;
using System.Collections;

namespace ui
{
	[CustomEditor(typeof(EmojiConfig))]
	public class EmojiConfigEditor : Editor
	{
		SerializedProperty propTexture;
		SerializedProperty propRects;
		SerializedProperty propNames;

		void Init()
		{
			lock (serializedObject)
			{
				propTexture = serializedObject.FindProperty("texture");
				propRects = serializedObject.FindProperty("rects");
				propNames = serializedObject.FindProperty("names");
			}
		}

		void OnEnable()
		{
			Init();
		}

		void OnAtlasBaked(string path, string[] names, Rect[] rects)
		{
			lock (serializedObject)
			{
				propTexture.objectReferenceValue = 
					AssetDatabase.LoadAssetAtPath<Texture>(path);
				propRects.arraySize = rects.Length;
				for (int i = 0; i < rects.Length; ++i)
				{
					var propR = propRects.GetArrayElementAtIndex(i);
					propR.rectValue = rects[i];
				}

				propNames.arraySize = names.Length;

				for (int i = 0; i < names.Length; ++i)
				{
					var sb = new System.Text.StringBuilder();
					var sp = names[i].Split('-');
					for (int j = 0; j < sp.Length; ++j)
					{
						sb.Append(char.ConvertFromUtf32(System.Convert.ToInt32(sp[j], 16)));
					}
					var propN = propNames.GetArrayElementAtIndex(i);
					propN.stringValue = sb.ToString();
				}

				serializedObject.ApplyModifiedProperties();
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			Init();
			lock (serializedObject)
			{
				if (propTexture.objectReferenceValue == null)
				{
					EditorGUILayout.HelpBox("Emoji Config not initialized. Create Atlas First.", MessageType.Error);
					if (GUILayout.Button("Create Atlas"))
					{
						AtlasBakerWizard.ShowAtlasBakerWizard(OnAtlasBaked);
					}
				}
				else
				{
					base.OnInspectorGUI();
					if (GUILayout.Button("Recreate Atlas"))
					{
						AtlasBakerWizard.ShowAtlasBakerWizard(OnAtlasBaked);
					}
				}
			}
		}
	}
}
