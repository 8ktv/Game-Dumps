using System;
using UnityEngine;

namespace Mirror;

[Serializable]
public struct SyncData
{
	public Changed changedDataByte;

	public Vector3 position;

	public Quaternion quatRotation;

	public Vector3 vecRotation;

	public Vector3 scale;

	public SyncData(Changed _dataChangedByte, Vector3 _position, Quaternion _rotation, Vector3 _scale)
	{
		changedDataByte = _dataChangedByte;
		position = _position;
		quatRotation = _rotation;
		vecRotation = quatRotation.eulerAngles;
		scale = _scale;
	}

	public SyncData(Changed _dataChangedByte, TransformSnapshot _snapshot)
	{
		changedDataByte = _dataChangedByte;
		position = _snapshot.position;
		quatRotation = _snapshot.rotation;
		vecRotation = quatRotation.eulerAngles;
		scale = _snapshot.scale;
	}

	public SyncData(Changed _dataChangedByte, Vector3 _position, Vector3 _vecRotation, Vector3 _scale)
	{
		changedDataByte = _dataChangedByte;
		position = _position;
		vecRotation = _vecRotation;
		quatRotation = Quaternion.Euler(vecRotation);
		scale = _scale;
	}
}
