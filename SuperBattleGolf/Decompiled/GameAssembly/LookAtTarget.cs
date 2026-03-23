using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
	[SerializeField]
	private Transform lookAtTarget;

	private static float minDistance = 0.2f;

	private static float minDistanceSqr = minDistance * minDistance;

	public void SetLookAtTarget(Transform newTarget)
	{
		lookAtTarget = newTarget;
		UpdateRotation();
	}

	private void Update()
	{
		UpdateRotation();
	}

	private void UpdateRotation()
	{
		if ((bool)lookAtTarget && !(Vector3.SqrMagnitude(lookAtTarget.position - base.transform.position) < minDistanceSqr))
		{
			base.transform.rotation = Quaternion.LookRotation(lookAtTarget.position - base.transform.position);
		}
	}
}
