using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class TextHandleTemporaryCache
{
	internal LinkedList<TextCacheEntry> s_Cache = new LinkedList<TextCacheEntry>();

	internal const int s_MinFramesInCache = 2;

	internal int currentFrame;

	private object syncRoot = new object();

	public void ClearTemporaryCache()
	{
		foreach (TextCacheEntry item in s_Cache)
		{
			ResetEntryState(item.textHandle);
		}
		s_Cache.Clear();
	}

	public void AddTextInfoToCache(TextHandle textHandle, int hashCode)
	{
		lock (syncRoot)
		{
			if (textHandle.IsCachedPermanentTextCore)
			{
				return;
			}
			if (!TextGenerator.IsExecutingJob)
			{
				currentFrame = Time.frameCount;
			}
			if (s_Cache.Count > 0 && ((float)currentFrame - s_Cache.Last.Value.lastTimeInCache < 0f || (float)currentFrame - s_Cache.First.Value.lastTimeInCache < 0f))
			{
				ClearTemporaryCache();
			}
			if (textHandle.IsCachedTemporary)
			{
				RefreshCaching(textHandle);
				return;
			}
			if (s_Cache.Count > 0 && (float)currentFrame - s_Cache.Last.Value.lastTimeInCache > 2f)
			{
				RecycleTextInfoFromCache(textHandle);
			}
			else
			{
				TextInfo info = new TextInfo();
				textHandle.TextInfoNode = new LinkedListNode<TextCacheEntry>(new TextCacheEntry(textHandle, info, currentFrame));
				s_Cache.AddFirst(textHandle.TextInfoNode);
			}
		}
		textHandle.IsCachedTemporary = true;
		textHandle.SetDirty();
		textHandle.UpdateWithHash(hashCode);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal void RemoveFromCache(TextHandle handle)
	{
		lock (syncRoot)
		{
			if (handle.IsCachedTemporary)
			{
				if (handle.TextInfoNode != null)
				{
					s_Cache.Remove(handle.TextInfoNode);
					s_Cache.AddLast(handle.TextInfoNode);
				}
				ResetEntryState(handle);
			}
		}
	}

	internal void ResetEntryState(TextHandle handle)
	{
		if (handle != null && handle.IsCachedTemporary)
		{
			handle.IsCachedTemporary = false;
			handle.TextInfoNode.SetTime(0f);
			handle.TextInfoNode.SetTextHandle(null);
			handle.TextInfoNode = null;
		}
	}

	private void RefreshCaching(TextHandle textHandle)
	{
		if (!TextGenerator.IsExecutingJob)
		{
			currentFrame = Time.frameCount;
		}
		textHandle.TextInfoNode.SetTime(currentFrame);
		s_Cache.Remove(textHandle.TextInfoNode);
		s_Cache.AddFirst(textHandle.TextInfoNode);
	}

	private void RecycleTextInfoFromCache(TextHandle textHandle)
	{
		if (!TextGenerator.IsExecutingJob)
		{
			currentFrame = Time.frameCount;
		}
		textHandle.RemoveFromTemporaryCache();
		if (s_Cache.Last.Value.textHandle != null)
		{
			s_Cache.Last.Value.textHandle.RemoveFromTemporaryCache();
		}
		textHandle.TextInfoNode = s_Cache.Last;
		textHandle.TextInfoNode.SetTextHandle(textHandle);
		textHandle.TextInfoNode.SetTime(currentFrame);
		textHandle.IsCachedTemporary = true;
		s_Cache.RemoveLast();
		s_Cache.AddFirst(textHandle.TextInfoNode);
	}

	public void UpdateCurrentFrame()
	{
		currentFrame = Time.frameCount;
	}
}
