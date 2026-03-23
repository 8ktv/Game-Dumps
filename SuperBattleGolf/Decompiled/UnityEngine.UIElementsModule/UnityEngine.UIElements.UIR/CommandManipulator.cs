#define UNITY_ASSERTIONS
namespace UnityEngine.UIElements.UIR;

internal static class CommandManipulator
{
	public static void ReplaceHeadCommands(RenderTreeManager renderTreeManager, RenderData renderData, EntryProcessor processor)
	{
		bool flag = false;
		RenderChainCommand prev = null;
		RenderChainCommand next = null;
		if (renderData.firstHeadCommand != null)
		{
			prev = renderData.firstHeadCommand.prev;
			next = renderData.lastHeadCommand.next;
			RemoveChain(renderData.renderTree, renderData.firstHeadCommand, renderData.lastHeadCommand);
			flag = true;
		}
		if (processor.firstHeadCommand != null)
		{
			if (!flag)
			{
				FindHeadCommandInsertionPoint(renderData, out prev, out next);
			}
			if (prev != null)
			{
				processor.firstHeadCommand.prev = prev;
				prev.next = processor.firstHeadCommand;
			}
			if (next != null)
			{
				processor.lastHeadCommand.next = next;
				next.prev = processor.lastHeadCommand;
			}
			renderData.renderTree.OnRenderCommandAdded(processor.firstHeadCommand);
		}
		renderData.firstHeadCommand = processor.firstHeadCommand;
		renderData.lastHeadCommand = processor.lastHeadCommand;
	}

	public static void ReplaceTailCommands(RenderTreeManager renderTreeManager, RenderData renderData, EntryProcessor processor)
	{
		bool flag = false;
		RenderChainCommand prev = null;
		RenderChainCommand next = null;
		if (renderData.firstTailCommand != null)
		{
			prev = renderData.firstTailCommand.prev;
			next = renderData.lastTailCommand.next;
			RemoveChain(renderData.renderTree, renderData.firstTailCommand, renderData.lastTailCommand);
			flag = true;
		}
		if (processor.firstTailCommand != null)
		{
			if (!flag)
			{
				FindTailCommandInsertionPoint(renderData, out prev, out next);
			}
			if (prev != null)
			{
				processor.firstTailCommand.prev = prev;
				prev.next = processor.firstTailCommand;
			}
			if (next != null)
			{
				processor.lastTailCommand.next = next;
				next.prev = processor.lastTailCommand;
			}
			renderData.renderTree.OnRenderCommandAdded(processor.firstTailCommand);
		}
		renderData.firstTailCommand = processor.firstTailCommand;
		renderData.lastTailCommand = processor.lastTailCommand;
	}

	private static RenderChainCommand FindPrevCommand(RenderData candidate, bool searchFromHead)
	{
		while (true)
		{
			if (!searchFromHead)
			{
				if (candidate.lastTailCommand != null)
				{
					return candidate.lastTailCommand;
				}
				if (candidate.lastChild != null)
				{
					candidate = candidate.lastChild;
					continue;
				}
			}
			searchFromHead = false;
			if (candidate.lastHeadCommand != null)
			{
				return candidate.lastHeadCommand;
			}
			if (candidate.prevSibling != null)
			{
				candidate = candidate.prevSibling;
				continue;
			}
			if (candidate.parent == null)
			{
				break;
			}
			candidate = candidate.parent;
			searchFromHead = true;
		}
		return null;
	}

	private static void FindHeadCommandInsertionPoint(RenderData renderData, out RenderChainCommand prev, out RenderChainCommand next)
	{
		Debug.Assert(renderData.firstHeadCommand == null);
		prev = FindPrevCommand(renderData, searchFromHead: true);
		if (prev == null)
		{
			next = renderData.renderTree.firstCommand;
		}
		else
		{
			next = prev.next;
		}
	}

	private static void FindTailCommandInsertionPoint(RenderData renderData, out RenderChainCommand prev, out RenderChainCommand next)
	{
		Debug.Assert(renderData.firstTailCommand == null);
		prev = FindPrevCommand(renderData, searchFromHead: false);
		Debug.Assert(prev != null);
		next = prev.next;
	}

	private static void RemoveChain(RenderTree renderTree, RenderChainCommand first, RenderChainCommand last)
	{
		Debug.Assert(first != null);
		Debug.Assert(last != null);
		renderTree.OnRenderCommandsRemoved(first, last);
		if (first.prev != null)
		{
			first.prev.next = last.next;
		}
		if (last.next != null)
		{
			last.next.prev = first.prev;
		}
		RenderChainCommand renderChainCommand = first;
		RenderChainCommand renderChainCommand2;
		do
		{
			RenderChainCommand next = renderChainCommand.next;
			renderTree.renderTreeManager.FreeCommand(renderChainCommand);
			renderChainCommand2 = renderChainCommand;
			renderChainCommand = next;
		}
		while (renderChainCommand2 != last);
	}

	public static void ResetCommands(RenderTreeManager renderTreeManager, RenderData renderData)
	{
		if (renderData.firstHeadCommand != null)
		{
			renderData.renderTree.OnRenderCommandsRemoved(renderData.firstHeadCommand, renderData.lastHeadCommand);
		}
		RenderChainCommand renderChainCommand = ((renderData.firstHeadCommand != null) ? renderData.firstHeadCommand.prev : null);
		RenderChainCommand renderChainCommand2 = ((renderData.lastHeadCommand != null) ? renderData.lastHeadCommand.next : null);
		Debug.Assert(renderChainCommand == null || renderChainCommand.owner != renderData);
		Debug.Assert(renderChainCommand2 == null || renderChainCommand2 == renderData.firstTailCommand || renderChainCommand2.owner != renderData);
		if (renderChainCommand != null)
		{
			renderChainCommand.next = renderChainCommand2;
		}
		if (renderChainCommand2 != null)
		{
			renderChainCommand2.prev = renderChainCommand;
		}
		if (renderData.firstHeadCommand != null)
		{
			RenderChainCommand renderChainCommand3 = renderData.firstHeadCommand;
			while (renderChainCommand3 != renderData.lastHeadCommand)
			{
				RenderChainCommand next = renderChainCommand3.next;
				renderTreeManager.FreeCommand(renderChainCommand3);
				renderChainCommand3 = next;
			}
			renderTreeManager.FreeCommand(renderChainCommand3);
		}
		renderData.firstHeadCommand = (renderData.lastHeadCommand = null);
		renderChainCommand = ((renderData.firstTailCommand != null) ? renderData.firstTailCommand.prev : null);
		renderChainCommand2 = ((renderData.lastTailCommand != null) ? renderData.lastTailCommand.next : null);
		Debug.Assert(renderChainCommand == null || renderChainCommand.owner != renderData);
		Debug.Assert(renderChainCommand2 == null || renderChainCommand2.owner != renderData);
		if (renderChainCommand != null)
		{
			renderChainCommand.next = renderChainCommand2;
		}
		if (renderChainCommand2 != null)
		{
			renderChainCommand2.prev = renderChainCommand;
		}
		if (renderData.firstTailCommand != null)
		{
			renderData.renderTree.OnRenderCommandsRemoved(renderData.firstTailCommand, renderData.lastTailCommand);
			RenderChainCommand renderChainCommand4 = renderData.firstTailCommand;
			while (renderChainCommand4 != renderData.lastTailCommand)
			{
				RenderChainCommand next2 = renderChainCommand4.next;
				renderTreeManager.FreeCommand(renderChainCommand4);
				renderChainCommand4 = next2;
			}
			renderTreeManager.FreeCommand(renderChainCommand4);
		}
		renderData.firstTailCommand = (renderData.lastTailCommand = null);
	}

	private static void InjectCommandInBetween(RenderTreeManager renderTreeManager, RenderChainCommand cmd, RenderChainCommand prev, RenderChainCommand next)
	{
		if (prev != null)
		{
			cmd.prev = prev;
			prev.next = cmd;
		}
		if (next != null)
		{
			cmd.next = next;
			next.prev = cmd;
		}
		RenderData owner = cmd.owner;
		if (!cmd.isTail)
		{
			if (owner.firstHeadCommand == null || owner.firstHeadCommand == next)
			{
				owner.firstHeadCommand = cmd;
			}
			if (owner.lastHeadCommand == null || owner.lastHeadCommand == prev)
			{
				owner.lastHeadCommand = cmd;
			}
		}
		else
		{
			if (owner.firstTailCommand == null || owner.firstTailCommand == next)
			{
				owner.firstTailCommand = cmd;
			}
			if (owner.lastTailCommand == null || owner.lastTailCommand == prev)
			{
				owner.lastTailCommand = cmd;
			}
		}
		owner.renderTree.OnRenderCommandAdded(cmd);
	}

	public static void DisableElementRendering(RenderTreeManager renderTreeManager, VisualElement ve, bool renderingDisabled)
	{
		RenderData renderData = ve.renderData;
		if (renderData == null)
		{
			return;
		}
		if (renderingDisabled)
		{
			if (renderData.firstHeadCommand == null || renderData.firstHeadCommand.type != CommandType.BeginDisable)
			{
				RenderChainCommand renderChainCommand = renderTreeManager.AllocCommand();
				renderChainCommand.type = CommandType.BeginDisable;
				renderChainCommand.owner = renderData;
				if (renderData.firstHeadCommand == null)
				{
					FindHeadCommandInsertionPoint(renderData, out var prev, out var next);
					InjectCommandInBetween(renderTreeManager, renderChainCommand, prev, next);
				}
				else
				{
					RenderChainCommand prev2 = renderData.firstHeadCommand.prev;
					RenderChainCommand firstHeadCommand = renderData.firstHeadCommand;
					RenderChainCommand lastHeadCommand = renderData.lastHeadCommand;
					Debug.Assert(lastHeadCommand != null);
					renderData.firstHeadCommand = null;
					InjectCommandInBetween(renderTreeManager, renderChainCommand, prev2, firstHeadCommand);
					renderData.lastHeadCommand = lastHeadCommand;
				}
			}
			if (renderData.lastTailCommand == null || renderData.lastTailCommand.type != CommandType.EndDisable)
			{
				RenderChainCommand renderChainCommand2 = renderTreeManager.AllocCommand();
				renderChainCommand2.type = CommandType.EndDisable;
				renderChainCommand2.isTail = true;
				renderChainCommand2.owner = renderData;
				if (renderData.lastTailCommand == null)
				{
					FindTailCommandInsertionPoint(renderData, out var prev3, out var next2);
					InjectCommandInBetween(renderTreeManager, renderChainCommand2, prev3, next2);
					return;
				}
				RenderChainCommand lastTailCommand = renderData.lastTailCommand;
				RenderChainCommand next3 = renderData.lastTailCommand.next;
				Debug.Assert(renderData.firstTailCommand != null);
				InjectCommandInBetween(renderTreeManager, renderChainCommand2, lastTailCommand, next3);
			}
		}
		else
		{
			if (renderData.firstHeadCommand != null && renderData.firstHeadCommand.type == CommandType.BeginDisable)
			{
				RemoveSingleCommand(renderTreeManager, renderData, renderData.firstHeadCommand);
			}
			if (renderData.lastTailCommand != null && renderData.lastTailCommand.type == CommandType.EndDisable)
			{
				RemoveSingleCommand(renderTreeManager, renderData, renderData.lastTailCommand);
			}
		}
	}

	private static void RemoveSingleCommand(RenderTreeManager renderTreeManager, RenderData renderData, RenderChainCommand cmd)
	{
		Debug.Assert(cmd != null);
		Debug.Assert(cmd.owner == renderData);
		renderData.renderTree.OnRenderCommandsRemoved(cmd, cmd);
		RenderChainCommand prev = cmd.prev;
		RenderChainCommand next = cmd.next;
		if (prev != null)
		{
			prev.next = next;
		}
		if (next != null)
		{
			next.prev = prev;
		}
		if (renderData.firstHeadCommand == cmd)
		{
			if (renderData.firstHeadCommand == renderData.lastHeadCommand)
			{
				Debug.Assert(cmd.prev?.owner != renderData, "When removing the first head command, the command before this one in the queue should belong to an other parent");
				Debug.Assert(cmd.next?.owner != renderData || cmd.next == renderData.firstTailCommand);
				renderData.firstHeadCommand = null;
				renderData.lastHeadCommand = null;
			}
			else
			{
				Debug.Assert(cmd.next.owner == renderData);
				Debug.Assert(renderData.lastHeadCommand != null);
				renderData.firstHeadCommand = cmd.next;
			}
		}
		else if (renderData.lastHeadCommand == cmd)
		{
			Debug.Assert(cmd.prev.owner == renderData);
			Debug.Assert(renderData.firstHeadCommand != null);
			renderData.lastHeadCommand = cmd.prev;
		}
		if (renderData.firstTailCommand == cmd)
		{
			if (renderData.firstTailCommand == renderData.lastTailCommand)
			{
				Debug.Assert(cmd.prev?.owner != renderData || cmd.prev == renderData.lastHeadCommand);
				Debug.Assert(cmd.next?.owner != renderData);
				renderData.firstTailCommand = null;
				renderData.lastTailCommand = null;
			}
			else
			{
				Debug.Assert(cmd.next.owner == renderData);
				Debug.Assert(renderData.lastTailCommand != null);
				renderData.firstTailCommand = cmd.next;
			}
		}
		else if (renderData.lastTailCommand == cmd)
		{
			Debug.Assert(cmd.prev.owner == renderData);
			Debug.Assert(renderData.firstTailCommand != null);
			renderData.lastTailCommand = cmd.prev;
		}
		renderTreeManager.FreeCommand(cmd);
	}
}
