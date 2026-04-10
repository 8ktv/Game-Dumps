using UnityEngine;

[CreateAssetMenu(fileName = "New Course Wind VFX Settings", menuName = "Settings/VFX/Course Wind VFX Settings")]
public class CourseWindVfxSettings : ScriptableObject
{
	[SerializeField]
	private Vector2 emissionRange;

	[SerializeField]
	private float emissionLowerMultiplier;

	[SerializeField]
	private Vector2 zSpeedRange;

	[SerializeField]
	private float zSpeedLowerMultiplier;

	[SerializeField]
	private Vector2 lifetimeRange;

	[SerializeField]
	private float lifetimeLowerMultiplier;

	public void ApplySettings(ParticleSystem particles, float interpolation)
	{
		if (!(particles == null))
		{
			float num = BMath.Lerp(emissionRange.x, emissionRange.y, interpolation);
			float min = num * emissionLowerMultiplier;
			float num2 = BMath.Lerp(zSpeedRange.x, zSpeedRange.y, interpolation);
			float min2 = num2 * zSpeedLowerMultiplier;
			float num3 = BMath.Lerp(lifetimeRange.x, lifetimeRange.y, interpolation);
			float min3 = num3 * lifetimeLowerMultiplier;
			ParticleSystem.MainModule main = particles.main;
			ParticleSystem.EmissionModule emission = particles.emission;
			ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particles.velocityOverLifetime;
			main.startLifetime = new ParticleSystem.MinMaxCurve(min3, num3);
			emission.rateOverTime = new ParticleSystem.MinMaxCurve(min, num);
			velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(min2, num2);
		}
	}
}
