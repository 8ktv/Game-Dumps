using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindInfoUi : MonoBehaviour, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private WindInfoUISettings settings;

	[SerializeField]
	private TextMeshProUGUI windSpeedLabel;

	[SerializeField]
	private Image arrow;

	[SerializeField]
	private RawImage arrowStripes;

	[SerializeField]
	private Image arrowOutline;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Image backgroundStripes;

	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private bool hideWhenSwingPowerBarIsVisible;

	private int windSpeed;

	private Vector3 windDirection;

	private float currentWindAngle;

	private float initialVisibilityFadeAlpha;

	private double showChangeTimestamp;

	private bool showArrow;

	private bool showBackgroundStripes;

	private bool isShowing;

	private const int minScrollSpeed = 10;

	private const int maxScrollSpeed = 50;

	private const string windSpeedTextSeparator = "{0}";

	private void Start()
	{
		BUpdate.RegisterCallback(this);
		visibilityController.SetDesiredAlpha(0f);
		WindManager.WindUpdated += OnUpdateWindInfo;
		CourseManager.CurrentHoleGlobalIndexChanged += OnUpdateWindInfo;
		GameSettings.GeneralSettings.SpeedUnitChanged = (Action)Delegate.Combine(GameSettings.GeneralSettings.SpeedUnitChanged, new Action(OnSpeedUnitChanged));
		LocalizationManager.LanguageChanged += OnLanguageChanged;
		OnUpdateWindInfo();
	}

	private void OnDestroy()
	{
		BUpdate.DeregisterCallback(this);
		WindManager.WindUpdated -= OnUpdateWindInfo;
		CourseManager.CurrentHoleGlobalIndexChanged -= OnUpdateWindInfo;
		GameSettings.GeneralSettings.SpeedUnitChanged = (Action)Delegate.Remove(GameSettings.GeneralSettings.SpeedUnitChanged, new Action(OnSpeedUnitChanged));
		LocalizationManager.LanguageChanged -= OnLanguageChanged;
	}

	public void OnLateBUpdate()
	{
		UpdateArrowRotation();
		UpdateArrowScrollEffect();
	}

	private void UpdateArrowScrollEffect()
	{
		if (showArrow)
		{
			float num = (float)windSpeed * settings.arrowWindScrollSpeedScale;
			float y = (arrowStripes.uvRect.y - Time.deltaTime * num).WrapAngleDeg();
			arrowStripes.uvRect = new Rect(arrowStripes.uvRect.x, y, arrowStripes.uvRect.width, arrowStripes.uvRect.height);
		}
	}

	private void OnUpdateWindInfo()
	{
		if ((int)MatchSetupRules.GetValue(MatchSetupRules.Rule.Wind) == 0 || WindManager.CurrentWindSpeed <= 0)
		{
			UpdateVisible(hideEverything: true);
			return;
		}
		windSpeed = WindManager.CurrentWindSpeed;
		windDirection = WindManager.CurrentWindDirection;
		showArrow = windSpeed > 0;
		showBackgroundStripes = !showArrow;
		UpdateVisible();
		UpdateWindSpeedText();
		UpdateArrowColors();
		UpdateArrowRotation();
		UpdateBackgroundColor();
	}

	private void UpdateVisible(bool hideEverything = false)
	{
		visibilityController.SetDesiredAlpha(hideEverything ? 0f : 1f);
		arrow.gameObject.SetActive(showArrow);
		arrowOutline.gameObject.SetActive(showArrow);
		backgroundStripes.gameObject.SetActive(showBackgroundStripes);
		if (showArrow)
		{
			arrow.color = settings.arrowColorA;
			arrowStripes.color = settings.arrowStripeColorA;
			arrowOutline.color = settings.arrowOutlineColorA;
			background.color = settings.backgroundColorA;
		}
	}

	private void UpdateArrowRotation()
	{
		Camera camera = GameManager.Camera;
		if (!(camera == null))
		{
			Vector3 forward = camera.transform.forward;
			forward.y = 0f;
			currentWindAngle = Vector3.SignedAngle(forward, windDirection, Vector3.up);
			RectTransform rectTransform = arrow.rectTransform;
			Quaternion localRotation = (arrowOutline.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f - currentWindAngle));
			rectTransform.localRotation = localRotation;
		}
	}

	private void UpdateWindSpeedText()
	{
		string localizedSpeedUnitName = GameSettings.All.General.GetLocalizedSpeedUnitName();
		int speedInCurrentUnits = GameSettings.All.General.GetSpeedInCurrentUnits(windSpeed);
		string[] array = localizedSpeedUnitName.Split("{0}");
		if (array.Length == 2)
		{
			string arg = array[0];
			string text = array[1];
			if (string.IsNullOrWhiteSpace(text))
			{
				windSpeedLabel.text = $"<size=30>{arg}</size>{speedInCurrentUnits}";
			}
			else
			{
				windSpeedLabel.text = $"{speedInCurrentUnits}<size=30>{text}</size>";
			}
		}
		else
		{
			windSpeedLabel.text = string.Format(localizedSpeedUnitName, speedInCurrentUnits);
		}
	}

	private void UpdateArrowColors()
	{
		if (showArrow)
		{
			if (windSpeed <= settings.lowWindThreshold)
			{
				arrow.color = settings.arrowColorA;
				arrowStripes.color = settings.arrowStripeColorA;
				arrowOutline.color = settings.arrowOutlineColorA;
			}
			else if (windSpeed <= settings.mediumWindThreshold)
			{
				arrow.color = settings.arrowColorB;
				arrowStripes.color = settings.arrowStripeColorB;
				arrowOutline.color = settings.arrowOutlineColorB;
			}
			else
			{
				arrow.color = settings.arrowColorC;
				arrowStripes.color = settings.arrowStripeColorC;
				arrowOutline.color = settings.arrowOutlineColorC;
			}
		}
	}

	private void UpdateBackgroundColor()
	{
		background.color = GetBackgroundColor();
		static Color GetBackgroundColor()
		{
			if (!IsValidHoleIndex())
			{
				return SingletonNetworkBehaviour<WindManager>.Instance.DefaultCourseData.WindBackroundColor;
			}
			return (GameManager.AllCourses.allHoles[CourseManager.CurrentHoleGlobalIndex]?.ParentCourse)?.WindBackroundColor ?? SingletonNetworkBehaviour<WindManager>.Instance.DefaultCourseData.WindBackroundColor;
		}
		static bool IsValidHoleIndex()
		{
			if (CourseManager.CurrentHoleGlobalIndex >= 0)
			{
				return CourseManager.CurrentHoleGlobalIndex < GameManager.AllCourses.allHoles.Count;
			}
			return false;
		}
	}

	private void OnSpeedUnitChanged()
	{
		UpdateWindSpeedText();
	}

	private void OnLanguageChanged()
	{
		UpdateWindSpeedText();
	}
}
