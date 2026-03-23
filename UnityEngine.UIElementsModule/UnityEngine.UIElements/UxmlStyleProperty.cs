using System;
using Unity.Collections;

namespace UnityEngine.UIElements;

internal struct UxmlStyleProperty : IDisposable, IEquatable<UxmlStyleProperty>
{
	public NativeArray<StyleValueHandle> values;

	public bool requireVariableResolve;

	public bool isInlined => values.Length > 0;

	public UxmlStyleProperty(StyleValueHandle[] values, bool requireVariableResolve)
	{
		this.values = new NativeArray<StyleValueHandle>(values, Allocator.Persistent);
		this.requireVariableResolve = requireVariableResolve;
	}

	public bool Equals(UxmlStyleProperty other)
	{
		if (requireVariableResolve != other.requireVariableResolve)
		{
			return false;
		}
		if (values.IsCreated != other.values.IsCreated)
		{
			return false;
		}
		if (!values.IsCreated)
		{
			return true;
		}
		if (values.Length != other.values.Length)
		{
			return false;
		}
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] != other.values[i])
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		return obj is UxmlStyleProperty other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(values, requireVariableResolve);
	}

	public void Dispose()
	{
		values.Dispose();
	}
}
