using UnityEngine;
using UnityEngine.UI;

public class HoleProgressBarPlayerEntryUi : MonoBehaviour
{
	[SerializeField]
	private float localPlayerScale;

	[SerializeField]
	private Image playerIcon;

	[SerializeField]
	private GameObject speakingIcon;

	private RectTransform rectTransform;

	private PlayerInfo player;

	public float NormalizedProgress { get; private set; }

	public bool IsLocalPlayer
	{
		get
		{
			if (player != null)
			{
				return player.isLocalPlayer;
			}
			return false;
		}
	}

	private void Awake()
	{
		rectTransform = base.transform as RectTransform;
		BNetworkManager.LobbyOpened += OnLobbyOpened;
		BNetworkManager.LobbyJoined += OnLobbyJoined;
	}

	private void OnDestroy()
	{
		BNetworkManager.LobbyOpened -= OnLobbyOpened;
		BNetworkManager.LobbyJoined -= OnLobbyJoined;
	}

	public void Initialize(PlayerInfo player)
	{
		this.player = player;
		player.PlayerId.GuidChanged += OnPlayerGuidChanged;
		rectTransform.anchoredPosition = Vector2.zero;
		float num = (player.isLocalPlayer ? localPlayerScale : 1f);
		rectTransform.localScale = Vector3.one * num;
		SetIcon();
		OnLateUpdate();
	}

	public void OnLateUpdate()
	{
		UpdatePosition();
		UpdateIsSpeaking();
		void SetNormalizedProgress(float normalizedProgress)
		{
			NormalizedProgress = normalizedProgress;
			rectTransform.anchorMin = new Vector2(normalizedProgress, rectTransform.anchorMin.y);
			rectTransform.anchorMax = new Vector2(normalizedProgress, rectTransform.anchorMax.y);
		}
		void UpdateIsSpeaking()
		{
			bool active = player != null && player.VoiceChat.voiceNetworker.IsTalking;
			speakingIcon.SetActive(active);
		}
		void UpdatePosition()
		{
			if (player == null || !GolfHoleManager.HasMaxReferenceDistance)
			{
				SetNormalizedProgress(1f);
			}
			else
			{
				float num = BMath.Clamp01((player.transform.position - GolfHoleManager.MainHole.transform.position).magnitude / GolfHoleManager.MaxReferenceDistance);
				SetNormalizedProgress(1f - num);
			}
		}
	}

	public void OnReturnedToPool()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown && player != null)
		{
			player.PlayerId.GuidChanged -= OnPlayerGuidChanged;
		}
	}

	private void OnLobbyOpened()
	{
		if (base.isActiveAndEnabled)
		{
			SetIcon();
		}
	}

	private void OnLobbyJoined()
	{
		if (base.isActiveAndEnabled)
		{
			SetIcon();
		}
	}

	private void OnPlayerGuidChanged()
	{
		SetIcon();
	}

	private void SetIcon()
	{
		playerIcon.sprite = PlayerIconManager.GetPlayerIcon(player, PlayerIconManager.IconSize.Medium);
	}
}
