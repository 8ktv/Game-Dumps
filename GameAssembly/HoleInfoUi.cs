using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class HoleInfoUi : SingletonBehaviour<HoleInfoUi>
{
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private GameObject courseParent;

	[SerializeField]
	private GameObject drivingRangeParent;

	[SerializeField]
	private LocalizeStringEvent holeNameLocalizeStringEvent;

	[SerializeField]
	private LocalizeStringEvent drivingRangeHoleNameLocalizeStringEvent;

	[SerializeField]
	private LocalizeStringEvent courseNameLocalizeStringEvent;

	[SerializeField]
	private TextMeshProUGUI holeNumberLabel;

	[SerializeField]
	private TextMeshProUGUI parLabel;

	[SerializeField]
	private TextMeshProUGUI drivingRangeParLabel;

	[SerializeField]
	private float defaultVisibilityDuration;

	[SerializeField]
	private float fadeAwayDuration;

	[SerializeField]
	private float drivingRangeHeight;

	private bool isVisible = true;

	public static bool IsVisible
	{
		get
		{
			if (SingletonBehaviour<HoleInfoUi>.HasInstance)
			{
				return SingletonBehaviour<HoleInfoUi>.Instance.isVisible;
			}
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		GameManager.CurrentCourseSet += OnCurrentCourseSet;
		CourseManager.CurrentHoleGlobalIndexChanged += OnCurrentHoleGlobalIndexChanged;
		LocalizationManager.LanguageChanged += OnLanguageChanged;
	}

	private void Start()
	{
		bool hasInstance = SingletonBehaviour<DrivingRangeManager>.HasInstance;
		courseParent.SetActive(!hasInstance);
		drivingRangeParent.SetActive(hasInstance);
		if (hasInstance)
		{
			UpdateAll();
			FadeAway(defaultVisibilityDuration);
			return;
		}
		OnCurrentHoleGlobalIndexChanged();
		if (CourseManager.MatchState > MatchState.TeeOff)
		{
			FadeAway(defaultVisibilityDuration);
		}
		else
		{
			CourseManager.MatchStateChanged += OnMatchStateChanged;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GameManager.CurrentCourseSet -= OnCurrentCourseSet;
		CourseManager.CurrentHoleGlobalIndexChanged -= OnCurrentHoleGlobalIndexChanged;
		LocalizationManager.LanguageChanged -= OnLanguageChanged;
		CourseManager.MatchStateChanged -= OnMatchStateChanged;
	}

	private void UpdateAll()
	{
		UpdateHoleName();
		UpdateCourseName();
		UpdateHoleNumberLabel();
		UpdateParLabel();
	}

	private void UpdateHoleName()
	{
		holeNameLocalizeStringEvent.StringReference = CourseManager.GetCurrentHoleLocalizedName();
		drivingRangeHoleNameLocalizeStringEvent.StringReference = holeNameLocalizeStringEvent.StringReference;
	}

	private void UpdateCourseName()
	{
		courseNameLocalizeStringEvent.StringReference = CourseManager.GetCurrentCourseLocalizedName();
	}

	private void UpdateHoleNumberLabel()
	{
		string text = (SingletonBehaviour<DrivingRangeManager>.HasInstance ? string.Empty : ((CourseManager.CurrentHoleCourseIndex < 0) ? string.Empty : string.Format(Localization.UI.HOLE_INFO_Hole, CourseManager.CurrentHoleCourseIndex + 1)));
		holeNumberLabel.text = text;
	}

	private void UpdateParLabel()
	{
		parLabel.text = string.Format(Localization.UI.HOLE_INFO_Par, CourseManager.GetCurrentHolePar());
		drivingRangeParLabel.text = parLabel.text;
	}

	private void FadeAway(float delay)
	{
		StartCoroutine(FadeAwayRoutine(delay));
		IEnumerator FadeAwayRoutine(float num)
		{
			if (num > 0f)
			{
				yield return new WaitForSeconds(num);
			}
			float initialAlpha = visibilityController.DesiredAlpha;
			for (float time = 0f; time < fadeAwayDuration; time += Time.deltaTime)
			{
				float t = BMath.EaseIn(time / fadeAwayDuration);
				visibilityController.SetDesiredAlpha(BMath.Lerp(initialAlpha, 0f, t));
				yield return null;
			}
			visibilityController.SetDesiredAlpha(0f);
			isVisible = false;
		}
	}

	private void OnCurrentCourseSet()
	{
		UpdateAll();
	}

	private void OnCurrentHoleGlobalIndexChanged()
	{
		UpdateAll();
	}

	private void OnLanguageChanged()
	{
		UpdateHoleNumberLabel();
		UpdateParLabel();
	}

	private void OnMatchStateChanged(MatchState previousState, MatchState currentState)
	{
		if (currentState > MatchState.TeeOff)
		{
			CourseManager.MatchStateChanged -= OnMatchStateChanged;
			FadeAway((previousState == MatchState.TeeOff) ? 0f : defaultVisibilityDuration);
		}
	}
}
