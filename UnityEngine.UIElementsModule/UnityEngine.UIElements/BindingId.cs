using System;
using Unity.Properties;

namespace UnityEngine.UIElements;

public readonly struct BindingId : IEquatable<BindingId>
{
	public static readonly BindingId Invalid = default(BindingId);

	private readonly PropertyPath m_PropertyPath;

	private readonly string m_Path;

	public BindingId(string path)
	{
		m_PropertyPath = new PropertyPath(path);
		m_Path = path;
	}

	public BindingId(in PropertyPath path)
	{
		m_PropertyPath = path;
		m_Path = path.ToString();
	}

	public static implicit operator PropertyPath(in BindingId vep)
	{
		return vep.m_PropertyPath;
	}

	public static implicit operator string(in BindingId vep)
	{
		return vep.m_Path;
	}

	public static implicit operator BindingId(string name)
	{
		return new BindingId(name);
	}

	public static implicit operator BindingId(in PropertyPath path)
	{
		return new BindingId(in path);
	}

	public override string ToString()
	{
		return m_Path;
	}

	public bool Equals(BindingId other)
	{
		return m_PropertyPath == other.m_PropertyPath;
	}

	public override bool Equals(object obj)
	{
		return obj is BindingId other && Equals(other);
	}

	public override int GetHashCode()
	{
		return m_PropertyPath.GetHashCode();
	}

	public static bool operator ==(in BindingId lhs, in BindingId rhs)
	{
		return lhs.m_PropertyPath == rhs.m_PropertyPath;
	}

	public static bool operator !=(in BindingId lhs, in BindingId rhs)
	{
		return !(lhs == rhs);
	}
}
