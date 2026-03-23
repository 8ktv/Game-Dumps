namespace UnityEngine.UIElements.Layout;

internal static class LayoutProcessor
{
	private static ILayoutProcessor s_Processor = new LayoutProcessorNative();

	public static ILayoutProcessor Processor
	{
		get
		{
			return s_Processor;
		}
		set
		{
			s_Processor = value ?? new LayoutProcessorNative();
		}
	}

	public static void CalculateLayout(LayoutNode node, float parentWidth, float parentHeight, LayoutDirection parentDirection)
	{
		s_Processor.CalculateLayout(node, parentWidth, parentHeight, parentDirection);
	}
}
