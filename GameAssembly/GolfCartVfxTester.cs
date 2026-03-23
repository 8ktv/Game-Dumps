using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class GolfCartVfxTester : MonoBehaviour
{
	[SerializeField]
	private GolfCartVfx golfCartVfx;

	[SerializeField]
	private GolfCartFlagPoleVfx flagPoleVfx;

	[SerializeField]
	private ParticleSystem golfCartImpactVfx;

	[SerializeField]
	private ParticleSystem golfCartJumpStartVfx;

	[SerializeField]
	private ParticleSystem golfCartJumpEndVfx;

	[SerializeField]
	private ParticleSystem golfCartHornShortVfx;

	[SerializeField]
	private ParticleSystem golfCartHornLongVfx;

	[SerializeField]
	private Shake golfCartShaker;

	[SerializeField]
	private float golfCartShakeDuration = 0.25f;

	[SerializeField]
	private Transform golfCartContainer;

	[SerializeField]
	private float jumpDuration = 0.25f;

	[SerializeField]
	private float jumpHeight = 1f;

	[SerializeField]
	private AnimationCurve jumpCurve;

	private float impactTimer;

	private bool jumping;

	private bool playingLongHorn;

	private void Update()
	{
		flagPoleVfx.SetForwardLean(Keyboard.current[Key.DownArrow].isPressed ? (-1) : (Keyboard.current[Key.UpArrow].isPressed ? 1 : 0));
		if (Keyboard.current[Key.E].wasPressedThisFrame)
		{
			TryPlayImpact();
		}
		if (Keyboard.current[Key.A].wasPressedThisFrame)
		{
			PlayHornShortVfx();
		}
		if (Keyboard.current[Key.S].wasPressedThisFrame)
		{
			ToggleHornLongVfx();
		}
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			golfCartVfx.SetPipeVfxPlaying(playing: true);
		}
		if (Keyboard.current[Key.Q].wasReleasedThisFrame)
		{
			golfCartVfx.SetPipeVfxPlaying(playing: false);
		}
		if (Keyboard.current[Key.Space].wasPressedThisFrame)
		{
			TryJump();
		}
		if (Keyboard.current[Key.LeftAlt].wasPressedThisFrame)
		{
			ToggleHeadlights();
		}
	}

	private void TryPlayImpact()
	{
		golfCartImpactVfx.Play();
		if (impactTimer > 0f)
		{
			impactTimer = golfCartShakeDuration;
		}
		else
		{
			PlayingImpact();
		}
	}

	private async void PlayingImpact()
	{
		golfCartShaker.ShakeFactor = 1f;
		impactTimer = golfCartShakeDuration;
		while (impactTimer > 0f)
		{
			float shakeFactor = impactTimer / golfCartShakeDuration;
			golfCartShaker.ShakeFactor = shakeFactor;
			impactTimer -= Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
			if (this == null)
			{
				return;
			}
		}
		golfCartShaker.ShakeFactor = 0f;
		impactTimer = 0f;
	}

	private void TryJump()
	{
		if (!jumping)
		{
			Jumping();
		}
	}

	private async void Jumping()
	{
		jumping = true;
		float timer = 0f;
		golfCartContainer.transform.localPosition = Vector3.zero;
		golfCartJumpStartVfx.Play();
		while (timer < jumpDuration)
		{
			float time = timer / jumpDuration;
			float t = jumpCurve.Evaluate(time);
			float y = Mathf.Lerp(0f, jumpHeight, t);
			golfCartContainer.transform.localPosition = new Vector3(0f, y, 0f);
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
			if (this == null)
			{
				return;
			}
		}
		golfCartJumpEndVfx.Play();
		golfCartContainer.transform.localPosition = Vector3.zero;
		jumping = false;
	}

	private void ToggleHeadlights()
	{
		golfCartVfx.SetHeadlightsOn(!golfCartVfx.HeadlightsAreOn);
	}

	private void PlayHornShortVfx()
	{
		golfCartHornShortVfx.Play();
	}

	private void ToggleHornLongVfx()
	{
		playingLongHorn = !playingLongHorn;
		if (playingLongHorn)
		{
			golfCartHornLongVfx.Play(withChildren: true);
		}
		else
		{
			golfCartHornLongVfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
	}
}
