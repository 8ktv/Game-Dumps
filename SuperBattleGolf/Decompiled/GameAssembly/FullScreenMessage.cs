using Cysharp.Threading.Tasks;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FullScreenMessage : MonoBehaviour
{
	public class ButtonEntry
	{
		public string name;

		public UnityAction callback;

		public bool cancel;

		public bool submit;

		public ButtonEntry(string name, UnityAction callback, bool cancel = false, bool submit = false)
		{
			this.name = name;
			this.callback = callback;
			this.cancel = cancel;
			this.submit = submit;
		}
	}

	private const string prefabPath = "Assets/Prefabs/UI/Full-screen message.prefab";

	private static FullScreenMessage instance;

	[SerializeField]
	private TMP_Text message;

	[SerializeField]
	private TMP_Text header;

	[SerializeField]
	private Button[] buttons;

	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private TMP_Text inputFieldPromptText;

	[SerializeField]
	private MenuNavigation navigation;

	private UnityAction cancel;

	private UnityAction submit;

	private int currentlyDisplayedErrorPriority = int.MaxValue;

	private bool isDisplayingAnyMessage;

	private bool isDisplayingError;

	public static FullScreenMessage Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Addressables.InstantiateAsync("Assets/Prefabs/UI/Full-screen message.prefab").WaitForCompletion().GetComponent<FullScreenMessage>();
				Object.DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}

	public static bool IsDisplayingAnyMessage
	{
		get
		{
			if (instance != null)
			{
				return instance.isDisplayingAnyMessage;
			}
			return false;
		}
	}

	public static bool IsDisplayingError
	{
		get
		{
			if (instance != null)
			{
				return instance.isDisplayingError;
			}
			return false;
		}
	}

	public static string InputFieldText
	{
		get
		{
			if (!(instance != null))
			{
				return string.Empty;
			}
			return instance.inputField.text;
		}
	}

	private void Awake()
	{
		if (!IsDisplayingAnyMessage)
		{
			HideInternal(playSfx: false);
		}
		SceneManager.activeSceneChanged += ActiveSceneChanged;
	}

	private void OnDestroy()
	{
		InputManager.DisableMode(InputMode.FullScreenMessage);
		SceneManager.activeSceneChanged -= ActiveSceneChanged;
	}

	private void ActiveSceneChanged(Scene prev, Scene active)
	{
		if (isDisplayingAnyMessage && BNetworkManager.singleton.offlineScene != active.path)
		{
			HideInternal();
		}
		else
		{
			AssertFocusDelayed();
		}
		async void AssertFocusDelayed()
		{
			await UniTask.WaitForEndOfFrame();
			navigation.AssertFocus();
		}
	}

	private void Update()
	{
		if (cancel != null)
		{
			Keyboard current = Keyboard.current;
			if (current == null || !current.escapeKey.wasPressedThisFrame)
			{
				Gamepad currentGamepad = InputManager.CurrentGamepad;
				if (currentGamepad == null || !currentGamepad.buttonEast.wasPressedThisFrame)
				{
					goto IL_0043;
				}
			}
			cancel();
		}
		goto IL_0043;
		IL_0043:
		if (submit == null)
		{
			return;
		}
		Keyboard current2 = Keyboard.current;
		if (current2 == null || !current2.enterKey.wasPressedThisFrame)
		{
			Gamepad currentGamepad2 = InputManager.CurrentGamepad;
			if (currentGamepad2 == null || !currentGamepad2.buttonSouth.wasPressedThisFrame)
			{
				return;
			}
		}
		submit();
	}

	public static void Show(string message, params ButtonEntry[] buttonEntries)
	{
		Show(message, string.Empty, buttonEntries);
	}

	public static void Show(string message, string header, params ButtonEntry[] buttonEntries)
	{
		if (Instance != null)
		{
			Instance.ShowInternal(message, header, isError: false, buttonEntries.Length != 0, buttonEntries);
		}
	}

	public static void ShowTextField(string message, string inputFieldPromptText, string defaultValue, bool isPasswordField, params ButtonEntry[] buttonEntries)
	{
		ShowTextField(message, inputFieldPromptText, defaultValue, string.Empty, isPasswordField, buttonEntries);
	}

	public static void ShowTextField(string message, string inputFieldPromptText, string defaultValue, string header, bool isPasswordField, params ButtonEntry[] buttonEntries)
	{
		if (Instance != null)
		{
			Instance.ShowTextFieldInternal(message, inputFieldPromptText, defaultValue, header, isPasswordField, 0, buttonEntries);
		}
	}

	public static void ShowTextField(string message, string inputFieldPromptText, string defaultValue, bool isPasswordField, int characterLimit, params ButtonEntry[] buttonEntries)
	{
		ShowTextField(message, inputFieldPromptText, defaultValue, string.Empty, isPasswordField, characterLimit, buttonEntries);
	}

	public static void ShowTextField(string message, string inputFieldPromptText, string defaultValue, string header, bool isPasswordField, int characterLimit, params ButtonEntry[] buttonEntries)
	{
		if (Instance != null)
		{
			Instance.ShowTextFieldInternal(message, inputFieldPromptText, defaultValue, header, isPasswordField, characterLimit, buttonEntries);
		}
	}

	public static void ShowErrorMessage(string message, string additionalMessage = "", string header = "", int priority = int.MaxValue, bool canOnlyQuit = false)
	{
		if (Instance != null)
		{
			Instance.ShowErrorMessageInternal(message, additionalMessage, header, priority, canOnlyQuit);
		}
	}

	public static void Hide()
	{
		if (Instance != null)
		{
			Instance.HideInternal();
		}
	}

	private void ShowInternal(string message, string header, bool isError, bool requiresMouse, params ButtonEntry[] buttonEntries)
	{
		if (requiresMouse)
		{
			CursorManager.SetCursorForceUnlocked(forceUnlocked: true);
		}
		Debug.Log("Showing full screen " + (isError ? "error" : "message") + ":\n" + message);
		this.message.gameObject.SetActive(!string.IsNullOrEmpty(message));
		this.message.text = message;
		this.header.text = header;
		cancel = null;
		isDisplayingError = isError;
		if (!isError)
		{
			ResetDisplayedErrorPriority();
		}
		for (int i = 0; i < BMath.Max(buttons.Length, buttonEntries.Length); i++)
		{
			if (i >= buttonEntries.Length)
			{
				buttons[i].gameObject.SetActive(value: false);
				continue;
			}
			Button button = buttons[i];
			button.GetComponentInChildren<TMP_Text>().text = buttonEntries[i].name;
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(buttonEntries[i].callback);
			button.gameObject.SetActive(value: true);
			Navigation navigation = button.navigation;
			navigation.mode = Navigation.Mode.Explicit;
			Selectable selectOnDown = (navigation.selectOnRight = null);
			navigation.selectOnDown = selectOnDown;
			navigation.selectOnLeft = ((i > 0 && i < buttonEntries.Length) ? buttons[i - 1] : null);
			navigation.selectOnRight = ((i < buttonEntries.Length - 1) ? buttons[i + 1] : null);
			button.navigation = navigation;
			button.interactable = false;
			if (i == 0)
			{
				button.Select();
			}
			if (buttonEntries[i].cancel)
			{
				cancel = buttonEntries[i].callback;
			}
			else if (buttonEntries[i].submit)
			{
				submit = buttonEntries[i].callback;
			}
		}
		EnableInputs();
		isDisplayingAnyMessage = true;
		base.gameObject.SetActive(value: true);
		inputField.gameObject.SetActive(value: false);
		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInChildren<VerticalLayoutGroup>().GetComponent<RectTransform>());
		Canvas.ForceUpdateCanvases();
		InputManager.EnableMode(InputMode.FullScreenMessage);
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.FullScreenMessageOpen);
		async void EnableInputs()
		{
			await UniTask.Yield();
			Button[] array = buttons;
			foreach (Button button2 in array)
			{
				if (button2.gameObject.activeInHierarchy)
				{
					button2.interactable = true;
				}
			}
			GetComponentInChildren<MenuNavigation>().Reselect();
		}
	}

	private void ShowTextFieldInternal(string message, string inputFieldPromptText, string defaultValue, string header, bool isPasswordField, int characterLimit, ButtonEntry[] buttonEntries)
	{
		ShowInternal(message, header, isError: false, requiresMouse: true, buttonEntries);
		inputField.text = defaultValue;
		inputField.gameObject.SetActive(value: true);
		inputField.contentType = (isPasswordField ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard);
		inputField.characterLimit = characterLimit;
		this.inputFieldPromptText.text = inputFieldPromptText;
		inputField.Select();
	}

	private void ShowErrorMessageInternal(string message, string additionalMessage, string header, int priority, bool canOnlyQuit)
	{
		if (priority > currentlyDisplayedErrorPriority)
		{
			Debug.LogWarning($"Skipped onscreen error message due to priority ({priority} vs the currently displayed {currentlyDisplayedErrorPriority}):\n{message}");
			return;
		}
		currentlyDisplayedErrorPriority = priority;
		if (!string.IsNullOrEmpty(additionalMessage) && LocalizationManager.CurrentLanguageIsEnglish)
		{
			message = message + "\n" + additionalMessage;
		}
		ButtonEntry buttonEntry = (canOnlyQuit ? new ButtonEntry(Localization.UI.MENU_ExitGame, MainMenu.Quit) : new ButtonEntry(Localization.UI.MISC_Ok, HideInternal));
		ShowInternal(message, header, true, true, buttonEntry);
	}

	private void HideInternal()
	{
		HideInternal(playSfx: true);
	}

	private void HideInternal(bool playSfx)
	{
		bool activeSelf = base.gameObject.activeSelf;
		ResetDisplayedErrorPriority();
		base.gameObject.SetActive(value: false);
		isDisplayingAnyMessage = false;
		CursorManager.SetCursorForceUnlocked(forceUnlocked: false);
		InputManager.DisableMode(InputMode.FullScreenMessage);
		if (activeSelf && playSfx)
		{
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.FullScreenMessageClose);
		}
	}

	private void ResetDisplayedErrorPriority()
	{
		currentlyDisplayedErrorPriority = int.MaxValue;
	}
}
