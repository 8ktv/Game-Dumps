using System;
using Unity.Properties;

namespace UnityEngine.UIElements;

public struct BackgroundSize : IEquatable<BackgroundSize>
{
	internal class PropertyBag : ContainerPropertyBag<BackgroundSize>
	{
		private class SizeTypeProperty : Property<BackgroundSize, BackgroundSizeType>
		{
			public override string Name { get; } = "sizeType";

			public override bool IsReadOnly { get; } = false;

			public override BackgroundSizeType GetValue(ref BackgroundSize container)
			{
				return container.sizeType;
			}

			public override void SetValue(ref BackgroundSize container, BackgroundSizeType value)
			{
				container.sizeType = value;
			}
		}

		private class XProperty : Property<BackgroundSize, Length>
		{
			public override string Name { get; } = "x";

			public override bool IsReadOnly { get; } = false;

			public override Length GetValue(ref BackgroundSize container)
			{
				return container.x;
			}

			public override void SetValue(ref BackgroundSize container, Length value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<BackgroundSize, Length>
		{
			public override string Name { get; } = "y";

			public override bool IsReadOnly { get; } = false;

			public override Length GetValue(ref BackgroundSize container)
			{
				return container.y;
			}

			public override void SetValue(ref BackgroundSize container, Length value)
			{
				container.y = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new SizeTypeProperty());
			AddProperty(new XProperty());
			AddProperty(new YProperty());
		}
	}

	private BackgroundSizeType m_SizeType;

	private Length m_X;

	private Length m_Y;

	public BackgroundSizeType sizeType
	{
		get
		{
			return m_SizeType;
		}
		set
		{
			m_SizeType = value;
			m_X = new Length(0f);
			m_Y = new Length(0f);
		}
	}

	public Length x
	{
		get
		{
			return m_X;
		}
		set
		{
			m_X = value;
			m_SizeType = BackgroundSizeType.Length;
		}
	}

	public Length y
	{
		get
		{
			return m_Y;
		}
		set
		{
			m_Y = value;
			m_SizeType = BackgroundSizeType.Length;
		}
	}

	public BackgroundSize(Length sizeX, Length sizeY)
	{
		m_SizeType = BackgroundSizeType.Length;
		m_X = sizeX;
		m_Y = sizeY;
	}

	public BackgroundSize(BackgroundSizeType sizeType)
	{
		m_SizeType = sizeType;
		m_X = new Length(0f);
		m_Y = new Length(0f);
	}

	internal static BackgroundSize Initial()
	{
		return BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize();
	}

	public override bool Equals(object obj)
	{
		return obj is BackgroundSize && Equals((BackgroundSize)obj);
	}

	public bool Equals(BackgroundSize other)
	{
		return other.x == x && other.y == y && other.sizeType == sizeType;
	}

	public override int GetHashCode()
	{
		int num = 1500536833;
		num = num * -1521134295 + m_SizeType.GetHashCode();
		num = num * -1521134295 + m_X.GetHashCode();
		return num * -1521134295 + m_Y.GetHashCode();
	}

	public static bool operator ==(BackgroundSize style1, BackgroundSize style2)
	{
		return style1.Equals(style2);
	}

	public static bool operator !=(BackgroundSize style1, BackgroundSize style2)
	{
		return !(style1 == style2);
	}

	public override string ToString()
	{
		return $"(sizeType:{sizeType} x:{x}, y:{y})";
	}
}
