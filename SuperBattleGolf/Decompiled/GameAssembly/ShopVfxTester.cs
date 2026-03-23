using UnityEngine;
using UnityEngine.InputSystem;

public class ShopVfxTester : MonoBehaviour
{
	[SerializeField]
	private PlayerCustomizeBuildingVfx buildingVfx;

	[SerializeField]
	private ParticleSystem doorParticles;

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			buildingVfx.OpenDoors();
			doorParticles.Play();
		}
	}
}
