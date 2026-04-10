#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR;

internal class CommandList : IDisposable
{
	private static readonly MemoryLabel k_MemoryLabel = new MemoryLabel("UIElements", "Renderer.CommandList");

	public VisualElement m_Owner;

	public UIRenderer m_Renderer;

	private readonly IntPtr m_VertexDecl;

	private readonly IntPtr m_StencilState;

	public MaterialPropertyBlock constantProps = new MaterialPropertyBlock();

	public GCHandle handle;

	public Material m_Material;

	public CommandFlags flags;

	private List<SerializedCommand> m_Commands = new List<SerializedCommand>();

	private Vector4[] m_GpuTextureData = new Vector4[TextureSlotManager.k_SlotSize * TextureSlotManager.k_MaxSlotCount];

	private NativeList<DrawBufferRange> m_DrawRanges;

	public int Count => m_Commands.Count;

	protected bool disposed { get; private set; }

	public CommandList(IntPtr vertexDecl, IntPtr stencilState)
	{
		m_VertexDecl = vertexDecl;
		m_StencilState = stencilState;
		m_DrawRanges = new NativeList<DrawBufferRange>(1024, k_MemoryLabel);
		handle = GCHandle.Alloc(this);
	}

	public void Reset()
	{
		m_Owner = null;
		m_Renderer = null;
		m_Material = null;
		m_Commands.Clear();
		m_DrawRanges.Clear();
		constantProps.Clear();
	}

	public void Init(VisualElement owner, Material material, CommandFlags commandFlags)
	{
		Debug.Assert(m_Owner == null);
		m_Owner = owner;
		m_Renderer = (owner as UIDocumentRootElement)?.uiRenderer;
		m_Material = material;
		flags = commandFlags;
		for (int i = 0; i < m_GpuTextureData.Length; i++)
		{
			m_GpuTextureData[i] = Vector4.zero;
		}
	}

	public unsafe void Execute()
	{
		IntPtr* ptr = stackalloc IntPtr[1];
		Utility.SetPropertyBlock(constantProps);
		Utility.SetStencilState(m_StencilState, 0);
		int num = 0;
		int* ptr2 = stackalloc int[8];
		IntPtr* ptr3 = stackalloc IntPtr[8];
		IntPtr shaderPropertySheet = Utility.AllocateShaderPropertySheet();
		try
		{
			for (int i = 0; i < m_Commands.Count; i++)
			{
				SerializedCommand serializedCommand = m_Commands[i];
				switch (serializedCommand.type)
				{
				case SerializedCommandType.SetTexture:
					ptr2[num] = serializedCommand.textureName;
					ptr3[num] = serializedCommand.texturePtr;
					num++;
					m_GpuTextureData[serializedCommand.gpuDataOffset] = serializedCommand.gpuData0;
					m_GpuTextureData[serializedCommand.gpuDataOffset + 1] = serializedCommand.gpuData1;
					break;
				case SerializedCommandType.ApplyBatchProps:
					Utility.SetAllTextures(shaderPropertySheet, new IntPtr(ptr2), new IntPtr(ptr3), num);
					num = 0;
					Utility.SetVectorArray(shaderPropertySheet, TextureSlotManager.textureTableId, m_GpuTextureData);
					Utility.ApplyShaderPropertySheet(shaderPropertySheet);
					break;
				case SerializedCommandType.ApplyUserProps:
					Utility.SetPropertyBlock(serializedCommand.userProps);
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
		finally
		{
			Utility.ReleasePropertySheet(shaderPropertySheet);
		}
	}

	public void SetTexture(int name, Texture texture, int gpuDataOffset, Vector4 gpuData0, Vector4 gpuData1)
	{
		SerializedCommand item = new SerializedCommand
		{
			type = SerializedCommandType.SetTexture,
			textureName = name,
			texturePtr = Object.MarshalledUnityObject.MarshalNotNull(texture),
			gpuDataOffset = gpuDataOffset,
			gpuData0 = gpuData0,
			gpuData1 = gpuData1
		};
		m_Commands.Add(item);
	}

	public void ApplyUserProps(MaterialPropertyBlock userProps)
	{
		SerializedCommand item = new SerializedCommand
		{
			type = SerializedCommandType.ApplyUserProps,
			userProps = userProps
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
