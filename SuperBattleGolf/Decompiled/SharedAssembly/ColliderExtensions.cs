using System;
using UnityEngine;

public static class ColliderExtensions
{
	public static Bounds GetLocalBounds(this Collider collider)
	{
		if (collider is BoxCollider boxCollider)
		{
			return new Bounds(boxCollider.center, boxCollider.size);
		}
		if (collider is MeshCollider meshCollider)
		{
			return meshCollider.sharedMesh.bounds;
		}
		if (collider is SphereCollider sphereCollider)
		{
			return new Bounds(sphereCollider.center, Vector3.one * sphereCollider.radius * 2f);
		}
		if (collider is CapsuleCollider capsuleCollider)
		{
			return new Bounds(capsuleCollider.center, new Vector3(capsuleCollider.radius * 2f, capsuleCollider.height, capsuleCollider.radius * 2f));
		}
		if (collider is WheelCollider wheelCollider)
		{
			return new Bounds(wheelCollider.center, new Vector3(0.1f, wheelCollider.radius, wheelCollider.radius));
		}
		if (collider is CharacterController characterController)
		{
			return new Bounds(characterController.center, new Vector3(characterController.radius * 2f, characterController.height, characterController.radius * 2f));
		}
		throw new InvalidOperationException($"Getting local bounds is not supported for {collider.GetType()}");
	}
}
