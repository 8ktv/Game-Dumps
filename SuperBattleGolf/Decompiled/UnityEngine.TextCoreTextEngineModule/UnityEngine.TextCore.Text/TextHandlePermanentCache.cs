using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class TextHandlePermanentCache
{
	internal LinkedList<TextInfo> s_TextInfoPool = new LinkedList<TextInfo>();

	private object syncRoot = new object();

	public virtual void AddTextInfoToCache(TextHandle textHandle)
	{
		lock (syncRoot)
		{
			if (textHandle.IsCachedPermanent)
			{
				return;
			}
			if (textHandle.IsCachedTemporary)
			{
				textHandle.RemoveTextInfoFromTemporaryCache();
			}
			if (s_TextInfoPool.Count > 0)
			{
				textHandle.TextInfoNode = s_TextInfoPool.Last;
				s_TextInfoPool.RemoveLast();
			}
			else
			{
				TextInfo value = new TextInfo();
				textHandle.TextInfoNode = new LinkedListNode<TextInfo>(value);
			}
		}
		textHandle.IsCachedPermanent = true;
		textHandle.SetDirty();
		textHandle.Update();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	public void RemoveTextInfoFromCache(TextHandle textHandle)
	{
		lock (syncRoot)
		{
			if (textHandle.IsCachedPermanent)
			{
				s_TextInfoPool.AddFirst(textHandle.TextInfoNode);
				textHandle.TextInfoNode = null;
				textHandle.IsCachedPermanent = false;
			}
		}
	}
}
