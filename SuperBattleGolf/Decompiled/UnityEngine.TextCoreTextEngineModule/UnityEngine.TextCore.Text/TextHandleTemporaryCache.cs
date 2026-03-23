using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class TextHandleTemporaryCache
{
	internal LinkedList<TextInfo> s_TextInfoPool = new LinkedList<TextInfo>();

	internal const int s_MinFramesInCache = 2;

	internal int currentFrame;

	private object syncRoot = new object();

	public void ClearTemporaryCache()
	{
		for (int i = 0; i < s_TextInfoPool.Count; i++)
		{
			s_TextInfoPool.First.Value.RemoveFromCache();
		}
		s_TextInfoPool.Clear();
	}

	public void AddTextInfoToCache(TextHandle textHandle, int hashCode)
	{
		lock (syncRoot)
		{
			if (textHandle.IsCachedPermanent)
			{
				return;
			}
			if (!TextGenerator.IsExecutingJob)
			{
				currentFrame = Time.frameCount;
			}
			if (s_TextInfoPool.Count > 0 && ((double)currentFrame - s_TextInfoPool.Last.Value.lastTimeInCache < 0.0 || (double)currentFrame - s_TextInfoPool.First.Value.lastTimeInCache < 0.0))
			{
				ClearTemporaryCache();
			}
			if (textHandle.IsCachedTemporary)
			{
				RefreshCaching(textHandle);
				return;
			}
			if (s_TextInfoPool.Count > 0 && (double)currentFrame - s_TextInfoPool.Last.Value.lastTimeInCache > 2.0)
			{
				RecycleTextInfoFromCache(textHandle);
			}
			else
			{
				TextInfo textInfo = new TextInfo();
				textHandle.TextInfoNode = new LinkedListNode<TextInfo>(textInfo);
				s_TextInfoPool.AddFirst(textHandle.TextInfoNode);
				textInfo.lastTimeInCache = currentFrame;
				textInfo.removedFromCache = (Action)Delegate.Combine(textInfo.removedFromCache, new Action(textHandle.RemoveTextInfoFromTemporaryCache));
			}
		}
		textHandle.IsCachedTemporary = true;
		textHandle.SetDirty();
		textHandle.UpdateWithHash(hashCode);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	public virtual void RemoveTextInfoFromCache(TextHandle textHandle)
	{
		lock (syncRoot)
		{
			if (textHandle.IsCachedTemporary)
			{
				textHandle.IsCachedTemporary = false;
				textHandle.TextInfoNode.Value.lastTimeInCache = 0.0;
				textHandle.TextInfoNode.Value.removedFromCache = null;
				if (textHandle.TextInfoNode != null)
				{
					s_TextInfoPool.Remove(textHandle.TextInfoNode);
					s_TextInfoPool.AddLast(textHandle.TextInfoNode);
				}
				textHandle.TextInfoNode = null;
			}
		}
	}

	private void RefreshCaching(TextHandle textHandle)
	{
		if (!TextGenerator.IsExecutingJob)
		{
			currentFrame = Time.frameCount;
		}
		textHandle.TextInfoNode.Value.lastTimeInCache = currentFrame;
		s_TextInfoPool.Remove(textHandle.TextInfoNode);
		s_TextInfoPool.AddFirst(textHandle.TextInfoNode);
	}

	private void RecycleTextInfoFromCache(TextHandle textHandle)
	{
		if (!TextGenerator.IsExecutingJob)
		{
			currentFrame = Time.frameCount;
		}
		textHandle.TextInfoNode = s_TextInfoPool.Last;
		textHandle.TextInfoNode.Value.RemoveFromCache();
		s_TextInfoPool.RemoveLast();
		s_TextInfoPool.AddFirst(textHandle.TextInfoNode);
		textHandle.IsCachedTemporary = true;
		TextInfo value = textHandle.TextInfoNode.Value;
		value.removedFromCache = (Action)Delegate.Combine(value.removedFromCache, new Action(textHandle.RemoveTextInfoFromTemporaryCache));
		textHandle.TextInfoNode.Value.lastTimeInCache = currentFrame;
	}

	public void UpdateCurrentFrame()
	{
		currentFrame = Time.frameCount;
	}
}
