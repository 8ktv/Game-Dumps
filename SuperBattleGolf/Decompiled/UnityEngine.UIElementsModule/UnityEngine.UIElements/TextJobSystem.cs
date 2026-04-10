namespace UnityEngine.UIElements;

internal class TextJobSystem
{
	internal UITKTextJobSystem m_UITKTextJobSystem;

	private ATGTextJobSystem m_ATGTextJobSystem;

	internal void GenerateText(MeshGenerationContext mgc, TextElement textElement)
	{
		if (TextUtilities.IsAdvancedTextEnabledForElement(textElement))
		{
			if (m_ATGTextJobSystem == null)
			{
				m_ATGTextJobSystem = new ATGTextJobSystem();
			}
			m_ATGTextJobSystem.GenerateText(mgc, textElement);
		}
		else
		{
			if (m_UITKTextJobSystem == null)
			{
				m_UITKTextJobSystem = new UITKTextJobSystem();
			}
			m_UITKTextJobSystem.GenerateText(mgc, textElement);
		}
	}

	internal void PrepareShapingBeforeLayout(BaseVisualElementPanel panel)
	{
		if (m_ATGTextJobSystem == null)
		{
			m_ATGTextJobSystem = new ATGTextJobSystem();
		}
		m_ATGTextJobSystem.PrepareShapingBeforeLayout(panel);
	}
}
