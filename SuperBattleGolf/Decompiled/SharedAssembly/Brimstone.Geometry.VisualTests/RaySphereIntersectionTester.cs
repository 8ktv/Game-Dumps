#define DEBUG_DRAW
using UnityEngine;

namespace Brimstone.Geometry.VisualTests;

public class RaySphereIntersectionTester : MonoBehaviour
{
	[SerializeField]
	private Sphere sphere;

	private void Update()
	{
		if (sphere == null)
		{
			return;
		}
		Vector3 forward = base.transform.forward;
		if (!(forward == Vector3.zero))
		{
			Vector3 closeIntersection;
			Vector3 farIntersection;
			int num = BGeo.RaySphereIntersection(base.transform.position, forward, sphere.transform.position, sphere.radius, out closeIntersection, out farIntersection);
			Vector3 end = base.transform.position + forward * 100000f;
			switch (num)
			{
			case 0:
				BDebug.DrawLine(base.transform.position, end, Color.red);
				break;
			case 1:
				BDebug.DrawLine(base.transform.position, end, Color.red);
				BDebug.DrawWireSphere(closeIntersection, 0.01f, Color.green);
				break;
			case 2:
				BDebug.DrawLine(base.transform.position, closeIntersection, Color.red);
				BDebug.DrawWireSphere(closeIntersection, 0.01f, Color.green);
				BDebug.DrawLine(closeIntersection, farIntersection, Color.green);
				BDebug.DrawWireSphere(farIntersection, 0.01f, Color.green);
				BDebug.DrawLine(farIntersection, end, Color.red);
				break;
			}
		}
	}
}
