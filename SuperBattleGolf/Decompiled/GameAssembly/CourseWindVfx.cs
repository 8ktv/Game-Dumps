using UnityEngine;

public class CourseWindVfx : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particles;

	[SerializeField]
	private CourseWindVfxSettings settings;

	private float previousInterpolation = -1f;

	public void SetInterpolation(float interpolation, bool forced = false)
	{
		if (forced || previousInterpolation != interpolation)
		{
			settings.ApplySettings(particles, interpolation);
			previousInterpolation = interpolation;
		}
	}
}
