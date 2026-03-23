using UnityEngine;

public class VfxDistanceScaler : MonoBehaviour
{
	[SerializeField]
	private Transform scaleTarget;

	[SerializeField]
	private ValueRamp alphaRamp;

	[SerializeField]
	private ParticleSystemRenderer[] particleRenderers;

	[SerializeField]
	private VfxDistanceScalerSettings settings;

	private Transform cameraTransform;

	private MaterialPropertyBlock propertyBlock;

	private void OnEnable()
	{
		cameraTransform = Camera.main.transform;
		if (propertyBlock == null)
		{
			propertyBlock = new MaterialPropertyBlock();
		}
		alphaRamp.ForceValue(1f);
		UpdateVfx();
	}

	private void Update()
	{
		UpdateVfx();
	}

	private void UpdateVfx()
	{
		if (!(scaleTarget == null) && !(cameraTransform == null) && !(settings == null))
		{
			float num = Vector3.SqrMagnitude(base.transform.position - cameraTransform.position) - settings.MinimumDistanceSqr;
			float num2 = settings.MaximumDistanceSqr - settings.MinimumDistanceSqr;
			float num3 = num / num2;
			float t = settings.Easing.Evaluate(num3);
			Vector3 localScale = Vector3.Lerp(settings.MinimumScale, settings.MaximumScale, t);
			if ((bool)scaleTarget)
			{
				scaleTarget.localScale = localScale;
			}
			else
			{
				base.transform.localScale = localScale;
			}
			alphaRamp.SetIncreasing(num3 > 0f);
			alphaRamp.Update(Time.deltaTime);
			propertyBlock.SetFloat("_Alpha", alphaRamp.GetValue());
			for (int i = 0; i < particleRenderers.Length; i++)
			{
				particleRenderers[i].SetPropertyBlock(propertyBlock);
			}
		}
	}
}
