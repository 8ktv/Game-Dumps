using System;
using UnityEngine;

namespace Mirror;

public static class PredictionUtils
{
	public static void MoveRigidbody(GameObject source, GameObject destination, bool destroySource = true)
	{
		Rigidbody component = source.GetComponent<Rigidbody>();
		if (component == null)
		{
			throw new Exception($"Prediction: attempted to move {source}'s Rigidbody to the predicted copy, but there was no component.");
		}
		if (!destination.TryGetComponent<Rigidbody>(out var component2))
		{
			component2 = destination.AddComponent<Rigidbody>();
		}
		component2.mass = component.mass;
		component2.linearDamping = component.linearDamping;
		component2.angularDamping = component.angularDamping;
		component2.useGravity = component.useGravity;
		component2.isKinematic = component.isKinematic;
		component2.interpolation = component.interpolation;
		component2.collisionDetectionMode = component.collisionDetectionMode;
		component2.freezeRotation = component.freezeRotation;
		component2.constraints = component.constraints;
		component2.sleepThreshold = component.sleepThreshold;
		if (!component.isKinematic)
		{
			component2.linearVelocity = component.linearVelocity;
			component2.angularVelocity = component.angularVelocity;
		}
		if (destroySource)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	public static GameObject CopyRelativeTransform(GameObject source, Transform sourceChild, GameObject destination)
	{
		if (sourceChild == source.transform)
		{
			return destination;
		}
		GameObject gameObject = new GameObject(sourceChild.name);
		gameObject.transform.SetParent(destination.transform, worldPositionStays: true);
		gameObject.transform.localPosition = sourceChild.localPosition;
		gameObject.transform.localRotation = sourceChild.localRotation;
		gameObject.transform.localScale = sourceChild.localScale;
		gameObject.layer = sourceChild.gameObject.layer;
		return gameObject;
	}

	public static void MoveBoxColliders(GameObject source, GameObject destination, bool destroySource = true)
	{
		BoxCollider[] componentsInChildren = source.GetComponentsInChildren<BoxCollider>();
		foreach (BoxCollider boxCollider in componentsInChildren)
		{
			BoxCollider boxCollider2 = CopyRelativeTransform(source, boxCollider.transform, destination).AddComponent<BoxCollider>();
			boxCollider2.center = boxCollider.center;
			boxCollider2.size = boxCollider.size;
			boxCollider2.isTrigger = boxCollider.isTrigger;
			boxCollider2.material = boxCollider.material;
			if (destroySource)
			{
				UnityEngine.Object.Destroy(boxCollider);
			}
		}
	}

	public static void MoveSphereColliders(GameObject source, GameObject destination, bool destroySource = true)
	{
		SphereCollider[] componentsInChildren = source.GetComponentsInChildren<SphereCollider>();
		foreach (SphereCollider sphereCollider in componentsInChildren)
		{
			SphereCollider sphereCollider2 = CopyRelativeTransform(source, sphereCollider.transform, destination).AddComponent<SphereCollider>();
			sphereCollider2.center = sphereCollider.center;
			sphereCollider2.radius = sphereCollider.radius;
			sphereCollider2.isTrigger = sphereCollider.isTrigger;
			sphereCollider2.material = sphereCollider.material;
			if (destroySource)
			{
				UnityEngine.Object.Destroy(sphereCollider);
			}
		}
	}

	public static void MoveCapsuleColliders(GameObject source, GameObject destination, bool destroySource = true)
	{
		CapsuleCollider[] componentsInChildren = source.GetComponentsInChildren<CapsuleCollider>();
		foreach (CapsuleCollider capsuleCollider in componentsInChildren)
		{
			CapsuleCollider capsuleCollider2 = CopyRelativeTransform(source, capsuleCollider.transform, destination).AddComponent<CapsuleCollider>();
			capsuleCollider2.center = capsuleCollider.center;
			capsuleCollider2.radius = capsuleCollider.radius;
			capsuleCollider2.height = capsuleCollider.height;
			capsuleCollider2.direction = capsuleCollider.direction;
			capsuleCollider2.isTrigger = capsuleCollider.isTrigger;
			capsuleCollider2.material = capsuleCollider.material;
			if (destroySource)
			{
				UnityEngine.Object.Destroy(capsuleCollider);
			}
		}
	}

	public static void MoveMeshColliders(GameObject source, GameObject destination, bool destroySource = true)
	{
		MeshCollider[] componentsInChildren = source.GetComponentsInChildren<MeshCollider>();
		foreach (MeshCollider meshCollider in componentsInChildren)
		{
			if (!meshCollider.sharedMesh.isReadable)
			{
				Debug.Log("[Prediction]: MeshCollider on " + meshCollider.name + " isn't readable, which may indicate that the Mesh only exists on the GPU. If " + meshCollider.name + " is missing collisions, then please select the model in the Project Area, and enable Mesh->Read/Write so it's also available on the CPU!");
			}
			MeshCollider meshCollider2 = CopyRelativeTransform(source, meshCollider.transform, destination).AddComponent<MeshCollider>();
			meshCollider2.sharedMesh = meshCollider.sharedMesh;
			meshCollider2.convex = meshCollider.convex;
			meshCollider2.isTrigger = meshCollider.isTrigger;
			meshCollider2.material = meshCollider.material;
			if (destroySource)
			{
				UnityEngine.Object.Destroy(meshCollider);
			}
		}
	}

	public static void MoveAllColliders(GameObject source, GameObject destination, bool destroySource = true)
	{
		MoveBoxColliders(source, destination, destroySource);
		MoveSphereColliders(source, destination, destroySource);
		MoveCapsuleColliders(source, destination, destroySource);
		MoveMeshColliders(source, destination, destroySource);
	}

	public static void MoveCharacterJoints(GameObject source, GameObject destination, bool destroySource = true)
	{
		CharacterJoint[] componentsInChildren = source.GetComponentsInChildren<CharacterJoint>();
		foreach (CharacterJoint characterJoint in componentsInChildren)
		{
			CharacterJoint characterJoint2 = CopyRelativeTransform(source, characterJoint.transform, destination).AddComponent<CharacterJoint>();
			characterJoint2.anchor = characterJoint.anchor;
			characterJoint2.autoConfigureConnectedAnchor = characterJoint.autoConfigureConnectedAnchor;
			characterJoint2.axis = characterJoint.axis;
			characterJoint2.breakForce = characterJoint.breakForce;
			characterJoint2.breakTorque = characterJoint.breakTorque;
			characterJoint2.connectedAnchor = characterJoint.connectedAnchor;
			characterJoint2.connectedBody = characterJoint.connectedBody;
			characterJoint2.connectedMassScale = characterJoint.connectedMassScale;
			characterJoint2.enableCollision = characterJoint.enableCollision;
			characterJoint2.enablePreprocessing = characterJoint.enablePreprocessing;
			characterJoint2.enableProjection = characterJoint.enableProjection;
			characterJoint2.highTwistLimit = characterJoint.highTwistLimit;
			characterJoint2.lowTwistLimit = characterJoint.lowTwistLimit;
			characterJoint2.massScale = characterJoint.massScale;
			characterJoint2.projectionAngle = characterJoint.projectionAngle;
			characterJoint2.projectionDistance = characterJoint.projectionDistance;
			characterJoint2.swing1Limit = characterJoint.swing1Limit;
			characterJoint2.swing2Limit = characterJoint.swing2Limit;
			characterJoint2.swingAxis = characterJoint.swingAxis;
			characterJoint2.swingLimitSpring = characterJoint.swingLimitSpring;
			characterJoint2.twistLimitSpring = characterJoint.twistLimitSpring;
			characterJoint2.connectedArticulationBody = characterJoint.connectedArticulationBody;
			if (destroySource)
			{
				UnityEngine.Object.Destroy(characterJoint);
			}
		}
	}

	public static void MoveConfigurableJoints(GameObject source, GameObject destination, bool destroySource = true)
	{
		ConfigurableJoint[] componentsInChildren = source.GetComponentsInChildren<ConfigurableJoint>();
		foreach (ConfigurableJoint configurableJoint in componentsInChildren)
		{
			ConfigurableJoint configurableJoint2 = CopyRelativeTransform(source, configurableJoint.transform, destination).AddComponent<ConfigurableJoint>();
			configurableJoint2.anchor = configurableJoint.anchor;
			configurableJoint2.angularXLimitSpring = configurableJoint.angularXLimitSpring;
			configurableJoint2.angularXDrive = configurableJoint.angularXDrive;
			configurableJoint2.angularXMotion = configurableJoint.angularXMotion;
			configurableJoint2.angularYLimit = configurableJoint.angularYLimit;
			configurableJoint2.angularYMotion = configurableJoint.angularYMotion;
			configurableJoint2.angularYZDrive = configurableJoint.angularYZDrive;
			configurableJoint2.angularYZLimitSpring = configurableJoint.angularYZLimitSpring;
			configurableJoint2.angularZLimit = configurableJoint.angularZLimit;
			configurableJoint2.angularZMotion = configurableJoint.angularZMotion;
			configurableJoint2.autoConfigureConnectedAnchor = configurableJoint.autoConfigureConnectedAnchor;
			configurableJoint2.axis = configurableJoint.axis;
			configurableJoint2.breakForce = configurableJoint.breakForce;
			configurableJoint2.breakTorque = configurableJoint.breakTorque;
			configurableJoint2.configuredInWorldSpace = configurableJoint.configuredInWorldSpace;
			configurableJoint2.connectedAnchor = configurableJoint.connectedAnchor;
			configurableJoint2.connectedBody = configurableJoint.connectedBody;
			configurableJoint2.connectedMassScale = configurableJoint.connectedMassScale;
			configurableJoint2.enableCollision = configurableJoint.enableCollision;
			configurableJoint2.enablePreprocessing = configurableJoint.enablePreprocessing;
			configurableJoint2.highAngularXLimit = configurableJoint.highAngularXLimit;
			configurableJoint2.linearLimitSpring = configurableJoint.linearLimitSpring;
			configurableJoint2.linearLimit = configurableJoint.linearLimit;
			configurableJoint2.lowAngularXLimit = configurableJoint.lowAngularXLimit;
			configurableJoint2.massScale = configurableJoint.massScale;
			configurableJoint2.projectionAngle = configurableJoint.projectionAngle;
			configurableJoint2.projectionDistance = configurableJoint.projectionDistance;
			configurableJoint2.projectionMode = configurableJoint.projectionMode;
			configurableJoint2.rotationDriveMode = configurableJoint.rotationDriveMode;
			configurableJoint2.secondaryAxis = configurableJoint.secondaryAxis;
			configurableJoint2.slerpDrive = configurableJoint.slerpDrive;
			configurableJoint2.swapBodies = configurableJoint.swapBodies;
			configurableJoint2.targetAngularVelocity = configurableJoint.targetAngularVelocity;
			configurableJoint2.targetPosition = configurableJoint.targetPosition;
			configurableJoint2.targetRotation = configurableJoint.targetRotation;
			configurableJoint2.targetVelocity = configurableJoint.targetVelocity;
			configurableJoint2.xDrive = configurableJoint.xDrive;
			configurableJoint2.xMotion = configurableJoint.xMotion;
			configurableJoint2.yDrive = configurableJoint.yDrive;
			configurableJoint2.yMotion = configurableJoint.yMotion;
			configurableJoint2.zDrive = configurableJoint.zDrive;
			configurableJoint2.zMotion = configurableJoint.zMotion;
			configurableJoint2.connectedArticulationBody = configurableJoint.connectedArticulationBody;
			if (destroySource)
			{
				UnityEngine.Object.Destroy(configurableJoint);
			}
		}
	}

	public static void MoveFixedJoints(GameObject source, GameObject destination, bool destroySource = true)
	{
		FixedJoint[] componentsInChildren = source.GetComponentsInChildren<FixedJoint>();
		foreach (FixedJoint fixedJoint in componentsInChildren)
		{
			FixedJoint fixedJoint2 = CopyRelativeTransform(source, fixedJoint.transform, destination).AddComponent<FixedJoint>();
			fixedJoint2.anchor = fixedJoint.anchor;
			fixedJoint2.autoConfigureConnectedAnchor = fixedJoint.autoConfigureConnectedAnchor;
			fixedJoint2.axis = fixedJoint.axis;
			fixedJoint2.breakForce = fixedJoint.breakForce;
			fixedJoint2.breakTorque = fixedJoint.breakTorque;
			fixedJoint2.connectedAnchor = fixedJoint.connectedAnchor;
			fixedJoint2.connectedBody = fixedJoint.connectedBody;
			fixedJoint2.connectedMassScale = fixedJoint.connectedMassScale;
			fixedJoint2.enableCollision = fixedJoint.enableCollision;
			fixedJoint2.enablePreprocessing = fixedJoint.enablePreprocessing;
			fixedJoint2.massScale = fixedJoint.massScale;
			fixedJoint2.connectedArticulationBody = fixedJoint.connectedArticulationBody;
			if (destroySource)
			{
				UnityEngine.Object.Destroy(fixedJoint);
			}
		}
	}

	public static void MoveHingeJoints(GameObject source, GameObject destination, bool destroySource = true)
	{
		HingeJoint[] componentsInChildren = source.GetComponentsInChildren<HingeJoint>();
		foreach (HingeJoint hingeJoint in componentsInChildren)
		{
			HingeJoint hingeJoint2 = CopyRelativeTransform(source, hingeJoint.transform, destination).AddComponent<HingeJoint>();
			hingeJoint2.anchor = hingeJoint.anchor;
			hingeJoint2.autoConfigureConnectedAnchor = hingeJoint.autoConfigureConnectedAnchor;
			hingeJoint2.axis = hingeJoint.axis;
			hingeJoint2.breakForce = hingeJoint.breakForce;
			hingeJoint2.breakTorque = hingeJoint.breakTorque;
			hingeJoint2.connectedAnchor = hingeJoint.connectedAnchor;
			hingeJoint2.connectedBody = hingeJoint.connectedBody;
			hingeJoint2.connectedMassScale = hingeJoint.connectedMassScale;
			hingeJoint2.enableCollision = hingeJoint.enableCollision;
			hingeJoint2.enablePreprocessing = hingeJoint.enablePreprocessing;
			hingeJoint2.limits = hingeJoint.limits;
			hingeJoint2.massScale = hingeJoint.massScale;
			hingeJoint2.motor = hingeJoint.motor;
			hingeJoint2.spring = hingeJoint.spring;
			hingeJoint2.useLimits = hingeJoint.useLimits;
			hingeJoint2.useMotor = hingeJoint.useMotor;
			hingeJoint2.useSpring = hingeJoint.useSpring;
			hingeJoint2.connectedArticulationBody = hingeJoint.connectedArticulationBody;
			hingeJoint2.extendedLimits = hingeJoint.extendedLimits;
			hingeJoint2.useAcceleration = hingeJoint.useAcceleration;
			if (destroySource)
			{
				UnityEngine.Object.Destroy(hingeJoint);
			}
		}
	}

	public static void MoveSpringJoints(GameObject source, GameObject destination, bool destroySource = true)
	{
		SpringJoint[] componentsInChildren = source.GetComponentsInChildren<SpringJoint>();
		foreach (SpringJoint springJoint in componentsInChildren)
		{
			SpringJoint springJoint2 = CopyRelativeTransform(source, springJoint.transform, destination).AddComponent<SpringJoint>();
			springJoint2.anchor = springJoint.anchor;
			springJoint2.autoConfigureConnectedAnchor = springJoint.autoConfigureConnectedAnchor;
			springJoint2.axis = springJoint.axis;
			springJoint2.breakForce = springJoint.breakForce;
			springJoint2.breakTorque = springJoint.breakTorque;
			springJoint2.connectedAnchor = springJoint.connectedAnchor;
			springJoint2.connectedBody = springJoint.connectedBody;
			springJoint2.connectedMassScale = springJoint.connectedMassScale;
			springJoint2.damper = springJoint.damper;
			springJoint2.enableCollision = springJoint.enableCollision;
			springJoint2.enablePreprocessing = springJoint.enablePreprocessing;
			springJoint2.massScale = springJoint.massScale;
			springJoint2.maxDistance = springJoint.maxDistance;
			springJoint2.minDistance = springJoint.minDistance;
			springJoint2.spring = springJoint.spring;
			springJoint2.tolerance = springJoint.tolerance;
			springJoint2.connectedArticulationBody = springJoint.connectedArticulationBody;
			if (destroySource)
			{
				UnityEngine.Object.Destroy(springJoint);
			}
		}
	}

	public static void MoveAllJoints(GameObject source, GameObject destination, bool destroySource = true)
	{
		MoveCharacterJoints(source, destination, destroySource);
		MoveConfigurableJoints(source, destination, destroySource);
		MoveFixedJoints(source, destination, destroySource);
		MoveHingeJoints(source, destination, destroySource);
		MoveSpringJoints(source, destination, destroySource);
	}

	public static void MovePhysicsComponents(GameObject source, GameObject destination, bool destroySource = true)
	{
		MoveAllJoints(source, destination, destroySource);
		MoveAllColliders(source, destination, destroySource);
		MoveRigidbody(source, destination, destroySource);
	}
}
