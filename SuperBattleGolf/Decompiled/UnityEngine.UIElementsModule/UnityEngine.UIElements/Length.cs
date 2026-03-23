using System;
using System.Globalization;
using Unity.Properties;

namespace UnityEngine.UIElements;

[Serializable]
public struct Length : IEquatable<Length>
{
	private enum Unit
	{
		Pixel,
		Percent,
		Auto,
		None
	}

	internal class PropertyBag : ContainerPropertyBag<Length>
	{
		private class ValueProperty : Property<Length, float>
		{
			public override string Name { get; } = "value";

			public override bool IsReadOnly { get; } = false;

			public override float GetValue(ref Length container)
			{
				return container.value;
			}

			public override void SetValue(ref Length container, float value)
			{
				container.value = value;
			}
		}

		private class UnitProperty : Property<Length, LengthUnit>
		{
			public override string Name { get; } = "unit";

			public override bool IsReadOnly { get; } = false;

			public override LengthUnit GetValue(ref Length container)
			{
				return container.unit;
			}

			public override void SetValue(ref Length container, LengthUnit value)
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

	internal const float k_MaxValue = 8388608f;

	[SerializeField]
	private float m_Value;

	[SerializeField]
	private Unit m_Unit;

	public float value
	{
		get
		{
			return m_Value;
		}
		set
		{
			m_Value = Mathf.Clamp(value, -8388608f, 8388608f);
		}
	}

	public LengthUnit unit
	{
		get
		{
			return (LengthUnit)m_Unit;
		}
		set
		{
			m_Unit = (Unit)value;
		}
	}

	public static Length Pixels(float value)
	{
		return new Length(value, LengthUnit.Pixel);
	}

	public static Length Percent(float value)
	{
		return new Length(value, LengthUnit.Percent);
	}

	public static Length Auto()
	{
		return new Length(0f, Unit.Auto);
	}

	public static Length None()
	{
		return new Length(0f, Unit.None);
	}

	public bool IsAuto()
	{
		return m_Unit == Unit.Auto;
	}

	public bool IsNone()
	{
		return m_Unit == Unit.None;
	}

	public Length(float value)
		: this(value, Unit.Pixel)
	{
	}

	public Length(float value, LengthUnit unit)
		: this(value, (Unit)unit)
	{
	}

	private Length(float value, Unit unit)
	{
		this = default(Length);
		this.value = value;
		m_Unit = unit;
	}

	public static implicit operator Length(float value)
	{
		return new Length(value, LengthUnit.Pixel);
	}

	public static bool operator ==(Length lhs, Length rhs)
	{
		return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
	}

	public static bool operator !=(Length lhs, Length rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(Length other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is Length other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (m_Value.GetHashCode() * 397) ^ (int)m_Unit;
	}

	public override string ToString()
	{
		string text = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		string text2 = string.Empty;
		switch (m_Unit)
		{
		case Unit.Pixel:
			if (!Mathf.Approximately(0f, value))
			{
				text2 = "px";
			}
			break;
		case Unit.Percent:
			text2 = "%";
			break;
		case Unit.Auto:
			text = "auto";
			break;
		case Unit.None:
			text = "none";
			break;
		}
		return text + text2;
	}

	internal static Length ParseString(string str, Length defaultValue = default(Length))
	{
		if (string.IsNullOrEmpty(str))
		{
			return defaultValue;
		}
		str = str.ToLowerInvariant().Trim();
		Length result = defaultValue;
		if (char.IsLetter(str[0]))
		{
			if (str == "auto")
			{
				result = Auto();
			}
			else if (str == "none")
			{
				result = None();
			}
		}
		else
		{
			int num = 0;
			int num2 = -1;
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (char.IsNumber(c) || c == '.')
				{
					num++;
					continue;
				}
				if (char.IsLetter(c) || c == '%')
				{
					num2 = i;
					break;
				}
				return defaultValue;
			}
			string s = str.Substring(0, num);
			string empty = string.Empty;
			empty = ((num2 <= 0) ? "px" : str.Substring(num2, str.Length - num2));
			float num3 = defaultValue.value;
			LengthUnit lengthUnit = defaultValue.unit;
			if (float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out var result2))
			{
				num3 = result2;
			}
			string text = empty;
			string text2 = text;
			if (!(text2 == "px"))
			{
				if (text2 == "%")
				{
					lengthUnit = LengthUnit.Percent;
				}
			}
			else
			{
				lengthUnit = LengthUnit.Pixel;
			}
			result = new Length(num3, lengthUnit);
		}
		return result;
	}
}
