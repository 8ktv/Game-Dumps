using System;

namespace UnityEngine.Rendering;

public struct MachineLearningTensorShape : IEquatable<MachineLearningTensorShape>
{
	public uint rank;

	public uint D0;

	public uint D1;

	public uint D2;

	public uint D3;

	public uint D4;

	public uint D5;

	public uint D6;

	public uint D7;

	public override int GetHashCode()
	{
		return (rank, D0, D1, D2, D3, D4, D5, D6, D7).GetHashCode();
	}

	public bool Equals(MachineLearningTensorShape other)
	{
		return rank == other.rank && D0 == other.D0 && D1 == other.D1 && D2 == other.D2 && D3 == other.D3 && D4 == other.D4 && D5 == other.D5 && D6 == other.D6 && D7 == other.D7;
	}

	public override bool Equals(object obj)
	{
		return obj is MachineLearningTensorShape other && Equals(other);
	}

	public static bool operator ==(MachineLearningTensorShape lhs, MachineLearningTensorShape rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(MachineLearningTensorShape lhs, MachineLearningTensorShape rhs)
	{
		return !lhs.Equals(rhs);
	}
}
