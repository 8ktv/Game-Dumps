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

	[SerializeField]
	private ParticleSystem cheer;

	[SerializeField]
	private ParticleSystem heartsLocal;

	[SerializeField]
	private ParticleSystem shockedLocal;

	[SerializeField]
	private ParticleSystem sadLocal;

	[SerializeField]
	private ParticleSystem confusedLocal;

	[SerializeField]
	private ParticleSystem cheerLocal;

	private Dictionary<SpectatorEmote, ParticleSystem> emoteVfx;

	private Dictionary<SpectatorEmote, ParticleSystem> emoteLocalVfx;

	private bool isTalking;

	private void Awake()
	{
		emoteVfx = new Dictionary<SpectatorEmote, ParticleSystem>
		{
			[SpectatorEmote.Heart] = hearts,
			[SpectatorEmote.Shocked] = shocked,
			[SpectatorEmote.Worried] = sad,
			[SpectatorEmote.Confused] = confused,
			[SpectatorEmote.Cheer] = cheer
		};
		emoteLocalVfx = new Dictionary<SpectatorEmote, ParticleSystem>
		{
			[SpectatorEmote.Heart] = heartsLocal,
			[SpectatorEmote.Shocked] = shockedLocal,
			[SpectatorEmote.Worried] = sadLocal,
			[SpectatorEmote.Confused] = confusedLocal,
			[SpectatorEmote.Cheer] = cheerLocal
		};
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			TryEmote(SpectatorEmote.Heart);
		}
		if (Keyboard.current[Key.W].wasPressedThisFrame)
		{
			TryEmote(SpectatorEmote.Shocked);
		}
		if (Keyboard.current[Key.E].wasPressedThisFrame)
		{
			TryEmote(SpectatorEmote.Worried);
		}
		if (Keyboard.current[Key.R].wasPressedThisFrame)
		{
			TryEmote(SpectatorEmote.Confused);
		}
		if (Keyboard.current[Key.T].wasPressedThisFrame)
		{
			TryEmote(SpectatorEmote.Cheer);
		}
		if (Keyboard.current[Key.LeftAlt].wasPressedThisFrame)
		{
			isTalking = !isTalking;
			spectator.SetTalking(isTalking);
		}
	}

	private void TryEmote(SpectatorEmote type)
	{
		bool isPressed = Keyboard.current[Key.LeftShift].isPressed;
		spectator.PlayEmote(type, isPressed, isTestScene: true);
		if (isPressed)
		{
			if (emoteLocalVfx.ContainsKey(type) && (bool)emoteLocalVfx[type])
			{
				emoteLocalVfx[type].Play();
			}
		}
		else if (emoteVfx.ContainsKey(type) && (bool)emoteVfx[type])
		{
			emoteVfx[type].Play();
		}
	}
}
