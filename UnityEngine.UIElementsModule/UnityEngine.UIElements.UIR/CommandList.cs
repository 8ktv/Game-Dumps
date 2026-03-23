using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR;

internal class CommandList : IDisposable
{
	public VisualElement m_Owner;

	private readonly IntPtr m_VertexDecl;

	private readonly IntPtr m_StencilState;

	public MaterialPropertyBlock constantProps = new MaterialPropertyBlock();

	public MaterialPropertyBlock batchProps = new MaterialPropertyBlock();

	public GCHandle handle;

	public Material m_Material;

	private List<SerializedCommand> m_Commands = new List<SerializedCommand>();

	private Vector4[] m_GpuTextureData = new Vector4[TextureSlotManager.k_SlotSize * TextureSlotManager.k_SlotCount];

	private NativeList<DrawBufferRange> m_DrawRanges;

	public int Count => m_Commands.Count;

	protected bool disposed { get; private set; }

	public CommandList(VisualElement owner, IntPtr vertexDecl, IntPtr stencilState, Material material)
	{
		m_Owner = owner;
		m_VertexDecl = vertexDecl;
		m_StencilState = stencilState;
		m_DrawRanges = new NativeList<DrawBufferRange>(1024);
		handle = GCHandle.Alloc(this);
		m_Material = material;
	}

	public void Reset(VisualElement newOwner, Material material)
	{
		m_Owner = newOwner;
		m_Commands.Clear();
		m_DrawRanges.Clear();
		m_Material = material;
		for (int i = 0; i < m_GpuTextureData.Length; i++)
		{
			m_GpuTextureData[i] = Vector4.zero;
		}
		batchProps.Clear();
	}

	public unsafe void Execute()
	{
		IntPtr* ptr = stackalloc IntPtr[1];
		Utility.SetPropertyBlock(constantProps);
		Utility.SetStencilState(m_StencilState, 0);
		for (int i = 0; i < m_Commands.Count; i++)
		{
			SerializedCommand serializedCommand = m_Commands[i];
			switch (serializedCommand.type)
			{
			case SerializedCommandType.SetTexture:
				batchProps.SetTexture(serializedCommand.textureName, serializedCommand.texture);
				m_GpuTextureData[serializedCommand.gpuDataOffset] = serializedCommand.gpuData0;
				m_GpuTextureData[serializedCommand.gpuDataOffset + 1] = serializedCommand.gpuData1;
				batchProps.SetVectorArray(TextureSlotManager.textureTableId, m_GpuTextureData);
				break;
			case SerializedCommandType.ApplyBatchProps:
				Utility.SetPropertyBlock(batchProps);
				break;
			case SerializedCommandType.DrawRanges:
				*ptr = serializedCommand.vertexBuffer;
				Utility.DrawRanges(serializedCommand.indexBuffer, ptr, 1, new IntPtr(m_DrawRanges.GetSlice(serializedCommand.firstRange, serializedCommand.rangeCount).GetUnsafePtr()), serializedCommand.rangeCount, m_VertexDecl);
				break;
			default:
				throw new NotImplementedException();
			}
		}
	}

	public void SetTexture(int name, Texture texture, int gpuDataOffset, Vector4 gpuData0, Vector4 gpuData1)
	{
		SerializedCommand item = new SerializedCommand
		{
			type = SerializedCommandType.SetTexture,
			textureName = name,
			texture = texture,
			gpuDataOffset = gpuDataOffset,
			gpuData0 = gpuData0,
			gpuData1 = gpuData1
		};
		m_Commands.Add(item);
	}

	public void ApplyBatchProps()
	{
		SerializedCommand item = new SerializedCommand
		{
			type = SerializedCommandType.ApplyBatchProps
		};
		m_Commands.Add(item);
	}

	public void DrawRanges(Utility.GPUBuffer<ushort> ib, Utility.GPUBuffer<Vertex> vb, NativeSlice<DrawBufferRange> ranges)
	{
		SerializedCommand item = new SerializedCommand
		{
			type = SerializedCommandType.DrawRanges,
			vertexBuffer = vb.BufferPointer,
			indexBuffer = ib.BufferPointer,
			firstRange = m_DrawRanges.Count,
			rangeCount = ranges.Length
		};
		m_Commands.Add(item);
		m_DrawRanges.Add(ranges);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			m_DrawRanges.Dispose();
			m_DrawRanges = null;
			if (handle.IsAllocated)
			{
				handle.Free();
			}
		}
		disposed = true;
	}
}
