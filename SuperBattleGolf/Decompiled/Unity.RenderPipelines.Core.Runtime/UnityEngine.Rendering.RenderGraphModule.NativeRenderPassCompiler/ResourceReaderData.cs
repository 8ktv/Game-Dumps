namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

internal readonly struct ResourceReaderData
{
	public readonly int passId;

	public readonly int inputSlot;

	public ResourceReaderData(int _passId, int _inputSlot)
	{
		passId = _passId;
		inputSlot = _inputSlot;
	}
}
