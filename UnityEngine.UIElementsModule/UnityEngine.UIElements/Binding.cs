using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[UxmlObject]
public abstract class Binding
{
	[Serializable]
	[ExcludeFromDocs]
	public abstract class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
	{
		[HideInInspector]
		[SerializeField]
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal string property;

		[SerializeField]
		[HideInInspector]
		[Tooltip("This informs the binding system of whether the binding object should be updated on every frame, when a change occurs in the source or on every frame if change detection is impossible, and when explicitly marked as dirty.")]
		private BindingUpdateTrigger updateTrigger;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags property_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags updateTrigger_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[2]
			{
				new UxmlAttributeNames("property", "property", null),
				new UxmlAttributeNames("updateTrigger", "update-trigger", null)
			});
		}

		public override void Deserialize(object obj)
		{
			Binding binding = (Binding)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(property_UxmlAttributeFlags))
			{
				binding.property = property;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(updateTrigger_UxmlAttributeFlags))
			{
				binding.updateTrigger = updateTrigger;
			}
		}
	}

	private bool m_Dirty;

	private BindingUpdateTrigger m_UpdateTrigger;

	internal const string k_UpdateTriggerTooltip = "This informs the binding system of whether the binding object should be updated on every frame, when a change occurs in the source or on every frame if change detection is impossible, and when explicitly marked as dirty.";

	internal string property
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get;
		set; }

	public bool isDirty => m_Dirty;

	[CreateProperty]
	public BindingUpdateTrigger updateTrigger
	{
		get
		{
			return m_UpdateTrigger;
		}
		set
		{
			m_UpdateTrigger = value;
		}
	}

	public static void SetGlobalLogLevel(BindingLogLevel logLevel)
	{
		DataBindingManager.globalLogLevel = logLevel;
	}

	public static BindingLogLevel GetGlobalLogLevel()
	{
		return DataBindingManager.globalLogLevel;
	}

	public static void SetPanelLogLevel(IPanel panel, BindingLogLevel logLevel)
	{
		if (panel is BaseVisualElementPanel baseVisualElementPanel)
		{
			baseVisualElementPanel.dataBindingManager.logLevel = logLevel;
		}
	}

	public static BindingLogLevel GetPanelLogLevel(IPanel panel)
	{
		if (panel is BaseVisualElementPanel baseVisualElementPanel)
		{
			return baseVisualElementPanel.dataBindingManager.logLevel;
		}
		return BindingLogLevel.None;
	}

	public static void ResetPanelLogLevel(IPanel panel)
	{
		if (panel is BaseVisualElementPanel baseVisualElementPanel)
		{
			baseVisualElementPanel.dataBindingManager.ResetLogLevel();
		}
	}

	internal Binding()
	{
		m_Dirty = true;
	}

	public void MarkDirty()
	{
		m_Dirty = true;
	}

	internal void ClearDirty()
	{
		m_Dirty = false;
	}

	protected internal virtual void OnActivated(in BindingActivationContext context)
	{
	}

	protected internal virtual void OnDeactivated(in BindingActivationContext context)
	{
	}

	protected internal virtual void OnDataSourceChanged(in DataSourceContextChanged context)
	{
	}
}
