using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadialMenu : SingletonBehaviour<RadialMenu>
{
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private RadialMenuUi ui;

	[SerializeField]
	private RadialMenuOptionUi optionUiPrefab;

	[SerializeField]
	private int maxOptionUiPoolSize;

	[SerializeField]
	private float mouseDeadZone;

	[SerializeField]
	private float gamepadDeadZone;

	[SerializeField]
	private float fadeDuration = 0.1f;

	[SerializeField]
	private float scaleDuration = 0.15f;

	[SerializeField]
	private RadialMenuOptionSettings optionSettings;

	private RectTransform rectTransform;

	private float size;

	private float initialScale;

	private RadialMenuMode currentMode;

	private int highlightedIndex;

	private int lastOpenFrame;

	private int lastSelectionFrame;

	private Coroutine visibilityRoutine;

	private static Transform optionUiPoolParent;

	private static readonly Stack<RadialMenuOptionUi> optionUiPool = new Stack<RadialMenuOptionUi>();

	public static bool WasOpenThisOrLastFrame
	{
		get
		{
			if (SingletonBehaviour<RadialMenu>.HasInstance)
			{
				if (SingletonBehaviour<RadialMenu>.Instance.lastOpenFrame != Time.frameCount)
				{
					return SingletonBehaviour<RadialMenu>.Instance.lastOpenFrame == Time.frameCount - 1;
				}
				return true;
			}
			return false;
		}
	}

	public static float Size
	{
		get
		{
			if (!SingletonBehaviour<RadialMenu>.HasInstance)
			{
				return 0f;
			}
			return SingletonBehaviour<RadialMenu>.Instance.size;
		}
	}

	public static RadialMenuOptionSettings OptionSettings
	{
		get
		{
			if (!SingletonBehaviour<RadialMenu>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<RadialMenu>.Instance.optionSettings;
		}
	}

	public static RadialMenuMode CurrentMode
	{
		get
		{
			if (!SingletonBehaviour<RadialMenu>.HasInstance)
			{
				return RadialMenuMode.None;
			}
			return SingletonBehaviour<RadialMenu>.Instance.currentMode;
		}
	}

	public static bool IsVisible
	{
		get
		{
			if (SingletonBehaviour<RadialMenu>.HasInstance)
			{
				return SingletonBehaviour<RadialMenu>.Instance.currentMode != RadialMenuMode.None;
			}
			return false;
		}
	}

	public static int LastSelectionFrame
	{
		get
		{
			if (!SingletonBehaviour<RadialMenu>.HasInstance)
			{
				return int.MinValue;
			}
			return SingletonBehaviour<RadialMenu>.Instance.lastSelectionFrame;
		}
	}

	private event Action<int> OnSelected;

	protected override void Awake()
	{
		base.Awake();
		rectTransform = base.transform as RectTransform;
		size = rectTransform.sizeDelta.x;
		initialScale = base.transform.localScale.x;
		base.transform.localScale = Vector3.zero;
		visibilityController.SetDesiredAlpha(0f);
		SetInputEnabled(enabled: false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (IsVisible)
		{
			SetInputEnabled(enabled: false);
			ClearCallbacks();
		}
	}

	private void ClearCallbacks()
	{
		this.OnSelected = null;
	}

	private void Update()
	{
		if (IsVisible)
		{
			lastOpenFrame = Time.frameCount;
			UpdateCursor();
		}
		void UpdateCursor()
		{
			int cursorIndex = GetCursorIndex();
			if (cursorIndex != highlightedIndex)
			{
				if (InputManager.UsingGamepad && highlightedIndex >= 0 && cursorIndex < 0)
				{
					SelectInternal();
				}
				else
				{
					highlightedIndex = cursorIndex;
					ui.SetHighlightedIndex(highlightedIndex, forced: false);
				}
			}
		}
	}

	public static bool TryShow(RadialMenuMode mode, Action<int> OnSelected)
	{
		if (!SingletonBehaviour<RadialMenu>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<RadialMenu>.Instance.TryShowInternal(mode, OnSelected);
	}

	public static void Hide()
	{
		if (SingletonBehaviour<RadialMenu>.HasInstance)
		{
			SingletonBehaviour<RadialMenu>.Instance.HideInternal();
		}
	}

	public static void Select()
	{
		if (SingletonBehaviour<RadialMenu>.HasInstance)
		{
			SingletonBehaviour<RadialMenu>.Instance.SelectInternal();
		}
	}

	public static void Cancel()
	{
		if (SingletonBehaviour<RadialMenu>.HasInstance)
		{
			SingletonBehaviour<RadialMenu>.Instance.CancelInternal();
		}
	}

	public static void AddOption(Sprite icon, int selectionIndex = -1)
	{
		if (SingletonBehaviour<RadialMenu>.HasInstance)
		{
			SingletonBehaviour<RadialMenu>.Instance.AddOptionInternal(icon, selectionIndex);
		}
	}

	public static void ClearOptions()
	{
		if (SingletonBehaviour<RadialMenu>.HasInstance)
		{
			SingletonBehaviour<RadialMenu>.Instance.ClearOptionsInternal();
		}
	}

	public static void DistributeOptions()
	{
		if (SingletonBehaviour<RadialMenu>.HasInstance)
		{
			SingletonBehaviour<RadialMenu>.Instance.DistributeOptionsInternal();
		}
	}

	public static void ClearHighlight()
	{
		if (SingletonBehaviour<RadialMenu>.HasInstance)
		{
			SingletonBehaviour<RadialMenu>.Instance.ClearHighlightInternal();
		}
	}

	public static void SetHighlightedIndex(int index)
	{
		if (SingletonBehaviour<RadialMenu>.HasInstance)
		{
			SingletonBehaviour<RadialMenu>.Instance.SetHighlightedIndexInternal(index);
		}
	}

	public static RadialMenuOptionUi GetUnusedOptionUi(Transform parent)
	{
		if (!SingletonBehaviour<RadialMenu>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<RadialMenu>.Instance.GetUnusedOptionUiInternal(parent);
	}

	public static void ReturnOptionUi(RadialMenuOptionUi optionUi)
	{
		if (SingletonBehaviour<RadialMenu>.HasInstance)
		{
			SingletonBehaviour<RadialMenu>.Instance.ReturnOptionUiInternal(optionUi);
		}
	}

	private bool TryShowInternal(RadialMenuMode mode, Action<int> OnSelected)
	{
		if (mode == currentMode)
		{
			HideInternal();
			return false;
		}
		ClearHighlightInternal();
		if (visibilityRoutine != null)
		{
			StopCoroutine(visibilityRoutine);
		}
		visibilityRoutine = StartCoroutine(ShowRoutine());
		return true;
		IEnumerator ShowRoutine()
		{
			currentMode = mode;
			this.OnSelected = OnSelected;
			SetInputEnabled(enabled: true);
			DOTween.To(() => visibilityController.DesiredAlpha, delegate(float alpha)
			{
				visibilityController.SetDesiredAlpha(alpha);
			}, 1f, fadeDuration);
			base.transform.DOScale(Vector3.one * initialScale, scaleDuration).SetEase(Ease.OutQuad);
			yield return new WaitForSeconds(scaleDuration);
		}
	}

	private void HideInternal()
	{
		if (IsVisible)
		{
			if (visibilityRoutine != null)
			{
				StopCoroutine(visibilityRoutine);
			}
			visibilityRoutine = StartCoroutine(HideRoutine());
		}
		IEnumerator HideRoutine()
		{
			currentMode = RadialMenuMode.None;
			ClearCallbacks();
			SetInputEnabled(enabled: false);
			DOTween.To(() => visibilityController.DesiredAlpha, delegate(float alpha)
			{
				visibilityController.SetDesiredAlpha(alpha);
			}, 0f, fadeDuration);
			base.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InQuad);
			yield return new WaitForSeconds(scaleDuration);
		}
	}

	private void SelectInternal()
	{
		if (highlightedIndex >= 0)
		{
			lastSelectionFrame = Time.frameCount;
			if (ui.Options[highlightedIndex].SelectionIndex >= 0)
			{
				this.OnSelected(ui.Options[highlightedIndex].SelectionIndex);
			}
			else
			{
				this.OnSelected(highlightedIndex);
			}
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.RadialMenuSelect);
		}
	}

	private void CancelInternal()
	{
		HideInternal();
	}

	private void AddOptionInternal(Sprite icon, int selectionIndex)
	{
		ui.AddOption(icon, selectionIndex);
	}

	private void ClearOptionsInternal()
	{
		ui.ClearOptions();
	}

	private void DistributeOptionsInternal()
	{
		ui.DistributeOptions();
	}

	private void ClearHighlightInternal()
	{
		if (highlightedIndex >= 0)
		{
			highlightedIndex = -1;
			ui.SetHighlightedIndex(highlightedIndex, forced: true);
		}
	}

	private void SetHighlightedIndexInternal(int index)
	{
		highlightedIndex = index;
		ui.SetHighlightedIndex(highlightedIndex, forced: false);
	}

	private RadialMenuOptionUi GetUnusedOptionUiInternal(Transform parent)
	{
		EnsurePoolParentExists();
		RadialMenuOptionUi result = null;
		while (result == null)
		{
			if (!optionUiPool.TryPop(out result))
			{
				result = UnityEngine.Object.Instantiate(optionUiPrefab);
			}
		}
		result.gameObject.SetActive(value: true);
		result.transform.SetParent(parent);
		result.RectTransform.anchoredPosition3D = Vector3.zero;
		result.RectTransform.sizeDelta = Vector2.zero;
		result.transform.localScale = Vector3.one;
		return result;
	}

	private void ReturnOptionUiInternal(RadialMenuOptionUi optionUi)
	{
		if (optionUiPool.Count >= maxOptionUiPoolSize)
		{
			UnityEngine.Object.Destroy(optionUi.gameObject);
			return;
		}
		optionUi.gameObject.SetActive(value: false);
		optionUi.transform.SetParent(optionUiPoolParent);
		optionUiPool.Push(optionUi);
	}

	private void EnsurePoolParentExists()
	{
		if (!(optionUiPoolParent != null))
		{
			GameObject obj = new GameObject("Info feed message pool");
			UnityEngine.Object.DontDestroyOnLoad(obj);
			optionUiPoolParent = obj.transform;
		}
	}

	private void SetInputEnabled(bool enabled)
	{
		InputManager.SetIsRadialMenuInputEnabled(enabled);
	}

	private int GetCursorIndex()
	{
		bool isInDeadZone;
		Vector2 cursorPosition = GetCursorPosition(out isInDeadZone);
		if (isInDeadZone)
		{
			return -1;
		}
		float num = 360f / (float)ui.Options.Count;
		return BMath.RoundToInt(BMath.FloorToMultipleOf((90f - cursorPosition.GetAngleDeg()).WrapAngle360Deg(), num) / num);
		Vector2 GetCursorPosition(out bool reference)
		{
			if (PlayerInput.UsingKeyboard)
			{
				Vector2 vector = ((Mouse.current != null) ? Mouse.current.position.ReadValue() : Vector2.zero);
				Vector2 vector2 = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
				cursorPosition = vector - vector2;
				reference = cursorPosition.Approximately(Vector2.zero, mouseDeadZone);
				return cursorPosition;
			}
			if (PlayerInput.UsingGamepad && InputManager.CurrentGamepad != null && !InputManager.CurrentModeMask.HasMode(InputMode.ForceDisabled))
			{
				cursorPosition = InputManager.CurrentGamepad.rightStick.value;
				reference = cursorPosition.Approximately(Vector2.zero, gamepadDeadZone);
				return cursorPosition;
			}
			reference = true;
			return Vector2.zero;
		}
	}
}
