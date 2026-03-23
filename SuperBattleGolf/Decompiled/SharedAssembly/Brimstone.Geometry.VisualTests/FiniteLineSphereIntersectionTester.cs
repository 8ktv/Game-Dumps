#define DEBUG_DRAW
using UnityEngine;

namespace Brimstone.Geometry.VisualTests;

public class FiniteLineSphereIntersectionTester : MonoBehaviour
{
	[SerializeField]
	private Sphere sphere;

	private void Update()
	{
		if (sphere == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = base.transform.GetChild(0).position;
		if (!(position2 - position == Vector3.zero))
		{
			Vector3 closeIntersection;
			Vector3 farIntersection;
			switch (BGeo.SegmentSphereIntersection(base.transform.position, base.transform.GetChild(0).position, sphere.transform.position, sphere.radius, out closeIntersection, out farIntersection))
			{
			case 0:
				BDebug.DrawLine(position, position2, Color.red);
				break;
			case 1:
				BDebug.DrawLine(position, position2, Color.red);
				BDebug.DrawWireSphere(closeIntersection, 0.01f, Color.green);
				break;
			case 2:
				BDebug.DrawLine(position, closeIntersection, Color.red);
				BDebug.DrawWireSphere(closeIntersection, 0.01f, Color.green);
				BDebug.DrawLine(closeIntersection, farIntersection, Color.green);
				BDebug.DrawWireSphere(farIntersection, 0.01f, Color.green);
				BDebug.DrawLine(farIntersection, position2, Color.red);
				break;
			}
		}
	}
}
