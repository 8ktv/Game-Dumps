using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[NativeType(Header = "Modules/UIElements/VisualPanelHandle.h")]
internal readonly struct VisualPanelHandle : IEquatable<VisualPanelHandle>
{
	public static readonly VisualPanelHandle Null;

	private readonly int m_Id;

	private readonly int m_Version;

	public int Id => m_Id;

	public int Version => m_Version;

	public VisualPanelHandle(int id, int version)
	{
		m_Id = id;
		m_Version = version;
	}

	public static bool operator ==(in VisualPanelHandle lhs, in VisualPanelHandle rhs)
	{
		return lhs.Id == rhs.Id && lhs.Version == rhs.Version;
	}

	public static bool operator !=(in VisualPanelHandle lhs, in VisualPanelHandle rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(VisualPanelHandle other)
	{
		return other.Id == Id && other.Version == Version;
	}

	public override string ToString()
	{
		return "VisualPanelHandle(" + ((this == Null) ? "Null" : $"{Id}:{Version}") + ")";
	}

	public override bool Equals(object obj)
	{
		return obj is VisualPanelHandle other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, Version);
	}
}
