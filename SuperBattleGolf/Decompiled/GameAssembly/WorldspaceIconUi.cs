using System;
using Brimstone.Geometry;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldspaceIconUi : MonoBehaviour
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private TextMeshProUGUI distanceLabel;

	[SerializeField]
	private RectTransform offscreenArrowPivot;

	[SerializeField]
	private Image offscreenArrow;

	[SerializeField]
	private Image onscreenArrow;

	[SerializeField]
	private CanvasGroup canvasGroup;

	private RectTransform rectTransform;

	private WorldspaceIconUiSettings settings;

	private Transform parent;

	private Transform distanceReference;

	private Vector3 worldPosition;

	private float distanceToCamera;

	private bool isOffscreen;

	private int displayedDistance = int.MaxValue;

	private void Awake()
	{
		rectTransform = base.transform as RectTransform;
	}

	private void OnEnable()
	{
		LocalizationManager.LanguageChanged += OnLocalizationLanguageChanged;
		GameSettings.GeneralSettings.DistanceUnitChanged = (Action)Delegate.Combine(GameSettings.GeneralSettings.DistanceUnitChanged, new Action(OnDistanceUnitChanged));
	}

	private void OnDisable()
	{
		LocalizationManager.LanguageChanged -= OnLocalizationLanguageChanged;
		GameSettings.GeneralSettings.DistanceUnitChanged = (Action)Delegate.Remove(GameSettings.GeneralSettings.DistanceUnitChanged, new Action(OnDistanceUnitChanged));
	}

	public void Initialize(WorldspaceIconUiSettings settings, Transform parent, Transform distanceReference, Sprite icon)
	{
		this.settings = settings;
		this.parent = parent;
		this.icon.sprite = icon;
		SetDistanceReference(distanceReference);
		distanceLabel.color = settings.DistanceLabelColor;
		offscreenArrow.color = settings.ArrowColor;
		onscreenArrow.color = settings.ArrowColor;
	}

	public void LateUpdate()
	{
		UpdatePosition();
		UpdateSize();
		UpdateAlpha();
		UpdateDistanceLabel();
		Vector3 GetOffscreenPosition(out float arrowAngle)
		{
			float num = settings.OffscreenDistanceFromScreenEdge * GameplayScreenspaceUiCanvas.Canvas.scaleFactor;
			float num2 = num * 2f;
			Rect rect = new Rect(num * Vector2.one, new Vector2((float)GameManager.Camera.pixelWidth - num2, (float)GameManager.Camera.pixelHeight - num2));
			Vector2 vector = GameManager.Camera.transform.InverseTransformPoint(worldPosition);
			Vector2 min = rect.min;
			Vector2 vector2 = new Vector2(rect.xMin, rect.yMax);
			Vector2 max = rect.max;
			Vector2 vector3 = new Vector2(rect.xMax, rect.yMin);
			if (!BGeo.RaySegmentIntersection2d(rect.center, vector, min, vector2, out var intersection) && !BGeo.RaySegmentIntersection2d(rect.center, vector, vector2, max, out intersection) && !BGeo.RaySegmentIntersection2d(rect.center, vector, max, vector3, out intersection))
			{
				BGeo.RaySegmentIntersection2d(rect.center, vector, vector3, min, out intersection);
			}
			arrowAngle = vector.GetAngleDeg();
			return intersection;
		}
		bool IsOffscreen(Vector3 screenPosition)
		{
			if (screenPosition.z < 0f)
			{
				return true;
			}
			Vector2 vector = screenPosition;
			float num = settings.MaxSize / 2f;
			if (IsScreenPointVisible(vector + new Vector2(0f - num, 0f - num)))
			{
				return false;
			}
			if (IsScreenPointVisible(vector + new Vector2(0f - num, num)))
			{
				return false;
			}
			if (IsScreenPointVisible(vector + new Vector2(num, num)))
			{
				return false;
			}
			if (IsScreenPointVisible(vector + new Vector2(num, 0f - num)))
			{
				return false;
			}
			return true;
		}
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
		bool ShouldBeCulled()
		{
			if (isOffscreen)
			{
				return false;
			}
			return distanceToCamera < settings.FadeStartDistance;
		}
		void UpdateAlpha()
		{
			if (ShouldBeCulled())
			{
				SetAlpha(0f);
			}
			else if (isOffscreen)
			{
				SetAlpha(1f);
			}
			else
			{
				SetAlpha(BMath.InverseLerpClamped(settings.FadeStartDistance, settings.FadeEndDistance, distanceToCamera));
			}
		}
		void UpdatePosition()
		{
			if (parent != null)
			{
				worldPosition = parent.position + settings.WorldOffset + parent.TransformVector(settings.LocalOffset);
			}
			distanceToCamera = (worldPosition - GameManager.Camera.transform.position).magnitude;
			Vector3 vector = CameraModuleController.WorldToScreenPoint(worldPosition);
			isOffscreen = IsOffscreen(vector);
			offscreenArrowPivot.gameObject.SetActive(isOffscreen);
			onscreenArrow.gameObject.SetActive(!isOffscreen);
			if (!isOffscreen)
			{
				rectTransform.position = vector;
				icon.rectTransform.pivot = new Vector2(0.5f, settings.OnscreenIconYPivot);
			}
			else
			{
				icon.rectTransform.pivot = new Vector2(0.5f, 0f);
				if (vector.z < 0f)
				{
					vector.x = 0f - vector.x;
					vector.y = 0f - vector.y;
				}
				rectTransform.position = GetOffscreenPosition(out var arrowAngle);
				offscreenArrowPivot.localRotation = Quaternion.Euler(0f, 0f, arrowAngle);
			}
		}
		void UpdateSize()
		{
			float num = ((!isOffscreen) ? BMath.RemapClamped(settings.MaxSizeDistance, settings.MinSizeDistance, settings.MaxSize, settings.MinSize, distanceToCamera) : settings.OffscreenSize);
			icon.rectTransform.sizeDelta = new Vector2(num, num);
			if (isOffscreen)
			{
				float num2 = num / 2f;
				Vector3 vector = (icon.rectTransform.localPosition.y + num2) * GameplayScreenspaceUiCanvas.Canvas.scaleFactor * Vector3.up;
				rectTransform.position -= vector;
				offscreenArrow.rectTransform.localPosition = (num2 + settings.OffscreenArrowDistance) * Vector2.right;
			}
			else
			{
				Vector3 vector2 = icon.rectTransform.anchoredPosition;
				vector2.y = BMath.RemapClamped(settings.MaxSizeDistance, settings.MinSizeDistance, settings.OnscreenMaxIconLocalYPosition, settings.OnscreenMinIconLocalYPosition, distanceToCamera);
				icon.rectTransform.anchoredPosition = vector2;
			}
		}
	}

	public void SetDistanceReference(Transform distanceReference)
	{
		this.distanceReference = distanceReference;
		distanceLabel.gameObject.SetActive(distanceReference != null);
	}

	private void UpdateDistanceLabel(bool forced = false)
	{
		if (!(distanceReference == null) && !(distanceLabel.color.a <= 0f))
		{
			float magnitude = (worldPosition - distanceReference.position).magnitude;
			float num = displayedDistance;
			displayedDistance = GameSettings.All.General.GetDistanceInCurrentUnits(magnitude);
			if (forced || (float)displayedDistance != num)
			{
				distanceLabel.text = string.Format(GameSettings.All.General.GetLocalizedDistanceUnitName(), displayedDistance);
			}
		}
	}

	private void SetAlpha(float alpha)
	{
		canvasGroup.alpha = alpha;
	}

	private void OnLocalizationLanguageChanged()
	{
		UpdateDistanceLabel(forced: true);
	}

	private void OnDistanceUnitChanged()
	{
		UpdateDistanceLabel(forced: true);
	}
}
