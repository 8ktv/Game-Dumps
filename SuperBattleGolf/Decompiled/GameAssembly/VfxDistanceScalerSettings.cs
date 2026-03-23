using UnityEngine;

[CreateAssetMenu(fileName = "New VFX Distance Scaler Settings", menuName = "Settings/VFX/VFX Distance Scaler Settings")]
public class VfxDistanceScalerSettings : ScriptableObject
{
	[SerializeField]
	private Vector3 minimumScale;

	[SerializeField]
	private Vector3 maximumScale;

	[SerializeField]
	private float minimumDistance;

	[SerializeField]
	private float maximumDistance;

	[SerializeField]
	private AnimationCurve easing;

	public Vector3 MinimumScale => minimumScale;

	public Vector3 MaximumScale => maximumScale;

	public float MinimumDistanceSqr => minimumDistance * minimumDistance;

	public float MaximumDistanceSqr => maximumDistance * maximumDistance;

	public AnimationCurve Easing => easing;
}
