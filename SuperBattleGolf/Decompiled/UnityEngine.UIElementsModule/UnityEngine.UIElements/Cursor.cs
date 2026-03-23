using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public struct Cursor : IEquatable<Cursor>
{
	internal class PropertyBag : ContainerPropertyBag<Cursor>
	{
		private class TextureProperty : Property<Cursor, Texture2D>
		{
			public override string Name { get; } = "texture";

			public override bool IsReadOnly { get; } = false;

			public override Texture2D GetValue(ref Cursor container)
			{
				return container.texture;
			}

			public override void SetValue(ref Cursor container, Texture2D value)
			{
				container.texture = value;
			}
		}

		private class HotspotProperty : Property<Cursor, Vector2>
		{
			public override string Name { get; } = "hotspot";

			public override bool IsReadOnly { get; } = false;

			public override Vector2 GetValue(ref Cursor container)
			{
				return container.hotspot;
			}

			public override void SetValue(ref Cursor container, Vector2 value)
			{
				container.hotspot = value;
			}
		}

		private class DefaultCursorIdProperty : Property<Cursor, int>
		{
			public override string Name { get; } = "defaultCursorId";

			public override bool IsReadOnly { get; } = false;

			public override int GetValue(ref Cursor container)
			{
				return container.defaultCursorId;
			}

			public override void SetValue(ref Cursor container, int value)
			{
				container.defaultCursorId = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new TextureProperty());
			AddProperty(new HotspotProperty());
			AddProperty(new DefaultCursorIdProperty());
		}
	}

	public Texture2D texture { get; set; }

	public Vector2 hotspot { get; set; }

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int defaultCursorId { get; set; }

	internal static IEnumerable<Type> allowedAssetTypes
	{
		get
		{
			yield return typeof(Texture2D);
		}
	}

	public override bool Equals(object obj)
	{
		return obj is Cursor && Equals((Cursor)obj);
	}

	public bool Equals(Cursor other)
	{
		return EqualityComparer<Texture2D>.Default.Equals(texture, other.texture) && hotspot.Equals(other.hotspot) && defaultCursorId == other.defaultCursorId;
	}

	public override int GetHashCode()
	{
		int num = 1500536833;
		num = num * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(texture);
		num = num * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(hotspot);
		return num * -1521134295 + defaultCursorId.GetHashCode();
	}

	public static bool operator ==(Cursor style1, Cursor style2)
	{
		return style1.Equals(style2);
	}

	public static bool operator !=(Cursor style1, Cursor style2)
	{
		return !(style1 == style2);
	}

	public override string ToString()
	{
		return $"texture={texture}, hotspot={hotspot}";
	}
}
