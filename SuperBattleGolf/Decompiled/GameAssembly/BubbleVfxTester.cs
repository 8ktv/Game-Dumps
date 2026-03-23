using UnityEngine;

public class BubbleVfxTester : MonoBehaviour
{
	[SerializeField]
	private ParticleSystemRenderer particleSystemRenderer;

	[SerializeField]
	private Material normalMaterial;

	[SerializeField]
	private Material stencilMaterial;

	private Camera currentCamera;

	private void OnEnable()
	{
		if (GameManager.Camera != null)
		{
			currentCamera = GameManager.Camera;
		}
		else
		{
			currentCamera = Camera.main;
		}
	}

	private void Update()
	{
		if ((bool)currentCamera)
		{
			Material material = ((Vector3.Dot(base.transform.position - currentCamera.transform.position, Vector3.down) > 0f) ? stencilMaterial : normalMaterial);
			if (particleSystemRenderer.sharedMaterial != material)
			{
				particleSystemRenderer.sharedMaterial = material;
			}
		}
	}
}
