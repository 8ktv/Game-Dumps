using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public abstract class Focusable : CallbackEventHandler
{
	internal static readonly BindingId focusableProperty = "focusable";

	internal static readonly BindingId tabIndexProperty = "tabIndex";

	internal static readonly BindingId delegatesFocusProperty = "delegatesFocus";

	internal static readonly BindingId canGrabFocusProperty = "canGrabFocus";

	private bool m_Focusable;

	private int m_TabIndex;

	private bool m_DelegatesFocus;

	private bool m_ExcludeFromFocusRing;

	public abstract FocusController focusController { get; }

	[CreateProperty]
	public virtual bool focusable
	{
		get
		{
			return m_Focusable;
		}
		set
		{
			if (m_Focusable != value)
			{
				m_Focusable = value;
				NotifyPropertyChanged(in focusableProperty);
			}
		}
	}

	[CreateProperty]
	public int tabIndex
	{
		get
		{
			return m_TabIndex;
		}
		set
		{
			if (m_TabIndex != value)
			{
				m_TabIndex = value;
				NotifyPropertyChanged(in tabIndexProperty);
			}
		}
	}

	[CreateProperty]
	public bool delegatesFocus
	{
		get
		{
			return m_DelegatesFocus;
		}
		set
		{
			if (m_DelegatesFocus != value)
			{
				m_DelegatesFocus = value;
				NotifyPropertyChanged(in delegatesFocusProperty);
			}
		}
	}

	internal bool excludeFromFocusRing
	{
		get
		{
			return m_ExcludeFromFocusRing;
		}
		set
		{
			if (!((VisualElement)this).isCompositeRoot)
			{
				throw new InvalidOperationException("excludeFromFocusRing should only be set on composite roots.");
			}
			m_ExcludeFromFocusRing = value;
		}
	}

	internal bool isEligibleToReceiveFocusFromDisabledChild { get; set; } = true;

	[CreateProperty(ReadOnly = true)]
	public virtual bool canGrabFocus => focusable;

	protected Focusable()
	{
		UIElementsRuntimeUtilityNative.VisualElementCreation();
		focusable = true;
		tabIndex = 0;
	}

	public virtual void Focus()
	{
		if (focusController != null)
		{
			if (canGrabFocus)
			{
				Focusable focusDelegate = GetFocusDelegate();
				focusController.SwitchFocus(focusDelegate, this != focusDelegate);
			}
			else
			{
				focusController.SwitchFocus(null);
			}
		}
	}

	public virtual void Blur()
	{
		focusController?.Blur(this);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void BlurImmediately()
	{
		focusController?.Blur(this, bIsFocusDelegated: false, DispatchMode.Immediate);
	}

	internal Focusable GetFocusDelegate()
	{
		Focusable focusable = this;
		while (focusable != null && focusable.delegatesFocus)
		{
			focusable = GetFirstFocusableChild(focusable as VisualElement);
		}
		return focusable;
	}

	private static Focusable GetFirstFocusableChild(VisualElement ve)
	{
		int childCount = ve.hierarchy.childCount;
		for (int i = 0; i < childCount; i++)
		{
			VisualElement visualElement = ve.hierarchy[i];
			if (visualElement.canGrabFocus && visualElement.tabIndex >= 0)
			{
				return visualElement;
			}
			bool flag = visualElement.hierarchy.parent != null && visualElement == visualElement.hierarchy.parent.contentContainer;
			if (!visualElement.isCompositeRoot && !flag)
			{
				Focusable firstFocusableChild = GetFirstFocusableChild(visualElement);
				if (firstFocusableChild != null)
				{
					return firstFocusableChild;
				}
			}
		}
		return null;
	}
}
