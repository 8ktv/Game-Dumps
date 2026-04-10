namespace UnityEngine.UIElements;

public struct FilterPassContext
{
	public FilterFunction filterFunction { get; internal set; }

	public PostProcessingPass postProcessingPass => filterFunction.GetDefinition().passes[filterPassIndex];

	public int filterPassIndex { get; internal set; }

	public bool readsGamma { get; internal set; }

	public bool writesGamma { get; internal set; }

	public float scaledPixelsPerPoint { get; internal set; }
}
