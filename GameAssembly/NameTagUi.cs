using TMPro;
using UnityEngine;

public class NameTagUi : MonoBehaviour
{
	[SerializeField]
	private new TextMeshProUGUI tag;

	private CanvasGroup canvasGroup;

	private RectTransform rectTransform;

	private NameTagUiSettings settings;

	private Transform parent;

	private Vector3 localOffset;

	private Vector3 worldOffset;

	private Vector3 worldPosition;

	private float distanceToCamera;

	private PlayerInfo playerInfo;

	private bool nameTagIsPlayer;

	private void Awake()
	{
		rectTransform = base.transform as RectTransform;
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void Initialize(NameTagUiSettings settings, Transform parent, Vector3 localOffset, Vector3 worldOffset, string name, PlayerInfo playerInfo, bool nameTagIsPlayer)
	{
		this.settings = settings;
		this.parent = parent;
		this.localOffset = localOffset;
		this.worldOffset = worldOffset;
		this.playerInfo = playerInfo;
		this.nameTagIsPlayer = nameTagIsPlayer;
		if (nameTagIsPlayer && playerInfo.isLocalPlayer)
		{
			name = string.Empty;
		}
		tag.text = name;
		tag.fontSize = settings.MaxTextSize;
	}

	private void LateUpdate()
	{
		UpdatePosition();
		UpdateSize();
		UpdateAlpha();
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
			if (distanceToCamera > settings.FadeEndDistance)
			{
				return true;
			}
			if (rectTransform.position.z < 0f)
			{
				return true;
			}
			if (settings.FadeOnPlayerProne && playerInfo.Movement.TimeSinceDiveGrounded > (double)(settings.ProneFadeoutDelay + settings.ProneFadeoutDuration))
			{
				return true;
			}
			if (nameTagIsPlayer && playerInfo.Occlusion.TimeSinceVisible() > settings.PlayerOcclusionFadeoutDelay + settings.PlayerOcclusionFadeoutDuration)
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
		}
		void UpdateAlpha()
		{
			if (ShouldBeCulled())
			{
				canvasGroup.alpha = 0f;
			}
			else
			{
				canvasGroup.alpha = BMath.InverseLerpClamped(settings.FadeEndDistance, settings.FadeStartDistance, distanceToCamera);
				if (settings.FadeOnPlayerProne && playerInfo.Movement.DivingState == DivingState.OnGround)
				{
					canvasGroup.alpha *= BMath.InverseLerpClamped(settings.ProneFadeoutDelay + settings.ProneFadeoutDuration, settings.ProneFadeoutDuration, (float)playerInfo.Movement.TimeSinceDiveGrounded);
				}
				if (nameTagIsPlayer && playerInfo.Occlusion.IsOccluded())
				{
					canvasGroup.alpha *= BMath.InverseLerpClamped(settings.PlayerOcclusionFadeoutDelay + settings.PlayerOcclusionFadeoutDuration, settings.PlayerOcclusionFadeoutDuration, playerInfo.Occlusion.TimeSinceVisible());
				}
			}
		}
		void UpdatePosition()
		{
			if (nameTagIsPlayer && playerInfo.ActiveGolfCartSeat.IsValid())
			{
				worldPosition = playerInfo.HeadBone.position + worldOffset;
			}
			else if (parent != null)
			{
				worldPosition = parent.position + worldOffset + parent.TransformVector(localOffset);
			}
			rectTransform.position = CameraModuleController.WorldToScreenPoint(worldPosition);
			distanceToCamera = (worldPosition - GameManager.Camera.transform.position).magnitude;
		}
		void UpdateSize()
		{
			rectTransform.localScale = Vector3.one * BMath.RemapClamped(settings.MaxTextSizeDistance, settings.MinTextSizeDistance, 1f, settings.MinScale, distanceToCamera);
		}
	}

	public void SetName(string name)
	{
		tag.name = name;
	}
}
