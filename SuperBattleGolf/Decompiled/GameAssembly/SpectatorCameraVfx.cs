using System.Collections.Generic;
using UnityEngine;

public class SpectatorCameraVfx : MonoBehaviour
{
	private const float emoteCooldown = 0.5f;

	private const float mouthOpenMaxAngle = 60f;

	[SerializeField]
	private LocalSpectatorCameraFollower spectatorCamera;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Transform mouth;

	[SerializeField]
	private ParticleSystem talkingParticles;

	private bool isTalking;

	private Quaternion defaultMouthLocalRotation;

	private double lastEmoteTimestamp = double.MinValue;

	private static readonly int onEmoteParameterHash = Animator.StringToHash("on_emote");

	private static readonly int angryParameterHash = Animator.StringToHash("angry");

	private static readonly int shockedParameterHash = Animator.StringToHash("shocked");

	private static readonly int sadParameterHash = Animator.StringToHash("sad");

	private static readonly int confusedParameterHash = Animator.StringToHash("confused");

	private static readonly int cheerParameterHash = Animator.StringToHash("cheer");

	private static readonly Dictionary<SpectatorEmote, int> emoteTriggers = new Dictionary<SpectatorEmote, int>
	{
		[SpectatorEmote.Heart] = angryParameterHash,
		[SpectatorEmote.Shocked] = shockedParameterHash,
		[SpectatorEmote.Worried] = sadParameterHash,
		[SpectatorEmote.Confused] = confusedParameterHash,
		[SpectatorEmote.Cheer] = cheerParameterHash
	};

	private void Awake()
	{
		defaultMouthLocalRotation = mouth.localRotation;
	}

	public void SetTalking(bool talking)
	{
		if (talking != isTalking)
		{
			isTalking = talking;
			if (talking)
			{
				talkingParticles.Play();
			}
			else
			{
				talkingParticles.Stop();
			}
		}
	}

	public void SetTalkingIntensity(float intensity)
	{
		float x = BMath.LerpClamped(0f, 60f, intensity);
		mouth.localRotation = Quaternion.Euler(x, 0f, 0f) * defaultMouthLocalRotation;
	}

	public void PlayEmote(SpectatorEmote emote, bool isLocalPlayer, bool isTestScene = false)
	{
		if (isTestScene)
		{
			if (emoteTriggers.TryGetValue(emote, out var value))
			{
				animator.SetTrigger(value);
				animator.SetTrigger(onEmoteParameterHash);
			}
		}
		else
		{
			if (BMath.GetTimeSince(lastEmoteTimestamp) < 0.5f)
			{
				return;
			}
			if (!GameManager.SpectatorEmoteSettings.emotesByType.TryGetValue(emote, out var value2))
			{
				Debug.LogError($"Could not find settings for spectator emote of type {emote}", base.gameObject);
				return;
			}
			lastEmoteTimestamp = Time.timeAsDouble;
			if (emoteTriggers.TryGetValue(emote, out var value3))
			{
				animator.SetTrigger(value3);
				animator.SetTrigger(onEmoteParameterHash);
			}
			VfxType vfxType = (isLocalPlayer ? value2.localPlayerVfxType : value2.remotePlayerVfxType);
			if (vfxType != VfxType.None)
			{
				if (!VfxPersistentData.TryGetPooledVfx(vfxType, out var particleSystem))
				{
					Debug.LogError($"Failed to get spectator emote VFX of type {vfxType}");
					return;
				}
				particleSystem.transform.SetParent(isLocalPlayer ? GameManager.Camera.transform : base.transform);
				particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				particleSystem.Play();
			}
		}
	}
}
