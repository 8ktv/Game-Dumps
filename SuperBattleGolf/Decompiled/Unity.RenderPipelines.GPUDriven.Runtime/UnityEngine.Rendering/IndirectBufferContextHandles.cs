using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering;

internal struct IndirectBufferContextHandles
{
	public BufferHandle instanceBuffer;

	public BufferHandle instanceInfoBuffer;

	public BufferHandle dispatchArgsBuffer;

	public BufferHandle drawArgsBuffer;

	public BufferHandle drawInfoBuffer;

	public void UseForOcclusionTest(IBaseRenderGraphBuilder builder)
	{
		instanceBuffer = builder.UseBuffer(in instanceBuffer, AccessFlags.ReadWrite);
		instanceInfoBuffer = builder.UseBuffer(in instanceInfoBuffer);
		dispatchArgsBuffer = builder.UseBuffer(in dispatchArgsBuffer, AccessFlags.ReadWrite);
		drawArgsBuffer = builder.UseBuffer(in drawArgsBuffer, AccessFlags.ReadWrite);
		drawInfoBuffer = builder.UseBuffer(in drawInfoBuffer);
	}
}
