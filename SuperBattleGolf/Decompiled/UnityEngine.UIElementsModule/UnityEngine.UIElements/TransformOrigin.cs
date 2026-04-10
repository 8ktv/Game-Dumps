using System;
using System.Globalization;
using Unity.Properties;

namespace UnityEngine.UIElements;

[Serializable]
public struct TransformOrigin : IEquatable<TransformOrigin>
{
	internal class PropertyBag : ContainerPropertyBag<TransformOrigin>
	{
		private class XProperty : Property<TransformOrigin, Length>
		{
			public override string Name { get; } = "x";

			public override bool IsReadOnly { get; } = false;

			public override Length GetValue(ref TransformOrigin container)
			{
				return container.x;
			}

			public override void SetValue(ref TransformOrigin container, Length value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<TransformOrigin, Length>
		{
			public override string Name { get; } = "y";

			public override bool IsReadOnly { get; } = false;

			public override Length GetValue(ref TransformOrigin container)
			{
				return container.y;
			}

			public override void SetValue(ref TransformOrigin container, Length value)
			{
				container.y = value;
			}
		}

		private class ZProperty : Property<TransformOrigin, float>
		{
			public override string Name { get; } = "z";

			public override bool IsReadOnly { get; } = false;

			public override float GetValue(ref TransformOrigin container)
			{
				return container.z;
			}

			public override void SetValue(ref TransformOrigin container, float value)
			{
				container.z = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new XProperty());
			AddProperty(new YProperty());
			AddProperty(new ZProperty());
		}
	}

	[SerializeField]
	private Length m_X;

	[SerializeField]
	private Length m_Y;

	[SerializeField]
	private float m_Z;

	public Length x
	{
		get
		{
			return m_X;
		}
		set
		{
			m_X = value;
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
		}
	}

	public float z
	{
		get
		{
			return m_Z;
		}
		set
		{
			m_Z = value;
		}
	}

	public TransformOrigin(Length x, Length y, float z)
	{
		m_X = x;
		m_Y = y;
		m_Z = z;
	}

	public TransformOrigin(Length x, Length y)
		: this(x, y, 0f)
	{
	}

	internal TransformOrigin(Vector3 vector)
		: this(vector.x, vector.y, vector.z)
	{
	}

	public static TransformOrigin Initial()
	{
		return new TransformOrigin(Length.Percent(50f), Length.Percent(50f), 0f);
	}

	public static bool operator ==(TransformOrigin lhs, TransformOrigin rhs)
	{
		return lhs.m_X == rhs.m_X && lhs.m_Y == rhs.m_Y && lhs.m_Z == rhs.m_Z;
	}

	public static bool operator !=(TransformOrigin lhs, TransformOrigin rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(TransformOrigin other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is TransformOrigin other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (m_X.GetHashCode() * 793) ^ (m_Y.GetHashCode() * 791) ^ (m_Z.GetHashCode() * 571);
	}

	public override string ToString()
	{
		string text = m_Z.ToString(CultureInfo.InvariantCulture.NumberFormat);
		return m_X.ToString() + " " + m_Y.ToString() + " " + text;
	}
}
