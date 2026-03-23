#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR;

internal class EntryProcessor
{
	private struct MaskMesh
	{
		public NativeSlice<Vertex> vertices;

		public NativeSlice<ushort> indices;

		public int indexOffset;
	}

	private EntryPreProcessor m_PreProcessor = new EntryPreProcessor();

	private RenderTreeManager m_RenderTreeManager;

	private RenderData m_CurrentRenderData;

	private int m_MaskDepth;

	private int m_MaskDepthPopped;

	private int m_MaskDepthPushed;

	private int m_StencilRef;

	private int m_StencilRefPopped;

	private int m_StencilRefPushed;

	private BMPAlloc m_ClipRectId;

	private BMPAlloc m_ClipRectIdPopped;

	private BMPAlloc m_ClipRectIdPushed;

	private bool m_IsDrawingMask;

	private Stack<MaskMesh> m_MaskMeshes = new Stack<MaskMesh>(1);

	private bool m_VertexDataComputed;

	private Matrix4x4 m_Transform;

	private Color32 m_TransformData;

	private Color32 m_OpacityData;

	private Color32 m_TextCoreSettingsPage;

	private MeshHandle m_Mesh;

	private NativeSlice<Vertex> m_Verts;

	private NativeSlice<ushort> m_Indices;

	private ushort m_IndexOffset;

	private int m_AllocVertexCount;

	private int m_AllocIndex;

	private int m_VertsFilled;

	private int m_IndicesFilled;

	private VertexFlags m_RenderType;

	private bool m_RemapUVs;

	private Rect m_AtlasRect;

	private int m_GradientSettingIndexOffset;

	private bool m_IsTail;

	private RenderChainCommand m_FirstCommand;

	private RenderChainCommand m_LastCommand;

	public RenderChainCommand firstHeadCommand { get; private set; }

	public RenderChainCommand lastHeadCommand { get; private set; }

	public RenderChainCommand firstTailCommand { get; private set; }

	public RenderChainCommand lastTailCommand { get; private set; }

	public void Init(Entry root, RenderTreeManager renderTreeManager, RenderData renderData)
	{
		UIRenderDevice device = renderTreeManager.device;
		m_RenderTreeManager = renderTreeManager;
		m_CurrentRenderData = renderData;
		m_PreProcessor.PreProcess(root);
		if (m_PreProcessor.headAllocs.Count == 0 && renderData.headMesh != null)
		{
			device.Free(renderData.headMesh);
			renderData.headMesh = null;
		}
		if (m_PreProcessor.tailAllocs.Count == 0 && renderData.tailMesh != null)
		{
			device.Free(renderData.tailMesh);
			renderData.tailMesh = null;
		}
		if (renderData.hasExtraMeshes)
		{
			renderTreeManager.FreeExtraMeshes(renderData);
		}
		renderTreeManager.ResetTextures(renderData);
		RenderData parent = renderData.parent;
		bool isGroupTransform = renderData.isGroupTransform;
		if (parent != null)
		{
			m_MaskDepthPopped = parent.childrenMaskDepth;
			m_StencilRefPopped = parent.childrenStencilRef;
			m_ClipRectIdPopped = (isGroupTransform ? UIRVEShaderInfoAllocator.infiniteClipRect : parent.clipRectID);
		}
		else
		{
			m_MaskDepthPopped = 0;
			m_StencilRefPopped = 0;
			m_ClipRectIdPopped = UIRVEShaderInfoAllocator.infiniteClipRect;
		}
		m_MaskDepthPushed = m_MaskDepthPopped + 1;
		m_StencilRefPushed = m_MaskDepthPopped;
		m_ClipRectIdPushed = renderData.clipRectID;
		m_MaskDepth = m_MaskDepthPopped;
		m_StencilRef = m_StencilRefPopped;
		m_ClipRectId = m_ClipRectIdPopped;
		m_VertexDataComputed = false;
		m_Transform = Matrix4x4.identity;
		m_TextCoreSettingsPage = new Color32(0, 0, 0, 0);
		m_MaskMeshes.Clear();
		m_IsDrawingMask = false;
	}

	public void ClearReferences()
	{
		m_PreProcessor.ClearReferences();
		m_RenderTreeManager = null;
		m_CurrentRenderData = null;
		m_Mesh = null;
		m_FirstCommand = null;
		m_LastCommand = null;
		firstHeadCommand = null;
		lastHeadCommand = null;
		firstTailCommand = null;
		lastTailCommand = null;
	}

	public void ProcessHead()
	{
		m_IsTail = false;
		ProcessFirstAlloc(m_PreProcessor.headAllocs, ref m_CurrentRenderData.headMesh);
		m_FirstCommand = null;
		m_LastCommand = null;
		ProcessRange(0, m_PreProcessor.childrenIndex - 1);
		firstHeadCommand = m_FirstCommand;
		lastHeadCommand = m_LastCommand;
	}

	public void ProcessTail()
	{
		m_IsTail = true;
		ProcessFirstAlloc(m_PreProcessor.tailAllocs, ref m_CurrentRenderData.tailMesh);
		m_FirstCommand = null;
		m_LastCommand = null;
		ProcessRange(m_PreProcessor.childrenIndex + 1, m_PreProcessor.flattenedEntries.Count - 1);
		firstTailCommand = m_FirstCommand;
		lastTailCommand = m_LastCommand;
		Debug.Assert(m_MaskDepth == m_MaskDepthPopped);
		Debug.Assert(m_MaskMeshes.Count == 0);
		Debug.Assert(!m_IsDrawingMask);
	}

	private void ProcessRange(int first, int last)
	{
		List<Entry> flattenedEntries = m_PreProcessor.flattenedEntries;
		for (int i = first; i <= last; i++)
		{
			Entry entry = flattenedEntries[i];
			switch (entry.type)
			{
			case EntryType.DrawSolidMesh:
				m_RenderType = VertexFlags.IsSolid;
				ProcessMeshEntry(entry, TextureId.invalid);
				break;
			case EntryType.DrawTexturedMesh:
			{
				Texture texture = entry.texture;
				TextureId atlas = TextureId.invalid;
				if (texture != null)
				{
					if (m_RenderTreeManager.atlas != null && m_RenderTreeManager.atlas.TryGetAtlas(m_CurrentRenderData.owner, texture as Texture2D, out atlas, out var atlasRect))
					{
						m_RenderType = VertexFlags.IsDynamic;
						m_AtlasRect = new Rect(atlasRect.x, atlasRect.y, atlasRect.width, atlasRect.height);
						m_RemapUVs = true;
						m_RenderTreeManager.InsertTexture(m_CurrentRenderData, texture, atlas, isAtlas: true);
					}
					else
					{
						m_RenderType = VertexFlags.IsTextured;
						atlas = TextureRegistry.instance.Acquire(texture);
						m_RenderTreeManager.InsertTexture(m_CurrentRenderData, texture, atlas, isAtlas: false);
					}
				}
				else
				{
					m_RenderType = VertexFlags.IsSolid;
				}
				ProcessMeshEntry(entry, atlas);
				m_RemapUVs = false;
				break;
			}
			case EntryType.DrawTexturedMeshSkipAtlas:
			{
				m_RenderType = VertexFlags.IsTextured;
				TextureId textureId2 = TextureRegistry.instance.Acquire(entry.texture);
				m_RenderTreeManager.InsertTexture(m_CurrentRenderData, entry.texture, textureId2, isAtlas: false);
				ProcessMeshEntry(entry, textureId2);
				break;
			}
			case EntryType.DrawDynamicTexturedMesh:
				m_RenderType = VertexFlags.IsTextured;
				ProcessMeshEntry(entry, entry.textureId);
				break;
			case EntryType.DrawTextMesh:
			{
				m_RenderType = VertexFlags.IsText;
				TextureId textureId3 = TextureRegistry.instance.Acquire(entry.texture);
				m_RenderTreeManager.InsertTexture(m_CurrentRenderData, entry.texture, textureId3, isAtlas: false);
				ProcessMeshEntry(entry, textureId3);
				break;
			}
			case EntryType.DrawGradients:
			{
				m_RenderType = VertexFlags.IsSvgGradients;
				GradientRemap gradientRemap = m_RenderTreeManager.vectorImageManager.AddUser(entry.gradientsOwner, m_CurrentRenderData.owner);
				m_GradientSettingIndexOffset = gradientRemap.destIndex;
				TextureId textureId;
				if (gradientRemap.atlas != TextureId.invalid)
				{
					textureId = gradientRemap.atlas;
				}
				else
				{
					textureId = TextureRegistry.instance.Acquire(entry.gradientsOwner.atlas);
					m_RenderTreeManager.InsertTexture(m_CurrentRenderData, entry.gradientsOwner.atlas, textureId, isAtlas: false);
				}
				ProcessMeshEntry(entry, textureId);
				m_GradientSettingIndexOffset = -1;
				break;
			}
			case EntryType.DrawImmediate:
			{
				RenderChainCommand renderChainCommand9 = m_RenderTreeManager.AllocCommand();
				renderChainCommand9.type = CommandType.Immediate;
				renderChainCommand9.owner = m_CurrentRenderData;
				renderChainCommand9.isTail = m_IsTail;
				renderChainCommand9.callback = entry.immediateCallback;
				AppendCommand(renderChainCommand9);
				break;
			}
			case EntryType.DrawImmediateCull:
			{
				RenderChainCommand renderChainCommand8 = m_RenderTreeManager.AllocCommand();
				renderChainCommand8.type = CommandType.ImmediateCull;
				renderChainCommand8.owner = m_CurrentRenderData;
				renderChainCommand8.isTail = m_IsTail;
				renderChainCommand8.callback = entry.immediateCallback;
				AppendCommand(renderChainCommand8);
				break;
			}
			case EntryType.DrawChildren:
			case EntryType.DedicatedPlaceholder:
				Debug.Assert(condition: false);
				break;
			case EntryType.BeginStencilMask:
				Debug.Assert(m_MaskDepth == m_MaskDepthPopped);
				Debug.Assert(!m_IsDrawingMask);
				m_IsDrawingMask = true;
				m_StencilRef = m_StencilRefPushed;
				Debug.Assert(m_MaskDepth == m_StencilRef);
				break;
			case EntryType.EndStencilMask:
				Debug.Assert(m_IsDrawingMask);
				m_IsDrawingMask = false;
				m_MaskDepth = m_MaskDepthPushed;
				break;
			case EntryType.PopStencilMask:
				Debug.Assert(m_MaskDepth == m_StencilRef + 1);
				DrawReverseMask();
				m_MaskDepth = m_MaskDepthPopped;
				m_StencilRef = m_StencilRefPopped;
				break;
			case EntryType.PushClippingRect:
				m_ClipRectId = m_ClipRectIdPushed;
				break;
			case EntryType.PopClippingRect:
				m_ClipRectId = m_ClipRectIdPopped;
				break;
			case EntryType.PushScissors:
			{
				RenderChainCommand renderChainCommand7 = m_RenderTreeManager.AllocCommand();
				renderChainCommand7.type = CommandType.PushScissor;
				renderChainCommand7.owner = m_CurrentRenderData;
				renderChainCommand7.isTail = m_IsTail;
				AppendCommand(renderChainCommand7);
				break;
			}
			case EntryType.PopScissors:
			{
				RenderChainCommand renderChainCommand6 = m_RenderTreeManager.AllocCommand();
				renderChainCommand6.type = CommandType.PopScissor;
				renderChainCommand6.owner = m_CurrentRenderData;
				renderChainCommand6.isTail = m_IsTail;
				AppendCommand(renderChainCommand6);
				break;
			}
			case EntryType.PushGroupMatrix:
			{
				RenderChainCommand renderChainCommand5 = m_RenderTreeManager.AllocCommand();
				renderChainCommand5.type = CommandType.PushView;
				renderChainCommand5.owner = m_CurrentRenderData;
				renderChainCommand5.isTail = m_IsTail;
				AppendCommand(renderChainCommand5);
				break;
			}
			case EntryType.PopGroupMatrix:
			{
				RenderChainCommand renderChainCommand4 = m_RenderTreeManager.AllocCommand();
				renderChainCommand4.type = CommandType.PopView;
				renderChainCommand4.owner = m_CurrentRenderData;
				renderChainCommand4.isTail = m_IsTail;
				AppendCommand(renderChainCommand4);
				break;
			}
			case EntryType.PushDefaultMaterial:
			{
				RenderChainCommand renderChainCommand3 = m_RenderTreeManager.AllocCommand();
				renderChainCommand3.type = CommandType.PushDefaultMaterial;
				renderChainCommand3.owner = m_CurrentRenderData;
				renderChainCommand3.isTail = m_IsTail;
				renderChainCommand3.state.material = entry.material;
				AppendCommand(renderChainCommand3);
				break;
			}
			case EntryType.PopDefaultMaterial:
			{
				RenderChainCommand renderChainCommand2 = m_RenderTreeManager.AllocCommand();
				renderChainCommand2.type = CommandType.PopDefaultMaterial;
				renderChainCommand2.owner = m_CurrentRenderData;
				renderChainCommand2.isTail = m_IsTail;
				AppendCommand(renderChainCommand2);
				break;
			}
			case EntryType.CutRenderChain:
			{
				RenderChainCommand renderChainCommand = m_RenderTreeManager.AllocCommand();
				renderChainCommand.type = CommandType.CutRenderChain;
				renderChainCommand.owner = m_CurrentRenderData;
				renderChainCommand.isTail = m_IsTail;
				AppendCommand(renderChainCommand);
				break;
			}
			default:
				throw new NotImplementedException();
			}
		}
	}

	private unsafe void ProcessMeshEntry(Entry entry, TextureId textureId)
	{
		int length = entry.vertices.Length;
		int length2 = entry.indices.Length;
		Debug.Assert(length > 0 == length2 > 0);
		if (length > 0 && length2 > 0)
		{
			if (m_VertsFilled + length > m_AllocVertexCount)
			{
				ProcessNextAlloc();
				Debug.Assert(m_VertsFilled + length <= m_AllocVertexCount);
			}
			if (!m_VertexDataComputed)
			{
				UIRUtility.GetVerticesTransformInfo(m_CurrentRenderData, out m_Transform);
				m_CurrentRenderData.verticesSpace = m_Transform;
				m_TransformData = m_RenderTreeManager.shaderInfoAllocator.TransformAllocToVertexData(m_CurrentRenderData.transformID);
				m_OpacityData = m_RenderTreeManager.shaderInfoAllocator.OpacityAllocToVertexData(m_CurrentRenderData.opacityID);
				m_VertexDataComputed = true;
			}
			Color32 opacityPage = new Color32(m_OpacityData.r, m_OpacityData.g, 0, 0);
			Color32 color = m_RenderTreeManager.shaderInfoAllocator.ClipRectAllocToVertexData(m_ClipRectId);
			Color32 ids = new Color32(m_TransformData.b, color.b, m_OpacityData.b, 0);
			Color32 xformClipPages = new Color32(m_TransformData.r, m_TransformData.g, color.r, color.g);
			Color32 addFlags = new Color32((byte)m_RenderType, 0, 0, 0);
			if ((entry.flags & EntryFlags.UsesTextCoreSettings) != 0)
			{
				Color32 color2 = m_RenderTreeManager.shaderInfoAllocator.TextCoreSettingsToVertexData(m_CurrentRenderData.textCoreSettingsID);
				m_TextCoreSettingsPage.r = color2.r;
				m_TextCoreSettingsPage.g = color2.g;
				ids.a = color2.b;
			}
			NativeSlice<Vertex> nativeSlice = m_Verts.Slice(m_VertsFilled, length);
			int indexOffset = m_VertsFilled + m_IndexOffset;
			NativeSlice<ushort> nativeSlice2 = m_Indices.Slice(m_IndicesFilled, length2);
			bool flag = UIRUtility.ShapeWindingIsClockwise(m_MaskDepth, m_StencilRef);
			bool worldFlipsWinding = m_CurrentRenderData.worldFlipsWinding;
			ConvertMeshJobData job = new ConvertMeshJobData
			{
				vertSrc = (IntPtr)entry.vertices.GetUnsafePtr(),
				vertDst = (IntPtr)nativeSlice.GetUnsafePtr(),
				vertCount = length,
				transform = m_Transform,
				xformClipPages = xformClipPages,
				ids = ids,
				addFlags = addFlags,
				opacityPage = opacityPage,
				textCoreSettingsPage = m_TextCoreSettingsPage,
				usesTextCoreSettings = (((entry.flags & EntryFlags.UsesTextCoreSettings) != 0) ? 1 : 0),
				textureId = textureId.ConvertToGpu(),
				gradientSettingsIndexOffset = m_GradientSettingIndexOffset,
				indexSrc = (IntPtr)entry.indices.GetUnsafePtr(),
				indexDst = (IntPtr)nativeSlice2.GetUnsafePtr(),
				indexCount = nativeSlice2.Length,
				indexOffset = indexOffset,
				flipIndices = ((flag == worldFlipsWinding) ? 1 : 0),
				forceZ = (m_RenderTreeManager.isFlat ? 1 : 0),
				positionZ = (m_IsDrawingMask ? 1f : 0f),
				remapUVs = (m_RemapUVs ? 1 : 0),
				atlasRect = m_AtlasRect
			};
			m_RenderTreeManager.jobManager.Add(ref job);
			if (m_IsDrawingMask)
			{
				m_MaskMeshes.Push(new MaskMesh
				{
					vertices = nativeSlice,
					indices = nativeSlice2,
					indexOffset = indexOffset
				});
			}
			RenderChainCommand renderChainCommand = CreateMeshDrawCommand(m_Mesh, length2, m_IndicesFilled, entry.material, textureId);
			AppendCommand(renderChainCommand);
			if (entry.type == EntryType.DrawTextMesh)
			{
				renderChainCommand.state.sdfScale = entry.textScale;
				renderChainCommand.state.sharpness = entry.fontSharpness;
			}
			renderChainCommand.state.isPremultiplied = (entry.flags & EntryFlags.IsPremultiplied) != 0;
			m_VertsFilled += length;
			m_IndicesFilled += length2;
		}
	}

	private unsafe void DrawReverseMask()
	{
		MaskMesh result;
		while (m_MaskMeshes.TryPop(out result))
		{
			Debug.Assert(result.indices.Length > 0 == result.vertices.Length > 0);
			if (result.indices.Length > 0 && result.vertices.Length > 0)
			{
				RenderChainCommand next = CreateMeshDrawCommand(m_Mesh, result.indices.Length, m_IndicesFilled, null, TextureId.invalid);
				AppendCommand(next);
				NativeSlice<Vertex> nativeSlice = m_Verts.Slice(m_VertsFilled, result.vertices.Length);
				NativeSlice<ushort> nativeSlice2 = m_Indices.Slice(m_IndicesFilled, result.indices.Length);
				CopyMeshJobData job = new CopyMeshJobData
				{
					vertSrc = (IntPtr)result.vertices.GetUnsafePtr(),
					vertDst = (IntPtr)nativeSlice.GetUnsafePtr(),
					vertCount = result.vertices.Length,
					indexSrc = (IntPtr)result.indices.GetUnsafePtr(),
					indexDst = (IntPtr)nativeSlice2.GetUnsafePtr(),
					indexCount = result.indices.Length,
					indexOffset = m_IndexOffset + m_VertsFilled - result.indexOffset
				};
				m_RenderTreeManager.jobManager.Add(ref job);
				m_IndicesFilled += result.indices.Length;
				m_VertsFilled += result.vertices.Length;
			}
		}
	}

	private RenderChainCommand CreateMeshDrawCommand(MeshHandle mesh, int indexCount, int indexOffset, Material material, TextureId texture)
	{
		RenderChainCommand renderChainCommand = m_RenderTreeManager.AllocCommand();
		renderChainCommand.type = CommandType.Draw;
		renderChainCommand.state = new State
		{
			material = material,
			texture = texture,
			stencilRef = m_StencilRef
		};
		renderChainCommand.mesh = mesh;
		renderChainCommand.indexOffset = indexOffset;
		renderChainCommand.indexCount = indexCount;
		renderChainCommand.owner = m_CurrentRenderData;
		renderChainCommand.isTail = m_IsTail;
		return renderChainCommand;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AppendCommand(RenderChainCommand next)
	{
		if (m_FirstCommand == null)
		{
			m_FirstCommand = next;
			m_LastCommand = next;
		}
		else
		{
			next.prev = m_LastCommand;
			m_LastCommand.next = next;
			m_LastCommand = next;
		}
	}

	private void ProcessFirstAlloc(List<EntryPreProcessor.AllocSize> allocList, ref MeshHandle mesh)
	{
		if (allocList.Count > 0)
		{
			EntryPreProcessor.AllocSize allocSize = allocList[0];
			UpdateOrAllocate(ref mesh, allocSize.vertexCount, allocSize.indexCount, m_RenderTreeManager.device, out m_Verts, out m_Indices, out m_IndexOffset, ref m_RenderTreeManager.statsByRef);
			m_AllocVertexCount = (int)mesh.allocVerts.size;
		}
		else
		{
			Debug.Assert(mesh == null);
			m_Verts = default(NativeSlice<Vertex>);
			m_Indices = default(NativeSlice<ushort>);
			m_IndexOffset = 0;
			m_AllocVertexCount = 0;
		}
		m_Mesh = mesh;
		m_VertsFilled = 0;
		m_IndicesFilled = 0;
		m_AllocIndex = 0;
	}

	private void ProcessNextAlloc()
	{
		List<EntryPreProcessor.AllocSize> list = (m_IsTail ? m_PreProcessor.tailAllocs : m_PreProcessor.headAllocs);
		Debug.Assert(m_AllocIndex < list.Count - 1);
		EntryPreProcessor.AllocSize allocSize = list[++m_AllocIndex];
		m_Mesh = null;
		UpdateOrAllocate(ref m_Mesh, allocSize.vertexCount, allocSize.indexCount, m_RenderTreeManager.device, out m_Verts, out m_Indices, out m_IndexOffset, ref m_RenderTreeManager.statsByRef);
		m_AllocVertexCount = (int)m_Mesh.allocVerts.size;
		m_RenderTreeManager.InsertExtraMesh(m_CurrentRenderData, m_Mesh);
		m_VertsFilled = 0;
		m_IndicesFilled = 0;
	}

	private static void UpdateOrAllocate(ref MeshHandle data, int vertexCount, int indexCount, UIRenderDevice device, out NativeSlice<Vertex> verts, out NativeSlice<ushort> indices, out ushort indexOffset, ref ChainBuilderStats stats)
	{
		if (data != null)
		{
			if (data.allocVerts.size >= vertexCount && data.allocIndices.size >= indexCount)
			{
				device.Update(data, (uint)vertexCount, (uint)indexCount, out verts, out indices, out indexOffset);
				stats.updatedMeshAllocations++;
			}
			else
			{
				device.Free(data);
				data = device.Allocate((uint)vertexCount, (uint)indexCount, out verts, out indices, out indexOffset);
				stats.newMeshAllocations++;
			}
		}
		else
		{
			data = device.Allocate((uint)vertexCount, (uint)indexCount, out verts, out indices, out indexOffset);
			stats.newMeshAllocations++;
		}
	}
}
