using UnityEngine;

public class FollowTarget : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private Transform target;

	[SerializeField]
	private bool copyRotation;

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnBUpdate()
	{
		if (!(target == null))
		{
			base.transform.position = target.position;
			if (copyRotation)
			{
				base.transform.rotation = target.rotation;
			}
		}
	}
}
