using System.Collections.Generic;
using Brimstone.Physics;
using UnityEngine;

public static class RigidbodyExtensions
{
	public static List<Collider> GetAttachedColliders(this Rigidbody rigidbody, bool includeInactive = false, bool includeTriggers = false)
	{
		List<Collider> list = new List<Collider>();
		Collider[] componentsInChildren = rigidbody.GetComponentsInChildren<Collider>(includeInactive);
		foreach (Collider collider in componentsInChildren)
		{
			if (!includeTriggers && collider.isTrigger)
			{
				continue;
			}
			if (collider.gameObject.activeInHierarchy && collider.enabled)
			{
				if (collider.attachedRigidbody != rigidbody)
				{
					continue;
				}
			}
			else if (collider.GetComponentInParent<Rigidbody>(includeInactive: true) != rigidbody)
			{
				continue;
			}
			list.Add(collider);
		}
		return list;
	}

	public static void SetCenterOfMassAndInertiaTensor(this Rigidbody rigidbody, Vector3 localCenterOfMass)
	{
		InertiaTensor inertiaTensor = InertiaTensor.FromPrincipal(rigidbody.inertiaTensor, rigidbody.inertiaTensorRotation);
		inertiaTensor.TranslateFromCenterOfMass(rigidbody.centerOfMass, localCenterOfMass, rigidbody.mass);
		inertiaTensor.ToPrincipal(out var principalMomentsOfInertia, out var principalRotation);
		rigidbody.centerOfMass = localCenterOfMass;
		rigidbody.inertiaTensor = principalMomentsOfInertia;
		rigidbody.inertiaTensorRotation = principalRotation;
	}
}
