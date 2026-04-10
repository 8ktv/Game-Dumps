using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal class DefaultDragAndDropClient : DragAndDropData, IDragAndDrop
{
	private readonly Hashtable m_GenericData = new Hashtable();

	private Label m_DraggedInfoLabel;

	private DragVisualMode m_VisualMode;

	private IReadOnlyList<EntityId> m_EntityIds;

	public override DragVisualMode visualMode => m_VisualMode;

	public override object source => GetGenericData("__unity-drag-and-drop__source-view");

	[Obsolete("Use entityIDs instead, and call Object.FindObjectFromInstanceID(entityId) if you need to get a Unity object from an EntityId.")]
	public override IEnumerable<Object> unityObjectReferences
	{
		get
		{
			foreach (EntityId entityId in m_EntityIds)
			{
				yield return Object.FindObjectFromInstanceID(entityId);
			}
		}
	}

	public override IReadOnlyList<EntityId> entityIds => m_EntityIds;

	public DragAndDropData data => this;

	public override object GetGenericData(string key)
	{
		return m_GenericData.ContainsKey(key) ? m_GenericData[key] : null;
	}

	public override void SetGenericData(string key, object value)
	{
		m_GenericData[key] = value;
	}

	public void StartDrag(StartDragArgs args, Vector3 pointerPosition)
	{
		if (args.entityIds != null)
		{
			m_EntityIds = args.entityIds;
		}
		paths = args.assetPaths;
		m_VisualMode = args.visualMode;
		foreach (DictionaryEntry genericDatum in args.genericData)
		{
			m_GenericData[(string)genericDatum.Key] = genericDatum.Value;
		}
		if (string.IsNullOrWhiteSpace(args.title))
		{
			return;
		}
		VisualElement visualElement = ((source is VisualElement visualElement2) ? visualElement2.panel.visualTree : null);
		if (visualElement != null)
		{
			if (m_DraggedInfoLabel == null)
			{
				Label label = new Label();
				label.pickingMode = PickingMode.Ignore;
				label.style.position = Position.Absolute;
				m_DraggedInfoLabel = label;
			}
			m_DraggedInfoLabel.text = args.title;
			m_DraggedInfoLabel.style.top = pointerPosition.y;
			m_DraggedInfoLabel.style.left = pointerPosition.x;
			visualElement.Add(m_DraggedInfoLabel);
		}
	}

	public void UpdateDrag(Vector3 pointerPosition)
	{
		if (m_DraggedInfoLabel != null)
		{
			m_DraggedInfoLabel.style.top = pointerPosition.y;
			m_DraggedInfoLabel.style.left = pointerPosition.x;
		}
	}

	public void AcceptDrag()
	{
	}

	public void SetVisualMode(DragVisualMode mode)
	{
		m_VisualMode = mode;
	}

	public void DragCleanup()
	{
		paths = null;
		m_EntityIds = null;
		m_GenericData?.Clear();
		SetVisualMode(DragVisualMode.None);
		m_DraggedInfoLabel?.RemoveFromHierarchy();
	}
}
