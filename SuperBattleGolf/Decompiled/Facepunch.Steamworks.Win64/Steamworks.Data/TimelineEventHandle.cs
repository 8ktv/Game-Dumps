using System;

namespace Steamworks.Data;

public struct TimelineEventHandle : IEquatable<TimelineEventHandle>, IComparable<TimelineEventHandle>
{
	public ulong Value;

	public static implicit operator TimelineEventHandle(ulong value)
	{
		return new TimelineEventHandle
		{
			Value = value
		};
	}

	public static implicit operator ulong(TimelineEventHandle value)
	{
		return value.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override bool Equals(object p)
	{
		return Equals((TimelineEventHandle)p);
	}

	public bool Equals(TimelineEventHandle p)
	{
		return p.Value == Value;
	}

	public static bool operator ==(TimelineEventHandle a, TimelineEventHandle b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(TimelineEventHandle a, TimelineEventHandle b)
	{
		return !a.Equals(b);
	}

	public int CompareTo(TimelineEventHandle other)
	{
		return Value.CompareTo(other.Value);
	}
}
