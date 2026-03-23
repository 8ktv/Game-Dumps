using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PostHitSpinTester : MonoBehaviour
{
	[SerializeField]
	private Rigidbody testRigidbody;

	[SerializeField]
	private float initialVelocity;

	[SerializeField]
	private float linearVelocity;

	[SerializeField]
	private float angularVelocity;

	[SerializeField]
	private ParticleSystem particles;

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			SimulatingHit().Forget();
		}
	}

	public async UniTaskVoid SimulatingHit()
	{
		testRigidbody.linearVelocity = Vector3.up * initialVelocity;
		await UniTask.WaitForSeconds(0.5f);
		particles.Play(withChildren: true);
		testRigidbody.linearVelocity = Vector3.up * linearVelocity;
		testRigidbody.angularVelocity = Vector3.right * angularVelocity;
	}
}
