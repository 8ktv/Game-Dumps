using UnityEngine;

public class IceBreakableFloorVfx : MonoBehaviour
{
	[SerializeField]
	private IceBreakableVfxParticleData[] particleData;

	public void SetData(BreakableIce source)
	{
		Vector3 localScale = source.transform.localScale;
		localScale.y = 1f;
		float num = localScale.x * localScale.z;
		for (int i = 0; i < particleData.Length; i++)
		{
			particleData[i].particles.emission.SetBurst(0, new ParticleSystem.Burst(0f, num * particleData[i].areaEmissionRatio));
			particleData[i].particles.transform.localScale = localScale;
		}
	}
}
