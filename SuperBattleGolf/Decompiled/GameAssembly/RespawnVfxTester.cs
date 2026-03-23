using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnVfxTester : MonoBehaviour
{
	[SerializeField]
	private GameObject respawnVfxPrefab;

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			PlayRespawnVfx();
		}
	}

	private void PlayRespawnVfx()
	{
		GameObject obj = Object.Instantiate(respawnVfxPrefab, base.transform);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		obj.GetComponent<RespawnVfx>().PlayAnimation();
	}
}
