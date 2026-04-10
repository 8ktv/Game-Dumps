namespace UnityEngine.UIElements.UIR;

internal class DefaultElementBuilder : BaseElementBuilder
{
	private RenderTreeManager m_RenderTreeManager;

	public DefaultElementBuilder(RenderTreeManager renderTreeManager)
	{
		m_RenderTreeManager = renderTreeManager;
	}

	public override bool RequiresStencilMask(VisualElement ve)
	{
		return UIRUtility.IsRoundRect(ve) || UIRUtility.IsVectorImageBackground(ve);
	}

	protected override void DrawVisualElementBackground(MeshGenerationContext mgc)
	{
		VisualElement visualElement = mgc.visualElement;
		RenderData renderData = mgc.renderData;
		if (visualElement.layout.width <= 1E-30f || visualElement.layout.height <= 1E-30f)
		{
			return;
		}
		ComputedStyle computedStyle = visualElement.computedStyle;
		Color backgroundColor = computedStyle.backgroundColor;
		renderData.backgroundAlpha = backgroundColor.a;
		if (backgroundColor.a > 1E-30f)
		{
			MeshGenerator.RectangleParams rectParams = new MeshGenerator.RectangleParams
			{
				rect = visualElement.rect,
				uv = new Rect(0f, 0f, 1f, 1f),
				color = backgroundColor,
				colorPage = ColorPage.Init(m_RenderTreeManager, renderData.backgroundColorID),
				playmodeTintColor = visualElement.playModeTintColor
			};
			MeshGenerator.GetVisualElementRadii(visualElement, out rectParams.topLeftRadius, out rectParams.bottomLeftRadius, out rectParams.topRightRadius, out rectParams.bottomRightRadius);
			MeshGenerator.AdjustBackgroundSizeForBorders(visualElement, ref rectParams);
			mgc.meshGenerator.DrawRectangle(rectParams);
		}
		Vector4 slices = new Vector4(computedStyle.unitySliceLeft, computedStyle.unitySliceTop, computedStyle.unitySliceRight, computedStyle.unitySliceBottom);
		MeshGenerator.RectangleParams rectangleParams = default(MeshGenerator.RectangleParams);
		MeshGenerator.GetVisualElementRadii(visualElement, out rectangleParams.topLeftRadius, out rectangleParams.bottomLeftRadius, out rectangleParams.topRightRadius, out rectangleParams.bottomRightRadius);
		Background backgroundImage = computedStyle.backgroundImage;
		if (!(backgroundImage.texture != null) && !(backgroundImage.sprite != null) && !(backgroundImage.vectorImage != null) && !(backgroundImage.renderTexture != null))
		{
			return;
		}
		MeshGenerator.RectangleParams rectParams2 = default(MeshGenerator.RectangleParams);
		float num = visualElement.resolvedStyle.unitySliceScale;
		Color playModeTintColor = visualElement.playModeTintColor;
		bool valid;
		ScaleMode scaleMode = BackgroundPropertyHelper.ResolveUnityBackgroundScaleMode(computedStyle.backgroundPositionX, computedStyle.backgroundPositionY, computedStyle.backgroundRepeat, computedStyle.backgroundSize, out valid);
		if (backgroundImage.texture != null)
		{
			bool flag = Mathf.RoundToInt(slices.x) != 0 || Mathf.RoundToInt(slices.y) != 0 || Mathf.RoundToInt(slices.z) != 0 || Mathf.RoundToInt(slices.w) != 0;
			rectParams2 = MeshGenerator.RectangleParams.MakeTextured(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.texture, (!flag) ? ScaleMode.ScaleToFit : (valid ? scaleMode : ScaleMode.StretchToFill), playModeTintColor);
			rectParams2.rect = new Rect(0f, 0f, rectParams2.texture.width, rectParams2.texture.height);
		}
		else if (backgroundImage.sprite != null)
		{
			bool flag2 = !valid || scaleMode == ScaleMode.ScaleAndCrop;
			rectParams2 = MeshGenerator.RectangleParams.MakeSprite(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.sprite, (!flag2) ? scaleMode : ScaleMode.StretchToFill, playModeTintColor, rectangleParams.HasRadius(0.001f), ref slices, flag2);
			if (rectParams2.texture != null)
			{
				rectParams2.rect = new Rect(0f, 0f, backgroundImage.sprite.rect.width, backgroundImage.sprite.rect.height);
			}
			num *= UIElementsUtility.PixelsPerUnitScaleForElement(visualElement, backgroundImage.sprite);
		}
		else if (backgroundImage.renderTexture != null)
		{
			rectParams2 = MeshGenerator.RectangleParams.MakeTextured(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.renderTexture, ScaleMode.ScaleToFit, playModeTintColor);
			rectParams2.rect = new Rect(0f, 0f, rectParams2.texture.width, rectParams2.texture.height);
		}
		else if (backgroundImage.vectorImage != null)
		{
			bool flag3 = !valid || scaleMode == ScaleMode.ScaleAndCrop;
			rectParams2 = MeshGenerator.RectangleParams.MakeVectorTextured(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.vectorImage, (!flag3) ? scaleMode : ScaleMode.StretchToFill, playModeTintColor);
			rectParams2.rect = new Rect(0f, 0f, rectParams2.vectorImage.size.x, rectParams2.vectorImage.size.y);
		}
		rectParams2.topLeftRadius = rectangleParams.topLeftRadius;
		rectParams2.topRightRadius = rectangleParams.topRightRadius;
		rectParams2.bottomRightRadius = rectangleParams.bottomRightRadius;
		rectParams2.bottomLeftRadius = rectangleParams.bottomLeftRadius;
		if (slices != Vector4.zero)
		{
			rectParams2.leftSlice = Mathf.RoundToInt(slices.x);
			rectParams2.topSlice = Mathf.RoundToInt(slices.y);
			rectParams2.rightSlice = Mathf.RoundToInt(slices.z);
			rectParams2.bottomSlice = Mathf.RoundToInt(slices.w);
			rectParams2.sliceScale = num;
			if (computedStyle.unitySliceType == SliceType.Tiled)
			{
				rectParams2.meshFlags |= MeshGenerationContext.MeshFlags.SliceTiled;
			}
			if (!valid)
			{
				rectParams2.backgroundPositionX = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition();
				rectParams2.backgroundPositionY = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition();
				rectParams2.backgroundRepeat = BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat();
				rectParams2.backgroundSize = BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize();
			}
			else
			{
				rectParams2.backgroundPositionX = computedStyle.backgroundPositionX;
				rectParams2.backgroundPositionY = computedStyle.backgroundPositionY;
				rectParams2.backgroundRepeat = computedStyle.backgroundRepeat;
				rectParams2.backgroundSize = computedStyle.backgroundSize;
			}
		}
		else
		{
			rectParams2.backgroundPositionX = computedStyle.backgroundPositionX;
			rectParams2.backgroundPositionY = computedStyle.backgroundPositionY;
			rectParams2.backgroundRepeat = computedStyle.backgroundRepeat;
			rectParams2.backgroundSize = computedStyle.backgroundSize;
		}
		rectParams2.color = computedStyle.unityBackgroundImageTintColor;
		rectParams2.colorPage = ColorPage.Init(m_RenderTreeManager, visualElement.renderData.tintColorID);
		MeshGenerator.AdjustBackgroundSizeForBorders(visualElement, ref rectParams2);
		if (rectParams2.texture != null || rectParams2.vectorImage != null)
		{
			mgc.meshGenerator.DrawRectangleRepeat(rectParams2, visualElement.rect, visualElement.scaledPixelsPerPoint);
		}
		else
		{
			mgc.meshGenerator.DrawRectangle(rectParams2);
		}
	}

	protected override void DrawVisualElementBorder(MeshGenerationContext mgc)
	{
		VisualElement visualElement = mgc.visualElement;
		RenderData renderData = mgc.renderData;
		if (visualElement.layout.width >= 1E-30f && visualElement.layout.height >= 1E-30f)
		{
			IResolvedStyle resolvedStyle = visualElement.resolvedStyle;
			if ((resolvedStyle.borderLeftColor != Color.clear && resolvedStyle.borderLeftWidth > 0f) || (resolvedStyle.borderTopColor != Color.clear && resolvedStyle.borderTopWidth > 0f) || (resolvedStyle.borderRightColor != Color.clear && resolvedStyle.borderRightWidth > 0f) || (resolvedStyle.borderBottomColor != Color.clear && resolvedStyle.borderBottomWidth > 0f))
			{
				MeshGenerator.BorderParams borderParams = new MeshGenerator.BorderParams
				{
					rect = visualElement.rect,
					leftColor = resolvedStyle.borderLeftColor,
					topColor = resolvedStyle.borderTopColor,
					rightColor = resolvedStyle.borderRightColor,
					bottomColor = resolvedStyle.borderBottomColor,
					leftWidth = resolvedStyle.borderLeftWidth,
					topWidth = resolvedStyle.borderTopWidth,
					rightWidth = resolvedStyle.borderRightWidth,
					bottomWidth = resolvedStyle.borderBottomWidth,
					leftColorPage = ColorPage.Init(m_RenderTreeManager, renderData.borderLeftColorID),
					topColorPage = ColorPage.Init(m_RenderTreeManager, renderData.borderTopColorID),
					rightColorPage = ColorPage.Init(m_RenderTreeManager, renderData.borderRightColorID),
					bottomColorPage = ColorPage.Init(m_RenderTreeManager, renderData.borderBottomColorID),
					playmodeTintColor = visualElement.playModeTintColor
				};
				MeshGenerator.GetVisualElementRadii(visualElement, out borderParams.topLeftRadius, out borderParams.bottomLeftRadius, out borderParams.topRightRadius, out borderParams.bottomRightRadius);
				mgc.meshGenerator.DrawBorder(borderParams);
			}
		}
	}

	protected override void DrawVisualElementStencilMask(MeshGenerationContext mgc)
	{
		if (UIRUtility.IsVectorImageBackground(mgc.visualElement))
		{
			DrawVisualElementBackground(mgc);
		}
		else
		{
			GenerateStencilClipEntryForRoundedRectBackground(mgc);
		}
	}

	private static void GenerateStencilClipEntryForRoundedRectBackground(MeshGenerationContext mgc)
	{
		VisualElement visualElement = mgc.visualElement;
		if (!(visualElement.layout.width <= 1E-30f) && !(visualElement.layout.height <= 1E-30f))
		{
			IResolvedStyle resolvedStyle = visualElement.resolvedStyle;
			MeshGenerator.GetVisualElementRadii(visualElement, out var topLeft, out var bottomLeft, out var topRight, out var bottomRight);
			float borderTopWidth = resolvedStyle.borderTopWidth;
			float borderLeftWidth = resolvedStyle.borderLeftWidth;
			float borderBottomWidth = resolvedStyle.borderBottomWidth;
			float borderRightWidth = resolvedStyle.borderRightWidth;
			MeshGenerator.RectangleParams rectParams = new MeshGenerator.RectangleParams
			{
				rect = visualElement.rect,
				color = Color.white,
				topLeftRadius = Vector2.Max(Vector2.zero, topLeft - new Vector2(borderLeftWidth, borderTopWidth)),
				topRightRadius = Vector2.Max(Vector2.zero, topRight - new Vector2(borderRightWidth, borderTopWidth)),
				bottomLeftRadius = Vector2.Max(Vector2.zero, bottomLeft - new Vector2(borderLeftWidth, borderBottomWidth)),
				bottomRightRadius = Vector2.Max(Vector2.zero, bottomRight - new Vector2(borderRightWidth, borderBottomWidth)),
				playmodeTintColor = visualElement.playModeTintColor
			};
			rectParams.rect.x += borderLeftWidth;
			rectParams.rect.y += borderTopWidth;
			rectParams.rect.width -= borderLeftWidth + borderRightWidth;
			rectParams.rect.height -= borderTopWidth + borderBottomWidth;
			if (visualElement.computedStyle.unityOverflowClipBox == OverflowClipBox.ContentBox)
			{
				rectParams.rect.x += resolvedStyle.paddingLeft;
				rectParams.rect.y += resolvedStyle.paddingTop;
				rectParams.rect.width -= resolvedStyle.paddingLeft + resolvedStyle.paddingRight;
				rectParams.rect.height -= resolvedStyle.paddingTop + resolvedStyle.paddingBottom;
			}
			mgc.meshGenerator.DrawRectangle(rectParams);
		}
	}

	public override void ScheduleMeshGenerationJobs(MeshGenerationContext mgc)
	{
		mgc.meshGenerator.ScheduleJobs(mgc);
		if (mgc.hasPainter2D)
		{
			mgc.painter2D.ScheduleJobs(mgc);
		}
	}
}
