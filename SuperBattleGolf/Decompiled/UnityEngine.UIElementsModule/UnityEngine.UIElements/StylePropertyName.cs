using System;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

public struct StylePropertyName : IEquatable<StylePropertyName>
{
	internal class PropertyBag : ContainerPropertyBag<StylePropertyName>
	{
		private class IdProperty : Property<StylePropertyName, StylePropertyId>
		{
			public override string Name { get; } = "id";

			public override bool IsReadOnly { get; } = true;

			public override StylePropertyId GetValue(ref StylePropertyName container)
			{
				return container.id;
			}

			public override void SetValue(ref StylePropertyName container, StylePropertyId value)
			{
			}
		}

		private class NameProperty : Property<StylePropertyName, string>
		{
			public override string Name { get; } = "name";

			public override bool IsReadOnly { get; } = true;

			public override string GetValue(ref StylePropertyName container)
			{
				return container.name;
			}

			public override void SetValue(ref StylePropertyName container, string value)
			{
			}
		}

		public PropertyBag()
		{
			AddProperty(new IdProperty());
			AddProperty(new NameProperty());
		}
	}

	internal StylePropertyId id { get; }

	private string name { get; }

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static StylePropertyId StylePropertyIdFromString(string name)
	{
		if (StylePropertyUtil.s_NameToId.TryGetValue(name, out var value))
		{
			return value;
		}
		return StylePropertyId.Unknown;
	}

	internal StylePropertyName(StylePropertyId stylePropertyId)
	{
		id = stylePropertyId;
		name = null;
		if (StylePropertyUtil.s_IdToName.TryGetValue(stylePropertyId, out var value))
		{
			name = value;
		}
	}

	public StylePropertyName(string name)
	{
		id = StylePropertyIdFromString(name);
		this.name = null;
		if (id != StylePropertyId.Unknown)
		{
			this.name = name;
		}
	}

	public static bool IsNullOrEmpty(StylePropertyName propertyName)
	{
		return propertyName.id == StylePropertyId.Unknown;
	}

	public static bool operator ==(StylePropertyName lhs, StylePropertyName rhs)
	{
		return lhs.id == rhs.id;
	}

	public static bool operator !=(StylePropertyName lhs, StylePropertyName rhs)
	{
		return lhs.id != rhs.id;
	}

	public static implicit operator StylePropertyName(string name)
	{
		return new StylePropertyName(name);
	}

	public override int GetHashCode()
	{
		return (int)id;
	}

	public override bool Equals(object other)
	{
		return other is StylePropertyName && Equals((StylePropertyName)other);
	}

	public bool Equals(StylePropertyName other)
	{
		return this == other;
	}

	public override string ToString()
	{
		return name;
	}
}
