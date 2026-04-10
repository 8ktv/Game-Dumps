using UnityEngine;

public class RocketDriverEquipmentVfx : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem idleParticles;

	[SerializeField]
	private ParticleSystem startParticles;

	[SerializeField]
	private ParticleSystem overchargedParticles;

	[SerializeField]
	private ParticleSystem emberParticles;

	[SerializeField]
	private Vector2 emberStartSpeedRange;

	[SerializeField]
	private ParticleSystem lightParticles;

	[SerializeField]
	private Vector2 lightSizeRange;

	[SerializeField]
	private RocketDriverThrusterVfxData[] thrusters;

	[SerializeField]
	private AnimationCurve interpolationCurve;

	private float currentThrusterInterpolation = -1f;

	private bool isOvercharged;

	private void OnEnable()
	{
		idleParticles.Play();
		SetOvercharged(overcharged: false);
		SetThrusterPower(0f);
	}

	private void Start()
	{
		SetOvercharged(overcharged: false, forced: true);
		SetThrusterPower(0f, forced: true);
	}

	public void OnStartedAiming()
	{
		startParticles.Play();
	}

	public void SetOvercharged(bool overcharged, bool forced = false)
	{
		if (isOvercharged != overcharged || forced)
		{
			isOvercharged = overcharged;
			if (overcharged)
			{
				overchargedParticles.Play();
			}
			else
			{
				overchargedParticles.Stop();
			}
		}
	}

	public void SetThrusterPower(float interpolation, bool forced = false)
	{
		if (currentThrusterInterpolation != interpolation || forced)
		{
			currentThrusterInterpolation = interpolation;
			float t = interpolationCurve.Evaluate(interpolation);
			for (int i = 0; i < thrusters.Length; i++)
			{
				RocketDriverThrusterVfxData rocketDriverThrusterVfxData = thrusters[i];
				ParticleSystem.MainModule main = rocketDriverThrusterVfxData.particles.main;
				float num = BMath.Lerp(rocketDriverThrusterVfxData.sizeRange.x, rocketDriverThrusterVfxData.sizeRange.y, t);
				main.startSizeZ = new ParticleSystem.MinMaxCurve(num * 0.85f, num);
				ParticleSystem.CustomDataModule customData = rocketDriverThrusterVfxData.particles.customData;
				float constant = BMath.Lerp(rocketDriverThrusterVfxData.topRadiusRange.x, rocketDriverThrusterVfxData.topRadiusRange.y, t);
				customData.SetVector(ParticleSystemCustomData.Custom2, 1, new ParticleSystem.MinMaxCurve(constant));
			}
			ParticleSystem.MainModule main2 = emberParticles.main;
			float num2 = BMath.Lerp(emberStartSpeedRange.x, emberStartSpeedRange.y, t);
			main2.startSpeed = new ParticleSystem.MinMaxCurve(num2 * 0.33f, num2);
			ParticleSystem.MainModule main3 = lightParticles.main;
			float constant2 = BMath.Lerp(lightSizeRange.x, lightSizeRange.y, t);
			main3.startSize = new ParticleSystem.MinMaxCurve(constant2);
		}
	}
}
