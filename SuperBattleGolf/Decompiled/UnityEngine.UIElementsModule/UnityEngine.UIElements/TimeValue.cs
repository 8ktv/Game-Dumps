using System;
using System.Globalization;
using Unity.Properties;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[Serializable]
public struct TimeValue : IEquatable<TimeValue>
{
	internal class PropertyBag : ContainerPropertyBag<TimeValue>
	{
		private class ValueProperty : Property<TimeValue, float>
		{
			public override string Name { get; } = "value";

			public override bool IsReadOnly { get; } = false;

			public override float GetValue(ref TimeValue container)
			{
				return container.value;
			}

			public override void SetValue(ref TimeValue container, float value)
			{
				container.value = value;
			}
		}

		private class UnitProperty : Property<TimeValue, TimeUnit>
		{
			public override string Name { get; } = "unit";

			public override bool IsReadOnly { get; } = false;

			public override TimeUnit GetValue(ref TimeValue container)
			{
				return container.unit;
			}

			public override void SetValue(ref TimeValue container, TimeUnit value)
			{
				container.unit = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new ValueProperty());
			AddProperty(new UnitProperty());
		}
	}

	[SerializeField]
	private float m_Value;

	[SerializeField]
	private TimeUnit m_Unit;

	public float value
	{
		get
		{
			return m_Value;
		}
		set
		{
			m_Value = value;
		}
	}

	public TimeUnit unit
	{
		get
		{
			return m_Unit;
		}
		set
		{
			m_Unit = value;
		}
	}

	public static TimeValue Seconds(float value)
	{
		return new TimeValue(value, TimeUnit.Second);
	}

	public static TimeValue Milliseconds(float value)
	{
		return new TimeValue(value, TimeUnit.Millisecond);
	}

	public TimeValue(float value)
		: this(value, TimeUnit.Second)
	{
	}

	public TimeValue(float value, TimeUnit unit)
	{
		m_Value = value;
		m_Unit = unit;
	}

	public static implicit operator TimeValue(float value)
	{
		return new TimeValue(value, TimeUnit.Second);
	}

	public static bool operator ==(TimeValue lhs, TimeValue rhs)
	{
		return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
	}

	public static bool operator !=(TimeValue lhs, TimeValue rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(TimeValue other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is TimeValue other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (m_Value.GetHashCode() * 397) ^ (int)m_Unit;
	}

	public override string ToString()
	{
		string text = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		string text2 = string.Empty;
		switch (unit)
		{
		case TimeUnit.Second:
			text2 = "s";
			break;
		case TimeUnit.Millisecond:
			text2 = "ms";
			break;
		}
		return text + text2;
	}

	internal static bool TryParseString(string str, out TimeValue timeValue)
	{
		timeValue = default(TimeValue);
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(str).Trim();
		int num = 0;
		int num2 = -1;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = readOnlySpan[i];
			if (char.IsNumber(c) || c == '.')
			{
				num++;
				continue;
			}
			if (char.IsLetter(c))
			{
				num2 = i;
				break;
			}
			return false;
		}
		ReadOnlySpan<char> floatStr = readOnlySpan.Slice(0, num);
		ReadOnlySpan<char> readOnlySpan2 = default(ReadOnlySpan<char>);
		readOnlySpan2 = ((num2 <= 0) ? ((ReadOnlySpan<char>)"s") : readOnlySpan.Slice(num2, readOnlySpan.Length - num2));
		if (StylePropertyUtil.TryParseFloat(floatStr, out var num3))
		{
			timeValue.value = num3;
		}
		if (MemoryExtensions.Equals(readOnlySpan2, "ms", StringComparison.OrdinalIgnoreCase))
		{
			timeValue.unit = TimeUnit.Millisecond;
		}
		else if (MemoryExtensions.Equals(readOnlySpan2, "s", StringComparison.OrdinalIgnoreCase))
		{
			timeValue.unit = TimeUnit.Second;
		}
		return true;
	}
}
