using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextChatUi : SingletonBehaviour<TextChatUi>
{
	public TextChatMessageUi messagePrefab;

	public float messageTimeout;

	public Transform historyParent;

	public Transform newParent;

	public int maxNewMessages;

	public int maxMessageHistory;

	public GameObject newMessages;

	public GameObject history;

	public GameObject message;

	public TMP_InputField messageField;

	public ControllerSelectable messageFieldSelectable;

	public TMP_Text openButtonPrompt;

	private Queue<TextChatMessageUi> historyQueue = new Queue<TextChatMessageUi>();

	private Queue<TextChatMessageUi> newMessagesQueue = new Queue<TextChatMessageUi>();

	private bool enabledInternal;

	[SerializeField]
	private MenuNavigation navigation;

	public static bool IsOpen
	{
		get
		{
			if (SingletonBehaviour<TextChatUi>.HasInstance)
			{
				return SingletonBehaviour<TextChatUi>.Instance.enabledInternal;
			}
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		messageField.onSubmit.AddListener(OnMessageSubmit);
		SetEnabledInternal(enabled: false);
		LocalizationManager.LanguageChanged += UpdateButtonPrompt;
		InputManager.SwitchedInputDeviceType += UpdateButtonPrompt;
		UpdateButtonPrompt();
		Color color = openButtonPrompt.color;
		color.a = 0f;
		openButtonPrompt.color = color;
		navigation.enabled = false;
		navigation.OnExitEvent += delegate
		{
			SetEnabledInternal(enabled: false);
		};
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		LocalizationManager.LanguageChanged -= UpdateButtonPrompt;
		InputManager.SwitchedInputDeviceType -= UpdateButtonPrompt;
	}

	private void Update()
	{
		if (enabledInternal && EventSystem.current != null && EventSystem.current.currentSelectedGameObject != messageField.gameObject)
		{
			EventSystem.current.SetSelectedGameObject(messageField.gameObject);
		}
	}

	private void UpdateButtonPrompt()
	{
		string inputIconRichTextTag = InputManager.GetInputIconRichTextTag(InputManager.Controls.Ingame.OpenChat);
		if (inputIconRichTextTag == null || inputIconRichTextTag == string.Empty)
		{
			openButtonPrompt.text = string.Empty;
		}
		else
		{
			openButtonPrompt.text = string.Format(Localization.UI.TEXTCHAT_ButtonPrompt_StartChat, inputIconRichTextTag);
		}
	}

	public void SetMessageLimit(int limit)
	{
		messageField.characterLimit = limit;
	}

	public static void ShowMessage(string message)
	{
		if (SingletonBehaviour<TextChatUi>.HasInstance)
		{
			SingletonBehaviour<TextChatUi>.Instance.ShowMessageInternal(message);
		}
	}

	public static void SetEnabled(bool enabled)
	{
		if (SingletonBehaviour<TextChatUi>.HasInstance)
		{
			SingletonBehaviour<TextChatUi>.Instance.SetEnabledInternal(enabled);
		}
	}

	private void SetEnabledInternal(bool enabled)
	{
		newMessages.SetActive(!enabled);
		openButtonPrompt.gameObject.SetActive(!enabled);
		history.SetActive(enabled);
		message.SetActive(enabled);
		navigation.enabled = enabled;
		if (enabled)
		{
			InputManager.EnableMode(InputMode.TextChat);
			if (MenuNavigation.ShouldShowVirtualKeyboard)
			{
				navigation.SelectAndSubmit(messageFieldSelectable);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(null);
				EventSystem.current.SetSelectedGameObject(messageField.gameObject);
			}
		}
		else
		{
			InputManager.DisableMode(InputMode.TextChat);
		}
		enabledInternal = enabled;
	}

	private void ShowMessageInternal(string message)
	{
		AddMessage(message, maxNewMessages, newMessagesQueue, newParent, newMessages: true);
		AddMessage(message, maxMessageHistory, historyQueue, historyParent, newMessages: false);
		StopAllCoroutines();
		StartCoroutine(UpdateButtonPromptAlpha());
		TextChatMessageUi AddMessage(string text, int maxQueue, Queue<TextChatMessageUi> queue, Transform parent, bool newMessages)
		{
			TextChatMessageUi textChatMessageUi = ((queue.Count >= maxQueue) ? queue.Dequeue() : Object.Instantiate(messagePrefab, parent));
			textChatMessageUi.timeout = (newMessages ? messageTimeout : float.MinValue);
			textChatMessageUi.gameObject.SetActive(value: true);
			textChatMessageUi.Initialize(text);
			textChatMessageUi.transform.SetAsFirstSibling();
			queue.Enqueue(textChatMessageUi);
			return textChatMessageUi;
		}
	}

	private async void OnMessageSubmit(string message)
	{
		if (message.Length > 0)
		{
			TextChatManager.SendChatMessage(message);
		}
		messageField.text = string.Empty;
		await UniTask.Yield();
		SetEnabledInternal(enabled: false);
	}

	private IEnumerator UpdateButtonPromptAlpha()
	{
		int activeCount;
		do
		{
			float num = 0f;
			activeCount = 0;
			foreach (TextChatMessageUi item in newMessagesQueue)
			{
				if (item.gameObject.activeSelf)
				{
					activeCount++;
					num = BMath.Max(item.canvasGroup.alpha, num);
				}
			}
			SetAlpha(num * 0.5f);
			yield return null;
		}
		while (activeCount > 0);
		void SetAlpha(float alpha)
		{
			Color color = openButtonPrompt.color;
			color.a = alpha;
			openButtonPrompt.color = color;
		}
	}
}
