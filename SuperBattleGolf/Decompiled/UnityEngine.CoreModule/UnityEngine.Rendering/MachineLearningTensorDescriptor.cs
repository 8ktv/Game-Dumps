using System;

namespace UnityEngine.Rendering;

public struct MachineLearningTensorDescriptor : IEquatable<MachineLearningTensorDescriptor>
{
	internal bool hasValue;

	public MachineLearningDataType dataType;

	public MachineLearningTensorShape shape;

	public override int GetHashCode()
	{
		return (dataType, shape).GetHashCode();
	}

	public bool Equals(MachineLearningTensorDescriptor other)
	{
		return dataType == other.dataType && shape.Equals(other.shape);
	}

	public override bool Equals(object obj)
	{
		return obj is MachineLearningTensorDescriptor other && Equals(other);
	}

	public static bool operator ==(MachineLearningTensorDescriptor lhs, MachineLearningTensorDescriptor rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(MachineLearningTensorDescriptor lhs, MachineLearningTensorDescriptor rhs)
	{
		return !lhs.Equals(rhs);
	}

	public MachineLearningTensorDescriptor(MachineLearningDataType dataType, MachineLearningTensorShape shape)
	{
		hasValue = true;
		this.dataType = dataType;
		this.shape = shape;
	}

	public static MachineLearningTensorDescriptor NullTensor()
	{
		return new MachineLearningTensorDescriptor
		{
			hasValue = false,
			dataType = MachineLearningDataType.Float32,
			shape = default(MachineLearningTensorShape)
		};
	}
}
