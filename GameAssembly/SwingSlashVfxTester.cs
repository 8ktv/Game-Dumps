using UnityEngine;
using UnityEngine.InputSystem;

public class SwingSlashVfxTester : MonoBehaviour
{
	[SerializeField]
	private SwingSlashVfxSettings settings;

	[SerializeField]
	private SwingSlashVfx slashVfx;

	[SerializeField]
	private ParticleSystem particles;

	private void Update()
	{
		if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
		{
			PlaySlash(0.25f);
		}
		if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
		{
			PlaySlash(0.5f);
		}
		if (Keyboard.current[Key.Digit3].wasPressedThisFrame)
		{
			PlaySlash(0.75f);
		}
		if (Keyboard.current[Key.Digit4].wasPressedThisFrame)
		{
			PlaySlash(1f);
		}
		if (Keyboard.current[Key.Digit5].wasPressedThisFrame)
		{
			PlaySlash(1.15f);
		}
	}

	private void PlaySlash(float power)
	{
		SwingSlashVfxData swingSlashData = settings.GetSwingSlashData(power);
		bool isPerfectShot = power == 1f;
		bool isOvercharged = power > 1f;
		slashVfx.SetData(power, isPerfectShot, isOvercharged, swingSlashData);
		particles.Play(withChildren: true);
	}
}
