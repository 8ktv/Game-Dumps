using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
internal class AuthoringChanges
{
	public HashSet<VisualElement> addedOrMovedElements { get; } = new HashSet<VisualElement>();

	public HashSet<VisualElement> removedFromPanel { get; } = new HashSet<VisualElement>();

	public HashSet<VisualElement> styleChanged { get; } = new HashSet<VisualElement>();

	public HashSet<VisualElement> stylingContextChanged { get; } = new HashSet<VisualElement>();

	public HashSet<VisualElement> bindingContextChanged { get; } = new HashSet<VisualElement>();

	public bool ContainsChanges()
	{
		return addedOrMovedElements.Count > 0 || removedFromPanel.Count > 0 || styleChanged.Count > 0 || stylingContextChanged.Count > 0 || bindingContextChanged.Count > 0;
	}

	public void Clear()
	{
		addedOrMovedElements.Clear();
		removedFromPanel.Clear();
		styleChanged.Clear();
		stylingContextChanged.Clear();
		bindingContextChanged.Clear();
	}
}
