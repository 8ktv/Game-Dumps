using UnityEngine;

public class PlayerVfx : MonoBehaviour
{
	[SerializeField]
	private PlayerInfo playerInfo;

	[SerializeField]
	private PlayerMovementVfx movementVfx;

	[SerializeField]
	private PlayerStateVfx stateVfx;

	private VoiceChatVfx voiceChatVfx;

	public PlayerInfo PlayerInfo => playerInfo;

	private void OnEnable()
	{
		PlayerInfo.AnimatorIo.Footstep += OnFootstep;
		PlayerInfo.AnimatorIo.FootLifted += OnFootLifted;
	}

	private void OnDisable()
	{
		PlayerInfo.AnimatorIo.Footstep -= OnFootstep;
		PlayerInfo.AnimatorIo.FootLifted -= OnFootLifted;
	}

	private void Awake()
	{
		movementVfx.Initialize(this);
		if (VfxPersistentData.TryGetPooledVfx(VfxType.VoiceChat, out var particleSystem))
		{
			if (!particleSystem.TryGetComponent<VoiceChatVfx>(out voiceChatVfx))
			{
				particleSystem.ReturnToPool();
				return;
			}
			voiceChatVfx.transform.SetParent(playerInfo.HeadBone);
			voiceChatVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}

	public void OnWillBeDestroyed()
	{
		if ((bool)voiceChatVfx)
		{
			voiceChatVfx.AsPoolable.ReturnToPool();
		}
	}

	private void Update()
	{
		UpdateVoiceChatVfx();
		bool ShouldPlayVoiceChatVfx()
		{
			if (!PlayerInfo.VoiceChat.voiceNetworker.IsTalking)
			{
				return false;
			}
			if (!playerInfo.Movement.IsVisible)
			{
				return false;
			}
			if (playerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			return true;
		}
		void UpdateVoiceChatVfx()
		{
			if (!(voiceChatVfx == null))
			{
				bool flag = ShouldPlayVoiceChatVfx();
				voiceChatVfx.SetPlaying(flag);
				if (flag)
				{
					voiceChatVfx.SetIntensity((playerInfo.VoiceChat.voiceNetworker.SlowSmoothedNormalizedVolume > 0.9f) ? 1f : 0f);
				}
			}
		}
	}

	public void PlaySpeedBoostEffects()
	{
		stateVfx.PlaySpeedBoostEffects();
	}

	public void StopSpeedBoostEffects()
	{
		stateVfx.StopSpeedBoostEffects();
	}

	public void SetSpeedUpVfxHidden(bool hidden)
	{
		stateVfx.SetSpeedUpVfxHidden(hidden);
	}

	private void OnFootstep(Foot foot)
	{
		if (foot == Foot.Left)
		{
			movementVfx.OnLeftFootstep();
		}
		else
		{
			movementVfx.OnRightFootstep();
		}
	}

	private void OnFootLifted(Foot foot)
	{
		if (foot == Foot.Left)
		{
			movementVfx.OnLeftFootLifted();
		}
		else
		{
			movementVfx.OnRightFootLifted();
		}
	}

	public void SetIsWadingInWater(bool isWading)
	{
		movementVfx.SetIsWadingInWater(isWading);
	}

	public void SetWadingWaterWorldHeight(float worldHeight)
	{
		movementVfx.SetWadingWaterWorldHeight(worldHeight);
	}
}
