namespace UnityEngine.UIElements;

internal interface IPanelRenderer
{
	bool forceGammaRendering { get; set; }

	uint vertexBudget { get; set; }

	void Reset();

	void Render();
}
