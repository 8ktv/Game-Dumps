namespace UnityEngine.Rendering;

public interface IRenderPipelineGraphicsSettings
{
	int version { get; }

	bool isAvailableInPlayerBuild => false;

	void Reset()
	{
	}
}
