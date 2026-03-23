using UnityEngine;

public class PlayerOcclusion : MonoBehaviour
{
	public const int transformCount = 2;

	public Transform pelvis;

	public Transform head;

	private int instanceId;

	public bool IsOccluded()
	{
		return PlayerOcclusionManager.IsOccluded(instanceId);
	}

	public float TimeSinceVisible()
	{
		return PlayerOcclusionManager.TimeSinceVisible(instanceId);
	}

	public void SetInstanceId(int instanceId)
	{
		this.instanceId = instanceId;
	}

	private void OnEnable()
	{
		PlayerOcclusionManager.RegisterInstance(this);
	}

	private void OnDisable()
	{
		PlayerOcclusionManager.DeregisterInstance(this);
	}
}
