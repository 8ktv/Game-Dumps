using UnityEngine;

[ExecuteAlways]
public class SplineTracker : MonoBehaviour
{
	[SerializeField]
	private TrackablePath trackablePath;

	[SerializeField]
	private SplineTrackingMode trackingMode;

	[SerializeField]
	[HideInInspector]
	private float pointInterpolation;

	[SerializeField]
	[HideInInspector]
	private float normalizedInterpolation;

	private float previousPointInterpolation = -99999f;

	private float previousNormalizedInterpolation = -99999f;

	private float interpolationOffset;

	public SplineTrackingMode TrackingMode => trackingMode;

	public float PointInterpolation => pointInterpolation;

	public float NormalizedInterpolation => normalizedInterpolation;

	private void Start()
	{
		interpolationOffset = 0f;
	}

	private void Update()
	{
		if ((bool)trackablePath && (previousPointInterpolation != pointInterpolation || previousNormalizedInterpolation != normalizedInterpolation))
		{
			previousPointInterpolation = pointInterpolation;
			previousNormalizedInterpolation = normalizedInterpolation;
			UpdateTransform();
		}
	}

	private void UpdateTransform()
	{
		float num = 0f;
		switch (trackingMode)
		{
		case SplineTrackingMode.Point:
			num = pointInterpolation;
			break;
		case SplineTrackingMode.Normalized:
			num = normalizedInterpolation;
			break;
		}
		num += interpolationOffset;
		OrientedPoint trackingPoint = trackablePath.GetTrackingPoint(trackingMode, num);
		if (trackingPoint != null)
		{
			base.transform.position = trackingPoint.position;
			base.transform.rotation = trackingPoint.rotation;
		}
	}

	public void SetPath(TrackablePath path)
	{
		trackablePath = path;
	}

	public void AddInterpolationOffset(float offset)
	{
		interpolationOffset += offset;
	}
}
