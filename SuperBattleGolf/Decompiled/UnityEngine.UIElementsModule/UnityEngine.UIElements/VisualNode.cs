using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements;

internal readonly struct VisualNode : IEnumerable<VisualNode>, IEnumerable, IEquatable<VisualNode>, IEquatable<VisualNodeHandle>
{
	public struct Enumerator : IEnumerator<VisualNode>, IEnumerator, IDisposable
	{
		private readonly VisualNode m_Node;

		private int m_Position;

		public VisualNode Current => m_Node[m_Position];

		object IEnumerator.Current => Current;

		public Enumerator(in VisualNode node)
		{
			m_Node = node;
			m_Position = -1;
		}

		public bool MoveNext()
		{
			return ++m_Position < m_Node.ChildCount;
		}

		public void Reset()
		{
			m_Position = -1;
		}

		public void Dispose()
		{
		}
	}

	private readonly VisualManager m_Manager;

	private readonly VisualNodeHandle m_Handle;

	public static VisualNode Null => new VisualNode(null, VisualNodeHandle.Null);

	public bool IsCreated => !m_Handle.Equals(VisualNodeHandle.Null) && m_Manager.ContainsNode(in m_Handle);

	public VisualNodeHandle Handle => m_Handle;

	public bool IsRoot => m_Handle.Id == 1;

	public int Id => m_Handle.Id;

	public uint ControlId => m_Manager.GetProperty<VisualNodeData>(m_Handle).ControlId;

	public VisualNode Parent => new VisualNode(m_Manager, m_Manager.GetParent(in m_Handle));

	public int ChildCount => m_Manager.GetChildrenCount(in m_Handle);

	public VisualNode this[int index] => GetChildren()[index];

	public ref bool Enabled => ref m_Manager.GetProperty<VisualNodeData>(m_Handle).Enabled;

	public ref VisualElementFlags Flags => ref m_Manager.GetProperty<VisualNodeData>(m_Handle).Flags;

	public PseudoStates PseudoStates
	{
		get
		{
			return m_Manager.GetPseudoStates(in m_Handle);
		}
		set
		{
			m_Manager.SetPseudoStates(in m_Handle, value);
		}
	}

	public bool EnabledInHierarchy => (PseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled;

	public RenderHints RenderHints
	{
		get
		{
			return m_Manager.GetRenderHints(in m_Handle);
		}
		set
		{
			m_Manager.SetRenderHints(in m_Handle, value);
		}
	}

	public LanguageDirection LanguageDirection
	{
		get
		{
			return m_Manager.GetLanguageDirection(in m_Handle);
		}
		set
		{
			m_Manager.SetLanguageDirection(in m_Handle, value);
		}
	}

	public LanguageDirection LocalLanguageDirection
	{
		get
		{
			return m_Manager.GetLocalLanguageDirection(in m_Handle);
		}
		set
		{
			m_Manager.SetLocalLanguageDirection(in m_Handle, value);
		}
	}

	internal bool areAncestorsAndSelfDisplayed => (Flags & VisualElementFlags.HierarchyDisplayed) == VisualElementFlags.HierarchyDisplayed;

	public ref VisualNodeCallbackInterest CallbackInterest => ref m_Manager.GetProperty<VisualNodeData>(m_Handle).CallbackInterest;

	internal VisualNode(VisualManager manager, VisualNodeHandle handle)
	{
		m_Manager = manager;
		m_Handle = handle;
	}

	internal void Destroy()
	{
		m_Manager.RemoveNode(in m_Handle);
	}

	public VisualPanel GetPanel()
	{
		return new VisualPanel(m_Manager, m_Manager.GetProperty<VisualNodeData>(m_Handle).Panel);
	}

	public void SetPanel(VisualPanel panel)
	{
		m_Manager.GetProperty<VisualNodeData>(m_Handle).Panel = panel.Handle;
	}

	public VisualElement GetOwner()
	{
		return m_Manager.GetOwner(in m_Handle);
	}

	public void SetOwner(VisualElement owner)
	{
		m_Manager.SetOwner(in m_Handle, owner);
	}

	public LayoutNode GetLayout()
	{
		return m_Manager.GetProperty<VisualNodeData>(m_Handle).LayoutNode;
	}

	public void SetLayout(LayoutNode value)
	{
		m_Manager.GetProperty<VisualNodeData>(m_Handle).LayoutNode = value;
	}

	public VisualNodeChildren GetChildren()
	{
		return new VisualNodeChildren(m_Manager, m_Handle);
	}

	public void InsertChildAtIndex(int index, in VisualNode child)
	{
		m_Manager.InsertChildAtIndex(in m_Handle, index, in child.m_Handle);
	}

	public void AddChild(in VisualNode child)
	{
		m_Manager.AddChild(in m_Handle, in child.m_Handle);
	}

	public void RemoveChild(in VisualNode child)
	{
		m_Manager.RemoveChild(in m_Handle, in child.m_Handle);
	}

	public int IndexOfChild(in VisualNode child)
	{
		return m_Manager.IndexOfChild(in m_Handle, in child.m_Handle);
	}

	public void RemoveChildAtIndex(int index)
	{
		m_Manager.RemoveChildAtIndex(in m_Handle, index);
	}

	public void ClearChildren()
	{
		m_Manager.ClearChildren(in m_Handle);
	}

	public void RemoveFromParent()
	{
		m_Manager.RemoveFromParent(in m_Handle);
	}

	public VisualNodeClassList GetClassList()
	{
		return new VisualNodeClassList(m_Manager, m_Handle);
	}

	public void AddToClassList(string className)
	{
		if (!string.IsNullOrEmpty(className))
		{
			m_Manager.AddToClassList(in m_Handle, className);
		}
	}

	public bool RemoveFromClassList(string className)
	{
		if (string.IsNullOrEmpty(className))
		{
			return false;
		}
		return m_Manager.RemoveFromClassList(in m_Handle, className);
	}

	public bool ClassListContains(string className)
	{
		if (string.IsNullOrEmpty(className))
		{
			return false;
		}
		return m_Manager.ClassListContains(in m_Handle, className);
	}

	public bool ClearClassList()
	{
		return m_Manager.ClearClassList(in m_Handle);
	}

	public void SetEnabled(bool value)
	{
		m_Manager.SetEnabled(in m_Handle, value);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(in this);
	}

	IEnumerator<VisualNode> IEnumerable<VisualNode>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Equals(VisualNode other)
	{
		return m_Handle.Equals(other.m_Handle);
	}

	public bool Equals(VisualNodeHandle other)
	{
		return m_Handle.Equals(other);
	}

	public override bool Equals(object obj)
	{
		return (obj is VisualNode other) ? Equals(other) : (obj is VisualNodeHandle other2 && Equals(other2));
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(m_Manager, m_Handle);
	}
}
