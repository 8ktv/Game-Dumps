using UnityEngine;

public class TrackablePath : MonoBehaviour
{
	public virtual OrientedPoint GetTrackingPoint(SplineTrackingMode mode, float trackingValue)
	{
		return new OrientedPoint(base.transform.position, base.transform.rotation);
	}
}
