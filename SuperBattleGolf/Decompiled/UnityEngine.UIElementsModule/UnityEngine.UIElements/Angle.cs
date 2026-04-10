using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Properties;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[Serializable]
public struct Angle : IEquatable<Angle>
{
	private enum Unit
	{
		Degree,
		Gradian,
		Radian,
		Turn,
		None
	}

	internal class PropertyBag : ContainerPropertyBag<Angle>
	{
		private class ValueProperty : Property<Angle, float>
		{
			public override string Name { get; } = "value";

			public override bool IsReadOnly { get; } = false;

			public override float GetValue(ref Angle container)
			{
				return container.value;
			}

			public override void SetValue(ref Angle container, float value)
			{
				container.value = value;
			}
		}

		private class UnitProperty : Property<Angle, AngleUnit>
		{
			public override string Name { get; } = "unit";

			public override bool IsReadOnly { get; } = false;

			public override AngleUnit GetValue(ref Angle container)
			{
				return container.unit;
			}

			public override void SetValue(ref Angle container, AngleUnit value)
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

	private static readonly Dictionary<string, AngleUnit> s_AngleUnitLookup = new Dictionary<string, AngleUnit>(StringComparer.OrdinalIgnoreCase)
	{
		{
			"deg",
			AngleUnit.Degree
		},
		{
			"rad",
			AngleUnit.Radian
		},
		{
			"grad",
			AngleUnit.Gradian
		},
		{
			"turn",
			AngleUnit.Turn
		}
	};

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
			m_Value = value;
		}
	}

	public AngleUnit unit
	{
		get
		{
			return (AngleUnit)m_Unit;
		}
		set
		{
			m_Unit = (Unit)value;
		}
	}

	public static Angle Degrees(float value)
	{
		return new Angle(value, AngleUnit.Degree);
	}

	public static Angle Gradians(float value)
	{
		return new Angle(value, AngleUnit.Gradian);
	}

	public static Angle Radians(float value)
	{
		return new Angle(value, AngleUnit.Radian);
	}

	public static Angle Turns(float value)
	{
		return new Angle(value, AngleUnit.Turn);
	}

	internal static Angle None()
	{
		return new Angle(0f, Unit.None);
	}

	internal bool IsNone()
	{
		return m_Unit == Unit.None;
	}

	public Angle(float value)
		: this(value, Unit.Degree)
	{
	}

	public Angle(float value, AngleUnit unit)
		: this(value, (Unit)unit)
	{
	}

	private Angle(float value, Unit unit)
	{
		m_Value = value;
		m_Unit = unit;
	}

	public float ToDegrees()
	{
		return m_Unit switch
		{
			Unit.Degree => m_Value, 
			Unit.Gradian => m_Value * 360f / 400f, 
			Unit.Radian => m_Value * 180f / MathF.PI, 
			Unit.Turn => m_Value * 360f, 
			Unit.None => 0f, 
			_ => 0f, 
		};
	}

	public float ToGradians()
	{
		return m_Unit switch
		{
			Unit.Degree => m_Value * 10f / 9f, 
			Unit.Gradian => m_Value, 
			Unit.Radian => m_Value * 200f / MathF.PI, 
			Unit.Turn => m_Value * 400f, 
			Unit.None => 0f, 
			_ => 0f, 
		};
	}

	public float ToRadians()
	{
		return m_Unit switch
		{
			Unit.Degree => m_Value * MathF.PI / 180f, 
			Unit.Gradian => m_Value * MathF.PI / 200f, 
			Unit.Radian => m_Value, 
			Unit.Turn => m_Value * MathF.PI * 2f, 
			Unit.None => 0f, 
			_ => 0f, 
		};
	}

	public float ToTurns()
	{
		return m_Unit switch
		{
			Unit.Degree => m_Value / 360f, 
			Unit.Gradian => m_Value / 400f, 
			Unit.Radian => m_Value / (MathF.PI * 2f), 
			Unit.Turn => m_Value, 
			Unit.None => 0f, 
			_ => 0f, 
		};
	}

	internal void ConvertTo(AngleUnit newUnit)
	{
		if (1 == 0)
		{
		}
		float num = newUnit switch
		{
			AngleUnit.Degree => ToDegrees(), 
			AngleUnit.Turn => ToTurns(), 
			AngleUnit.Radian => ToRadians(), 
			AngleUnit.Gradian => ToGradians(), 
			_ => throw new NotImplementedException(), 
		};
		if (1 == 0)
		{
		}
		m_Value = num;
		m_Unit = (Unit)newUnit;
	}

	public static implicit operator Angle(float value)
	{
		return new Angle(value, AngleUnit.Degree);
	}

	public static bool operator ==(Angle lhs, Angle rhs)
	{
		return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
	}

	public static bool operator !=(Angle lhs, Angle rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(Angle other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is Angle other && Equals(other);
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
		case Unit.Degree:
			if (!Mathf.Approximately(0f, value))
			{
				text2 = "deg";
			}
			break;
		case Unit.Gradian:
			text2 = "grad";
			break;
		case Unit.Radian:
			text2 = "rad";
			break;
		case Unit.Turn:
			text2 = "turn";
			break;
		case Unit.None:
			text = "";
			text2 = "none";
			break;
		}
		return text + text2;
	}

	internal static bool TryParseString(string str, out Angle angle)
	{
		angle = default(Angle);
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		ReadOnlySpan<char> span = MemoryExtensions.AsSpan(str).Trim();
		if (MemoryExtensions.Equals(span, "none", StringComparison.OrdinalIgnoreCase))
		{
			angle = None();
			return true;
		}
		int num = 0;
		int num2 = -1;
		for (int i = 0; i < span.Length; i++)
		{
			char c = span[i];
			if (char.IsNumber(c) || c == '.' || c == '-')
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
		ReadOnlySpan<char> floatStr = span.Slice(0, num);
		string text = ((num2 > 0) ? span.Slice(num2, span.Length - num2).ToString() : "deg");
		if (StylePropertyUtil.TryParseFloat(floatStr, out var num3))
		{
			angle.value = num3;
		}
		if (s_AngleUnitLookup.TryGetValue(text, out var angleUnit))
		{
			angle.unit = angleUnit;
		}
		else
		{
			if (!text.Equals("none", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			angle.unit = AngleUnit.Degree;
			angle.value = 0f;
		}
		return true;
	}
}
