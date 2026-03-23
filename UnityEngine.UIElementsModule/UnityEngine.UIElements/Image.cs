using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Internal;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

public class Image : VisualElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		public override object CreateInstance()
		{
			return new Image();
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<Image, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield break;
			}
		}
	}

	internal static readonly BindingId imageProperty = "image";

	internal static readonly BindingId spriteProperty = "sprite";

	internal static readonly BindingId vectorImageProperty = "vectorImage";

	internal static readonly BindingId sourceRectProperty = "sourceRect";

	internal static readonly BindingId uvProperty = "uv";

	internal static readonly BindingId scaleModeProperty = "scaleMode";

	internal static readonly BindingId tintColorProperty = "tintColor";

	private ScaleMode m_ScaleMode;

	private Texture m_Image;

	private Sprite m_Sprite;

	private VectorImage m_VectorImage;

	private Rect m_UV;

	private Color m_TintColor;

	internal bool m_ImageIsInline;

	private bool m_ScaleModeIsInline;

	private bool m_TintColorIsInline;

	public static readonly string ussClassName = "unity-image";

	private static CustomStyleProperty<Texture2D> s_ImageProperty = new CustomStyleProperty<Texture2D>("--unity-image");

	private static CustomStyleProperty<Sprite> s_SpriteProperty = new CustomStyleProperty<Sprite>("--unity-image");

	private static CustomStyleProperty<VectorImage> s_VectorImageProperty = new CustomStyleProperty<VectorImage>("--unity-image");

	private static CustomStyleProperty<string> s_ScaleModeProperty = new CustomStyleProperty<string>("--unity-image-size");

	private static CustomStyleProperty<Color> s_TintColorProperty = new CustomStyleProperty<Color>("--unity-image-tint-color");

	[CreateProperty]
	public Texture image
	{
		get
		{
			return m_Image;
		}
		set
		{
			if (!(m_Image == value) || !m_ImageIsInline)
			{
				m_ImageIsInline = value != null;
				SetProperty(value, ref m_Image, ref m_Sprite, ref m_VectorImage, imageProperty);
			}
		}
	}

	[CreateProperty]
	public Sprite sprite
	{
		get
		{
			return m_Sprite;
		}
		set
		{
			if (!(m_Sprite == value) || !m_ImageIsInline)
			{
				m_ImageIsInline = value != null;
				SetProperty(value, ref m_Sprite, ref m_Image, ref m_VectorImage, spriteProperty);
			}
		}
	}

	[CreateProperty]
	public VectorImage vectorImage
	{
		get
		{
			return m_VectorImage;
		}
		set
		{
			if (!(m_VectorImage == value) || !m_ImageIsInline)
			{
				m_ImageIsInline = value != null;
				SetProperty(value, ref m_VectorImage, ref m_Image, ref m_Sprite, vectorImageProperty);
			}
		}
	}

	[CreateProperty]
	public Rect sourceRect
	{
		get
		{
			return GetSourceRect();
		}
		set
		{
			if (!(GetSourceRect() == value))
			{
				if (sprite != null)
				{
					Debug.LogError("Cannot set sourceRect on a sprite image");
					return;
				}
				CalculateUV(value);
				NotifyPropertyChanged(in sourceRectProperty);
			}
		}
	}

	[CreateProperty]
	public Rect uv
	{
		get
		{
			return m_UV;
		}
		set
		{
			if (!(m_UV == value))
			{
				m_UV = value;
				NotifyPropertyChanged(in uvProperty);
			}
		}
	}

	[CreateProperty]
	public ScaleMode scaleMode
	{
		get
		{
			return m_ScaleMode;
		}
		set
		{
			if (m_ScaleMode != value || !m_ScaleModeIsInline)
			{
				m_ScaleModeIsInline = true;
				SetScaleMode(value);
			}
		}
	}

	[CreateProperty]
	public Color tintColor
	{
		get
		{
			return m_TintColor;
		}
		set
		{
			if (!(m_TintColor == value) || !m_TintColorIsInline)
			{
				m_TintColorIsInline = true;
				SetTintColor(value);
			}
		}
	}

	public Image()
	{
		AddToClassList(ussClassName);
		m_ScaleMode = ScaleMode.ScaleToFit;
		m_TintColor = Color.white;
		m_UV = new Rect(0f, 0f, 1f, 1f);
		base.requireMeasureFunction = true;
		RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
	}

	private Vector2 GetTextureDisplaySize(Texture texture)
	{
		Vector2 result = Vector2.zero;
		if (texture != null)
		{
			result = new Vector2(texture.width, texture.height);
		}
		return result;
	}

	private Vector2 GetTextureDisplaySize(Sprite sprite)
	{
		Vector2 result = Vector2.zero;
		if (sprite != null)
		{
			float num = UIElementsUtility.PixelsPerUnitScaleForElement(this, sprite);
			result = (Vector2)(sprite.bounds.size * sprite.pixelsPerUnit) * num;
		}
		return result;
	}

	protected internal override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode)
	{
		float x = float.NaN;
		float y = float.NaN;
		if (image == null && sprite == null && vectorImage == null)
		{
			return new Vector2(x, y);
		}
		Vector2 zero = Vector2.zero;
		zero = ((image != null) ? GetTextureDisplaySize(image) : ((!(sprite != null)) ? vectorImage.size : GetTextureDisplaySize(sprite)));
		Rect rect = sourceRect;
		bool flag = rect != Rect.zero;
		x = (flag ? Mathf.Abs(rect.width) : zero.x);
		y = (flag ? Mathf.Abs(rect.height) : zero.y);
		if (widthMode == MeasureMode.AtMost)
		{
			x = Mathf.Min(x, desiredWidth);
		}
		if (heightMode == MeasureMode.AtMost)
		{
			y = Mathf.Min(y, desiredHeight);
		}
		return new Vector2(x, y);
	}

	private void OnGenerateVisualContent(MeshGenerationContext mgc)
	{
		if (!(image == null) || !(sprite == null) || !(vectorImage == null))
		{
			Rect containerRect = GUIUtility.AlignRectToDevice(base.contentRect);
			Color color = mgc.visualElement?.playModeTintColor ?? Color.white;
			MeshGenerator.RectangleParams rectParams = default(MeshGenerator.RectangleParams);
			if (image != null)
			{
				rectParams = MeshGenerator.RectangleParams.MakeTextured(containerRect, uv, image, scaleMode, color);
			}
			else if (sprite != null)
			{
				Vector4 slices = Vector4.zero;
				rectParams = MeshGenerator.RectangleParams.MakeSprite(containerRect, uv, sprite, scaleMode, color, hasRadius: false, ref slices);
			}
			else if (vectorImage != null)
			{
				rectParams = MeshGenerator.RectangleParams.MakeVectorTextured(containerRect, uv, vectorImage, scaleMode, color);
			}
			rectParams.color = tintColor;
			mgc.meshGenerator.DrawRectangle(rectParams);
		}
	}

	private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
	{
		ReadCustomProperties(e.customStyle);
	}

	private void ReadCustomProperties(ICustomStyle customStyleProvider)
	{
		if (!m_ImageIsInline)
		{
			Sprite value2;
			VectorImage value3;
			if (customStyleProvider.TryGetValue(s_ImageProperty, out var value))
			{
				SetProperty(value, ref m_Image, ref m_Sprite, ref m_VectorImage, imageProperty);
			}
			else if (customStyleProvider.TryGetValue(s_SpriteProperty, out value2))
			{
				SetProperty(value2, ref m_Sprite, ref m_Image, ref m_VectorImage, spriteProperty);
			}
			else if (customStyleProvider.TryGetValue(s_VectorImageProperty, out value3))
			{
				SetProperty(value3, ref m_VectorImage, ref m_Image, ref m_Sprite, vectorImageProperty);
			}
			else
			{
				ClearProperty();
			}
		}
		if (!m_ScaleModeIsInline && customStyleProvider.TryGetValue(s_ScaleModeProperty, out var value4))
		{
			StylePropertyUtil.TryGetEnumIntValue(StyleEnumType.ScaleMode, value4, out var intValue);
			SetScaleMode((ScaleMode)intValue);
		}
		if (!m_TintColorIsInline)
		{
			if (customStyleProvider.TryGetValue(s_TintColorProperty, out var value5))
			{
				SetTintColor(value5);
			}
			else
			{
				SetTintColor(Color.white);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetProperty<T0, T1, T2>(T0 src, ref T0 dst, ref T1 alt0, ref T2 alt1, BindingId binding) where T0 : Object where T1 : Object where T2 : Object
	{
		if (!(src == dst))
		{
			dst = src;
			if (dst != null)
			{
				alt0 = null;
				alt1 = null;
			}
			if (dst == null)
			{
				uv = new Rect(0f, 0f, 1f, 1f);
				ReadCustomProperties(base.customStyle);
			}
			IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
			NotifyPropertyChanged(in binding);
		}
	}

	private void ClearProperty()
	{
		if (!m_ImageIsInline)
		{
			image = null;
			sprite = null;
			vectorImage = null;
		}
	}

	private void SetScaleMode(ScaleMode mode)
	{
		if (m_ScaleMode != mode)
		{
			m_ScaleMode = mode;
			IncrementVersion(VersionChangeType.Repaint);
			NotifyPropertyChanged(in scaleModeProperty);
		}
	}

	private void SetTintColor(Color color)
	{
		if (m_TintColor != color)
		{
			m_TintColor = color;
			IncrementVersion(VersionChangeType.Repaint);
			NotifyPropertyChanged(in tintColorProperty);
		}
	}

	private void CalculateUV(Rect srcRect)
	{
		m_UV = new Rect(0f, 0f, 1f, 1f);
		Vector2 vector = Vector2.zero;
		Texture texture = image;
		if (texture != null)
		{
			vector = GetTextureDisplaySize(texture);
		}
		VectorImage vectorImage = this.vectorImage;
		if (vectorImage != null)
		{
			vector = vectorImage.size;
		}
		if (vector != Vector2.zero)
		{
			m_UV.x = srcRect.x / vector.x;
			m_UV.width = srcRect.width / vector.x;
			m_UV.height = srcRect.height / vector.y;
			m_UV.y = 1f - m_UV.height - srcRect.y / vector.y;
		}
	}

	private Rect GetSourceRect()
	{
		Rect zero = Rect.zero;
		Vector2 vector = Vector2.zero;
		Texture texture = image;
		if (texture != null)
		{
			vector = GetTextureDisplaySize(texture);
		}
		VectorImage vectorImage = this.vectorImage;
		if (vectorImage != null)
		{
			vector = vectorImage.size;
		}
		if (vector != Vector2.zero)
		{
			zero.x = uv.x * vector.x;
			zero.width = uv.width * vector.x;
			zero.y = (1f - uv.y - uv.height) * vector.y;
			zero.height = uv.height * vector.y;
		}
		return zero;
	}
}
