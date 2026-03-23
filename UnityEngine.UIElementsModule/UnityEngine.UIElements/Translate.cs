using System;
using System.Globalization;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public struct Translate : IEquatable<Translate>
{
	internal class PropertyBag : ContainerPropertyBag<Translate>
	{
		private class XProperty : Property<Translate, Length>
		{
			public override string Name { get; } = "x";

			public override bool IsReadOnly { get; } = false;

			public override Length GetValue(ref Translate container)
			{
				return container.x;
			}

			public override void SetValue(ref Translate container, Length value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<Translate, Length>
		{
			public override string Name { get; } = "y";

			public override bool IsReadOnly { get; } = false;

			public override Length GetValue(ref Translate container)
			{
				return container.y;
			}

			public override void SetValue(ref Translate container, Length value)
			{
				container.y = value;
			}
		}

		private class ZProperty : Property<Translate, float>
		{
			public override string Name { get; } = "z";

			public override bool IsReadOnly { get; } = false;

			public override float GetValue(ref Translate container)
			{
				return container.z;
			}

			public override void SetValue(ref Translate container, float value)
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

	private Length m_X;

	private Length m_Y;

	private float m_Z;

	private bool m_isNone;

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

	public Translate(Length x, Length y, float z)
	{
		m_X = x;
		m_Y = y;
		m_Z = z;
		m_isNone = false;
	}

	public Translate(Length x, Length y)
		: this(x, y, 0f)
	{
	}

	internal Translate(Vector3 v)
		: this(v.x, v.y, v.z)
	{
	}

	public static implicit operator Translate(Vector3 v)
	{
		return new Translate(v);
	}

	public static implicit operator Translate(Vector2 v)
	{
		return new Translate(v);
	}

	public static Translate None()
	{
		return new Translate
		{
			m_isNone = true
		};
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool IsNone()
	{
		return m_isNone;
	}

	public static bool operator ==(Translate lhs, Translate rhs)
	{
		return lhs.m_X == rhs.m_X && lhs.m_Y == rhs.m_Y && lhs.m_Z == rhs.m_Z && lhs.m_isNone == rhs.m_isNone;
	}

	public static bool operator !=(Translate lhs, Translate rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(Translate other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is Translate other && Equals(other);
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
