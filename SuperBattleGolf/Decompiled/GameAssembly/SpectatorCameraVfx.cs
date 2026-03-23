using System.Collections.Generic;
using UnityEngine;

public class SpectatorCameraVfx : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private ParticleSystem talkingParticles;

	private static readonly int talkingParameterHash = Animator.StringToHash("talking");

	private static readonly int onEmoteParameterHash = Animator.StringToHash("on_emote");

	private static readonly int angryParameterHash = Animator.StringToHash("angry");

	private static readonly int shockedParameterHash = Animator.StringToHash("shocked");

	private static readonly int sadParameterHash = Animator.StringToHash("sad");

	private static readonly int confusedParameterHash = Animator.StringToHash("confused");

	private static Dictionary<SpectatorCameraEmoteType, int> emoteTriggers = new Dictionary<SpectatorCameraEmoteType, int>
	{
		[SpectatorCameraEmoteType.Heart] = angryParameterHash,
		[SpectatorCameraEmoteType.Shocked] = shockedParameterHash,
		[SpectatorCameraEmoteType.Sad] = sadParameterHash,
		[SpectatorCameraEmoteType.Confused] = confusedParameterHash
	};

	private static Dictionary<SpectatorCameraEmoteType, VfxType> emoteVfx = new Dictionary<SpectatorCameraEmoteType, VfxType>
	{
		[SpectatorCameraEmoteType.Heart] = VfxType.SpectatorCameraHeart,
		[SpectatorCameraEmoteType.Shocked] = VfxType.SpectatorCameraShocked,
		[SpectatorCameraEmoteType.Sad] = VfxType.SpectatorCameraSad,
		[SpectatorCameraEmoteType.Confused] = VfxType.SpectatorCameraConfused
	};

	public void SetTalking(bool talking)
	{
		animator.SetBool(talkingParameterHash, talking);
		if (talking)
		{
			talkingParticles.Play();
		}
		else
		{
			talkingParticles.Stop();
		}
	}

	public void TryEmote(SpectatorCameraEmoteType emoteType)
	{
		if (emoteTriggers.ContainsKey(emoteType))
		{
			animator.SetTrigger(emoteTriggers[emoteType]);
			animator.SetTrigger(onEmoteParameterHash);
		}
		if (emoteVfx.ContainsKey(emoteType))
		{
			VfxManager.PlayPooledVfxLocalOnly(emoteVfx[emoteType], base.transform.position, Quaternion.identity);
		}
	}
}
