namespace UnityEngine.UIElements;

public static class RuntimePanelUtils
{
	public static Vector2 ScreenToPanel(IPanel panel, Vector2 screenPosition)
	{
		return ((BaseRuntimePanel)panel).ScreenToPanel(screenPosition);
	}

	public static Vector2 CameraTransformWorldToPanel(IPanel panel, Vector3 worldPosition, Camera camera)
	{
		Vector2 p = camera.WorldToScreenPoint(worldPosition);
		float editorDisplayHeight = UIElementsRuntimeUtility.GetEditorDisplayHeight(camera.targetDisplay);
		p = UIElementsRuntimeUtility.FlipY(p, editorDisplayHeight);
		return ((BaseRuntimePanel)panel).ScreenToPanel(p);
	}

	public static Rect CameraTransformWorldToPanelRect(IPanel panel, Vector3 worldPosition, Vector2 worldSize, Camera camera)
	{
		worldSize.y = 0f - worldSize.y;
		Vector2 vector = CameraTransformWorldToPanel(panel, worldPosition, camera);
		Vector3 worldPosition2 = worldPosition + camera.worldToCameraMatrix.MultiplyVector((Vector3)worldSize);
		Vector2 vector2 = CameraTransformWorldToPanel(panel, worldPosition2, camera);
		return new Rect(vector, vector2 - vector);
	}

	public static void ResetDynamicAtlas(this IPanel panel)
	{
		if (panel is BaseVisualElementPanel { atlas: DynamicAtlas atlas })
		{
			atlas.Reset();
		}
	}

	public static void ResetRenderer(this IPanel panel)
	{
		if (panel is BaseVisualElementPanel { panelRenderer: { } panelRenderer })
		{
			panelRenderer.Reset();
		}
	}

	public static void SetTextureDirty(this IPanel panel, Texture2D texture)
	{
		if (panel is BaseVisualElementPanel { atlas: DynamicAtlas atlas })
		{
			atlas.SetDirty(texture);
		}
	}
}
