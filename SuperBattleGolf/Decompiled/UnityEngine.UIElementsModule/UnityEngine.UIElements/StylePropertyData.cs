using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal struct StylePropertyData<TInline, TComputedValue> : IEquatable<StylePropertyData<TInline, TComputedValue>>, IDisposable
{
	public VisualElement target { get; internal set; }

	[CreateProperty(ReadOnly = true)]
	public TInline inlineValue { get; internal set; }

	[CreateProperty(ReadOnly = true)]
	public UxmlStyleProperty uxmlValue { get; internal set; }

	[CreateProperty(ReadOnly = true)]
	public TComputedValue computedValue { get; internal set; }

	[CreateProperty(ReadOnly = true)]
	public Binding binding { get; internal set; }

	[CreateProperty(ReadOnly = true)]
	public SelectorMatchRecord selector { get; internal set; }

	[CreateProperty(ReadOnly = true)]
	public bool isUxmlOverridden => uxmlValue.isInlined || binding != null;

	public bool Equals(StylePropertyData<TInline, TComputedValue> other)
	{
		return EqualityComparer<TInline>.Default.Equals(inlineValue, other.inlineValue) && EqualityComparer<UxmlStyleProperty>.Default.Equals(uxmlValue, other.uxmlValue) && EqualityComparer<TComputedValue>.Default.Equals(computedValue, other.computedValue) && binding == other.binding && EqualityComparer<SelectorMatchRecord>.Default.Equals(selector, other.selector);
	}

	public override bool Equals(object obj)
	{
		return obj is StylePropertyData<StyleLength, Length> stylePropertyData && Equals(stylePropertyData);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(inlineValue, uxmlValue, computedValue, binding, selector);
	}

	public static bool operator ==(StylePropertyData<TInline, TComputedValue> lhs, StylePropertyData<TInline, TComputedValue> rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(StylePropertyData<TInline, TComputedValue> lhs, StylePropertyData<TInline, TComputedValue> rhs)
	{
		return !(lhs == rhs);
	}

	public void Dispose()
	{
		uxmlValue.Dispose();
	}
}
