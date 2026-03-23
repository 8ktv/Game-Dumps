using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityEngine.UIElements;

internal class PropagationPaths : IDisposable
{
	private static readonly ObjectPool<PropagationPaths> s_Pool = new ObjectPool<PropagationPaths>(() => new PropagationPaths());

	public readonly List<VisualElement> trickleDownPath;

	public readonly List<VisualElement> bubbleUpPath;

	private const int k_DefaultPropagationDepth = 8;

	public PropagationPaths()
	{
		trickleDownPath = new List<VisualElement>(8);
		bubbleUpPath = new List<VisualElement>(8);
	}

	public PropagationPaths(PropagationPaths paths)
		: this()
	{
		if (paths != null)
		{
			trickleDownPath.AddRange(paths.trickleDownPath);
			bubbleUpPath.AddRange(paths.bubbleUpPath);
		}
	}

	[NotNull]
	public static PropagationPaths Build(VisualElement elem, EventBase evt, int eventCategories)
	{
		PropagationPaths propagationPaths = s_Pool.Get();
		if (elem.HasTrickleDownEventInterests(eventCategories))
		{
			propagationPaths.trickleDownPath.Add(elem);
		}
		if (elem.HasBubbleUpEventInterests(eventCategories))
		{
			propagationPaths.bubbleUpPath.Add(elem);
		}
		VisualElement nextParentWithEventInterests = elem.nextParentWithEventInterests;
		while (nextParentWithEventInterests != null && nextParentWithEventInterests.HasParentEventInterests(eventCategories))
		{
			if (evt.tricklesDown && nextParentWithEventInterests.HasTrickleDownEventInterests(eventCategories))
			{
				propagationPaths.trickleDownPath.Add(nextParentWithEventInterests);
			}
			if (evt.bubbles && nextParentWithEventInterests.HasBubbleUpEventInterests(eventCategories))
			{
				propagationPaths.bubbleUpPath.Add(nextParentWithEventInterests);
			}
			nextParentWithEventInterests = nextParentWithEventInterests.nextParentWithEventInterests;
		}
		return propagationPaths;
	}

	public void Dispose()
	{
		bubbleUpPath.Clear();
		trickleDownPath.Clear();
		s_Pool.Release(this);
	}
}
