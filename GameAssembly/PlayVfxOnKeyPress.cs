using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ParticleSystem))]
public class PlayVfxOnKeyPress : MonoBehaviour
{
	[SerializeField]
	private Key key = Key.PageDown;

	private void Update()
	{
		if (Keyboard.current[key].wasPressedThisFrame)
		{
			GetComponent<ParticleSystem>()?.Play(withChildren: true);
		}
	}
}
