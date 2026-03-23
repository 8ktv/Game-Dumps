using UnityEngine;

public class SwingSlashVfx : MonoBehaviour
{
	[SerializeField]
	private ParticleSystemRenderer swingSlashRenderer;

	[SerializeField]
	private ParticleSystem swingSpecks;

	[SerializeField]
	private GameObject niceShotSpecks;

	[SerializeField]
	private GameObject overchargeSpecks;

	public void SetData(float power, bool isPerfectShot, bool isOvercharged)
	{
		if (SingletonBehaviour<VfxPersistentData>.HasInstance)
		{
			SwingSlashVfxData swingSlashData = VfxPersistentData.DefaultSwingSlashVfxSettings.GetSwingSlashData(power);
			SetData(power, isPerfectShot, isOvercharged, swingSlashData);
		}
	}

	public void SetData(float power, bool isPerfectShot, bool isOvercharged, SwingSlashVfxData data)
	{
		SetDataInternal(power, isPerfectShot, isOvercharged, data);
	}

	private void SetDataInternal(float power, bool isPerfectShot, bool isOvercharged, SwingSlashVfxData data)
	{
		niceShotSpecks.SetActive(isPerfectShot);
		overchargeSpecks.SetActive(isOvercharged);
		ParticleSystem.EmissionModule emission = swingSpecks.emission;
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(data.speckEmissionRate);
		emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)data.speckBurstRange.x, (short)data.speckBurstRange.y));
		swingSlashRenderer.SetMeshes(new Mesh[1] { data.mesh });
	}
}
