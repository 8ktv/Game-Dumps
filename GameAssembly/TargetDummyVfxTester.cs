using UnityEngine;
using UnityEngine.InputSystem;

public class TargetDummyVfxTester : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private ParticleSystem hitVfx;

	[SerializeField]
	private ParticleSystem spinClockwiseVfx;

	[SerializeField]
	private ParticleSystem spinCounterClockwiseVfx;

	private void Update()
	{
		if (Keyboard.current[Key.W].wasPressedThisFrame)
		{
			Hit("hit_front");
		}
		if (Keyboard.current[Key.S].wasPressedThisFrame)
		{
			Hit("hit_back");
		}
		if (Keyboard.current[Key.D].wasPressedThisFrame)
		{
			Hit("hit_spin_cw", spinClockwiseVfx);
		}
		if (Keyboard.current[Key.A].wasPressedThisFrame)
		{
			Hit("hit_spin_ccw", spinCounterClockwiseVfx);
		}
	}

	private void Hit(string trigger, ParticleSystem extraParticles = null)
	{
		hitVfx.Play();
		animator.SetTrigger(trigger);
		if (extraParticles != null)
		{
			extraParticles.Play();
		}
	}
}
