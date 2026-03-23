using UnityEngine;

public class BubbleVfxMaterialHandler : MonoBehaviour
{
	[SerializeField]
	private ParticleSystemRenderer particleSystemRenderer;

	[SerializeField]
	private Material normalMaterial;

	[SerializeField]
	private Material stencilMaterial;

	private LevelBoundsTracker cameraLevelBoundsTracker;

	private void OnEnable()
	{
		cameraLevelBoundsTracker = GameManager.CameraLevelBoundsTracker;
	}

	private void Update()
	{
		if (cameraLevelBoundsTracker == null)
		{
			return;
		}
		float num = float.NegativeInfinity;
		if (cameraLevelBoundsTracker.CurrentSecondaryHazardLocalOnly == null)
		{
			if (MainOutOfBoundsHazard.Type == OutOfBoundsHazard.Water)
			{
				num = cameraLevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly;
			}
		}
		else if (cameraLevelBoundsTracker.CurrentSecondaryHazardLocalOnly.Type == OutOfBoundsHazard.Water)
		{
			num = cameraLevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly;
		}
		Material material = ((cameraLevelBoundsTracker.transform.position.y > num) ? stencilMaterial : normalMaterial);
		if (particleSystemRenderer.sharedMaterial != material)
		{
			particleSystemRenderer.sharedMaterial = material;
		}
	}
}
