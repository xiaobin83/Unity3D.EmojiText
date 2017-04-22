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

// to fix input field double char / four char unicode input problem

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;


namespace ui
{
	public class EmojiTouchScreenInputField : Selectable, IPointerClickHandler
	{
		[SerializeField]
		EmojiConfig m_Config;
		[SerializeField]
		bool m_ExcludeEmojiCharaceters;

		bool excludeEmojiCharacters
		{
			get
			{
				return m_ExcludeEmojiCharaceters && m_Config != null;
			}
		}

		protected TouchScreenKeyboard m_Keyboard;
		protected FakeTouchScreenKeyboard m_FakeKeyboard;

		[SerializeField]
		EmojiText m_TextComponent;
		[SerializeField]
		Text m_Placeholder;
		[SerializeField]
		Char m_EmojiReplaceChar = '?';

		string m_OriginalText;

		[SerializeField]
		string m_Text = string.Empty;

		public string text
		{
			get
			{
				return m_Text;
			}
			set
			{
				if (m_Text != value)
				{
					m_Text = value;
					if (m_TextComponent != null)
						m_TextComponent.text = value;
				}
				if (m_Placeholder != null)
					m_Placeholder.enabled = string.IsNullOrEmpty(m_Text);
			}
		}

		[SerializeField]
		TouchScreenKeyboardType m_KeyboardType = TouchScreenKeyboardType.Default;

		[Serializable]
		public class SubmitEvent : UnityEvent<string> { }
		public SubmitEvent onEndEdit = new SubmitEvent();

		void ActivateInputFieldInternal()
		{
			if (EventSystem.current == null)
				return;

			if (EventSystem.current.currentSelectedGameObject != gameObject)
				EventSystem.current.SetSelectedGameObject(gameObject);

			if (TouchScreenKeyboard.isSupported)
			{
				m_Keyboard = TouchScreenKeyboard.Open(m_Text, m_KeyboardType, autocorrection: false, multiline: false);
			}
			else
			{
				m_FakeKeyboard = FakeTouchScreenKeyboard.Create(m_Text);
			}

			m_AllowInput = true;
			m_OriginalText = m_Text;
			m_WasCanceled = false;
		}


		bool shouldActivateOnSelect
		{
			get
			{
				return Application.platform != RuntimePlatform.tvOS;
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (shouldActivateOnSelect)
				ActivateInputField();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			if (m_Keyboard != null)
			{
				DeactivateInputField();
			}
			base.OnDeselect(eventData);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			ActivateInputField();
		}

		bool m_AllowInput = false;
		bool m_ShouldActivateNextUpdate = false;
		bool m_WasCanceled = false;

		public void ActivateInputField()
		{
			if (m_TextComponent == null || m_TextComponent.font == null || !IsActive() || !IsInteractable())
				return;

			if (m_AllowInput)
			{
				if (m_Keyboard != null)
				{
					if (!m_Keyboard.active)
					{
						m_Keyboard.active = true;
						m_Keyboard.text = m_Text;
					}
				}
				else
				{
					Debug.Assert(m_FakeKeyboard != null);
					m_FakeKeyboard.text = m_Text;
				}
			}

			m_ShouldActivateNextUpdate = true;
		}

		public void DeactivateInputField()
		{
			// Not activated do nothing.
			if (!m_AllowInput)
				return;

			m_AllowInput = false;


			if (m_TextComponent != null && IsInteractable())
			{
				if (m_WasCanceled)
				{
					text = m_OriginalText;
				}
				else
				{
					string keyboardText = string.Empty;
					if (m_Keyboard != null)
					{
						keyboardText = m_Keyboard.text;
					}
					else
					{
						keyboardText = m_FakeKeyboard.text;
					}

					if (excludeEmojiCharacters)
					{
						var sb = new System.Text.StringBuilder();
						EmojiText.UpdateEmojiReplacements(
							keyboardText, m_Config,
							(emojiChar, emojiIndex) =>
							{
								if (emojiIndex != -1)
								{
									sb.Append(m_EmojiReplaceChar);
								}
								else
								{
									sb.Append(emojiChar);
								}
							});
						text = sb.ToString();
					}
					else
					{
						text = keyboardText;
					}
				}

				if (m_Keyboard != null)
				{
					m_Keyboard.active = false;
					m_Keyboard = null;
				}
				else
				{
					Debug.Assert(m_FakeKeyboard != null);
					m_FakeKeyboard.Destroy();
					m_FakeKeyboard = null;
				}

				SendOnSubmit();

				Input.imeCompositionMode = IMECompositionMode.Auto;
			}

			if (m_Placeholder != null)
				m_Placeholder.enabled = string.IsNullOrEmpty(m_Text);
		}

		protected void SendOnSubmit()
		{
			if (onEndEdit != null)
				onEndEdit.Invoke(m_Text);
		}

		void LateUpdate()
		{
			// Only	activate if	we are not already activated.
			if (m_ShouldActivateNextUpdate)
			{
				if (!m_AllowInput)
				{
					ActivateInputFieldInternal();
					m_ShouldActivateNextUpdate = false;
					return;
				}

				// Reset as we are already activated.
				m_ShouldActivateNextUpdate = false;
			}

			if (!m_AllowInput)
				return;

			if (m_Keyboard != null)
 			{
				if (!m_Keyboard.active)
				{
					if (m_Keyboard.wasCanceled)
						m_WasCanceled = true;
					OnDeselect(null);
					return;
				}
			}
			else
			{
				Debug.Assert(m_FakeKeyboard != null);
				if (!m_FakeKeyboard.active)
				{
					if (m_FakeKeyboard.wasCanceled)
						m_WasCanceled = true;
					DeactivateInputField();
					OnDeselect(null);
					return;
				}
			}

			if (m_Keyboard != null)
			{
				if (m_Keyboard.done)
				{
					if (m_Keyboard.wasCanceled)
						m_WasCanceled = true;
					OnDeselect(null);
				}
			}
			else
			{
				Debug.Assert(m_FakeKeyboard != null);
				if (m_FakeKeyboard.done)
				{
					if (m_FakeKeyboard.wasCanceled)
						m_WasCanceled = true;
					DeactivateInputField();
					OnDeselect(null);
				}
			}
		}
	}
}
