using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement;

[Serializable]
[NativeClass("UnitySceneHandle")]
[NativeHeader("Runtime/SceneManager/UnitySceneHandle.h")]
[UsedByNativeCode]
public struct SceneHandle : IEquatable<SceneHandle>
{
	internal EntityId m_Value;

	public static SceneHandle None => default(SceneHandle);

	internal static SceneHandle From(EntityId entityId)
	{
		return new SceneHandle
		{
			m_Value = entityId
		};
	}

	public override bool Equals(object obj)
	{
		return obj is SceneHandle other && Equals(other);
	}

	public bool Equals(SceneHandle other)
	{
		return m_Value == other.m_Value;
	}

	public static bool operator ==(SceneHandle left, SceneHandle right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(SceneHandle left, SceneHandle right)
	{
		return !left.Equals(right);
	}

	public static implicit operator int(SceneHandle handle)
	{
		return handle.m_Value;
	}

	public static implicit operator SceneHandle(int handle)
	{
		return From(handle);
	}

	public static implicit operator uint(SceneHandle handle)
	{
		return (uint)(int)handle.m_Value;
	}

	public static implicit operator SceneHandle(uint handle)
	{
		return From((int)handle);
	}

	public override int GetHashCode()
	{
		return m_Value.GetHashCode();
	}

	public override string ToString()
	{
		return m_Value.ToString();
	}

	public string ToString(string format)
	{
		return m_Value.ToString(format);
	}

	internal EntityId ToEntityId()
	{
		return m_Value;
	}
}
