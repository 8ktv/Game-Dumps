using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SplineSequence : TrackablePath
{
	[SerializeField]
	private List<BezierSpline> splines = new List<BezierSpline>();

	public List<BezierSpline> Splines => splines;

	public override OrientedPoint GetTrackingPoint(SplineTrackingMode mode, float trackingValue)
	{
		if (splines.Count == 0)
		{
			return new OrientedPoint(base.transform.position, base.transform.rotation);
		}
		int num = 0;
		foreach (BezierSpline spline in Splines)
		{
			num += spline.Nodes.Count - 1;
		}
		switch (mode)
		{
		case SplineTrackingMode.Point:
		{
			while (trackingValue < 0f)
			{
				trackingValue += (float)num;
			}
			while (trackingValue > (float)num)
			{
				trackingValue -= (float)num;
			}
			int index = 0;
			float num2 = trackingValue;
			for (int i = 0; i < splines.Count; i++)
			{
				index = i;
				int count = splines[i].Nodes.Count;
				if (num2 < (float)(count - 1) || i == splines.Count - 1)
				{
					break;
				}
				num2 -= (float)(splines[i].Nodes.Count - 1);
			}
			return splines[index].GetOrientedPoint(num2);
		}
		case SplineTrackingMode.Normalized:
			return GetTrackingPoint(SplineTrackingMode.Point, (float)num * trackingValue);
		default:
			return new OrientedPoint(base.transform.position, base.transform.rotation);
		}
	}
}
