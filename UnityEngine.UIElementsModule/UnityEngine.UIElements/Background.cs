using System;
using System.Collections.Generic;
using Unity.Properties;

namespace UnityEngine.UIElements;

public struct Background : IEquatable<Background>
{
	internal class PropertyBag : ContainerPropertyBag<Background>
	{
		private class TextureProperty : Property<Background, Texture2D>
		{
			public override string Name { get; } = "texture";

			public override bool IsReadOnly { get; } = false;

			public override Texture2D GetValue(ref Background container)
			{
				return container.texture;
			}

			public override void SetValue(ref Background container, Texture2D value)
			{
				container.texture = value;
			}
		}

		private class SpriteProperty : Property<Background, Sprite>
		{
			public override string Name { get; } = "sprite";

			public override bool IsReadOnly { get; } = false;

			public override Sprite GetValue(ref Background container)
			{
				return container.sprite;
			}

			public override void SetValue(ref Background container, Sprite value)
			{
				container.sprite = value;
			}
		}

		private class RenderTextureProperty : Property<Background, RenderTexture>
		{
			public override string Name { get; } = "renderTexture";

			public override bool IsReadOnly { get; } = false;

			public override RenderTexture GetValue(ref Background container)
			{
				return container.renderTexture;
			}

			public override void SetValue(ref Background container, RenderTexture value)
			{
				container.renderTexture = value;
			}
		}

		private class VectorImageProperty : Property<Background, VectorImage>
		{
			public override string Name { get; } = "vectorImage";

			public override bool IsReadOnly { get; } = false;

			public override VectorImage GetValue(ref Background container)
			{
				return container.vectorImage;
			}

			public override void SetValue(ref Background container, VectorImage value)
			{
				container.vectorImage = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new TextureProperty());
			AddProperty(new SpriteProperty());
			AddProperty(new RenderTextureProperty());
			AddProperty(new VectorImageProperty());
		}
	}

	private Texture2D m_Texture;

	private Sprite m_Sprite;

	private RenderTexture m_RenderTexture;

	private VectorImage m_VectorImage;

	public Texture2D texture
	{
		get
		{
			return m_Texture;
		}
		set
		{
			if (!(m_Texture == value))
			{
				m_Texture = value;
				m_Sprite = null;
				m_RenderTexture = null;
				m_VectorImage = null;
			}
		}
	}

	public Sprite sprite
	{
		get
		{
			return m_Sprite;
		}
		set
		{
			if (!(m_Sprite == value))
			{
				m_Texture = null;
				m_Sprite = value;
				m_RenderTexture = null;
				m_VectorImage = null;
			}
		}
	}

	public RenderTexture renderTexture
	{
		get
		{
			return m_RenderTexture;
		}
		set
		{
			if (!(m_RenderTexture == value))
			{
				m_Texture = null;
				m_Sprite = null;
				m_RenderTexture = value;
				m_VectorImage = null;
			}
		}
	}

	public VectorImage vectorImage
	{
		get
		{
			return m_VectorImage;
		}
		set
		{
			if (!(vectorImage == value))
			{
				m_Texture = null;
				m_Sprite = null;
				m_RenderTexture = null;
				m_VectorImage = value;
			}
		}
	}

	internal static IEnumerable<Type> allowedAssetTypes
	{
		get
		{
			yield return typeof(Texture2D);
			yield return typeof(RenderTexture);
			yield return typeof(Sprite);
			yield return typeof(VectorImage);
		}
	}

	[Obsolete("Use Background.FromTexture2D instead")]
	public Background(Texture2D t)
	{
		m_Texture = t;
		m_Sprite = null;
		m_RenderTexture = null;
		m_VectorImage = null;
	}

	public static Background FromTexture2D(Texture2D t)
	{
		return new Background
		{
			texture = t
		};
	}

	public static Background FromRenderTexture(RenderTexture rt)
	{
		return new Background
		{
			renderTexture = rt
		};
	}

	public static Background FromSprite(Sprite s)
	{
		return new Background
		{
			sprite = s
		};
	}

	public static Background FromVectorImage(VectorImage vi)
	{
		return new Background
		{
			vectorImage = vi
		};
	}

	internal static Background FromObject(object obj)
	{
		Texture2D texture2D = obj as Texture2D;
		if (texture2D != null)
		{
			return FromTexture2D(texture2D);
		}
		RenderTexture renderTexture = obj as RenderTexture;
		if (renderTexture != null)
		{
			return FromRenderTexture(renderTexture);
		}
		Sprite sprite = obj as Sprite;
		if (sprite != null)
		{
			return FromSprite(sprite);
		}
		VectorImage vectorImage = obj as VectorImage;
		if (vectorImage != null)
		{
			return FromVectorImage(vectorImage);
		}
		return default(Background);
	}

	public Object GetSelectedImage()
	{
		if (texture != null)
		{
			return texture;
		}
		if (sprite != null)
		{
			return sprite;
		}
		if (renderTexture != null)
		{
			return renderTexture;
		}
		if (vectorImage != null)
		{
			return vectorImage;
		}
		return null;
	}

	public bool IsEmpty()
	{
		return texture == null && sprite == null && vectorImage == null && renderTexture == null;
	}

	public static bool operator ==(Background lhs, Background rhs)
	{
		return lhs.texture == rhs.texture && lhs.sprite == rhs.sprite && lhs.renderTexture == rhs.renderTexture && lhs.vectorImage == rhs.vectorImage;
	}

	public static bool operator !=(Background lhs, Background rhs)
	{
		return !(lhs == rhs);
	}

	public static implicit operator Background(Texture2D v)
	{
		return FromTexture2D(v);
	}

	public bool Equals(Background other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Background background))
		{
			return false;
		}
		return background == this;
	}

	public override int GetHashCode()
	{
		int num = 851985039;
		if ((object)texture != null)
		{
			num = num * -1521134295 + texture.GetHashCode();
		}
		if ((object)sprite != null)
		{
			num = num * -1521134295 + sprite.GetHashCode();
		}
		if ((object)renderTexture != null)
		{
			num = num * -1521134295 + renderTexture.GetHashCode();
		}
		if ((object)vectorImage != null)
		{
			num = num * -1521134295 + vectorImage.GetHashCode();
		}
		return num;
	}

	public override string ToString()
	{
		if (texture != null)
		{
			return texture.ToString();
		}
		if (sprite != null)
		{
			return sprite.ToString();
		}
		if (renderTexture != null)
		{
			return renderTexture.ToString();
		}
		if (vectorImage != null)
		{
			return vectorImage.ToString();
		}
		return "";
	}
}
