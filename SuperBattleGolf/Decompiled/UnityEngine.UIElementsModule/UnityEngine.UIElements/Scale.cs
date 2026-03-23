using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public struct Scale : IEquatable<Scale>
{
	internal class PropertyBag : ContainerPropertyBag<Scale>
	{
		private class ValueProperty : Property<Scale, Vector3>
		{
			public override string Name { get; } = "value";

			public override bool IsReadOnly { get; } = false;

			public override Vector3 GetValue(ref Scale container)
			{
				return container.value;
			}

			public override void SetValue(ref Scale container, Vector3 value)
			{
				container.value = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new ValueProperty());
		}
	}

	private Vector3 m_Scale;

	private bool m_IsNone;

	public Vector3 value
	{
		get
		{
			return m_Scale;
		}
		set
		{
			m_Scale = value;
		}
	}

	public Scale(Vector2 scale)
	{
		m_Scale = new Vector3(scale.x, scale.y, 1f);
		m_IsNone = false;
	}

	public Scale(Vector3 scale)
	{
		m_Scale = scale;
		m_IsNone = false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static Scale Initial()
	{
		return new Scale(Vector3.one);
	}

	public static Scale None()
	{
		Scale result = Initial();
		result.m_IsNone = true;
		return result;
	}

	internal bool IsNone()
	{
		return m_IsNone;
	}

	public static implicit operator Scale(Vector2 scale)
	{
		return new Scale(scale);
	}

	public static implicit operator Scale(Vector3 scale)
	{
		return new Scale(scale);
	}

	public static bool operator ==(Scale lhs, Scale rhs)
	{
		return lhs.m_Scale == rhs.m_Scale;
	}

	public static bool operator !=(Scale lhs, Scale rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(Scale other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is Scale other && Equals(other);
	}

	public override int GetHashCode()
	{
		return m_Scale.GetHashCode() * 793;
	}

	public override string ToString()
	{
		return m_Scale.ToString();
	}
}
