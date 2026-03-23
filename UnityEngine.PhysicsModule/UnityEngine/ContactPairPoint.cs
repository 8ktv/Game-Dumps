using System;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public readonly struct ContactPairPoint
{
	internal readonly Vector3 m_Position;

	internal readonly float m_Separation;

	internal readonly Vector3 m_Normal;

	internal readonly uint m_InternalFaceIndex0;

	internal readonly Vector3 m_Impulse;

	internal readonly uint m_InternalFaceIndex1;

	public Vector3 position => m_Position;

	public float separation => m_Separation;

	public Vector3 normal => m_Normal;

	public Vector3 impulse => m_Impulse;

	[Obsolete("Please use ContactPairPoint.position instead. (UnityUpgradable) -> position", false)]
	public Vector3 Position => position;

	[Obsolete("Please use ContactPairPoint.separation instead. (UnityUpgradable) -> separation", false)]
	public float Separation => separation;

	[Obsolete("Please use ContactPairPoint.normal instead. (UnityUpgradable) -> normal", false)]
	public Vector3 Normal => normal;

	[Obsolete("Please use ContactPairPoint.impulse instead. (UnityUpgradable) -> impulse", false)]
	public Vector3 Impulse => impulse;
}
