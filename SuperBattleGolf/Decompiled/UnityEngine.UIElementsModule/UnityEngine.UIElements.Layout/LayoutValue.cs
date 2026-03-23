namespace UnityEngine.UIElements.Layout;

internal struct LayoutValue
{
	private float value;

	private LayoutUnit unit;

	public LayoutUnit Unit => unit;

	public float Value => value;

	public static LayoutValue Point(float value)
	{
		return new LayoutValue
		{
			value = value,
			unit = ((!float.IsNaN(value)) ? LayoutUnit.Point : LayoutUnit.Undefined)
		};
	}

	public bool Equals(LayoutValue other)
	{
		return Unit == other.Unit && (Value.Equals(other.Value) || Unit == LayoutUnit.Undefined);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return obj is LayoutValue other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (Value.GetHashCode() * 397) ^ (int)Unit;
	}

	public static LayoutValue Undefined()
	{
		return new LayoutValue
		{
			value = float.NaN,
			unit = LayoutUnit.Undefined
		};
	}

	public static LayoutValue Auto()
	{
		return new LayoutValue
		{
			value = float.NaN,
			unit = LayoutUnit.Auto
		};
	}

	public static LayoutValue Percent(float value)
	{
		return new LayoutValue
		{
			value = value,
			unit = ((!float.IsNaN(value)) ? LayoutUnit.Percent : LayoutUnit.Undefined)
		};
	}

	public static implicit operator LayoutValue(float value)
	{
		return Point(value);
	}
}
