using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpectatorCameraVfxTester : MonoBehaviour
{
	[SerializeField]
	private SpectatorCameraVfx spectator;

	[SerializeField]
	private ParticleSystem hearts;

	[SerializeField]
	private ParticleSystem shocked;

	[SerializeField]
	private ParticleSystem sad;

	[SerializeField]
	private ParticleSystem confused;

	private Dictionary<SpectatorCameraEmoteType, ParticleSystem> emoteVfx;

	private bool isTalking;

	private void Awake()
	{
		emoteVfx = new Dictionary<SpectatorCameraEmoteType, ParticleSystem>
		{
			[SpectatorCameraEmoteType.Heart] = hearts,
			[SpectatorCameraEmoteType.Shocked] = shocked,
			[SpectatorCameraEmoteType.Sad] = sad,
			[SpectatorCameraEmoteType.Confused] = confused
		};
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			TryEmote(SpectatorCameraEmoteType.Heart);
		}
		if (Keyboard.current[Key.W].wasPressedThisFrame)
		{
			TryEmote(SpectatorCameraEmoteType.Shocked);
		}
		if (Keyboard.current[Key.E].wasPressedThisFrame)
		{
			TryEmote(SpectatorCameraEmoteType.Sad);
		}
		if (Keyboard.current[Key.R].wasPressedThisFrame)
		{
			TryEmote(SpectatorCameraEmoteType.Confused);
		}
		if (Keyboard.current[Key.LeftAlt].wasPressedThisFrame)
		{
			isTalking = !isTalking;
			spectator.SetTalking(isTalking);
		}
	}

	private void TryEmote(SpectatorCameraEmoteType type)
	{
		spectator.TryEmote(type);
		if (emoteVfx.ContainsKey(type) && (bool)emoteVfx[type])
		{
			emoteVfx[type].Play();
		}
	}
}
