using UnityEngine;

[CreateAssetMenu(fileName = "Wind settings", menuName = "Settings/Gameplay/Wind")]
public class WindSettings : ScriptableObject
{
	[Header("Possible speed limits")]
	[Name("Minimum Possible Wind Speed (km/h)")]
	public int minPossibleWindSpeed;

	[Name("Maximum Possible Wind Speed (km/h)")]
	public int maxPossibleWindSpeed = 100;

	[Header("Wind settings")]
	[Name("Low Minimum Wind Speed (km/h)")]
	public int minLowWindSpeed = 15;

	[Name("Low Maximum Wind Speed (km/h)")]
	public int maxLowWindSpeed = 30;

	[Name("Moderate Minimum Wind Speed (km/h)")]
	public int minModerateWindSpeed = 20;

	[Name("Moderate Maximum Wind Speed (km/h)")]
	public int maxModerateWindSpeed = 60;

	[Name("High Minimum Wind Speed (km/h)")]
	public int minHighWindSpeed = 50;

	[Name("High Maximum Wind Speed (km/h)")]
	public int maxHighWindSpeed = 99;

	[Name("Minimum Wind Angle (deg)")]
	public int minWindAngle;

	[Name("Maximum Wind Angle (deg)")]
	public int maxWindAngle = 359;

	[HideInInspector]
	public float forceScale = 0.005f;

	[Name("Wind Sound Intensity Curve")]
	public AnimationCurve windSoundIntensityCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private void OnValidate()
	{
		minWindAngle = Mathf.Clamp(minWindAngle, 0, 359);
		maxWindAngle = Mathf.Clamp(maxWindAngle, 0, 359);
		if (maxWindAngle < minWindAngle)
		{
			maxWindAngle = minWindAngle;
		}
		if (minPossibleWindSpeed < 0)
		{
			minPossibleWindSpeed = 0;
		}
		if (maxPossibleWindSpeed < minPossibleWindSpeed)
		{
			maxPossibleWindSpeed = minPossibleWindSpeed;
		}
		if (minLowWindSpeed < minPossibleWindSpeed)
		{
			minLowWindSpeed = minPossibleWindSpeed;
		}
		if (maxLowWindSpeed > maxPossibleWindSpeed)
		{
			maxLowWindSpeed = maxPossibleWindSpeed;
		}
	}
}
