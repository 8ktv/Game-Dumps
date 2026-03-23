#define DEBUG_DRAW
using UnityEngine;

namespace Brimstone.Geometry.VisualTests;

public class RayCapsuleIntersectionTester : MonoBehaviour
{
	[SerializeField]
	private Capsule capsule;

	private void Update()
	{
		if (capsule == null)
		{
			return;
		}
		Vector3 forward = base.transform.forward;
		if (!(forward == Vector3.zero))
		{
			Vector3 vector = (0.5f * capsule.length - capsule.radius) * capsule.transform.up;
			Vector3 closeIntersection;
			Vector3 farIntersection;
			int num = BGeo.RayCapsuleIntersection(base.transform.position, forward, capsule.transform.position - vector, capsule.transform.position + vector, capsule.radius, out closeIntersection, out farIntersection);
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
