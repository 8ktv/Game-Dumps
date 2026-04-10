using UnityEngine;
using UnityEngine.InputSystem;

public class JumpPadVfxTester : MonoBehaviour
{
	[SerializeField]
	private JumpPadVfx vfx;

	[SerializeField]
	private ParticleSystem activationParticles;

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			vfx.OnActivated(playVfx: false);
			activationParticles.Play();
		}
	}
}
