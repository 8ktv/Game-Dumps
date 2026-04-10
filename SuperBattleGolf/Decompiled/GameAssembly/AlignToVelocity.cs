using UnityEngine;

public class AlignToVelocity : MonoBehaviour
{
	private Vector3 previousPosition;

	private void OnEnable()
	{
		previousPosition = base.transform.position;
	}

	private void Update()
	{
		if (!(previousPosition == base.transform.position))
		{
			Vector3 forward = base.transform.position - previousPosition;
			base.transform.rotation = Quaternion.LookRotation(forward);
			previousPosition = base.transform.position;
		}
	}
}
