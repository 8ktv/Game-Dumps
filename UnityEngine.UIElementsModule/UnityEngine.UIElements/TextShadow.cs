using System;
using Unity.Properties;

namespace UnityEngine.UIElements;

public struct TextShadow : IEquatable<TextShadow>
{
	internal class PropertyBag : ContainerPropertyBag<TextShadow>
	{
		private class OffsetProperty : Property<TextShadow, Vector2>
		{
			public override string Name { get; } = "offset";

			public override bool IsReadOnly { get; } = false;

			public override Vector2 GetValue(ref TextShadow container)
			{
				return container.offset;
			}

			public override void SetValue(ref TextShadow container, Vector2 value)
			{
				container.offset = value;
			}
		}

		private class BlurRadiusProperty : Property<TextShadow, float>
		{
			public override string Name { get; } = "blurRadius";

			public override bool IsReadOnly { get; } = false;

			public override float GetValue(ref TextShadow container)
			{
				return container.blurRadius;
			}

			public override void SetValue(ref TextShadow container, float value)
			{
				container.blurRadius = value;
			}
		}

		private class ColorProperty : Property<TextShadow, Color>
		{
			public override string Name { get; } = "color";

			public override bool IsReadOnly { get; } = false;

			public override Color GetValue(ref TextShadow container)
			{
				return container.color;
			}

			public override void SetValue(ref TextShadow container, Color value)
			{
				container.color = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new OffsetProperty());
			AddProperty(new BlurRadiusProperty());
			AddProperty(new ColorProperty());
		}
	}

	public Vector2 offset;

	public float blurRadius;

	public Color color;

	public override bool Equals(object obj)
	{
		return obj is TextShadow && Equals((TextShadow)obj);
	}

	public bool Equals(TextShadow other)
	{
		return other.offset == offset && other.blurRadius == blurRadius && other.color == color;
	}

	public override int GetHashCode()
	{
		int num = 1500536833;
		num = num * -1521134295 + offset.GetHashCode();
		num = num * -1521134295 + blurRadius.GetHashCode();
		return num * -1521134295 + color.GetHashCode();
	}

	public static bool operator ==(TextShadow style1, TextShadow style2)
	{
		return style1.Equals(style2);
	}

	public static bool operator !=(TextShadow style1, TextShadow style2)
	{
		return !(style1 == style2);
	}

	public override string ToString()
	{
		return $"offset={offset}, blurRadius={blurRadius}, color={color}";
	}

	internal static TextShadow LerpUnclamped(TextShadow a, TextShadow b, float t)
	{
		return new TextShadow
		{
			offset = Vector2.LerpUnclamped(a.offset, b.offset, t),
			blurRadius = Mathf.LerpUnclamped(a.blurRadius, b.blurRadius, t),
			color = Color.LerpUnclamped(a.color, b.color, t)
		};
	}
}
