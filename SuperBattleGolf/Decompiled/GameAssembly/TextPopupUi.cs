using System;
using TMPro;
using UnityEngine;

public class TextPopupUi : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text;

	private RectTransform rectTransform;

	private TextPopupUiSettings settings;

	private Transform parent;

	private Vector3 localOffset;

	private Vector3 worldOffset;

	private Vector3 worldPosition;

	private float distanceToCamera;

	private double initializationTimestamp;

	public event Action<TextPopupUi> Disappeared;

	private void Awake()
	{
		rectTransform = base.transform as RectTransform;
	}

	public void Initialize(TextPopupUiSettings settings, Transform parent, Vector3 localOffset, Vector3 worldOffset, string text)
	{
		this.settings = settings;
		this.parent = parent;
		this.localOffset = localOffset;
		this.worldOffset = worldOffset;
		this.text.text = text;
		this.text.fontSize = settings.MaxTextSize;
		this.text.color = settings.Color;
		initializationTimestamp = Time.timeAsDouble;
		UpdatePosition(0f);
		UpdateSize(0f);
		UpdateAlpha(0f);
	}

	public void LateUpdate()
	{
		float timeSince = BMath.GetTimeSince(initializationTimestamp);
		if (timeSince > settings.DurationIncludingFadeOut)
		{
			Disappear();
			return;
		}
		UpdatePosition(timeSince);
		UpdateSize(timeSince);
		UpdateAlpha(timeSince);
	}

	private void UpdatePosition(float age)
	{
		if (parent != null)
		{
			float num = BMath.Lerp(0f, settings.FinalHeightOffset, age / settings.HeightOffsetDuration);
			worldPosition = parent.position + worldOffset + parent.TransformVector(localOffset) + num * Vector3.up;
		}
		rectTransform.position = CameraModuleController.WorldToScreenPoint(worldPosition);
		distanceToCamera = (worldPosition - GameManager.Camera.transform.position).magnitude;
	}

	private void UpdateSize(float age)
	{
		float num = BMath.RemapClamped(settings.MaxTextSizeDistance, settings.MinTextSizeDistance, 1f, settings.MinScale, distanceToCamera);
		float num2 = BMath.RemapClamped(0f, settings.PopDuration, settings.PopSizeFactor, 1f, age, BMath.EaseIn);
		rectTransform.localScale = num * num2 * Vector3.one;
	}

	private void UpdateAlpha(float age)
	{
		if (ShouldBeCulled())
		{
			text.alpha = 0f;
			return;
		}
		float num = BMath.InverseLerpClamped(settings.FadeEndDistance, settings.FadeStartDistance, distanceToCamera);
		float num2 = BMath.EaseIn(BMath.InverseLerpClamped(settings.DurationIncludingFadeOut, settings.Duration, age));
		text.alpha = num * num2;
	}

	private bool ShouldBeCulled()
	{
		if (distanceToCamera > settings.FadeEndDistance)
		{
			return true;
		}
		if (rectTransform.position.z < 0f)
		{
			return true;
		}
		Vector2 vector = rectTransform.position;
		float num = rectTransform.sizeDelta.x / 2f;
		float num2 = rectTransform.sizeDelta.y / 2f;
		if (IsScreenPointVisible(vector + new Vector2(0f - num, 0f - num2)))
		{
			return false;
		}
		if (IsScreenPointVisible(vector + new Vector2(0f - num, num2)))
		{
			return false;
		}
		if (IsScreenPointVisible(vector + new Vector2(num, num2)))
		{
			return false;
		}
		if (IsScreenPointVisible(vector + new Vector2(num, 0f - num2)))
		{
			return false;
		}
		return true;
		static bool IsScreenPointVisible(Vector2 screenPoint)
		{
			if (screenPoint.x < 0f)
			{
				return false;
			}
			if (screenPoint.y < 0f)
			{
				return false;
			}
			if (screenPoint.x > (float)GameManager.Camera.pixelWidth)
			{
				return false;
			}
			if (screenPoint.y > (float)GameManager.Camera.pixelHeight)
			{
				return false;
			}
			return true;
		}
	}

	private void Disappear()
	{
		TextPopupManager.ReturnPopup(this);
		this.Disappeared?.Invoke(this);
	}
}
