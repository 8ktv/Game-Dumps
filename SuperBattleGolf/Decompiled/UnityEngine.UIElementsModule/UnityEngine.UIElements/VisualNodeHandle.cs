using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[NativeType(Header = "Modules/UIElements/VisualNodeHandle.h")]
internal readonly struct VisualNodeHandle : IEquatable<VisualNodeHandle>
{
	public static readonly VisualNodeHandle Null;

	private readonly int m_Id;

	private readonly int m_Version;

	public int Id => m_Id;

	public int Version => m_Version;

	public VisualNodeHandle(int id, int version)
	{
		m_Id = id;
		m_Version = version;
	}

	public static bool operator ==(in VisualNodeHandle lhs, in VisualNodeHandle rhs)
	{
		return lhs.Id == rhs.Id && lhs.Version == rhs.Version;
	}

	public static bool operator !=(in VisualNodeHandle lhs, in VisualNodeHandle rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(VisualNodeHandle other)
	{
		return other.Id == Id && other.Version == Version;
	}

	public override string ToString()
	{
		return "VisualNodeHandle(" + ((this == Null) ? "Null" : $"{Id}:{Version}") + ")";
	}

	public override bool Equals(object obj)
	{
		return obj is VisualNodeHandle other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, Version);
	}
}
