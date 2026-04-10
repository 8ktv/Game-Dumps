namespace UnityEngine.UIElements;

public static class PanelExtensions
{
	public static AbstractGenericMenu CreateMenu(this IPanel panel)
	{
		if (panel is BaseVisualElementPanel baseVisualElementPanel)
		{
			return baseVisualElementPanel.CreateMenu();
		}
		return null;
	}
}
