using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointVfxTester : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private MeshRenderer baseMesh;

	[SerializeField]
	private MeshRenderer screenMesh;

	[SerializeField]
	private Material inactiveMaterial;

	[SerializeField]
	private Material activeMaterial;

	[SerializeField]
	private Material screenInactiveMaterial;

	[SerializeField]
	private Material screenActiveMaterial;

	[SerializeField]
	private ParticleSystem activeParticles;

	private void Start()
	{
		SetCheckpointActive(isActive: false);
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			SetCheckpointActive(isActive: false);
		}
		if (Keyboard.current[Key.W].wasPressedThisFrame)
		{
			SetCheckpointActive(isActive: true);
			activeParticles.Play(withChildren: true);
			animator.SetTrigger("activate");
		}
	}

	private void SetCheckpointActive(bool isActive)
	{
		baseMesh.sharedMaterial = (isActive ? activeMaterial : inactiveMaterial);
		screenMesh.sharedMaterial = (isActive ? screenActiveMaterial : screenInactiveMaterial);
	}
}
