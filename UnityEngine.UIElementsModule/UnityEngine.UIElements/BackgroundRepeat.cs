using System;
using Unity.Properties;

namespace UnityEngine.UIElements;

public struct BackgroundRepeat : IEquatable<BackgroundRepeat>
{
	internal class PropertyBag : ContainerPropertyBag<BackgroundRepeat>
	{
		private class XProperty : Property<BackgroundRepeat, Repeat>
		{
			public override string Name { get; } = "x";

			public override bool IsReadOnly { get; } = false;

			public override Repeat GetValue(ref BackgroundRepeat container)
			{
				return container.x;
			}

			public override void SetValue(ref BackgroundRepeat container, Repeat value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<BackgroundRepeat, Repeat>
		{
			public override string Name { get; } = "y";

			public override bool IsReadOnly { get; } = false;

			public override Repeat GetValue(ref BackgroundRepeat container)
			{
				return container.y;
			}

			public override void SetValue(ref BackgroundRepeat container, Repeat value)
			{
				container.y = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new XProperty());
			AddProperty(new YProperty());
		}
	}

	public Repeat x;

	public Repeat y;

	public BackgroundRepeat(Repeat repeatX, Repeat repeatY)
	{
		x = repeatX;
		y = repeatY;
	}

	internal static BackgroundRepeat Initial()
	{
		return BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat();
	}

	public override bool Equals(object obj)
	{
		return obj is BackgroundRepeat && Equals((BackgroundRepeat)obj);
	}

	public bool Equals(BackgroundRepeat other)
	{
		return other.x == x && other.y == y;
	}

	public override int GetHashCode()
	{
		int num = 1500536833;
		num = num * -1521134295 + x.GetHashCode();
		return num * -1521134295 + y.GetHashCode();
	}

	public static bool operator ==(BackgroundRepeat style1, BackgroundRepeat style2)
	{
		return style1.Equals(style2);
	}

	public static bool operator !=(BackgroundRepeat style1, BackgroundRepeat style2)
	{
		return !(style1 == style2);
	}

	public override string ToString()
	{
		return $"(x:{x}, y:{y})";
	}
}
