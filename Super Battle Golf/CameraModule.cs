using System;
using UnityEngine;

public abstract class CameraModule : MonoBehaviour
{
	[NonSerialized]
	public Vector3 position = Vector3.zero;

	[NonSerialized]
	public Quaternion rotation = Quaternion.identity;

	public abstract CameraModuleType Type { get; }

	public abstract bool ControlsFieldOfView { get; }

	public virtual float FieldOfView { get; }

	public Vector3 GetCurrentForward()
	{
		return rotation * Vector3.forward;
	}

	public Vector3 GetCurrentRight()
	{
		return rotation * Vector3.right;
	}

	public Vector3 GetCurrentUp()
	{
		return rotation * Vector3.up;
	}

	public abstract void UpdateModule();

	public virtual void OnTransitionStart(bool transitioningToThis)
	{
	}

	public virtual void OnTransitionEnd(bool transitionedToThis)
	{
	}

	public virtual void SyncFromTransform()
	{
		position = base.transform.position;
		rotation = base.transform.rotation;
	}
}
