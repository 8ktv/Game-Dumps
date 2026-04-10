using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.UIR;

internal class CommandListManager : IDisposable
{
	public static class Testing
	{
		public static List<CommandList> GetCurrentFrameCommandLists(CommandListManager instance)
		{
			return instance.m_CurrentFrameCommandLists;
		}
	}

	private readonly IntPtr m_VertexDecl;

	private readonly IntPtr m_DefaultStencilState;

	private uint m_CurrentIndex = 3u;

	private Stack<CommandList> m_CommandListPool = new Stack<CommandList>();

	private CommandList m_DefaultCommandList = new CommandList(IntPtr.Zero, IntPtr.Zero);

	private List<CommandList>[] m_CommandListsArray;

	private List<CommandList> m_CurrentFrameCommandLists;

	private List<UIRenderer> m_UIRenderersWithDrawCallData = new List<UIRenderer>();

	private TextureSlotCount m_TextureSlotCount;

	public CommandList defaultCommandList => m_DefaultCommandList;

	protected bool disposed { get; private set; }

	public CommandListManager(IntPtr vertexDecl, IntPtr defaultStencilState)
	{
		m_VertexDecl = vertexDecl;
		m_DefaultStencilState = defaultStencilState;
		m_CommandListsArray = new List<CommandList>[4];
		for (int i = 0; (long)i < 4L; i++)
		{
			m_CommandListsArray[i] = new List<CommandList>();
		}
	}

	public CommandList GetOrCreateCommandList(VisualElement owner, Material material, CommandFlags commandFlags)
	{
		CommandList commandList = ((m_CommandListPool.Count <= 0) ? new CommandList(m_VertexDecl, m_DefaultStencilState) : m_CommandListPool.Pop());
		commandList.Init(owner, material, commandFlags);
		m_CurrentFrameCommandLists.Add(commandList);
		return commandList;
	}

	public void AdvanceFrame()
	{
		m_CurrentIndex++;
		if (m_CurrentIndex == 4)
		{
			m_CurrentIndex = 0u;
		}
		m_CurrentFrameCommandLists = m_CommandListsArray[m_CurrentIndex];
		for (int i = 0; i < m_CurrentFrameCommandLists.Count; i++)
		{
			CommandList commandList = m_CurrentFrameCommandLists[i];
			commandList.Reset();
			m_CommandListPool.Push(commandList);
		}
		m_CurrentFrameCommandLists.Clear();
		ResetUIRendererDrawCallData();
	}

	public void BeginSerialize(TextureSlotCount textureSlotCount)
	{
		m_TextureSlotCount = textureSlotCount;
		m_DefaultCommandList.Init(null, null, CommandFlags.None);
	}

	public void EndSerialize()
	{
		for (int i = 0; i < m_CurrentFrameCommandLists.Count; i++)
		{
			CommandList commandList = m_CurrentFrameCommandLists[i];
			UIRenderer renderer = commandList.m_Renderer;
			if (renderer != null)
			{
				renderer.commandLists = m_CommandListsArray;
				bool flag = (commandList.flags & CommandFlags.ForceSingleTextureSlot) != 0;
				uint forceRenderType = (uint)(commandList.flags & CommandFlags.ForceRenderTypeBits) >> 1;
				renderer.AddDrawCallData((int)m_CurrentIndex, i, commandList.m_Material, (uint)(flag ? TextureSlotCount.One : m_TextureSlotCount), forceRenderType);
				if (m_UIRenderersWithDrawCallData.Count == 0 || m_UIRenderersWithDrawCallData[m_UIRenderersWithDrawCallData.Count - 1] != renderer)
				{
					m_UIRenderersWithDrawCallData.Add(renderer);
				}
			}
		}
		m_DefaultCommandList.Reset();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	public void ResetUIRendererDrawCallData()
	{
		foreach (UIRenderer uIRenderersWithDrawCallDatum in m_UIRenderersWithDrawCallData)
		{
			if (uIRenderersWithDrawCallDatum != null)
			{
				uIRenderersWithDrawCallDatum.ResetDrawCallData();
			}
		}
		m_UIRenderersWithDrawCallData.Clear();
	}

	protected void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			m_DefaultCommandList.Dispose();
			m_DefaultCommandList = null;
			for (int i = 0; i < m_CommandListsArray.Length; i++)
			{
				List<CommandList> list = m_CommandListsArray[i];
				for (int j = 0; j < list.Count; j++)
				{
					list[j].Dispose();
				}
				list.Clear();
			}
			m_CommandListsArray = null;
		}
		disposed = true;
	}
}
