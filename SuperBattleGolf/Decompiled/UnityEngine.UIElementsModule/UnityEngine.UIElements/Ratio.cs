using System;
using System.Globalization;
using Unity.Properties;

namespace UnityEngine.UIElements;

public readonly struct Ratio : IEquatable<Ratio>
{
	internal class PropertyBag : ContainerPropertyBag<Ratio>
	{
		private class ValueProperty : Property<Ratio, float>
		{
			public override string Name { get; } = "value";

			public override bool IsReadOnly { get; } = false;

			public override float GetValue(ref Ratio container)
			{
				return container.value;
			}

			public override void SetValue(ref Ratio container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class AutoProperty : Property<Ratio, bool>
		{
			public override string Name { get; } = "IsAuto";

			public override bool IsReadOnly { get; } = true;

			public override bool GetValue(ref Ratio container)
			{
				return container.IsAuto();
			}

			public override void SetValue(ref Ratio container, bool value)
			{
				throw new InvalidOperationException();
			}
		}

		public PropertyBag()
		{
			AddProperty(new ValueProperty());
			AddProperty(new AutoProperty());
		}
	}

	private readonly float m_Value;

	public float value => m_Value;

	public Ratio(float value)
	{
		m_Value = value;
	}

	public static Ratio Auto()
	{
		return new Ratio(float.NaN);
	}

	public bool IsAuto()
	{
		return float.IsNaN(value);
	}

	public static implicit operator Ratio(float value)
	{
		return new Ratio(value);
	}

	public static implicit operator float(Ratio value)
	{
		return value.value;
	}

	public static bool operator ==(Ratio lhs, Ratio rhs)
	{
		if (lhs.IsAuto() && rhs.IsAuto())
		{
			return true;
		}
		return lhs.m_Value == rhs.m_Value;
	}

	public static bool operator !=(Ratio lhs, Ratio rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(Ratio other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is Ratio other && Equals(other);
	}

	public override int GetHashCode()
	{
		return m_Value.GetHashCode() * 793;
	}

	public override string ToString()
	{
		return IsAuto() ? StyleValueKeyword.Auto.ToUssString() : m_Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}
}
