using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoffeeDispenserVfxTester : MonoBehaviour
{
	[SerializeField]
	private CoffeeDispenserVfx dispenserVfx;

	[SerializeField]
	private ParticleSystem testStartVfx;

	[SerializeField]
	private ParticleSystem testEndVfx;

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			Dispensing();
		}
	}

	private async void Dispensing()
	{
		testStartVfx.Play();
		dispenserVfx.Dispensing();
		await UniTask.WaitForSeconds(0.5f);
		testEndVfx.Play();
	}
}
