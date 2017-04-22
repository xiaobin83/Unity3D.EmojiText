using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ui
{
	public class FakeTouchScreenKeyboard : MonoBehaviour
	{
		public static FakeTouchScreenKeyboard Create(string text)
		{
			var obj = Resources.Load("P_FakeTouchScreenKeyboard");
			var go = GameObject.Instantiate(obj) as GameObject;
			var comp = go.GetComponent<FakeTouchScreenKeyboard>();
			comp.inputField.text = text;
			return comp;
		}

		public string text
		{
			set
			{
				inputField.text = value;
			}
			get
			{
				return inputField.text;
			}
		}

		public bool active
		{
			get
			{
				return gameObject.activeSelf;
			}
			set
			{
				gameObject.SetActive(value);
 			}
		}

		[System.NonSerialized]
		public bool wasCanceled = false;
		[System.NonSerialized]
		public bool done = false;

		public InputField inputField;

		void OnBtnOk()
		{
			done = true;
			active = false;
		}
		
		void OnBtnCancel()
		{
			done = true;
			wasCanceled = true;
			active = false;
		}

		public void Destroy()
		{
			if (Application.isPlaying)
				GameObject.Destroy(gameObject);
			else
				GameObject.DestroyImmediate(gameObject);
        }
	}
}
