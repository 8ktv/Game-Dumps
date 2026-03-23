using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[UxmlObject]
public class DataBinding : Binding, IDataSourceProvider
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : Binding.UxmlSerializedData
	{
		[Tooltip("The path to the value in the data source used by this binding. To see resolved bindings in the UI Builder, define a path that is compatible with the target source property.")]
		[UxmlAttribute("data-source-path")]
		[HideInInspector]
		[SerializeField]
		private string dataSourcePathString;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags dataSourcePathString_UxmlAttributeFlags;

		[SerializeField]
		[Tooltip("A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.")]
		[HideInInspector]
		[DataSourceDrawer]
		private Object dataSource;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags dataSource_UxmlAttributeFlags;

		[UxmlAttribute("data-source-type")]
		[SerializeField]
		[HideInInspector]
		[UxmlTypeReference(typeof(object))]
		[Tooltip("A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.")]
		private string dataSourceTypeString;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags dataSourceTypeString_UxmlAttributeFlags;

		[Tooltip("Controls how a binding is updated, which can include the direction in which data is written.")]
		[BindingModeDrawer]
		[HideInInspector]
		[SerializeField]
		private BindingMode bindingMode;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags bindingMode_UxmlAttributeFlags;

		[ConverterDrawer(isConverterToSource = false)]
		[SerializeField]
		[UxmlAttribute("source-to-ui-converters")]
		[UxmlAttributeBindingPath("uiToSourceConverters")]
		[HideInInspector]
		[Tooltip("Define one or more converter groups for this binding that will be used between the data source to the target UI.")]
		private string sourceToUiConvertersString;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags sourceToUiConvertersString_UxmlAttributeFlags;

		[Tooltip("Define one or more converter groups for this binding that will be used between the target UI to the data source.")]
		[UxmlAttributeBindingPath("sourceToUiConverters")]
		[ConverterDrawer(isConverterToSource = true)]
		[SerializeField]
		[UxmlAttribute("ui-to-source-converters")]
		[HideInInspector]
		private string uiToSourceConvertersString;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags uiToSourceConvertersString_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[6]
			{
				new UxmlAttributeNames("dataSourcePathString", "data-source-path", null),
				new UxmlAttributeNames("dataSource", "data-source", null),
				new UxmlAttributeNames("dataSourceTypeString", "data-source-type", typeof(object)),
				new UxmlAttributeNames("bindingMode", "binding-mode", null),
				new UxmlAttributeNames("sourceToUiConvertersString", "source-to-ui-converters", null),
				new UxmlAttributeNames("uiToSourceConvertersString", "ui-to-source-converters", null)
			});
		}

		public override object CreateInstance()
		{
			return new DataBinding();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			DataBinding dataBinding = (DataBinding)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(dataSourcePathString_UxmlAttributeFlags))
			{
				dataBinding.dataSourcePathString = dataSourcePathString;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(dataSource_UxmlAttributeFlags))
			{
				dataBinding.dataSource = (dataSource ? dataSource : null);
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(dataSourceTypeString_UxmlAttributeFlags))
			{
				dataBinding.dataSourceTypeString = dataSourceTypeString;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(bindingMode_UxmlAttributeFlags))
			{
				dataBinding.bindingMode = bindingMode;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(uiToSourceConvertersString_UxmlAttributeFlags))
			{
				dataBinding.uiToSourceConvertersString = uiToSourceConvertersString;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(sourceToUiConvertersString_UxmlAttributeFlags))
			{
				dataBinding.sourceToUiConvertersString = sourceToUiConvertersString;
			}
		}
	}

	private static MethodInfo s_UpdateUIMethodInfo;

	private BindingMode m_BindingMode;

	private ConverterGroup m_SourceToUiConverters;

	private ConverterGroup m_UiToSourceConverters;

	private List<string> m_SourceToUIConvertersString;

	private List<string> m_UiToSourceConvertersString;

	internal const string k_DataSourceTooltip = "A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.";

	internal const string k_DataSourcePathTooltip = "The path to the value in the data source used by this binding. To see resolved bindings in the UI Builder, define a path that is compatible with the target source property.";

	internal const string k_BindingModeTooltip = "Controls how a binding is updated, which can include the direction in which data is written.";

	internal const string k_SourceToUiConvertersTooltip = "Define one or more converter groups for this binding that will be used between the data source to the target UI.";

	internal const string k_UiToSourceConvertersTooltip = "Define one or more converter groups for this binding that will be used between the target UI to the data source.";

	internal static MethodInfo updateUIMethod => s_UpdateUIMethodInfo ?? (s_UpdateUIMethodInfo = CacheReflectionInfo());

	[CreateProperty]
	public object dataSource { get; set; }

	public Type dataSourceType { get; set; }

	internal string dataSourceTypeString
	{
		get
		{
			return UxmlUtility.TypeToString(dataSourceType);
		}
		set
		{
			dataSourceType = UxmlUtility.ParseType(value);
		}
	}

	[CreateProperty]
	public PropertyPath dataSourcePath { get; set; }

	internal string dataSourcePathString
	{
		get
		{
			return dataSourcePath.ToString();
		}
		set
		{
			dataSourcePath = new PropertyPath(value);
		}
	}

	[CreateProperty]
	public BindingMode bindingMode
	{
		get
		{
			return m_BindingMode;
		}
		set
		{
			if (m_BindingMode != value)
			{
				m_BindingMode = value;
				MarkDirty();
			}
		}
	}

	[CreateProperty(ReadOnly = true)]
	public ConverterGroup sourceToUiConverters => m_SourceToUiConverters ?? (m_SourceToUiConverters = new ConverterGroup(string.Empty));

	[CreateProperty(ReadOnly = true)]
	public ConverterGroup uiToSourceConverters => m_UiToSourceConverters ?? (m_UiToSourceConverters = new ConverterGroup(string.Empty));

	internal string sourceToUiConvertersString
	{
		get
		{
			return (m_SourceToUIConvertersString != null) ? string.Join(", ", m_SourceToUIConvertersString) : null;
		}
		set
		{
			m_SourceToUIConvertersString = UxmlUtility.ParseStringListAttribute(value);
			if (m_SourceToUIConvertersString == null)
			{
				return;
			}
			foreach (string item in m_SourceToUIConvertersString)
			{
				if (ConverterGroups.TryGetConverterGroup(item, out var converterGroup))
				{
					ApplyConverterGroupToUI(converterGroup);
				}
			}
		}
	}

	internal string uiToSourceConvertersString
	{
		get
		{
			return (m_UiToSourceConvertersString != null) ? string.Join(", ", m_UiToSourceConvertersString) : null;
		}
		set
		{
			m_UiToSourceConvertersString = UxmlUtility.ParseStringListAttribute(value);
			if (m_UiToSourceConvertersString == null)
			{
				return;
			}
			foreach (string item in m_UiToSourceConvertersString)
			{
				if (ConverterGroups.TryGetConverterGroup(item, out var converterGroup))
				{
					ApplyConverterGroupToSource(converterGroup);
				}
			}
		}
	}

	private static MethodInfo CacheReflectionInfo()
	{
		MethodInfo[] methods = typeof(DataBinding).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
		foreach (MethodInfo methodInfo in methods)
		{
			if (!(methodInfo.Name != "UpdateUI") && methodInfo.GetParameters().Length == 2)
			{
				return s_UpdateUIMethodInfo = methodInfo;
			}
		}
		throw new InvalidOperationException("Could not find method UpdateUI by reflection. This is an internal bug. Please report using `Help > Report a Bug...` ");
	}

	public DataBinding()
	{
		base.updateTrigger = BindingUpdateTrigger.OnSourceChanged;
	}

	public void ApplyConverterGroupToSource(ConverterGroup group)
	{
		ConverterGroup converterGroup = uiToSourceConverters;
		converterGroup.registry.Apply(group.registry);
	}

	public void ApplyConverterGroupToUI(ConverterGroup group)
	{
		ConverterGroup converterGroup = sourceToUiConverters;
		converterGroup.registry.Apply(group.registry);
	}

	protected internal virtual BindingResult UpdateUI<TValue>(in BindingContext context, ref TValue value)
	{
		VisualElement container = context.targetElement;
		FocusController focusController = container.focusController;
		if (focusController != null && focusController.IsFocused(container))
		{
			Focusable leafFocusedElement = focusController.GetLeafFocusedElement();
			if (leafFocusedElement is TextElement textElement && textElement.ClassListContains("unity-text-element--inner-input-field-component") && (container is IDelayedField { isDelayed: not false } || textElement.edition.touchScreenKeyboard != null))
			{
				return new BindingResult(BindingStatus.Pending);
			}
		}
		if (sourceToUiConverters.TrySetValue(ref container, (PropertyPath)context.bindingId, value, out var returnCode))
		{
			return default(BindingResult);
		}
		string setValueErrorString = GetSetValueErrorString(returnCode, context.dataSource, context.dataSourcePath, container, context.bindingId, value);
		return new BindingResult(BindingStatus.Failure, setValueErrorString);
	}

	protected internal virtual BindingResult UpdateSource<TValue>(in BindingContext context, ref TValue value)
	{
		object container = context.dataSource;
		if (uiToSourceConverters.TrySetValue(ref container, context.dataSourcePath, value, out var returnCode))
		{
			return default(BindingResult);
		}
		string setValueErrorString = GetSetValueErrorString(returnCode, context.targetElement, (PropertyPath)context.bindingId, context.dataSource, (BindingId)context.dataSourcePath, value);
		return new BindingResult(BindingStatus.Failure, setValueErrorString);
	}

	internal static string GetSetValueErrorString<TValue>(VisitReturnCode returnCode, object source, in PropertyPath sourcePath, object target, in BindingId targetPath, TValue extractedValueFromSource)
	{
		string text = $"[UI Toolkit] Could not set value for target of type '<b>{target.GetType().Name}</b>' at path '<b>{targetPath}</b>':";
		switch (returnCode)
		{
		case VisitReturnCode.MissingPropertyBag:
			return text + " the type '" + target.GetType().Name + "' is missing a property bag.";
		case VisitReturnCode.InvalidPath:
			return text + " the path is either invalid or contains a null value.";
		case VisitReturnCode.InvalidCast:
		{
			if (sourcePath.IsEmpty && PropertyContainer.TryGetValue(ref target, (string)targetPath, out object value) && value != null)
			{
				return (extractedValueFromSource == null) ? (text + " could not convert from '<b>null</b>' to '<b>" + value.GetType().Name + "</b>'.") : (text + " could not convert from type '<b>" + extractedValueFromSource.GetType().Name + "</b>' to type '<b>" + value.GetType().Name + "</b>'.");
			}
			if (PropertyContainer.TryGetProperty(ref source, in sourcePath, out var property) && PropertyContainer.TryGetValue(ref target, (string)targetPath, out object value2) && value2 != null)
			{
				return (extractedValueFromSource == null) ? (text + " could not convert from '<b>null (" + property.DeclaredValueType().Name + ")</b>' to '<b>" + value2.GetType().Name + "</b>'.") : (text + " could not convert from type '<b>" + extractedValueFromSource.GetType().Name + "</b>' to type '<b>" + value2.GetType().Name + "</b>'.");
			}
			return text + " conversion failed.";
		}
		case VisitReturnCode.AccessViolation:
			return text + " the path is read-only.";
		case VisitReturnCode.Ok:
		case VisitReturnCode.NullContainer:
		case VisitReturnCode.InvalidContainerType:
			throw new InvalidOperationException(text + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
