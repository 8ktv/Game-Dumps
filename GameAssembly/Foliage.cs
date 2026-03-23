using UnityEngine;

public class Foliage : MonoBehaviour
{
	[SerializeField]
	private GameObject colliderParent;

	[SerializeField]
	[HideInInspector]
	private Collider[] colliders;

	private void Awake()
	{
		Collider[] array = colliders;
		foreach (Collider obj in array)
		{
			if (obj is MeshCollider meshCollider)
			{
				meshCollider.convex = true;
			}
			obj.gameObject.layer = LayerMask.NameToLayer("Foliage");
			obj.isTrigger = false;
			obj.hasModifiableContacts = true;
		}
	}

	private void Start()
	{
		PhysicsManager.RegisterFoliageColliders(colliders);
	}

	private void OnDestroy()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			PhysicsManager.DeregisterFoliageColliders(colliders);
		}
	}
}
