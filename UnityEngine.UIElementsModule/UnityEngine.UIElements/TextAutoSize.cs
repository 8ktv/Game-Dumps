using System;
using Unity.Properties;

namespace UnityEngine.UIElements;

public struct TextAutoSize : IEquatable<TextAutoSize>
{
	internal class PropertyBag : ContainerPropertyBag<TextAutoSize>
	{
		private class ModeProperty : Property<TextAutoSize, TextAutoSizeMode>
		{
			public override string Name { get; } = "mode";

			public override bool IsReadOnly { get; } = false;

			public override TextAutoSizeMode GetValue(ref TextAutoSize container)
			{
				return container.mode;
			}

			public override void SetValue(ref TextAutoSize container, TextAutoSizeMode value)
			{
				container.mode = value;
			}
		}

		private class MinSizeProperty : Property<TextAutoSize, Length>
		{
			public override string Name { get; } = "minSize";

			public override bool IsReadOnly { get; } = false;

			public override Length GetValue(ref TextAutoSize container)
			{
				return container.minSize;
			}

			public override void SetValue(ref TextAutoSize container, Length value)
			{
				container.minSize = value;
			}
		}

		private class MaxSizeProperty : Property<TextAutoSize, Length>
		{
			public override string Name { get; } = "maxSize";

			public override bool IsReadOnly { get; } = false;

			public override Length GetValue(ref TextAutoSize container)
			{
				return container.maxSize;
			}

			public override void SetValue(ref TextAutoSize container, Length value)
			{
				container.maxSize = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new ModeProperty());
			AddProperty(new MinSizeProperty());
			AddProperty(new MaxSizeProperty());
		}
	}

	public TextAutoSizeMode mode { get; set; }

	public Length minSize { get; set; }

	public Length maxSize { get; set; }

	public TextAutoSize(TextAutoSizeMode mode, Length minSize, Length maxSize)
	{
		this.mode = mode;
		this.minSize = minSize;
		this.maxSize = maxSize;
	}

	public static TextAutoSize None()
	{
		return new TextAutoSize
		{
			mode = TextAutoSizeMode.None,
			maxSize = 100f,
			minSize = 10f
		};
	}

	public bool Equals(TextAutoSize other)
	{
		return mode == other.mode && minSize.Equals(other.minSize) && maxSize.Equals(other.maxSize);
	}

	public override bool Equals(object obj)
	{
		return obj is TextAutoSize other && Equals(other);
	}

	public override int GetHashCode()
	{
		int num = 1500536833;
		num = num * -1521134295 + mode.GetHashCode();
		num = num * -1521134295 + minSize.GetHashCode();
		return num * -1521134295 + maxSize.GetHashCode();
	}

	public static bool operator ==(TextAutoSize left, TextAutoSize right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TextAutoSize left, TextAutoSize right)
	{
		return !(left == right);
	}
}
