using UnityEngine;

public class FollowRigidbody : MonoBehaviour
{
	private Rigidbody targetRigidbody;

	private void OnDisable()
	{
		targetRigidbody = null;
	}

	public void SetTarget(Rigidbody targetRigidbody)
	{
		this.targetRigidbody = targetRigidbody;
	}

	private void Update()
	{
		if (!(targetRigidbody == null))
		{
			base.transform.position = targetRigidbody.worldCenterOfMass;
		}
	}
}
