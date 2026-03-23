#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Profiling;
using Unity.Properties;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Pool;
using UnityEngine.UIElements.Experimental;
using UnityEngine.UIElements.Layout;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

public class VisualElement : Focusable, IResolvedStyle, IStylePropertyAnimations, ITransform, ITransitionAnimations, IExperimentalFeatures, IVisualElementScheduler
{
	internal static class ResolvedStyleProperties
	{
		internal static readonly BindingId alignContentProperty = "resolvedStyle.alignContent";

		internal static readonly BindingId alignItemsProperty = "resolvedStyle.alignItems";

		internal static readonly BindingId alignSelfProperty = "resolvedStyle.alignSelf";

		internal static readonly BindingId backgroundColorProperty = "resolvedStyle.backgroundColor";

		internal static readonly BindingId backgroundImageProperty = "resolvedStyle.backgroundImage";

		internal static readonly BindingId backgroundPositionXProperty = "resolvedStyle.backgroundPositionX";

		internal static readonly BindingId backgroundPositionYProperty = "resolvedStyle.backgroundPositionY";

		internal static readonly BindingId backgroundRepeatProperty = "resolvedStyle.backgroundRepeat";

		internal static readonly BindingId backgroundSizeProperty = "resolvedStyle.backgroundSize";

		internal static readonly BindingId borderBottomColorProperty = "resolvedStyle.borderBottomColor";

		internal static readonly BindingId borderBottomLeftRadiusProperty = "resolvedStyle.borderBottomLeftRadius";

		internal static readonly BindingId borderBottomRightRadiusProperty = "resolvedStyle.borderBottomRightRadius";

		internal static readonly BindingId borderBottomWidthProperty = "resolvedStyle.borderBottomWidth";

		internal static readonly BindingId borderLeftColorProperty = "resolvedStyle.borderLeftColor";

		internal static readonly BindingId borderLeftWidthProperty = "resolvedStyle.borderLeftWidth";

		internal static readonly BindingId borderRightColorProperty = "resolvedStyle.borderRightColor";

		internal static readonly BindingId borderRightWidthProperty = "resolvedStyle.borderRightWidth";

		internal static readonly BindingId borderTopColorProperty = "resolvedStyle.borderTopColor";

		internal static readonly BindingId borderTopLeftRadiusProperty = "resolvedStyle.borderTopLeftRadius";

		internal static readonly BindingId borderTopRightRadiusProperty = "resolvedStyle.borderTopRightRadius";

		internal static readonly BindingId borderTopWidthProperty = "resolvedStyle.borderTopWidth";

		internal static readonly BindingId bottomProperty = "resolvedStyle.bottom";

		internal static readonly BindingId colorProperty = "resolvedStyle.color";

		internal static readonly BindingId displayProperty = "resolvedStyle.display";

		internal static readonly BindingId flexBasisProperty = "resolvedStyle.flexBasis";

		internal static readonly BindingId flexDirectionProperty = "resolvedStyle.flexDirection";

		internal static readonly BindingId flexGrowProperty = "resolvedStyle.flexGrow";

		internal static readonly BindingId flexShrinkProperty = "resolvedStyle.flexShrink";

		internal static readonly BindingId flexWrapProperty = "resolvedStyle.flexWrap";

		internal static readonly BindingId fontSizeProperty = "resolvedStyle.fontSize";

		internal static readonly BindingId heightProperty = "resolvedStyle.height";

		internal static readonly BindingId justifyContentProperty = "resolvedStyle.justifyContent";

		internal static readonly BindingId leftProperty = "resolvedStyle.left";

		internal static readonly BindingId letterSpacingProperty = "resolvedStyle.letterSpacing";

		internal static readonly BindingId marginBottomProperty = "resolvedStyle.marginBottom";

		internal static readonly BindingId marginLeftProperty = "resolvedStyle.marginLeft";

		internal static readonly BindingId marginRightProperty = "resolvedStyle.marginRight";

		internal static readonly BindingId marginTopProperty = "resolvedStyle.marginTop";

		internal static readonly BindingId maxHeightProperty = "resolvedStyle.maxHeight";

		internal static readonly BindingId maxWidthProperty = "resolvedStyle.maxWidth";

		internal static readonly BindingId minHeightProperty = "resolvedStyle.minHeight";

		internal static readonly BindingId minWidthProperty = "resolvedStyle.minWidth";

		internal static readonly BindingId opacityProperty = "resolvedStyle.opacity";

		internal static readonly BindingId paddingBottomProperty = "resolvedStyle.paddingBottom";

		internal static readonly BindingId paddingLeftProperty = "resolvedStyle.paddingLeft";

		internal static readonly BindingId paddingRightProperty = "resolvedStyle.paddingRight";

		internal static readonly BindingId paddingTopProperty = "resolvedStyle.paddingTop";

		internal static readonly BindingId positionProperty = "resolvedStyle.position";

		internal static readonly BindingId rightProperty = "resolvedStyle.right";

		internal static readonly BindingId rotateProperty = "resolvedStyle.rotate";

		internal static readonly BindingId scaleProperty = "resolvedStyle.scale";

		internal static readonly BindingId textOverflowProperty = "resolvedStyle.textOverflow";

		internal static readonly BindingId topProperty = "resolvedStyle.top";

		internal static readonly BindingId transformOriginProperty = "resolvedStyle.transformOrigin";

		internal static readonly BindingId transitionDelayProperty = "resolvedStyle.transitionDelay";

		internal static readonly BindingId transitionDurationProperty = "resolvedStyle.transitionDuration";

		internal static readonly BindingId transitionPropertyProperty = "resolvedStyle.transitionProperty";

		internal static readonly BindingId transitionTimingFunctionProperty = "resolvedStyle.transitionTimingFunction";

		internal static readonly BindingId translateProperty = "resolvedStyle.translate";

		internal static readonly BindingId unityBackgroundImageTintColorProperty = "resolvedStyle.unityBackgroundImageTintColor";

		internal static readonly BindingId unityEditorTextRenderingModeProperty = "resolvedStyle.unityEditorTextRenderingMode";

		internal static readonly BindingId unityFontProperty = "resolvedStyle.unityFont";

		internal static readonly BindingId unityFontDefinitionProperty = "resolvedStyle.unityFontDefinition";

		internal static readonly BindingId unityFontStyleAndWeightProperty = "resolvedStyle.unityFontStyleAndWeight";

		internal static readonly BindingId unityParagraphSpacingProperty = "resolvedStyle.unityParagraphSpacing";

		internal static readonly BindingId unitySliceBottomProperty = "resolvedStyle.unitySliceBottom";

		internal static readonly BindingId unitySliceLeftProperty = "resolvedStyle.unitySliceLeft";

		internal static readonly BindingId unitySliceRightProperty = "resolvedStyle.unitySliceRight";

		internal static readonly BindingId unitySliceScaleProperty = "resolvedStyle.unitySliceScale";

		internal static readonly BindingId unitySliceTopProperty = "resolvedStyle.unitySliceTop";

		internal static readonly BindingId unitySliceTypeProperty = "resolvedStyle.unitySliceType";

		internal static readonly BindingId unityTextAlignProperty = "resolvedStyle.unityTextAlign";

		internal static readonly BindingId unityTextGeneratorProperty = "resolvedStyle.unityTextGenerator";

		internal static readonly BindingId unityTextOutlineColorProperty = "resolvedStyle.unityTextOutlineColor";

		internal static readonly BindingId unityTextOutlineWidthProperty = "resolvedStyle.unityTextOutlineWidth";

		internal static readonly BindingId unityTextOverflowPositionProperty = "resolvedStyle.unityTextOverflowPosition";

		internal static readonly BindingId visibilityProperty = "resolvedStyle.visibility";

		internal static readonly BindingId whiteSpaceProperty = "resolvedStyle.whiteSpace";

		internal static readonly BindingId widthProperty = "resolvedStyle.width";

		internal static readonly BindingId wordSpacingProperty = "resolvedStyle.wordSpacing";
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static class StyleProperties
	{
		internal static readonly BindingId alignContentProperty = "style.alignContent";

		internal static readonly BindingId alignItemsProperty = "style.alignItems";

		internal static readonly BindingId alignSelfProperty = "style.alignSelf";

		internal static readonly BindingId backgroundColorProperty = "style.backgroundColor";

		internal static readonly BindingId backgroundImageProperty = "style.backgroundImage";

		internal static readonly BindingId backgroundPositionXProperty = "style.backgroundPositionX";

		internal static readonly BindingId backgroundPositionYProperty = "style.backgroundPositionY";

		internal static readonly BindingId backgroundRepeatProperty = "style.backgroundRepeat";

		internal static readonly BindingId backgroundSizeProperty = "style.backgroundSize";

		internal static readonly BindingId borderBottomColorProperty = "style.borderBottomColor";

		internal static readonly BindingId borderBottomLeftRadiusProperty = "style.borderBottomLeftRadius";

		internal static readonly BindingId borderBottomRightRadiusProperty = "style.borderBottomRightRadius";

		internal static readonly BindingId borderBottomWidthProperty = "style.borderBottomWidth";

		internal static readonly BindingId borderLeftColorProperty = "style.borderLeftColor";

		internal static readonly BindingId borderLeftWidthProperty = "style.borderLeftWidth";

		internal static readonly BindingId borderRightColorProperty = "style.borderRightColor";

		internal static readonly BindingId borderRightWidthProperty = "style.borderRightWidth";

		internal static readonly BindingId borderTopColorProperty = "style.borderTopColor";

		internal static readonly BindingId borderTopLeftRadiusProperty = "style.borderTopLeftRadius";

		internal static readonly BindingId borderTopRightRadiusProperty = "style.borderTopRightRadius";

		internal static readonly BindingId borderTopWidthProperty = "style.borderTopWidth";

		internal static readonly BindingId bottomProperty = "style.bottom";

		internal static readonly BindingId colorProperty = "style.color";

		internal static readonly BindingId cursorProperty = "style.cursor";

		internal static readonly BindingId displayProperty = "style.display";

		internal static readonly BindingId flexBasisProperty = "style.flexBasis";

		internal static readonly BindingId flexDirectionProperty = "style.flexDirection";

		internal static readonly BindingId flexGrowProperty = "style.flexGrow";

		internal static readonly BindingId flexShrinkProperty = "style.flexShrink";

		internal static readonly BindingId flexWrapProperty = "style.flexWrap";

		internal static readonly BindingId fontSizeProperty = "style.fontSize";

		internal static readonly BindingId heightProperty = "style.height";

		internal static readonly BindingId justifyContentProperty = "style.justifyContent";

		internal static readonly BindingId leftProperty = "style.left";

		internal static readonly BindingId letterSpacingProperty = "style.letterSpacing";

		internal static readonly BindingId marginBottomProperty = "style.marginBottom";

		internal static readonly BindingId marginLeftProperty = "style.marginLeft";

		internal static readonly BindingId marginRightProperty = "style.marginRight";

		internal static readonly BindingId marginTopProperty = "style.marginTop";

		internal static readonly BindingId maxHeightProperty = "style.maxHeight";

		internal static readonly BindingId maxWidthProperty = "style.maxWidth";

		internal static readonly BindingId minHeightProperty = "style.minHeight";

		internal static readonly BindingId minWidthProperty = "style.minWidth";

		internal static readonly BindingId opacityProperty = "style.opacity";

		internal static readonly BindingId overflowProperty = "style.overflow";

		internal static readonly BindingId paddingBottomProperty = "style.paddingBottom";

		internal static readonly BindingId paddingLeftProperty = "style.paddingLeft";

		internal static readonly BindingId paddingRightProperty = "style.paddingRight";

		internal static readonly BindingId paddingTopProperty = "style.paddingTop";

		internal static readonly BindingId positionProperty = "style.position";

		internal static readonly BindingId rightProperty = "style.right";

		internal static readonly BindingId rotateProperty = "style.rotate";

		internal static readonly BindingId scaleProperty = "style.scale";

		internal static readonly BindingId textOverflowProperty = "style.textOverflow";

		internal static readonly BindingId textShadowProperty = "style.textShadow";

		internal static readonly BindingId topProperty = "style.top";

		internal static readonly BindingId transformOriginProperty = "style.transformOrigin";

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static readonly BindingId transitionDelayProperty = "style.transitionDelay";

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static readonly BindingId transitionDurationProperty = "style.transitionDuration";

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static readonly BindingId transitionPropertyProperty = "style.transitionProperty";

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static readonly BindingId transitionTimingFunctionProperty = "style.transitionTimingFunction";

		internal static readonly BindingId translateProperty = "style.translate";

		internal static readonly BindingId unityBackgroundImageTintColorProperty = "style.unityBackgroundImageTintColor";

		internal static readonly BindingId unityEditorTextRenderingModeProperty = "style.unityEditorTextRenderingMode";

		internal static readonly BindingId unityFontProperty = "style.unityFont";

		internal static readonly BindingId unityFontDefinitionProperty = "style.unityFontDefinition";

		internal static readonly BindingId unityFontStyleAndWeightProperty = "style.unityFontStyleAndWeight";

		internal static readonly BindingId unityOverflowClipBoxProperty = "style.unityOverflowClipBox";

		internal static readonly BindingId unityParagraphSpacingProperty = "style.unityParagraphSpacing";

		internal static readonly BindingId unitySliceBottomProperty = "style.unitySliceBottom";

		internal static readonly BindingId unitySliceLeftProperty = "style.unitySliceLeft";

		internal static readonly BindingId unitySliceRightProperty = "style.unitySliceRight";

		internal static readonly BindingId unitySliceScaleProperty = "style.unitySliceScale";

		internal static readonly BindingId unitySliceTopProperty = "style.unitySliceTop";

		internal static readonly BindingId unitySliceTypeProperty = "style.unitySliceType";

		internal static readonly BindingId unityTextAlignProperty = "style.unityTextAlign";

		internal static readonly BindingId unityTextAutoSizeProperty = "style.unityTextAutoSize";

		internal static readonly BindingId unityTextGeneratorProperty = "style.unityTextGenerator";

		internal static readonly BindingId unityTextOutlineColorProperty = "style.unityTextOutlineColor";

		internal static readonly BindingId unityTextOutlineWidthProperty = "style.unityTextOutlineWidth";

		internal static readonly BindingId unityTextOverflowPositionProperty = "style.unityTextOverflowPosition";

		internal static readonly BindingId visibilityProperty = "style.visibility";

		internal static readonly BindingId whiteSpaceProperty = "style.whiteSpace";

		internal static readonly BindingId widthProperty = "style.width";

		internal static readonly BindingId wordSpacingProperty = "style.wordSpacing";
	}

	[Serializable]
	public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
	{
		[HideInInspector]
		[SerializeField]
		private string name;

		[HideInInspector]
		[SerializeReference]
		[UxmlObjectReference("Bindings")]
		private List<Binding.UxmlSerializedData> bindings;

		[SerializeField]
		private string tooltip;

		[HideInInspector]
		[SerializeField]
		[Tooltip("The path to the value in the data source used by this binding. To see resolved bindings in the UI Builder, define a path that is compatible with the target source property.")]
		[UxmlAttribute("data-source-path")]
		private string dataSourcePathString;

		[Tooltip("A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.")]
		[UxmlAttribute("data-source-type")]
		[UxmlTypeReference(typeof(object))]
		[HideInInspector]
		[SerializeField]
		private string dataSourceTypeString;

		[UxmlAttribute("data-source")]
		[DataSourceDrawer]
		[HideInInspector]
		[SerializeField]
		[Tooltip("A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.")]
		private Object dataSourceUnityObject;

		[SerializeField]
		private string viewDataKey;

		[SerializeField]
		[UxmlAttribute(obsoleteNames = new string[] { "pickingMode" })]
		private PickingMode pickingMode;

		[SerializeField]
		private UsageHints usageHints;

		[SerializeField]
		private LanguageDirection languageDirection;

		[UxmlAttribute("tabindex")]
		[SerializeField]
		private int tabIndex;

		[SerializeField]
		private bool focusable;

		[SerializeField]
		[Tooltip("Sets the element to disabled which will not accept input. Utilizes the :disabled pseudo state.")]
		[UxmlAttribute("enabled")]
		private bool enabledSelf;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags name_UxmlAttributeFlags;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags enabledSelf_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags viewDataKey_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags pickingMode_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags tooltip_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags usageHints_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags tabIndex_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags focusable_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags languageDirection_UxmlAttributeFlags;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags dataSourceUnityObject_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags dataSourcePathString_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags dataSourceTypeString_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags bindings_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[13]
			{
				new UxmlAttributeNames("name", "name", null),
				new UxmlAttributeNames("enabledSelf", "enabled", null),
				new UxmlAttributeNames("viewDataKey", "view-data-key", null),
				new UxmlAttributeNames("pickingMode", "picking-mode", null, "pickingMode"),
				new UxmlAttributeNames("tooltip", "tooltip", null),
				new UxmlAttributeNames("usageHints", "usage-hints", null),
				new UxmlAttributeNames("tabIndex", "tabindex", null),
				new UxmlAttributeNames("focusable", "focusable", null),
				new UxmlAttributeNames("languageDirection", "language-direction", null),
				new UxmlAttributeNames("dataSourceUnityObject", "data-source", null),
				new UxmlAttributeNames("dataSourcePathString", "data-source-path", null),
				new UxmlAttributeNames("dataSourceTypeString", "data-source-type", typeof(object)),
				new UxmlAttributeNames("bindings", "Bindings", null)
			});
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool HasBindingInternal(string property)
		{
			if (bindings == null)
			{
				return false;
			}
			foreach (Binding.UxmlSerializedData binding in bindings)
			{
				if (binding.property == property)
				{
					return true;
				}
			}
			return false;
		}

		[ExcludeFromDocs]
		public override object CreateInstance()
		{
			return new VisualElement();
		}

		[ExcludeFromDocs]
		public override void Deserialize(object obj)
		{
			VisualElement visualElement = (VisualElement)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(name_UxmlAttributeFlags))
			{
				visualElement.name = name;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(enabledSelf_UxmlAttributeFlags))
			{
				visualElement.enabledSelf = enabledSelf;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(viewDataKey_UxmlAttributeFlags))
			{
				visualElement.viewDataKey = viewDataKey;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(pickingMode_UxmlAttributeFlags))
			{
				visualElement.pickingMode = pickingMode;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(tooltip_UxmlAttributeFlags))
			{
				visualElement.tooltip = tooltip;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(usageHints_UxmlAttributeFlags))
			{
				visualElement.usageHints = usageHints;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(tabIndex_UxmlAttributeFlags))
			{
				visualElement.tabIndex = tabIndex;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(focusable_UxmlAttributeFlags))
			{
				visualElement.focusable = focusable;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(dataSourceUnityObject_UxmlAttributeFlags))
			{
				visualElement.dataSourceUnityObject = (dataSourceUnityObject ? dataSourceUnityObject : null);
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(dataSourcePathString_UxmlAttributeFlags))
			{
				visualElement.dataSourcePathString = dataSourcePathString;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(dataSourceTypeString_UxmlAttributeFlags))
			{
				visualElement.dataSourceTypeString = dataSourceTypeString;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(languageDirection_UxmlAttributeFlags))
			{
				visualElement.languageDirection = languageDirection;
			}
			if (!UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(bindings_UxmlAttributeFlags))
			{
				return;
			}
			visualElement.bindings.Clear();
			if (bindings == null)
			{
				return;
			}
			foreach (Binding.UxmlSerializedData binding2 in bindings)
			{
				Binding binding = (Binding)binding2.CreateInstance();
				binding2.Deserialize(binding);
				visualElement.SetBinding(binding.property, binding);
				visualElement.bindings.Add(binding);
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public class UxmlFactory : UxmlFactory<VisualElement, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public class UxmlTraits : UnityEngine.UIElements.UxmlTraits
	{
		protected UxmlStringAttributeDescription m_Name = new UxmlStringAttributeDescription
		{
			name = "name"
		};

		private UxmlBoolAttributeDescription m_EnabledSelf = new UxmlBoolAttributeDescription
		{
			name = "enabled",
			defaultValue = true
		};

		private UxmlStringAttributeDescription m_ViewDataKey = new UxmlStringAttributeDescription
		{
			name = "view-data-key"
		};

		protected UxmlEnumAttributeDescription<PickingMode> m_PickingMode = new UxmlEnumAttributeDescription<PickingMode>
		{
			name = "picking-mode",
			obsoleteNames = new string[1] { "pickingMode" }
		};

		private UxmlStringAttributeDescription m_Tooltip = new UxmlStringAttributeDescription
		{
			name = "tooltip"
		};

		private UxmlEnumAttributeDescription<UsageHints> m_UsageHints = new UxmlEnumAttributeDescription<UsageHints>
		{
			name = "usage-hints"
		};

		private UxmlIntAttributeDescription m_TabIndex = new UxmlIntAttributeDescription
		{
			name = "tabindex",
			defaultValue = 0
		};

		private UxmlStringAttributeDescription m_Class = new UxmlStringAttributeDescription
		{
			name = "class"
		};

		private UxmlStringAttributeDescription m_ContentContainer = new UxmlStringAttributeDescription
		{
			name = "content-container",
			obsoleteNames = new string[1] { "contentContainer" }
		};

		private UxmlStringAttributeDescription m_Style = new UxmlStringAttributeDescription
		{
			name = "style"
		};

		private UxmlAssetAttributeDescription<Object> m_DataSource = new UxmlAssetAttributeDescription<Object>
		{
			name = "data-source"
		};

		private UxmlStringAttributeDescription m_DataSourcePath = new UxmlStringAttributeDescription
		{
			name = "data-source-path"
		};

		protected UxmlIntAttributeDescription focusIndex { get; set; } = new UxmlIntAttributeDescription
		{
			name = null,
			obsoleteNames = new string[2] { "focus-index", "focusIndex" },
			defaultValue = -1
		};

		protected UxmlBoolAttributeDescription focusable { get; set; } = new UxmlBoolAttributeDescription
		{
			name = "focusable",
			defaultValue = false
		};

		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield return new UxmlChildElementDescription(typeof(VisualElement));
			}
		}

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			if (ve == null)
			{
				throw new ArgumentNullException("ve");
			}
			ve.name = m_Name.GetValueFromBag(bag, cc);
			ve.enabledSelf = m_EnabledSelf.GetValueFromBag(bag, cc);
			ve.viewDataKey = m_ViewDataKey.GetValueFromBag(bag, cc);
			ve.pickingMode = m_PickingMode.GetValueFromBag(bag, cc);
			ve.usageHints = m_UsageHints.GetValueFromBag(bag, cc);
			ve.tooltip = m_Tooltip.GetValueFromBag(bag, cc);
			int value = 0;
			if (focusIndex.TryGetValueFromBag(bag, cc, ref value))
			{
				ve.tabIndex = ((value >= 0) ? value : 0);
				ve.focusable = value >= 0;
			}
			ve.tabIndex = m_TabIndex.GetValueFromBag(bag, cc);
			ve.focusable = focusable.GetValueFromBag(bag, cc);
			ve.dataSource = m_DataSource.GetValueFromBag(bag, cc);
			ve.dataSourcePath = new PropertyPath(m_DataSourcePath.GetValueFromBag(bag, cc));
		}
	}

	public enum MeasureMode
	{
		Undefined,
		Exactly,
		AtMost
	}

	public struct Hierarchy
	{
		private const string k_InvalidHierarchyChangeMsg = "Cannot modify VisualElement hierarchy during layout calculation";

		private readonly VisualElement m_Owner;

		public VisualElement parent => m_Owner.m_PhysicalParent;

		internal List<VisualElement> children => m_Owner.m_Children;

		public int childCount => m_Owner.m_Children.Count;

		public VisualElement this[int key] => m_Owner.m_Children[key];

		internal Hierarchy(VisualElement element)
		{
			m_Owner = element;
		}

		public void Add(VisualElement child)
		{
			if (child == null)
			{
				throw new ArgumentException("Cannot add null child");
			}
			Insert(childCount, child);
		}

		public void Insert(int index, VisualElement child)
		{
			if (child == null)
			{
				throw new ArgumentException("Cannot insert null child");
			}
			if (index > childCount)
			{
				throw new ArgumentOutOfRangeException("Index out of range: " + index);
			}
			if (child == m_Owner)
			{
				throw new ArgumentException("Cannot insert element as its own child");
			}
			if (m_Owner.elementPanel != null && m_Owner.elementPanel.duringLayoutPhase)
			{
				throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
			}
			child.RemoveFromHierarchy();
			if (m_Owner.m_Children == s_EmptyList)
			{
				m_Owner.m_Children = VisualElementListPool.Get();
			}
			if (m_Owner.layoutNode.UsesMeasure)
			{
				m_Owner.RemoveMeasureFunction();
			}
			PutChildAtIndex(child, index);
			int num = child.imguiContainerDescendantCount + (child.isIMGUIContainer ? 1 : 0);
			if (num > 0)
			{
				m_Owner.ChangeIMGUIContainerCount(num);
			}
			child.hierarchy.SetParent(m_Owner);
			child.PropagateEnabledToChildren(m_Owner.enabledInHierarchy);
			if (child.languageDirection == LanguageDirection.Inherit)
			{
				child.localLanguageDirection = m_Owner.localLanguageDirection;
			}
			child.InvokeHierarchyChanged(HierarchyChangeType.AddedToParent);
			child.IncrementVersion(VersionChangeType.Hierarchy);
			m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
			m_Owner.elementAdded?.Invoke(child, index);
		}

		public void Remove(VisualElement child)
		{
			if (child == null)
			{
				throw new ArgumentException("Cannot remove null child");
			}
			if (child.hierarchy.parent != m_Owner)
			{
				throw new ArgumentException("This VisualElement is not my child");
			}
			int index = m_Owner.m_Children.IndexOf(child);
			RemoveAt(index);
		}

		public void RemoveAt(int index)
		{
			if (m_Owner.elementPanel != null && m_Owner.elementPanel.duringLayoutPhase)
			{
				throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
			}
			if (index < 0 || index >= childCount)
			{
				throw new ArgumentOutOfRangeException("Index out of range: " + index);
			}
			VisualElement visualElement = m_Owner.m_Children[index];
			BaseVisualElementPanel elementPanel = visualElement.elementPanel;
			if (elementPanel is RuntimePanel && !elementPanel.isFlat)
			{
				WorldSpaceDataStore.ClearWorldSpaceData(visualElement);
			}
			visualElement.InvokeHierarchyChanged(HierarchyChangeType.RemovedFromParent);
			RemoveChildAtIndex(index);
			int num = visualElement.imguiContainerDescendantCount + (visualElement.isIMGUIContainer ? 1 : 0);
			if (num > 0)
			{
				m_Owner.ChangeIMGUIContainerCount(-num);
			}
			visualElement.hierarchy.SetParent(null);
			if (childCount == 0)
			{
				ReleaseChildList();
				if (m_Owner.requireMeasureFunction)
				{
					m_Owner.AssignMeasureFunction();
				}
			}
			m_Owner.elementPanel?.OnVersionChanged(visualElement, VersionChangeType.Hierarchy);
			m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
			m_Owner.elementRemoved?.Invoke(visualElement);
		}

		public void Clear()
		{
			if (m_Owner.elementPanel != null && m_Owner.elementPanel.duringLayoutPhase)
			{
				throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
			}
			if (childCount <= 0)
			{
				return;
			}
			List<VisualElement> list = VisualElementListPool.Copy(m_Owner.m_Children);
			BaseVisualElementPanel elementPanel = m_Owner.elementPanel;
			if (elementPanel is RuntimePanel && !elementPanel.isFlat)
			{
				foreach (VisualElement child in m_Owner.m_Children)
				{
					WorldSpaceDataStore.ClearWorldSpaceData(child);
				}
			}
			ReleaseChildList();
			m_Owner.layoutNode.Clear();
			if (m_Owner.requireMeasureFunction)
			{
				m_Owner.AssignMeasureFunction();
			}
			foreach (VisualElement item in list)
			{
				item.InvokeHierarchyChanged(HierarchyChangeType.RemovedFromParent);
				item.hierarchy.SetParent(null);
				item.m_LogicalParent = null;
				m_Owner.elementPanel?.OnVersionChanged(item, VersionChangeType.Hierarchy);
				m_Owner.elementRemoved?.Invoke(item);
			}
			if (m_Owner.imguiContainerDescendantCount > 0)
			{
				int num = m_Owner.imguiContainerDescendantCount;
				if (m_Owner.isIMGUIContainer)
				{
					num--;
				}
				m_Owner.ChangeIMGUIContainerCount(-num);
			}
			VisualElementListPool.Release(list);
			m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
		}

		internal void BringToFront(VisualElement child)
		{
			if (childCount > 1)
			{
				int num = m_Owner.m_Children.IndexOf(child);
				if (num >= 0 && num < childCount - 1)
				{
					MoveChildElement(child, num, childCount);
				}
			}
		}

		internal void SendToBack(VisualElement child)
		{
			if (childCount > 1)
			{
				int num = m_Owner.m_Children.IndexOf(child);
				if (num > 0)
				{
					MoveChildElement(child, num, 0);
				}
			}
		}

		internal void PlaceBehind(VisualElement child, VisualElement over)
		{
			if (childCount <= 0)
			{
				return;
			}
			int num = m_Owner.m_Children.IndexOf(child);
			if (num >= 0)
			{
				int num2 = m_Owner.m_Children.IndexOf(over);
				if (num2 > 0 && num < num2)
				{
					num2--;
				}
				MoveChildElement(child, num, num2);
			}
		}

		internal void PlaceInFront(VisualElement child, VisualElement under)
		{
			if (childCount <= 0)
			{
				return;
			}
			int num = m_Owner.m_Children.IndexOf(child);
			if (num >= 0)
			{
				int num2 = m_Owner.m_Children.IndexOf(under);
				if (num > num2)
				{
					num2++;
				}
				MoveChildElement(child, num, num2);
			}
		}

		private void MoveChildElement(VisualElement child, int currentIndex, int nextIndex)
		{
			if (m_Owner.elementPanel != null && m_Owner.elementPanel.duringLayoutPhase)
			{
				throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
			}
			child.InvokeHierarchyChanged(HierarchyChangeType.RemovedFromParent);
			RemoveChildAtIndex(currentIndex);
			PutChildAtIndex(child, nextIndex);
			child.InvokeHierarchyChanged(HierarchyChangeType.AddedToParent);
			m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
		}

		public int IndexOf(VisualElement element)
		{
			return m_Owner.m_Children.IndexOf(element);
		}

		public VisualElement ElementAt(int index)
		{
			return this[index];
		}

		public IEnumerable<VisualElement> Children()
		{
			return m_Owner.m_Children;
		}

		private void SetParent(VisualElement value)
		{
			m_Owner.m_PhysicalParent = value;
			m_Owner.m_LogicalParent = value;
			m_Owner.DirtyNextParentWithEventInterests();
			m_Owner.SetPanel(value?.elementPanel);
			if (m_Owner.m_PhysicalParent != value)
			{
				Debug.LogError("Modifying the parent of a VisualElement while it’s already being modified is not allowed and can cause undefined behavior. Did you change the hierarchy during an AttachToPanelEvent or DetachFromPanelEvent?");
			}
		}

		public void Sort(Comparison<VisualElement> comp)
		{
			if (m_Owner.elementPanel != null && m_Owner.elementPanel.duringLayoutPhase)
			{
				throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
			}
			if (childCount > 1)
			{
				m_Owner.m_Children.Sort(comp);
				m_Owner.layoutNode.Clear();
				for (int i = 0; i < m_Owner.m_Children.Count; i++)
				{
					m_Owner.layoutNode.Insert(i, m_Owner.m_Children[i].layoutNode);
				}
				m_Owner.InvokeHierarchyChanged(HierarchyChangeType.ChildrenReordered);
				m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
			}
		}

		private void PutChildAtIndex(VisualElement child, int index)
		{
			if (index >= childCount)
			{
				m_Owner.m_Children.Add(child);
				m_Owner.layoutNode.Insert(m_Owner.layoutNode.Count, child.layoutNode);
			}
			else
			{
				m_Owner.m_Children.Insert(index, child);
				m_Owner.layoutNode.Insert(index, child.layoutNode);
			}
		}

		private void RemoveChildAtIndex(int index)
		{
			m_Owner.m_Children.RemoveAt(index);
			m_Owner.layoutNode.RemoveAt(index);
		}

		private void ReleaseChildList()
		{
			if (m_Owner.m_Children != s_EmptyList)
			{
				List<VisualElement> elements = m_Owner.m_Children;
				m_Owner.m_Children = s_EmptyList;
				VisualElementListPool.Release(elements);
			}
		}

		public bool Equals(Hierarchy other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return obj is Hierarchy && Equals((Hierarchy)obj);
		}

		public override int GetHashCode()
		{
			return (m_Owner != null) ? m_Owner.GetHashCode() : 0;
		}

		public static bool operator ==(Hierarchy x, Hierarchy y)
		{
			return x.m_Owner == y.m_Owner;
		}

		public static bool operator !=(Hierarchy x, Hierarchy y)
		{
			return !(x == y);
		}
	}

	private abstract class BaseVisualElementScheduledItem : ScheduledItem, IVisualElementScheduledItem
	{
		public bool isScheduled = false;

		private readonly EventCallback<AttachToPanelEvent> m_OnAttachToPanelCallback;

		private readonly EventCallback<DetachFromPanelEvent> m_OnDetachFromPanelCallback;

		public VisualElement element { get; private set; }

		public bool isActive { get; private set; }

		public bool isDetaching { get; private set; }

		protected BaseVisualElementScheduledItem(VisualElement handler)
		{
			element = handler;
			m_OnAttachToPanelCallback = OnElementAttachToPanelCallback;
			m_OnDetachFromPanelCallback = OnElementDetachFromPanelCallback;
		}

		private void SetActive(bool action)
		{
			if (isActive != action)
			{
				isActive = action;
				if (isActive)
				{
					element.RegisterCallback(m_OnAttachToPanelCallback);
					element.RegisterCallback(m_OnDetachFromPanelCallback);
					SendActivation();
				}
				else
				{
					element.UnregisterCallback(m_OnAttachToPanelCallback);
					element.UnregisterCallback(m_OnDetachFromPanelCallback);
					SendDeactivation();
				}
			}
		}

		private void SendActivation()
		{
			if (CanBeActivated())
			{
				OnPanelActivate();
			}
		}

		private void SendDeactivation()
		{
			if (CanBeActivated())
			{
				OnPanelDeactivate();
			}
		}

		private void OnElementAttachToPanelCallback(AttachToPanelEvent evt)
		{
			if (isActive)
			{
				SendActivation();
			}
		}

		private void OnElementDetachFromPanelCallback(DetachFromPanelEvent evt)
		{
			if (!isActive)
			{
				return;
			}
			isDetaching = true;
			try
			{
				SendDeactivation();
			}
			finally
			{
				isDetaching = false;
			}
		}

		public IVisualElementScheduledItem StartingIn(long delayMs)
		{
			base.delayMs = delayMs;
			return this;
		}

		public IVisualElementScheduledItem Until(Func<bool> stopCondition)
		{
			if (stopCondition == null)
			{
				stopCondition = ScheduledItem.ForeverCondition;
			}
			timerUpdateStopCondition = stopCondition;
			return this;
		}

		public IVisualElementScheduledItem ForDuration(long durationMs)
		{
			SetDuration(durationMs);
			return this;
		}

		public IVisualElementScheduledItem Every(long intervalMs)
		{
			base.intervalMs = intervalMs;
			if (timerUpdateStopCondition == ScheduledItem.OnceCondition)
			{
				timerUpdateStopCondition = ScheduledItem.ForeverCondition;
			}
			return this;
		}

		internal override void OnItemUnscheduled()
		{
			base.OnItemUnscheduled();
			isScheduled = false;
			if (!isDetaching)
			{
				SetActive(action: false);
			}
		}

		public void Resume()
		{
			SetActive(action: true);
		}

		public void Pause()
		{
			SetActive(action: false);
		}

		public void ExecuteLater(long delayMs)
		{
			if (!isScheduled)
			{
				Resume();
			}
			ResetStartTime();
			StartingIn(delayMs);
		}

		public void OnPanelActivate()
		{
			if (!isScheduled)
			{
				isScheduled = true;
				ResetStartTime();
				element.elementPanel.scheduler.Schedule(this);
			}
		}

		public void OnPanelDeactivate()
		{
			if (isScheduled)
			{
				isScheduled = false;
				element.elementPanel.scheduler.Unschedule(this);
			}
		}

		public bool CanBeActivated()
		{
			return element != null && element.elementPanel != null && element.elementPanel.scheduler != null;
		}
	}

	private abstract class VisualElementScheduledItem<ActionType> : BaseVisualElementScheduledItem
	{
		public ActionType updateEvent;

		public VisualElementScheduledItem(VisualElement handler, ActionType upEvent)
			: base(handler)
		{
			updateEvent = upEvent;
		}

		public static bool Matches(ScheduledItem item, ActionType updateEvent)
		{
			if (item is VisualElementScheduledItem<ActionType> visualElementScheduledItem)
			{
				return EqualityComparer<ActionType>.Default.Equals(visualElementScheduledItem.updateEvent, updateEvent);
			}
			return false;
		}
	}

	private class TimerStateScheduledItem : VisualElementScheduledItem<Action<TimerState>>
	{
		public TimerStateScheduledItem(VisualElement handler, Action<TimerState> updateEvent)
			: base(handler, updateEvent)
		{
		}

		public override void PerformTimerUpdate(TimerState state)
		{
			if (isScheduled)
			{
				updateEvent(state);
			}
		}
	}

	private class SimpleScheduledItem : VisualElementScheduledItem<Action>
	{
		public SimpleScheduledItem(VisualElement handler, Action updateEvent)
			: base(handler, updateEvent)
		{
		}

		public override void PerformTimerUpdate(TimerState state)
		{
			if (isScheduled)
			{
				updateEvent();
			}
		}
	}

	internal class CustomStyleAccess : ICustomStyle
	{
		private Dictionary<string, StylePropertyValue> m_CustomProperties;

		private float m_DpiScaling;

		public void SetContext(Dictionary<string, StylePropertyValue> customProperties, float dpiScaling)
		{
			m_CustomProperties = customProperties;
			m_DpiScaling = dpiScaling;
		}

		public bool TryGetValue(CustomStyleProperty<float> property, out float value)
		{
			if (TryGetValue(property.name, StyleValueType.Float, out var customProp) && customProp.sheet.TryReadFloat(customProp.handle, out value))
			{
				return true;
			}
			value = 0f;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<int> property, out int value)
		{
			if (TryGetValue(property.name, StyleValueType.Float, out var customProp) && customProp.sheet.TryReadFloat(customProp.handle, out var value2))
			{
				value = (int)value2;
				return true;
			}
			value = 0;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<bool> property, out bool value)
		{
			if (m_CustomProperties != null && m_CustomProperties.TryGetValue(property.name, out var value2))
			{
				value = value2.sheet.ReadKeyword(value2.handle) == StyleValueKeyword.True;
				return true;
			}
			value = false;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<Color> property, out Color value)
		{
			if (m_CustomProperties != null && m_CustomProperties.TryGetValue(property.name, out var value2))
			{
				StyleValueHandle handle = value2.handle;
				StyleValueType valueType = handle.valueType;
				StyleValueType styleValueType = valueType;
				if (styleValueType == StyleValueType.Color || styleValueType == StyleValueType.Enum)
				{
					return value2.sheet.TryReadColor(value2.handle, out value);
				}
				LogCustomPropertyWarning(property.name, StyleValueType.Color, value2);
			}
			value = Color.clear;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<Texture2D> property, out Texture2D value)
		{
			if (m_CustomProperties != null && m_CustomProperties.TryGetValue(property.name, out var value2))
			{
				ImageSource source = default(ImageSource);
				if (StylePropertyReader.TryGetImageSourceFromValue(value2, m_DpiScaling, out source) && source.texture != null)
				{
					value = source.texture;
					return true;
				}
			}
			value = null;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<Sprite> property, out Sprite value)
		{
			if (m_CustomProperties != null && m_CustomProperties.TryGetValue(property.name, out var value2))
			{
				ImageSource source = default(ImageSource);
				if (StylePropertyReader.TryGetImageSourceFromValue(value2, m_DpiScaling, out source) && source.sprite != null)
				{
					value = source.sprite;
					return true;
				}
			}
			value = null;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<VectorImage> property, out VectorImage value)
		{
			if (m_CustomProperties != null && m_CustomProperties.TryGetValue(property.name, out var value2))
			{
				ImageSource source = default(ImageSource);
				if (StylePropertyReader.TryGetImageSourceFromValue(value2, m_DpiScaling, out source) && source.vectorImage != null)
				{
					value = source.vectorImage;
					return true;
				}
			}
			value = null;
			return false;
		}

		public bool TryGetValue<T>(CustomStyleProperty<T> property, out T value) where T : Object
		{
			if (m_CustomProperties != null && m_CustomProperties.TryGetValue(property.name, out var value2) && value2.sheet.TryReadAssetReference(value2.handle, out var value3))
			{
				value = value3 as T;
				return value != null;
			}
			value = null;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<string> property, out string value)
		{
			if (m_CustomProperties != null && m_CustomProperties.TryGetValue(property.name, out var value2))
			{
				value = value2.sheet.ReadAsString(value2.handle);
				return true;
			}
			value = string.Empty;
			return false;
		}

		private bool TryGetValue(string propertyName, StyleValueType valueType, out StylePropertyValue customProp)
		{
			customProp = default(StylePropertyValue);
			if (m_CustomProperties != null && m_CustomProperties.TryGetValue(propertyName, out customProp))
			{
				StyleValueHandle handle = customProp.handle;
				if (handle.valueType != valueType)
				{
					LogCustomPropertyWarning(propertyName, valueType, customProp);
					return false;
				}
				return true;
			}
			return false;
		}

		private static void LogCustomPropertyWarning(string propertyName, StyleValueType valueType, StylePropertyValue customProp)
		{
			Debug.LogWarning($"Trying to read custom property {propertyName} value as {valueType} while parsed type is {customProp.handle.valueType}");
		}
	}

	internal class TypeData
	{
		private string m_FullTypeName = string.Empty;

		private string m_TypeName = string.Empty;

		public Type type { get; }

		public string fullTypeName
		{
			get
			{
				if (string.IsNullOrEmpty(m_FullTypeName))
				{
					m_FullTypeName = type.FullName;
				}
				return m_FullTypeName;
			}
		}

		public string typeName
		{
			get
			{
				if (string.IsNullOrEmpty(m_TypeName))
				{
					bool isGenericType = type.IsGenericType;
					m_TypeName = type.Name;
					if (isGenericType)
					{
						int num = m_TypeName.IndexOf('`');
						if (num >= 0)
						{
							m_TypeName = m_TypeName.Remove(num);
						}
					}
				}
				return m_TypeName;
			}
		}

		public TypeData(Type type)
		{
			this.type = type;
		}
	}

	internal static uint s_NextId;

	private static List<string> s_EmptyClassList = new List<string>(0);

	internal static readonly PropertyName userDataPropertyKey = new PropertyName("--unity-user-data");

	public static readonly string disabledUssClassName = "unity-disabled";

	private string m_Name;

	private List<string> m_ClassList;

	private Dictionary<PropertyName, object> m_PropertyBag;

	private VisualElementFlags m_Flags;

	private string m_ViewDataKey;

	private RenderHints m_RenderHints;

	internal Rect lastLayout;

	internal Rect lastPseudoPadding;

	internal RenderData renderData;

	internal RenderData nestedRenderData;

	internal int hierarchyDepth;

	internal int insertionIndex = -1;

	private Rect m_Layout;

	private Rect m_BoundingBox;

	private const VisualElementFlags worldBoundingBoxDirtyDependencies = VisualElementFlags.WorldTransformDirty | VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty;

	private Rect m_WorldBoundingBox;

	private const VisualElementFlags worldTransformInverseDirtyDependencies = VisualElementFlags.WorldTransformDirty | VisualElementFlags.WorldTransformInverseDirty;

	private Matrix4x4 m_WorldTransformCache = Matrix4x4.identity;

	private Matrix4x4 m_WorldTransformInverseCache = Matrix4x4.identity;

	internal PseudoStates triggerPseudoMask;

	internal PseudoStates dependencyPseudoMask;

	private PseudoStates m_PseudoStates;

	private PickingMode m_PickingMode;

	private LayoutNode m_LayoutNode;

	internal ComputedStyle m_Style = InitialStyle.Acquire();

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StyleVariableContext variableContext = StyleVariableContext.none;

	internal int inheritedStylesHash = 0;

	internal readonly uint controlid;

	internal int imguiContainerDescendantCount = 0;

	internal static int s_FinalizerCount = 0;

	private bool m_EnabledSelf;

	private LanguageDirection m_LanguageDirection;

	private LanguageDirection m_LocalLanguageDirection;

	private static readonly ProfilerMarker k_GenerateVisualContentMarker = new ProfilerMarker("GenerateVisualContent");

	private static Material s_runtimeMaterial;

	private Material m_defaultMaterial;

	private List<IValueAnimationUpdate> m_RunningAnimations;

	internal static readonly BindingId childCountProperty = "childCount";

	internal static readonly BindingId contentRectProperty = "contentRect";

	internal static readonly BindingId dataSourcePathProperty = "dataSourcePath";

	internal static readonly BindingId dataSourceProperty = "dataSource";

	internal static readonly BindingId disablePlayModeTintProperty = "disablePlayModeTint";

	internal static readonly BindingId enabledInHierarchyProperty = "enabledInHierarchy";

	internal static readonly BindingId enabledSelfProperty = "enabledSelf";

	internal static readonly BindingId layoutProperty = "layout";

	internal static readonly BindingId languageDirectionProperty = "languageDirection";

	internal static readonly BindingId localBoundProperty = "localBound";

	internal static readonly BindingId nameProperty = "name";

	internal static readonly BindingId panelProperty = "panel";

	internal static readonly BindingId pickingModeProperty = "pickingMode";

	internal static readonly BindingId styleSheetsProperty = "styleSheets";

	internal static readonly BindingId tooltipProperty = "tooltip";

	internal static readonly BindingId usageHintsProperty = "usageHints";

	internal static readonly BindingId userDataProperty = "userData";

	internal static readonly BindingId viewDataKeyProperty = "viewDataKey";

	internal static readonly BindingId visibleProperty = "visible";

	internal static readonly BindingId visualTreeAssetSourceProperty = "visualTreeAssetSource";

	internal static readonly BindingId worldBoundProperty = "worldBound";

	internal static readonly BindingId worldTransformProperty = "worldTransform";

	private object m_DataSource;

	private PathRef m_DataSourcePath;

	private List<Binding> m_Bindings;

	private readonly int m_TrickleDownHandleEventCategories;

	private readonly int m_BubbleUpHandleEventCategories;

	private int m_BubbleUpEventCallbackCategories = 0;

	private int m_TrickleDownEventCallbackCategories = 0;

	private int m_EventInterestSelfCategories = 0;

	private int m_CachedEventInterestParentCategories = 0;

	private static uint s_NextParentVersion;

	private uint m_NextParentCachedVersion;

	private uint m_NextParentRequiredVersion;

	private VisualElement m_CachedNextParentWithEventInterests;

	internal const string k_RootVisualContainerName = "rootVisualContainer";

	private VisualElement m_PhysicalParent;

	private VisualElement m_LogicalParent;

	private static readonly List<VisualElement> s_EmptyList = new List<VisualElement>();

	private List<VisualElement> m_Children;

	private VisualTreeAsset m_VisualTreeAssetSource = null;

	internal static CustomStyleAccess s_CustomStyleAccess = new CustomStyleAccess();

	internal InlineStyleAccess inlineStyleAccess;

	internal ResolvedStyleAccess resolvedStyleAccess;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal List<StyleSheet> styleSheetList;

	private static readonly Regex s_InternalStyleSheetPath = new Regex("^instanceId:[-0-9]+$", RegexOptions.Compiled);

	internal static readonly PropertyName tooltipPropertyKey = new PropertyName("--unity-tooltip");

	private static readonly Dictionary<Type, TypeData> s_TypeData = new Dictionary<Type, TypeData>();

	private TypeData m_TypeData;

	Align IResolvedStyle.alignContent => resolvedStyle.alignContent;

	Align IResolvedStyle.alignItems => resolvedStyle.alignItems;

	Align IResolvedStyle.alignSelf => resolvedStyle.alignSelf;

	Color IResolvedStyle.backgroundColor => resolvedStyle.backgroundColor;

	Background IResolvedStyle.backgroundImage => resolvedStyle.backgroundImage;

	BackgroundPosition IResolvedStyle.backgroundPositionX => resolvedStyle.backgroundPositionX;

	BackgroundPosition IResolvedStyle.backgroundPositionY => resolvedStyle.backgroundPositionY;

	BackgroundRepeat IResolvedStyle.backgroundRepeat => resolvedStyle.backgroundRepeat;

	BackgroundSize IResolvedStyle.backgroundSize => resolvedStyle.backgroundSize;

	Color IResolvedStyle.borderBottomColor => resolvedStyle.borderBottomColor;

	float IResolvedStyle.borderBottomLeftRadius => resolvedStyle.borderBottomLeftRadius;

	float IResolvedStyle.borderBottomRightRadius => resolvedStyle.borderBottomRightRadius;

	float IResolvedStyle.borderBottomWidth => resolvedStyle.borderBottomWidth;

	Color IResolvedStyle.borderLeftColor => resolvedStyle.borderLeftColor;

	float IResolvedStyle.borderLeftWidth => resolvedStyle.borderLeftWidth;

	Color IResolvedStyle.borderRightColor => resolvedStyle.borderRightColor;

	float IResolvedStyle.borderRightWidth => resolvedStyle.borderRightWidth;

	Color IResolvedStyle.borderTopColor => resolvedStyle.borderTopColor;

	float IResolvedStyle.borderTopLeftRadius => resolvedStyle.borderTopLeftRadius;

	float IResolvedStyle.borderTopRightRadius => resolvedStyle.borderTopRightRadius;

	float IResolvedStyle.borderTopWidth => resolvedStyle.borderTopWidth;

	float IResolvedStyle.bottom => resolvedStyle.bottom;

	Color IResolvedStyle.color => resolvedStyle.color;

	DisplayStyle IResolvedStyle.display => resolvedStyle.display;

	StyleFloat IResolvedStyle.flexBasis => resolvedStyle.flexBasis;

	FlexDirection IResolvedStyle.flexDirection => resolvedStyle.flexDirection;

	float IResolvedStyle.flexGrow => resolvedStyle.flexGrow;

	float IResolvedStyle.flexShrink => resolvedStyle.flexShrink;

	Wrap IResolvedStyle.flexWrap => resolvedStyle.flexWrap;

	float IResolvedStyle.fontSize => resolvedStyle.fontSize;

	float IResolvedStyle.height => resolvedStyle.height;

	Justify IResolvedStyle.justifyContent => resolvedStyle.justifyContent;

	float IResolvedStyle.left => resolvedStyle.left;

	float IResolvedStyle.letterSpacing => resolvedStyle.letterSpacing;

	float IResolvedStyle.marginBottom => resolvedStyle.marginBottom;

	float IResolvedStyle.marginLeft => resolvedStyle.marginLeft;

	float IResolvedStyle.marginRight => resolvedStyle.marginRight;

	float IResolvedStyle.marginTop => resolvedStyle.marginTop;

	StyleFloat IResolvedStyle.maxHeight => resolvedStyle.maxHeight;

	StyleFloat IResolvedStyle.maxWidth => resolvedStyle.maxWidth;

	StyleFloat IResolvedStyle.minHeight => resolvedStyle.minHeight;

	StyleFloat IResolvedStyle.minWidth => resolvedStyle.minWidth;

	float IResolvedStyle.opacity => resolvedStyle.opacity;

	float IResolvedStyle.paddingBottom => resolvedStyle.paddingBottom;

	float IResolvedStyle.paddingLeft => resolvedStyle.paddingLeft;

	float IResolvedStyle.paddingRight => resolvedStyle.paddingRight;

	float IResolvedStyle.paddingTop => resolvedStyle.paddingTop;

	Position IResolvedStyle.position => resolvedStyle.position;

	float IResolvedStyle.right => resolvedStyle.right;

	Rotate IResolvedStyle.rotate => resolvedStyle.rotate;

	Scale IResolvedStyle.scale => resolvedStyle.scale;

	TextOverflow IResolvedStyle.textOverflow => resolvedStyle.textOverflow;

	float IResolvedStyle.top => resolvedStyle.top;

	Vector3 IResolvedStyle.transformOrigin => resolvedStyle.transformOrigin;

	IEnumerable<TimeValue> IResolvedStyle.transitionDelay => resolvedStyle.transitionDelay;

	IEnumerable<TimeValue> IResolvedStyle.transitionDuration => resolvedStyle.transitionDuration;

	IEnumerable<StylePropertyName> IResolvedStyle.transitionProperty => resolvedStyle.transitionProperty;

	IEnumerable<EasingFunction> IResolvedStyle.transitionTimingFunction => resolvedStyle.transitionTimingFunction;

	Vector3 IResolvedStyle.translate => resolvedStyle.translate;

	Color IResolvedStyle.unityBackgroundImageTintColor => resolvedStyle.unityBackgroundImageTintColor;

	EditorTextRenderingMode IResolvedStyle.unityEditorTextRenderingMode => resolvedStyle.unityEditorTextRenderingMode;

	Font IResolvedStyle.unityFont => resolvedStyle.unityFont;

	FontDefinition IResolvedStyle.unityFontDefinition => resolvedStyle.unityFontDefinition;

	FontStyle IResolvedStyle.unityFontStyleAndWeight => resolvedStyle.unityFontStyleAndWeight;

	float IResolvedStyle.unityParagraphSpacing => resolvedStyle.unityParagraphSpacing;

	int IResolvedStyle.unitySliceBottom => resolvedStyle.unitySliceBottom;

	int IResolvedStyle.unitySliceLeft => resolvedStyle.unitySliceLeft;

	int IResolvedStyle.unitySliceRight => resolvedStyle.unitySliceRight;

	float IResolvedStyle.unitySliceScale => resolvedStyle.unitySliceScale;

	int IResolvedStyle.unitySliceTop => resolvedStyle.unitySliceTop;

	SliceType IResolvedStyle.unitySliceType => resolvedStyle.unitySliceType;

	TextAnchor IResolvedStyle.unityTextAlign => resolvedStyle.unityTextAlign;

	TextGeneratorType IResolvedStyle.unityTextGenerator => resolvedStyle.unityTextGenerator;

	Color IResolvedStyle.unityTextOutlineColor => resolvedStyle.unityTextOutlineColor;

	float IResolvedStyle.unityTextOutlineWidth => resolvedStyle.unityTextOutlineWidth;

	TextOverflowPosition IResolvedStyle.unityTextOverflowPosition => resolvedStyle.unityTextOverflowPosition;

	Visibility IResolvedStyle.visibility => resolvedStyle.visibility;

	WhiteSpace IResolvedStyle.whiteSpace => resolvedStyle.whiteSpace;

	float IResolvedStyle.width => resolvedStyle.width;

	float IResolvedStyle.wordSpacing => resolvedStyle.wordSpacing;

	internal bool hasRunningAnimations => styleAnimation.runningAnimationCount > 0;

	internal bool hasCompletedAnimations => styleAnimation.completedAnimationCount > 0;

	int IStylePropertyAnimations.runningAnimationCount { get; set; }

	int IStylePropertyAnimations.completedAnimationCount { get; set; }

	internal IStylePropertyAnimations styleAnimation => this;

	internal bool isCompositeRoot
	{
		get
		{
			return (m_Flags & VisualElementFlags.CompositeRoot) == VisualElementFlags.CompositeRoot;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.CompositeRoot) : (m_Flags & ~VisualElementFlags.CompositeRoot));
		}
	}

	internal bool areAncestorsAndSelfDisplayed
	{
		get
		{
			return (m_Flags & VisualElementFlags.HierarchyDisplayed) == VisualElementFlags.HierarchyDisplayed;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.HierarchyDisplayed) : (m_Flags & ~VisualElementFlags.HierarchyDisplayed));
			if (renderData != null && value && (renderData.pendingRepaint || renderData.pendingHierarchicalRepaint))
			{
				IncrementVersion(VersionChangeType.Repaint);
			}
		}
	}

	internal bool hasOneOrMorePointerCaptures
	{
		get
		{
			return (m_Flags & VisualElementFlags.PointerCapture) == VisualElementFlags.PointerCapture;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.PointerCapture) : (m_Flags & ~VisualElementFlags.PointerCapture));
		}
	}

	internal VisualElementFlags flags
	{
		get
		{
			RenderData obj = renderData;
			if (obj == null || (obj.flags & RenderDataFlags.IsClippingRectDirty) != RenderDataFlags.IsClippingRectDirty)
			{
				RenderData obj2 = nestedRenderData;
				if (obj2 == null || (obj2.flags & RenderDataFlags.IsClippingRectDirty) != RenderDataFlags.IsClippingRectDirty)
				{
					return m_Flags;
				}
			}
			return m_Flags | VisualElementFlags.WorldClipDirty;
		}
		set
		{
			m_Flags = value;
			if ((m_Flags & VisualElementFlags.WorldClipDirty) == VisualElementFlags.WorldClipDirty)
			{
				if (renderData != null)
				{
					renderData.flags |= RenderDataFlags.IsClippingRectDirty;
					m_Flags &= ~VisualElementFlags.WorldClipDirty;
				}
				if (nestedRenderData != null)
				{
					nestedRenderData.flags |= RenderDataFlags.IsClippingRectDirty;
					Debug.Assert(renderData != null, "renderData should not be null when nestedRenderData is not null");
				}
			}
		}
	}

	[CreateProperty]
	public string viewDataKey
	{
		get
		{
			return m_ViewDataKey;
		}
		set
		{
			if (m_ViewDataKey != value)
			{
				m_ViewDataKey = value;
				if (!string.IsNullOrEmpty(value))
				{
					IncrementVersion(VersionChangeType.ViewData);
				}
				NotifyPropertyChanged(in viewDataKeyProperty);
			}
		}
	}

	internal bool enableViewDataPersistence
	{
		get
		{
			return (m_Flags & VisualElementFlags.EnableViewDataPersistence) == VisualElementFlags.EnableViewDataPersistence;
		}
		private set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.EnableViewDataPersistence) : (m_Flags & ~VisualElementFlags.EnableViewDataPersistence));
		}
	}

	[CreateProperty]
	public object userData
	{
		get
		{
			if (m_PropertyBag != null)
			{
				m_PropertyBag.TryGetValue(userDataPropertyKey, out var value);
				return value;
			}
			return null;
		}
		set
		{
			object obj = userData;
			SetPropertyInternal(userDataPropertyKey, value);
			if (obj != userData)
			{
				NotifyPropertyChanged(in userDataProperty);
			}
		}
	}

	public override bool canGrabFocus
	{
		get
		{
			bool flag = false;
			for (VisualElement visualElement = hierarchy.parent; visualElement != null; visualElement = visualElement.parent)
			{
				if (visualElement.isCompositeRoot)
				{
					flag |= !visualElement.canGrabFocus;
					break;
				}
			}
			return !flag && visible && resolvedStyle.display != DisplayStyle.None && enabledInHierarchy && base.canGrabFocus;
		}
	}

	public override FocusController focusController => panel?.focusController;

	[CreateProperty]
	public bool disablePlayModeTint
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	internal Color playModeTintColor => disablePlayModeTint ? Color.white : UIElementsUtility.editorPlayModeTintColor;

	[CreateProperty]
	public UsageHints usageHints
	{
		get
		{
			return (UsageHints)((((renderHints & RenderHints.GroupTransform) != RenderHints.None) ? 2 : 0) | (((renderHints & RenderHints.BoneTransform) != RenderHints.None) ? 1 : 0) | (((renderHints & RenderHints.MaskContainer) != RenderHints.None) ? 4 : 0) | (((renderHints & RenderHints.DynamicColor) != RenderHints.None) ? 8 : 0));
		}
		set
		{
			if ((value & UsageHints.GroupTransform) != UsageHints.None)
			{
				renderHints |= RenderHints.GroupTransform;
			}
			else
			{
				renderHints &= ~RenderHints.GroupTransform;
			}
			if ((value & UsageHints.DynamicTransform) != UsageHints.None)
			{
				renderHints |= RenderHints.BoneTransform;
			}
			else
			{
				renderHints &= ~RenderHints.BoneTransform;
			}
			if ((value & UsageHints.MaskContainer) != UsageHints.None)
			{
				renderHints |= RenderHints.MaskContainer;
			}
			else
			{
				renderHints &= ~RenderHints.MaskContainer;
			}
			if ((value & UsageHints.DynamicColor) != UsageHints.None)
			{
				renderHints |= RenderHints.DynamicColor;
			}
			else
			{
				renderHints &= ~RenderHints.DynamicColor;
			}
			NotifyPropertyChanged(in usageHintsProperty);
		}
	}

	internal RenderHints renderHints
	{
		get
		{
			return m_RenderHints;
		}
		set
		{
			RenderHints renderHints = m_RenderHints & ~RenderHints.DirtyAll;
			RenderHints renderHints2 = value & ~RenderHints.DirtyAll;
			RenderHints renderHints3 = renderHints ^ renderHints2;
			if (renderHints3 != RenderHints.None)
			{
				RenderHints renderHints4 = m_RenderHints & RenderHints.DirtyAll;
				RenderHints renderHints5 = (RenderHints)((int)renderHints3 << 6);
				m_RenderHints = renderHints2 | renderHints4 | renderHints5;
				IncrementVersion(VersionChangeType.RenderHints);
			}
		}
	}

	internal bool useRenderTexture => false;

	[Obsolete("When writing the value, use VisualElement.style.translate, VisualElement.style.rotate or VisualElement.style.scale instead. When reading the value, use VisualElement.resolvedStyle.translate, scale and rotate")]
	public ITransform transform => this;

	Vector3 ITransform.position
	{
		get
		{
			return resolvedStyle.translate;
		}
		set
		{
			style.translate = new Translate(value.x, value.y, value.z);
		}
	}

	Quaternion ITransform.rotation
	{
		get
		{
			return resolvedStyle.rotate.ToQuaternion();
		}
		set
		{
			style.rotate = new Rotate(value);
		}
	}

	Vector3 ITransform.scale
	{
		get
		{
			Vector3 value = resolvedStyle.scale.value;
			BaseVisualElementPanel baseVisualElementPanel = elementPanel;
			if (baseVisualElementPanel != null && baseVisualElementPanel.isFlat)
			{
				value.z = 1f;
			}
			return value;
		}
		set
		{
			style.scale = new Scale((Vector2)value);
		}
	}

	Matrix4x4 ITransform.matrix
	{
		get
		{
			Vector3 value = resolvedStyle.scale.value;
			BaseVisualElementPanel baseVisualElementPanel = elementPanel;
			if (baseVisualElementPanel != null && baseVisualElementPanel.isFlat)
			{
				value.z = 1f;
			}
			return Matrix4x4.TRS(resolvedStyle.translate, resolvedStyle.rotate.ToQuaternion(), value);
		}
	}

	internal bool isLayoutManual
	{
		get
		{
			return (m_Flags & VisualElementFlags.LayoutManual) == VisualElementFlags.LayoutManual;
		}
		private set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.LayoutManual) : (m_Flags & ~VisualElementFlags.LayoutManual));
		}
	}

	public float scaledPixelsPerPoint
	{
		get
		{
			if (elementPanel == null)
			{
				Debug.LogWarning("Trying to access the DPI setting of a visual element that is not on a panel.");
				return GUIUtility.pixelsPerPoint;
			}
			return elementPanel.scaledPixelsPerPoint;
		}
	}

	[Obsolete("scaledPixelsPerPoint_noChecks is deprecated. Use scaledPixelsPerPoint instead.")]
	internal float scaledPixelsPerPoint_noChecks => elementPanel?.scaledPixelsPerPoint ?? GUIUtility.pixelsPerPoint;

	[Obsolete("unityBackgroundScaleMode is deprecated. Use background-* properties instead.")]
	StyleEnum<ScaleMode> IResolvedStyle.unityBackgroundScaleMode => resolvedStyle.unityBackgroundScaleMode;

	[CreateProperty(ReadOnly = true)]
	public Rect layout
	{
		get
		{
			Rect result = m_Layout;
			if (!layoutNode.IsUndefined && !isLayoutManual)
			{
				result.x = layoutNode.LayoutX;
				result.y = layoutNode.LayoutY;
				result.width = layoutNode.LayoutWidth;
				result.height = layoutNode.LayoutHeight;
			}
			return result;
		}
		internal set
		{
			if (!isLayoutManual || !(m_Layout == value))
			{
				Rect rect = layout;
				VersionChangeType versionChangeType = (VersionChangeType)0;
				if (!Mathf.Approximately(rect.x, value.x) || !Mathf.Approximately(rect.y, value.y))
				{
					versionChangeType |= VersionChangeType.Transform;
				}
				if (!Mathf.Approximately(rect.width, value.width) || !Mathf.Approximately(rect.height, value.height))
				{
					versionChangeType |= VersionChangeType.Size;
				}
				m_Layout = value;
				isLayoutManual = true;
				IStyle style = this.style;
				style.position = Position.Absolute;
				style.marginLeft = 0f;
				style.marginRight = 0f;
				style.marginBottom = 0f;
				style.marginTop = 0f;
				style.left = value.x;
				style.top = value.y;
				style.right = float.NaN;
				style.bottom = float.NaN;
				style.width = value.width;
				style.height = value.height;
				if (versionChangeType != 0)
				{
					IncrementVersion(versionChangeType);
				}
			}
		}
	}

	[CreateProperty(ReadOnly = true)]
	public Rect contentRect
	{
		get
		{
			Spacing spacing = new Spacing(resolvedStyle.paddingLeft, resolvedStyle.paddingTop, resolvedStyle.paddingRight, resolvedStyle.paddingBottom);
			return paddingRect - spacing;
		}
	}

	protected Rect paddingRect
	{
		get
		{
			Spacing spacing = new Spacing(resolvedStyle.borderLeftWidth, resolvedStyle.borderTopWidth, resolvedStyle.borderRightWidth, resolvedStyle.borderBottomWidth);
			return rect - spacing;
		}
	}

	internal bool needs3DBounds
	{
		get
		{
			return (m_Flags & VisualElementFlags.Needs3DBounds) != 0;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.Needs3DBounds) : (m_Flags & ~VisualElementFlags.Needs3DBounds));
		}
	}

	internal bool isLocalBounds3DDirty
	{
		get
		{
			return (m_Flags & VisualElementFlags.LocalBounds3DDirty) != 0;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.LocalBounds3DDirty) : (m_Flags & ~VisualElementFlags.LocalBounds3DDirty));
		}
	}

	internal bool isLocalBoundsWithoutNested3DDirty
	{
		get
		{
			return (m_Flags & VisualElementFlags.LocalBoundsWithoutNested3DDirty) != 0;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.LocalBoundsWithoutNested3DDirty) : (m_Flags & ~VisualElementFlags.LocalBoundsWithoutNested3DDirty));
		}
	}

	internal bool isBoundingBoxDirty
	{
		get
		{
			return (m_Flags & VisualElementFlags.BoundingBoxDirty) == VisualElementFlags.BoundingBoxDirty;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.BoundingBoxDirty) : (m_Flags & ~VisualElementFlags.BoundingBoxDirty));
		}
	}

	internal bool isWorldBoundingBoxDirty
	{
		get
		{
			return (m_Flags & VisualElementFlags.WorldBoundingBoxDirty) == VisualElementFlags.WorldBoundingBoxDirty;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.WorldBoundingBoxDirty) : (m_Flags & ~VisualElementFlags.WorldBoundingBoxDirty));
		}
	}

	internal bool isWorldBoundingBoxOrDependenciesDirty => (m_Flags & (VisualElementFlags.WorldTransformDirty | VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty)) != 0;

	internal Rect boundingBox
	{
		get
		{
			if (isBoundingBoxDirty)
			{
				UpdateBoundingBox();
				isBoundingBoxDirty = false;
			}
			return m_BoundingBox;
		}
	}

	internal Rect boundingBoxWithoutNested
	{
		get
		{
			if (isBoundingBoxDirty)
			{
				UpdateBoundingBox();
				isBoundingBoxDirty = false;
			}
			return WorldSpaceDataStore.GetWorldSpaceData(this).boundingBoxWithoutNested;
		}
	}

	internal Rect worldBoundingBox
	{
		get
		{
			if (isWorldBoundingBoxOrDependenciesDirty)
			{
				UpdateWorldBoundingBox();
				isWorldBoundingBoxDirty = false;
			}
			return m_WorldBoundingBox;
		}
	}

	private Rect boundingBoxInParentSpace
	{
		get
		{
			Rect result = boundingBox;
			TransformAlignedRectToParentSpace(ref result);
			return result;
		}
	}

	internal Bounds localBounds3D
	{
		get
		{
			if (isLocalBounds3DDirty)
			{
				UpdateBounds3D();
				isLocalBounds3DDirty = false;
			}
			return WorldSpaceDataStore.GetWorldSpaceData(this).localBounds3D;
		}
	}

	internal Bounds localBoundsPicking3D
	{
		get
		{
			if (isLocalBounds3DDirty)
			{
				UpdateBounds3D();
				isLocalBounds3DDirty = false;
			}
			return WorldSpaceDataStore.GetWorldSpaceData(this).localBounds3D;
		}
	}

	internal Bounds localBounds3DWithoutNested3D
	{
		get
		{
			if (isLocalBoundsWithoutNested3DDirty)
			{
				UpdateBounds3D();
				isLocalBoundsWithoutNested3DDirty = false;
			}
			return WorldSpaceDataStore.GetWorldSpaceData(this).localBoundsWithoutNested3D;
		}
	}

	[CreateProperty(ReadOnly = true)]
	public Rect worldBound
	{
		get
		{
			Rect result = rect;
			TransformAlignedRect(ref worldTransformRef, ref result);
			return result;
		}
	}

	[CreateProperty(ReadOnly = true)]
	public Rect localBound
	{
		get
		{
			Rect result = rect;
			TransformAlignedRectToParentSpace(ref result);
			return result;
		}
	}

	internal Rect rect
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			Rect rect = layout;
			return new Rect(0f, 0f, rect.width, rect.height);
		}
	}

	internal bool isWorldSpaceRootUIDocument
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (m_Flags & VisualElementFlags.IsWorldSpaceRootUIDocument) == VisualElementFlags.IsWorldSpaceRootUIDocument;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.IsWorldSpaceRootUIDocument) : (m_Flags & ~VisualElementFlags.IsWorldSpaceRootUIDocument));
		}
	}

	internal bool isWorldTransformDirty
	{
		get
		{
			return (m_Flags & VisualElementFlags.WorldTransformDirty) == VisualElementFlags.WorldTransformDirty;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.WorldTransformDirty) : (m_Flags & ~VisualElementFlags.WorldTransformDirty));
		}
	}

	internal bool isWorldTransformInverseDirty
	{
		get
		{
			return (m_Flags & VisualElementFlags.WorldTransformInverseDirty) == VisualElementFlags.WorldTransformInverseDirty;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.WorldTransformInverseDirty) : (m_Flags & ~VisualElementFlags.WorldTransformInverseDirty));
		}
	}

	internal bool isWorldTransformInverseOrDependenciesDirty => (m_Flags & (VisualElementFlags.WorldTransformDirty | VisualElementFlags.WorldTransformInverseDirty)) != 0;

	[CreateProperty(ReadOnly = true)]
	public Matrix4x4 worldTransform
	{
		get
		{
			if (isWorldTransformDirty)
			{
				UpdateWorldTransform();
			}
			return m_WorldTransformCache;
		}
	}

	internal ref Matrix4x4 worldTransformRef
	{
		get
		{
			if (isWorldTransformDirty)
			{
				UpdateWorldTransform();
			}
			return ref m_WorldTransformCache;
		}
	}

	internal ref Matrix4x4 worldTransformInverse
	{
		get
		{
			if (isWorldTransformInverseOrDependenciesDirty)
			{
				UpdateWorldTransformInverse();
			}
			return ref m_WorldTransformInverseCache;
		}
	}

	internal bool isWorldClipDirty => (flags & VisualElementFlags.WorldClipDirty) == VisualElementFlags.WorldClipDirty;

	internal Rect worldClip
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return renderData?.clippingRect ?? Rect.zero;
		}
	}

	internal Rect nestedTreeWorldClip => nestedRenderData?.clippingRect ?? Rect.zero;

	internal bool receivesHierarchyGeometryChangedEvents
	{
		get
		{
			return (m_Flags & VisualElementFlags.ReceivesHierarchyGeometryChangedEvents) == VisualElementFlags.ReceivesHierarchyGeometryChangedEvents;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.ReceivesHierarchyGeometryChangedEvents) : (m_Flags & ~VisualElementFlags.ReceivesHierarchyGeometryChangedEvents));
		}
	}

	internal bool boundingBoxDirtiedSinceLastLayoutPass
	{
		get
		{
			return (m_Flags & VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass) == VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass) : (m_Flags & ~VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass));
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal PseudoStates pseudoStates
	{
		get
		{
			return m_PseudoStates;
		}
		set
		{
			PseudoStates pseudoStates = m_PseudoStates ^ value;
			m_PseudoStates = value;
			if (pseudoStates <= (PseudoStates)0)
			{
				return;
			}
			if ((value & PseudoStates.Root) == PseudoStates.Root)
			{
				isRootVisualContainer = true;
			}
			if (pseudoStates != PseudoStates.Root)
			{
				PseudoStates pseudoStates2 = pseudoStates & value;
				PseudoStates pseudoStates3 = pseudoStates ^ pseudoStates2;
				if ((triggerPseudoMask & pseudoStates2) != 0 || (dependencyPseudoMask & pseudoStates3) != 0)
				{
					IncrementVersion(VersionChangeType.StyleSheet);
				}
			}
		}
	}

	internal int containedPointerIds { get; set; }

	[CreateProperty]
	public PickingMode pickingMode
	{
		get
		{
			return m_PickingMode;
		}
		set
		{
			if (m_PickingMode != value)
			{
				m_PickingMode = value;
				IncrementVersion(VersionChangeType.Picking);
				NotifyPropertyChanged(in pickingModeProperty);
			}
		}
	}

	[CreateProperty]
	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			if (!(m_Name == value))
			{
				m_Name = value;
				IncrementVersion(VersionChangeType.StyleSheet);
				NotifyPropertyChanged(in nameProperty);
			}
		}
	}

	internal List<string> classList
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			if (m_ClassList == s_EmptyClassList)
			{
				m_ClassList = ObjectListPool<string>.Get();
			}
			return m_ClassList;
		}
	}

	internal string fullTypeName
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return typeData.fullTypeName;
		}
	}

	internal string typeName
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return typeData.typeName;
		}
	}

	internal ref LayoutNode layoutNode => ref m_LayoutNode;

	internal ref ComputedStyle computedStyle
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return ref m_Style;
		}
	}

	internal bool hasInlineStyle => inlineStyleAccess != null;

	internal bool styleInitialized
	{
		get
		{
			return (m_Flags & VisualElementFlags.StyleInitialized) == VisualElementFlags.StyleInitialized;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.StyleInitialized) : (m_Flags & ~VisualElementFlags.StyleInitialized));
		}
	}

	internal float opacity
	{
		get
		{
			return resolvedStyle.opacity;
		}
		set
		{
			style.opacity = value;
		}
	}

	private bool isParentEnabledInHierarchy => hierarchy.parent == null || hierarchy.parent.enabledInHierarchy;

	[CreateProperty(ReadOnly = true)]
	public bool enabledInHierarchy => (pseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled;

	[CreateProperty]
	public bool enabledSelf
	{
		get
		{
			return m_EnabledSelf;
		}
		set
		{
			if (m_EnabledSelf != value)
			{
				m_EnabledSelf = value;
				NotifyPropertyChanged(in enabledSelfProperty);
				PropagateEnabledToChildren(value);
			}
		}
	}

	[CreateProperty]
	public LanguageDirection languageDirection
	{
		get
		{
			return m_LanguageDirection;
		}
		set
		{
			if (m_LanguageDirection != value)
			{
				m_LanguageDirection = value;
				localLanguageDirection = m_LanguageDirection;
				NotifyPropertyChanged(in languageDirectionProperty);
			}
		}
	}

	internal LanguageDirection localLanguageDirection
	{
		get
		{
			return m_LocalLanguageDirection;
		}
		set
		{
			if (m_LocalLanguageDirection == value)
			{
				return;
			}
			m_LocalLanguageDirection = value;
			IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
			int count = m_Children.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_Children[i].languageDirection == LanguageDirection.Inherit)
				{
					m_Children[i].localLanguageDirection = m_LocalLanguageDirection;
				}
			}
		}
	}

	[CreateProperty]
	public bool visible
	{
		get
		{
			return resolvedStyle.visibility == Visibility.Visible;
		}
		set
		{
			bool flag = visible;
			style.visibility = ((!value) ? Visibility.Hidden : Visibility.Visible);
			if (flag != visible)
			{
				NotifyPropertyChanged(in visibleProperty);
			}
		}
	}

	public Action<MeshGenerationContext> generateVisualContent { get; set; }

	internal bool requireMeasureFunction
	{
		get
		{
			return (m_Flags & VisualElementFlags.RequireMeasureFunction) == VisualElementFlags.RequireMeasureFunction;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.RequireMeasureFunction) : (m_Flags & ~VisualElementFlags.RequireMeasureFunction));
			if (value && !layoutNode.UsesMeasure)
			{
				AssignMeasureFunction();
			}
			else if (!value && layoutNode.UsesMeasure)
			{
				RemoveMeasureFunction();
			}
		}
	}

	internal Material defaultMaterial
	{
		get
		{
			return m_defaultMaterial;
		}
		private set
		{
			if (!(m_defaultMaterial == value))
			{
				m_defaultMaterial = value;
				IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
			}
		}
	}

	[CreateProperty]
	public object dataSource
	{
		get
		{
			return m_DataSource;
		}
		set
		{
			if (m_DataSource != value)
			{
				object previous = m_DataSource;
				m_DataSource = value;
				TrackSource(previous, m_DataSource);
				IncrementVersion(VersionChangeType.DataSource);
				NotifyPropertyChanged(in dataSourceProperty);
			}
		}
	}

	[UxmlAttributeBindingPath("dataSource")]
	internal Object dataSourceUnityObject
	{
		get
		{
			return dataSource as Object;
		}
		set
		{
			dataSource = value;
		}
	}

	[CreateProperty]
	public PropertyPath dataSourcePath
	{
		get
		{
			PathRef pathRef = m_DataSourcePath;
			return (pathRef != null) ? pathRef.path : default(PropertyPath);
		}
		set
		{
			if (m_DataSourcePath != null || !value.IsEmpty)
			{
				ref PropertyPath path = ref (m_DataSourcePath ?? (m_DataSourcePath = new PathRef())).path;
				if (!(path == value))
				{
					path = value;
					IncrementVersion(VersionChangeType.DataSource);
					NotifyPropertyChanged(in dataSourcePathProperty);
				}
			}
		}
	}

	internal bool isDataSourcePathEmpty => m_DataSourcePath == null || m_DataSourcePath.IsEmpty;

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

	private List<Binding> bindings
	{
		get
		{
			return m_Bindings ?? (m_Bindings = new List<Binding>());
		}
		set
		{
			m_Bindings = value;
		}
	}

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

	internal VisualElement nextParentWithEventInterests
	{
		get
		{
			if (GetCachedNextParentWithEventInterests(out var nextParent))
			{
				return nextParent;
			}
			for (VisualElement visualElement = hierarchy.parent; visualElement != null; visualElement = visualElement.hierarchy.parent)
			{
				if (visualElement.m_NextParentRequiredVersion != 0)
				{
					PropagateCachedNextParentWithEventInterests(visualElement, visualElement);
					return visualElement;
				}
				if (visualElement.GetCachedNextParentWithEventInterests(out var nextParent2))
				{
					PropagateCachedNextParentWithEventInterests(nextParent2, visualElement);
					return nextParent2;
				}
			}
			m_CachedNextParentWithEventInterests = null;
			return null;
		}
	}

	internal int eventInterestSelfCategories => m_EventInterestSelfCategories;

	internal int eventInterestParentCategories
	{
		get
		{
			if (elementPanel == null)
			{
				return -1;
			}
			if (isEventInterestParentCategoriesDirty)
			{
				UpdateEventInterestParentCategories();
				isEventInterestParentCategoriesDirty = false;
			}
			return m_CachedEventInterestParentCategories;
		}
	}

	internal bool isEventInterestParentCategoriesDirty
	{
		get
		{
			return (m_Flags & VisualElementFlags.EventInterestParentCategoriesDirty) == VisualElementFlags.EventInterestParentCategoriesDirty;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.EventInterestParentCategoriesDirty) : (m_Flags & ~VisualElementFlags.EventInterestParentCategoriesDirty));
		}
	}

	public IExperimentalFeatures experimental => this;

	ITransitionAnimations IExperimentalFeatures.animation => this;

	public Hierarchy hierarchy { get; }

	internal bool isRootVisualContainer { get; set; }

	[Obsolete("VisualElement.cacheAsBitmap is deprecated and has no effect")]
	public bool cacheAsBitmap { get; set; }

	internal bool disableClipping
	{
		get
		{
			return (m_Flags & VisualElementFlags.DisableClipping) == VisualElementFlags.DisableClipping;
		}
		set
		{
			m_Flags = (value ? (m_Flags | VisualElementFlags.DisableClipping) : (m_Flags & ~VisualElementFlags.DisableClipping));
		}
	}

	internal bool disableRendering
	{
		get
		{
			return (m_Flags & VisualElementFlags.DisableRendering) == VisualElementFlags.DisableRendering;
		}
		set
		{
			VisualElementFlags visualElementFlags = m_Flags;
			m_Flags = (value ? (m_Flags | VisualElementFlags.DisableRendering) : (m_Flags & ~VisualElementFlags.DisableRendering));
			if (visualElementFlags != m_Flags)
			{
				IncrementVersion(VersionChangeType.DisableRendering);
			}
		}
	}

	public VisualElement parent => m_LogicalParent;

	internal BaseVisualElementPanel elementPanel
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get;
		private set; }

	[CreateProperty(ReadOnly = true)]
	public IPanel panel => elementPanel;

	public virtual VisualElement contentContainer => this;

	[CreateProperty(ReadOnly = true)]
	public VisualTreeAsset visualTreeAssetSource
	{
		get
		{
			return m_VisualTreeAssetSource;
		}
		internal set
		{
			m_VisualTreeAssetSource = value;
		}
	}

	public VisualElement this[int key]
	{
		get
		{
			if (contentContainer == this)
			{
				return hierarchy[key];
			}
			return contentContainer?[key];
		}
	}

	[CreateProperty(ReadOnly = true)]
	public int childCount
	{
		get
		{
			if (contentContainer == this)
			{
				return hierarchy.childCount;
			}
			return contentContainer?.childCount ?? 0;
		}
	}

	private Vector3 positionWithLayout => ResolveTranslate() + (Vector3)layout.min;

	internal bool hasDefaultRotationAndScale
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return computedStyle.rotate.angle.value == 0f && computedStyle.scale.value == Vector3.one;
		}
	}

	internal bool has3DTransform => has3DTranslation || has3DRotation;

	private bool has3DTranslation => computedStyle.translate.z != 0f;

	private bool has3DRotation
	{
		get
		{
			Rotate rotate = computedStyle.rotate;
			return rotate.angle != 0f && rotate.axis != Vector3.forward;
		}
	}

	public IVisualElementScheduler schedule => this;

	[CreateProperty]
	public IStyle style
	{
		get
		{
			if (inlineStyleAccess == null)
			{
				inlineStyleAccess = new InlineStyleAccess(this);
			}
			return inlineStyleAccess;
		}
	}

	[CreateProperty]
	public IResolvedStyle resolvedStyle
	{
		get
		{
			if (resolvedStyleAccess == null)
			{
				resolvedStyleAccess = new ResolvedStyleAccess(this);
			}
			return resolvedStyleAccess;
		}
	}

	public ICustomStyle customStyle
	{
		get
		{
			s_CustomStyleAccess.SetContext(computedStyle.customProperties, computedStyle.dpiScaling);
			return s_CustomStyleAccess;
		}
	}

	[CreateProperty(ReadOnly = true)]
	public VisualElementStyleSheetSet styleSheets => new VisualElementStyleSheetSet(this);

	[CreateProperty]
	public string tooltip
	{
		get
		{
			string text = GetProperty(tooltipPropertyKey) as string;
			return text ?? string.Empty;
		}
		set
		{
			if (!HasProperty(tooltipPropertyKey))
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				RegisterCallback<TooltipEvent>(SetTooltip);
			}
			string strA = GetProperty(tooltipPropertyKey) as string;
			if (string.CompareOrdinal(strA, value) != 0)
			{
				SetProperty(tooltipPropertyKey, value);
				NotifyPropertyChanged(in tooltipProperty);
			}
		}
	}

	private TypeData typeData
	{
		get
		{
			if (m_TypeData == null)
			{
				Type type = GetType();
				if (!s_TypeData.TryGetValue(type, out m_TypeData))
				{
					m_TypeData = new TypeData(type);
					s_TypeData.Add(type, m_TypeData);
				}
			}
			return m_TypeData;
		}
	}

	internal event Action<VisualElement, int> elementAdded;

	internal event Action<VisualElement> elementRemoved;

	private IStylePropertyAnimationSystem GetStylePropertyAnimationSystem()
	{
		return elementPanel?.styleAnimationSystem;
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, float from, float to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, int from, int to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, Length from, Length to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, Color from, Color to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.StartEnum(StylePropertyId id, int from, int to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransitionEnum(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, Background from, Background to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, FontDefinition from, FontDefinition to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, Font from, Font to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, TextShadow from, TextShadow to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, Scale from, Scale to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, Translate from, Translate to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, Rotate from, Rotate to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, TransformOrigin from, TransformOrigin to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, BackgroundPosition from, BackgroundPosition to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, BackgroundRepeat from, BackgroundRepeat to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, BackgroundSize from, BackgroundSize to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	bool IStylePropertyAnimations.Start(StylePropertyId id, List<FilterFunction> from, List<FilterFunction> to, int durationMs, int delayMs, Func<float, float> easingCurve)
	{
		return GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
	}

	void IStylePropertyAnimations.CancelAnimation(StylePropertyId id)
	{
		GetStylePropertyAnimationSystem()?.CancelAnimation(this, id);
	}

	void IStylePropertyAnimations.CancelAllAnimations()
	{
		if (hasRunningAnimations || hasCompletedAnimations)
		{
			GetStylePropertyAnimationSystem()?.CancelAllAnimations(this);
		}
	}

	bool IStylePropertyAnimations.HasRunningAnimation(StylePropertyId id)
	{
		return hasRunningAnimations && GetStylePropertyAnimationSystem().HasRunningAnimation(this, id);
	}

	void IStylePropertyAnimations.UpdateAnimation(StylePropertyId id)
	{
		GetStylePropertyAnimationSystem().UpdateAnimation(this, id);
	}

	void IStylePropertyAnimations.GetAllAnimations(List<StylePropertyId> outPropertyIds)
	{
		if (hasRunningAnimations || hasCompletedAnimations)
		{
			GetStylePropertyAnimationSystem().GetAllAnimations(this, outPropertyIds);
		}
	}

	internal bool TryConvertLengthUnits(StylePropertyId id, ref Length from, ref Length to, int subPropertyIndex = 0)
	{
		if (from.IsAuto() || from.IsNone() || to.IsAuto() || to.IsNone())
		{
			return false;
		}
		if (float.IsNaN(from.value) || float.IsNaN(to.value))
		{
			return false;
		}
		if (from.unit == to.unit)
		{
			return true;
		}
		if (to.unit == LengthUnit.Pixel)
		{
			if (Mathf.Approximately(from.value, 0f))
			{
				from = new Length(0f, LengthUnit.Pixel);
				return true;
			}
			float? parentSizeForLengthConversion = GetParentSizeForLengthConversion(id, subPropertyIndex);
			if (!parentSizeForLengthConversion.HasValue || !(parentSizeForLengthConversion.Value >= 0f))
			{
				return false;
			}
			from = new Length(from.value * parentSizeForLengthConversion.Value / 100f, LengthUnit.Pixel);
		}
		else
		{
			Assert.AreEqual(LengthUnit.Percent, to.unit);
			float? parentSizeForLengthConversion2 = GetParentSizeForLengthConversion(id, subPropertyIndex);
			if (!parentSizeForLengthConversion2.HasValue || !(parentSizeForLengthConversion2.Value > 0f))
			{
				return false;
			}
			from = new Length(from.value * 100f / parentSizeForLengthConversion2.Value, LengthUnit.Percent);
		}
		return true;
	}

	internal bool TryConvertTransformOriginUnits(ref TransformOrigin from, ref TransformOrigin to)
	{
		Length from2 = from.x;
		Length from3 = from.y;
		Length to2 = to.x;
		Length to3 = to.y;
		if (!TryConvertLengthUnits(StylePropertyId.TransformOrigin, ref from2, ref to2))
		{
			return false;
		}
		if (!TryConvertLengthUnits(StylePropertyId.TransformOrigin, ref from3, ref to3, 1))
		{
			return false;
		}
		from.x = from2;
		from.y = from3;
		return true;
	}

	internal bool TryConvertTranslateUnits(ref Translate from, ref Translate to)
	{
		Length from2 = from.x;
		Length from3 = from.y;
		Length to2 = to.x;
		Length to3 = to.y;
		if (!TryConvertLengthUnits(StylePropertyId.Translate, ref from2, ref to2))
		{
			return false;
		}
		if (!TryConvertLengthUnits(StylePropertyId.Translate, ref from3, ref to3, 1))
		{
			return false;
		}
		from.x = from2;
		from.y = from3;
		return true;
	}

	internal bool TryConvertBackgroundPositionUnits(ref BackgroundPosition from, ref BackgroundPosition to)
	{
		Length from2 = from.offset;
		Length to2 = to.offset;
		if (!TryConvertLengthUnits(StylePropertyId.BackgroundPosition, ref from2, ref to2))
		{
			return false;
		}
		from.offset = from2;
		return true;
	}

	internal bool TryConvertBackgroundSizeUnits(ref BackgroundSize from, ref BackgroundSize to)
	{
		Length from2 = from.x;
		Length from3 = from.y;
		Length to2 = to.x;
		Length to3 = to.y;
		if (!TryConvertLengthUnits(StylePropertyId.BackgroundSize, ref from2, ref to2))
		{
			return false;
		}
		if (!TryConvertLengthUnits(StylePropertyId.BackgroundSize, ref from3, ref to3, 1))
		{
			return false;
		}
		from.x = from2;
		from.y = from3;
		return true;
	}

	private float? GetParentSizeForLengthConversion(StylePropertyId id, int subPropertyIndex = 0)
	{
		switch (id)
		{
		case StylePropertyId.Bottom:
		case StylePropertyId.Height:
		case StylePropertyId.MaxHeight:
		case StylePropertyId.MinHeight:
		case StylePropertyId.Top:
			return hierarchy.parent?.resolvedStyle.height;
		case StylePropertyId.Left:
		case StylePropertyId.MarginBottom:
		case StylePropertyId.MarginLeft:
		case StylePropertyId.MarginRight:
		case StylePropertyId.MarginTop:
		case StylePropertyId.MaxWidth:
		case StylePropertyId.MinWidth:
		case StylePropertyId.PaddingBottom:
		case StylePropertyId.PaddingLeft:
		case StylePropertyId.PaddingRight:
		case StylePropertyId.PaddingTop:
		case StylePropertyId.Right:
		case StylePropertyId.Width:
			return hierarchy.parent?.resolvedStyle.width;
		case StylePropertyId.FlexBasis:
		{
			if (hierarchy.parent == null)
			{
				return null;
			}
			FlexDirection flexDirection = hierarchy.parent.resolvedStyle.flexDirection;
			FlexDirection flexDirection2 = flexDirection;
			if ((uint)flexDirection2 <= 1u)
			{
				return hierarchy.parent.resolvedStyle.height;
			}
			return hierarchy.parent.resolvedStyle.width;
		}
		case StylePropertyId.BorderBottomLeftRadius:
		case StylePropertyId.BorderBottomRightRadius:
		case StylePropertyId.BorderTopLeftRadius:
		case StylePropertyId.BorderTopRightRadius:
			return resolvedStyle.width;
		case StylePropertyId.FontSize:
		case StylePropertyId.LetterSpacing:
		case StylePropertyId.UnityParagraphSpacing:
		case StylePropertyId.WordSpacing:
			return null;
		case StylePropertyId.TransformOrigin:
		case StylePropertyId.Translate:
			return (subPropertyIndex == 0) ? resolvedStyle.width : resolvedStyle.height;
		default:
			return null;
		}
	}

	internal void MarkRenderHintsClean()
	{
		m_RenderHints &= ~RenderHints.DirtyAll;
	}

	internal void ClearManualLayout()
	{
		isLayoutManual = false;
		IStyle style = this.style;
		style.position = StyleKeyword.Null;
		style.marginLeft = StyleKeyword.Null;
		style.marginRight = StyleKeyword.Null;
		style.marginBottom = StyleKeyword.Null;
		style.marginTop = StyleKeyword.Null;
		style.left = StyleKeyword.Null;
		style.top = StyleKeyword.Null;
		style.right = StyleKeyword.Null;
		style.bottom = StyleKeyword.Null;
		style.width = StyleKeyword.Null;
		style.height = StyleKeyword.Null;
	}

	internal void UpdateBoundingBox()
	{
		bool flag = elementPanel != null && !elementPanel.isFlat;
		Rect rect = this.rect;
		Rect rect2;
		if (float.IsNaN(rect.x) || float.IsNaN(rect.y) || float.IsNaN(rect.width) || float.IsNaN(rect.height))
		{
			m_BoundingBox = Rect.zero;
			rect2 = Rect.zero;
		}
		else
		{
			m_BoundingBox = rect;
			rect2 = rect;
			if (!ShouldClip() && resolvedStyle.display == DisplayStyle.Flex)
			{
				int count = m_Children.Count;
				for (int i = 0; i < count; i++)
				{
					VisualElement visualElement = m_Children[i];
					if (visualElement.areAncestorsAndSelfDisplayed)
					{
						Rect rect3 = visualElement.boundingBoxInParentSpace;
						m_BoundingBox.xMin = Math.Min(m_BoundingBox.xMin, rect3.xMin);
						m_BoundingBox.xMax = Math.Max(m_BoundingBox.xMax, rect3.xMax);
						m_BoundingBox.yMin = Math.Min(m_BoundingBox.yMin, rect3.yMin);
						m_BoundingBox.yMax = Math.Max(m_BoundingBox.yMax, rect3.yMax);
						if (flag && !(visualElement is UIDocumentRootElement))
						{
							rect2.xMin = Math.Min(rect2.xMin, rect3.xMin);
							rect2.xMax = Math.Max(rect2.xMax, rect3.xMax);
							rect2.yMin = Math.Min(rect2.yMin, rect3.yMin);
							rect2.yMax = Math.Max(rect2.yMax, rect3.yMax);
						}
					}
				}
			}
		}
		if (flag)
		{
			WorldSpaceData worldSpaceData = WorldSpaceDataStore.GetWorldSpaceData(this);
			worldSpaceData.boundingBoxWithoutNested = rect2;
			WorldSpaceDataStore.SetWorldSpaceData(this, worldSpaceData);
		}
		isWorldBoundingBoxDirty = true;
	}

	internal void UpdateWorldBoundingBox()
	{
		m_WorldBoundingBox = boundingBox;
		TransformAlignedRect(ref worldTransformRef, ref m_WorldBoundingBox);
	}

	private void UpdateBounds3D()
	{
		if (!areAncestorsAndSelfDisplayed)
		{
			WorldSpaceDataStore.ClearLocalBounds3DData(this);
			return;
		}
		if (!needs3DBounds)
		{
			Rect rect = boundingBox;
			Bounds bounds = new Bounds(rect.center, rect.size);
			Rect rect2 = boundingBoxWithoutNested;
			Bounds localBoundsWithoutNested3D = new Bounds(rect2.center, rect2.size);
			WorldSpaceDataStore.SetWorldSpaceData(this, new WorldSpaceData
			{
				localBounds3D = bounds,
				localBoundsPicking3D = bounds,
				localBoundsWithoutNested3D = localBoundsWithoutNested3D,
				boundingBoxWithoutNested = rect2
			});
			return;
		}
		Bounds bounds2 = new Bounds(this.rect.center, this.rect.size);
		Bounds bounds3 = bounds2;
		Bounds bounds4 = ((pickingMode == PickingMode.Position) ? bounds3 : WorldSpaceData.k_Empty3DBounds);
		if (!ShouldClip())
		{
			int num = hierarchy.childCount;
			for (int i = 0; i < num; i++)
			{
				VisualElement visualElement = hierarchy[i];
				if (!(visualElement is UIDocumentRootElement))
				{
					Bounds bounds5 = visualElement.localBounds3DWithoutNested3D;
					if (bounds5.extents.x >= 0f)
					{
						visualElement.TransformAlignedBoundsToParentSpace(ref bounds5);
						bounds2.Encapsulate(bounds5);
					}
				}
				Bounds bounds6 = visualElement.localBounds3D;
				if (bounds6.extents.x >= 0f)
				{
					visualElement.TransformAlignedBoundsToParentSpace(ref bounds6);
					bounds3.Encapsulate(bounds6);
				}
				Bounds bounds7 = visualElement.localBoundsPicking3D;
				if (bounds7.extents.x >= 0f)
				{
					visualElement.TransformAlignedBoundsToParentSpace(ref bounds7);
					bounds4.Encapsulate(bounds7);
				}
			}
		}
		WorldSpaceData worldSpaceData = WorldSpaceDataStore.GetWorldSpaceData(this);
		worldSpaceData.localBounds3D = bounds3;
		worldSpaceData.localBoundsPicking3D = bounds4;
		worldSpaceData.localBoundsWithoutNested3D = bounds2;
		WorldSpaceDataStore.SetWorldSpaceData(this, worldSpaceData);
	}

	internal void UpdateWorldTransform()
	{
		if (elementPanel != null && !elementPanel.duringLayoutPhase)
		{
			isWorldTransformDirty = false;
		}
		if (hierarchy.parent != null)
		{
			if (hasDefaultRotationAndScale)
			{
				TranslateMatrix34(ref hierarchy.parent.worldTransformRef, positionWithLayout, out m_WorldTransformCache);
			}
			else
			{
				GetPivotedMatrixWithLayout(out var result);
				MultiplyMatrix34(ref hierarchy.parent.worldTransformRef, ref result, out m_WorldTransformCache);
			}
		}
		else
		{
			GetPivotedMatrixWithLayout(out m_WorldTransformCache);
		}
		isWorldTransformInverseDirty = true;
		isWorldBoundingBoxDirty = true;
	}

	internal void UpdateWorldTransformInverse()
	{
		Matrix4x4.Inverse3DAffine(worldTransform, ref m_WorldTransformInverseCache);
		isWorldTransformInverseDirty = false;
	}

	internal void EnsureWorldTransformAndClipUpToDate()
	{
		if (renderData != null)
		{
			if (isWorldTransformDirty)
			{
				UpdateWorldTransform();
			}
			renderData.UpdateClippingRect();
			renderData.flags &= ~RenderDataFlags.IsClippingRectDirty;
		}
	}

	internal static Rect ComputeAAAlignedBound(Rect position, Matrix4x4 mat)
	{
		Rect rect = position;
		Vector3 vector = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y, 0f));
		Vector3 vector2 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y, 0f));
		Vector3 vector3 = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y + rect.height, 0f));
		Vector3 vector4 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y + rect.height, 0f));
		return Rect.MinMaxRect(Mathf.Min(vector.x, Mathf.Min(vector2.x, Mathf.Min(vector3.x, vector4.x))), Mathf.Min(vector.y, Mathf.Min(vector2.y, Mathf.Min(vector3.y, vector4.y))), Mathf.Max(vector.x, Mathf.Max(vector2.x, Mathf.Max(vector3.x, vector4.x))), Mathf.Max(vector.y, Mathf.Max(vector2.y, Mathf.Max(vector3.y, vector4.y))));
	}

	internal void UpdateHoverPseudoState()
	{
		if (containedPointerIds == 0 || panel == null)
		{
			pseudoStates &= ~PseudoStates.Hover;
			return;
		}
		bool flag = false;
		for (int i = 0; i < PointerId.maxPointers; i++)
		{
			if ((containedPointerIds & (1 << i)) != 0 && IsPartOfCapturedChain(this, panel.GetCapturingElement(i)))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			pseudoStates |= PseudoStates.Hover;
		}
		else
		{
			pseudoStates &= ~PseudoStates.Hover;
		}
	}

	private static bool IsPartOfCapturedChain(VisualElement self, in IEventHandler capturingElement)
	{
		if (self == null)
		{
			return false;
		}
		if (capturingElement == null)
		{
			return true;
		}
		if (capturingElement == self)
		{
			return true;
		}
		return self.Contains(capturingElement as VisualElement);
	}

	internal void UpdateHoverPseudoStateAfterCaptureChange(int pointerId)
	{
		for (VisualElement visualElement = this; visualElement != null; visualElement = visualElement.parent)
		{
			visualElement.UpdateHoverPseudoState();
		}
		VisualElement visualElement2 = elementPanel?.GetTopElementUnderPointer(pointerId);
		VisualElement visualElement3 = visualElement2;
		while (visualElement3 != null && visualElement3 != this)
		{
			visualElement3.UpdateHoverPseudoState();
			visualElement3 = visualElement3.parent;
		}
	}

	internal void UpdatePointerCaptureFlag()
	{
		bool flag = false;
		for (int i = 0; i < PointerId.maxPointers; i++)
		{
			if (this.HasPointerCapture(i))
			{
				flag = true;
				break;
			}
		}
		hasOneOrMorePointerCaptures = flag;
	}

	private void ChangeIMGUIContainerCount(int delta)
	{
		for (VisualElement visualElement = this; visualElement != null; visualElement = visualElement.hierarchy.parent)
		{
			visualElement.imguiContainerDescendantCount += delta;
		}
	}

	public VisualElement()
	{
		m_Children = s_EmptyList;
		controlid = ++s_NextId;
		hierarchy = new Hierarchy(this);
		m_ClassList = s_EmptyClassList;
		flags = VisualElementFlags.Init;
		enabledSelf = true;
		focusable = false;
		name = string.Empty;
		layoutNode = LayoutManager.SharedManager.CreateNode();
		renderHints = RenderHints.None;
		EventInterestReflectionUtils.GetDefaultEventInterests(GetType(), out var defaultActionCategories, out var defaultActionAtTargetCategories, out var handleEventTrickleDownCategories, out var handleEventBubbleUpCategories);
		m_TrickleDownHandleEventCategories = handleEventTrickleDownCategories;
		m_BubbleUpHandleEventCategories = handleEventBubbleUpCategories | defaultActionAtTargetCategories | defaultActionCategories;
		UpdateEventInterestSelfCategories();
	}

	~VisualElement()
	{
		try
		{
			LayoutManager.SharedManager.EnqueueNodeForRecycling(ref m_LayoutNode);
			s_FinalizerCount++;
		}
		catch (Exception exception)
		{
			Debug.LogError("An exception occured in a VisualElement finalizer, please report a bug.");
			Debug.LogException(exception);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal virtual Rect GetTooltipRect()
	{
		return worldBound;
	}

	internal void SetTooltip(TooltipEvent e)
	{
		if (e.currentTarget is VisualElement visualElement && !string.IsNullOrEmpty(visualElement.tooltip))
		{
			e.rect = visualElement.GetTooltipRect();
			e.tooltip = visualElement.tooltip;
			e.StopImmediatePropagation();
		}
	}

	public sealed override void Focus()
	{
		if (!canGrabFocus && hierarchy.parent != null)
		{
			hierarchy.parent.Focus();
		}
		else
		{
			base.Focus();
		}
	}

	internal void SetPanel(BaseVisualElementPanel p)
	{
		if (panel == p)
		{
			return;
		}
		List<VisualElement> list = VisualElementListPool.Get();
		try
		{
			list.Add(this);
			GatherAllChildren(list);
			EventDispatcherGate? eventDispatcherGate = null;
			if (p?.dispatcher != null)
			{
				eventDispatcherGate = new EventDispatcherGate(p.dispatcher);
			}
			EventDispatcherGate? eventDispatcherGate2 = null;
			if (panel?.dispatcher != null && panel.dispatcher != p?.dispatcher)
			{
				eventDispatcherGate2 = new EventDispatcherGate(panel.dispatcher);
			}
			BaseVisualElementPanel baseVisualElementPanel = elementPanel;
			uint num = baseVisualElementPanel?.hierarchyVersion ?? 0;
			using (eventDispatcherGate)
			{
				using (eventDispatcherGate2)
				{
					panel?.dispatcher?.m_ClickDetector.Cleanup(list);
					foreach (VisualElement item in list)
					{
						item.WillChangePanel(p);
					}
					uint num2 = baseVisualElementPanel?.hierarchyVersion ?? 0;
					if (num != num2)
					{
						list.Clear();
						list.Add(this);
						GatherAllChildren(list);
					}
					VisualElementFlags visualElementFlags = ((p != null) ? VisualElementFlags.NeedsAttachToPanelEvent : ((VisualElementFlags)0));
					InvokeHierarchyChanged(HierarchyChangeType.DetachedFromPanel, list);
					foreach (VisualElement item2 in list)
					{
						item2.elementPanel = p;
						item2.flags |= visualElementFlags;
						item2.m_CachedNextParentWithEventInterests = null;
					}
					InvokeHierarchyChanged(HierarchyChangeType.AttachedToPanel, list);
					foreach (VisualElement item3 in list)
					{
						item3.HasChangedPanel(baseVisualElementPanel);
					}
				}
			}
		}
		finally
		{
			VisualElementListPool.Release(list);
		}
	}

	private void WillChangePanel(BaseVisualElementPanel destinationPanel)
	{
		if (elementPanel == null)
		{
			return;
		}
		UnregisterRunningAnimations();
		CreateBindingRequests();
		DetachDataSource();
		if (containedPointerIds != 0)
		{
			elementPanel.RemoveElementFromPointerCache(this);
			elementPanel.CommitElementUnderPointers();
		}
		if (hasOneOrMorePointerCaptures)
		{
			for (int i = 0; i < PointerId.maxPointers; i++)
			{
				if (this.HasPointerCapture(i))
				{
					this.ReleasePointer(i);
					elementPanel.ProcessPointerCapture(i);
				}
			}
		}
		if ((m_Flags & VisualElementFlags.NeedsAttachToPanelEvent) == 0 && HasSelfEventInterests(EventBase<DetachFromPanelEvent>.EventCategory))
		{
			using DetachFromPanelEvent detachFromPanelEvent = PanelChangedEventBase<DetachFromPanelEvent>.GetPooled(elementPanel, destinationPanel);
			detachFromPanelEvent.elementTarget = this;
			EventDispatchUtilities.HandleEventAtTargetAndDefaultPhase(detachFromPanelEvent, elementPanel, this);
		}
		UnregisterRunningAnimations();
	}

	private void HasChangedPanel(BaseVisualElementPanel prevPanel)
	{
		if (elementPanel != null)
		{
			layoutNode.Config = elementPanel.layoutConfig;
			layoutNode.SoftReset();
			RegisterRunningAnimations();
			ProcessBindingRequests();
			AttachDataSource();
			pseudoStates &= ~(PseudoStates.Active | PseudoStates.Hover);
			if ((pseudoStates & PseudoStates.Focus) != 0 && !focusController.IsFocused(this))
			{
				pseudoStates &= ~PseudoStates.Focus;
			}
			m_Flags &= ~VisualElementFlags.HierarchyDisplayed;
			if ((m_Flags & VisualElementFlags.NeedsAttachToPanelEvent) == VisualElementFlags.NeedsAttachToPanelEvent)
			{
				if (HasSelfEventInterests(EventBase<AttachToPanelEvent>.EventCategory))
				{
					using AttachToPanelEvent attachToPanelEvent = PanelChangedEventBase<AttachToPanelEvent>.GetPooled(prevPanel, elementPanel);
					attachToPanelEvent.elementTarget = this;
					EventDispatchUtilities.HandleEventAtTargetAndDefaultPhase(attachToPanelEvent, elementPanel, this);
				}
				m_Flags &= ~VisualElementFlags.NeedsAttachToPanelEvent;
			}
		}
		else
		{
			layoutNode.Config = LayoutManager.SharedManager.GetDefaultConfig();
		}
		styleInitialized = false;
		IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Transform);
		if (!string.IsNullOrEmpty(viewDataKey))
		{
			IncrementVersion(VersionChangeType.ViewData);
		}
	}

	public sealed override void SendEvent(EventBase e)
	{
		elementPanel?.SendEvent(e);
	}

	internal sealed override void SendEvent(EventBase e, DispatchMode dispatchMode)
	{
		elementPanel?.SendEvent(e, dispatchMode);
	}

	internal sealed override void HandleEvent(EventBase e)
	{
		EventDispatchUtilities.HandleEvent(e, this);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void IncrementVersion(VersionChangeType changeType)
	{
		elementPanel?.OnVersionChanged(this, changeType);
	}

	internal void InvokeHierarchyChanged(HierarchyChangeType changeType, IReadOnlyList<VisualElement> additionalContext = null)
	{
		elementPanel?.InvokeHierarchyChanged(this, changeType, additionalContext);
	}

	[Obsolete("SetEnabledFromHierarchy is deprecated and will be removed in a future release. Please use SetEnabled instead.")]
	protected internal bool SetEnabledFromHierarchy(bool state)
	{
		return SetEnabledFromHierarchyPrivate(state);
	}

	private bool SetEnabledFromHierarchyPrivate(bool state)
	{
		bool flag = enabledInHierarchy;
		bool flag2 = false;
		if (state)
		{
			if (isParentEnabledInHierarchy)
			{
				if (enabledSelf)
				{
					RemoveFromClassList(disabledUssClassName);
				}
				else
				{
					flag2 = true;
					AddToClassList(disabledUssClassName);
				}
			}
			else
			{
				flag2 = true;
				RemoveFromClassList(disabledUssClassName);
			}
		}
		else
		{
			flag2 = true;
			EnableInClassList(disabledUssClassName, isParentEnabledInHierarchy);
		}
		if (flag2)
		{
			if (focusController != null && focusController.IsFocused(this))
			{
				EventDispatcherGate? eventDispatcherGate = null;
				if (panel?.dispatcher != null)
				{
					eventDispatcherGate = new EventDispatcherGate(panel.dispatcher);
				}
				using (eventDispatcherGate)
				{
					BlurImmediately();
				}
			}
			pseudoStates |= PseudoStates.Disabled;
		}
		else
		{
			pseudoStates &= ~PseudoStates.Disabled;
		}
		return flag != enabledInHierarchy;
	}

	public void SetEnabled(bool value)
	{
		enabledSelf = value;
	}

	private void PropagateEnabledToChildren(bool value)
	{
		if (SetEnabledFromHierarchyPrivate(value))
		{
			int count = m_Children.Count;
			for (int i = 0; i < count; i++)
			{
				m_Children[i].PropagateEnabledToChildren(value);
			}
		}
	}

	public void MarkDirtyRepaint()
	{
		IncrementVersion(VersionChangeType.Repaint);
	}

	internal void InvokeGenerateVisualContent(MeshGenerationContext mgc)
	{
		if (generateVisualContent == null)
		{
			return;
		}
		try
		{
			using (k_GenerateVisualContentMarker.Auto())
			{
				generateVisualContent(mgc);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	internal void GetFullHierarchicalViewDataKey(StringBuilder key)
	{
		if (parent != null)
		{
			parent.GetFullHierarchicalViewDataKey(key);
		}
		if (!string.IsNullOrEmpty(viewDataKey))
		{
			key.Append("__");
			key.Append(viewDataKey);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal string GetFullHierarchicalViewDataKey()
	{
		StringBuilder stringBuilder = new StringBuilder();
		GetFullHierarchicalViewDataKey(stringBuilder);
		return stringBuilder.ToString();
	}

	internal T GetOrCreateViewData<T>(object existing, string key) where T : class, new()
	{
		Debug.Assert(elementPanel != null, "VisualElement.elementPanel is null! Cannot load persistent data.");
		ISerializableJsonDictionary serializableJsonDictionary = ((elementPanel == null || elementPanel.getViewDataDictionary == null) ? null : elementPanel.getViewDataDictionary());
		if (serializableJsonDictionary == null || string.IsNullOrEmpty(viewDataKey) || !enableViewDataPersistence)
		{
			if (existing != null)
			{
				return existing as T;
			}
			return new T();
		}
		string key2 = key + "__" + typeof(T);
		if (!serializableJsonDictionary.ContainsKey(key2))
		{
			serializableJsonDictionary.Set(key2, new T());
		}
		return serializableJsonDictionary.Get<T>(key2);
	}

	internal T GetOrCreateViewData<T>(ScriptableObject existing, string key) where T : ScriptableObject
	{
		Debug.Assert(elementPanel != null, "VisualElement.elementPanel is null! Cannot load view data.");
		ISerializableJsonDictionary serializableJsonDictionary = ((elementPanel == null || elementPanel.getViewDataDictionary == null) ? null : elementPanel.getViewDataDictionary());
		if (serializableJsonDictionary == null || string.IsNullOrEmpty(viewDataKey) || !enableViewDataPersistence)
		{
			if (existing != null)
			{
				return existing as T;
			}
			return ScriptableObject.CreateInstance<T>();
		}
		string key2 = key + "__" + typeof(T);
		if (!serializableJsonDictionary.ContainsKey(key2))
		{
			serializableJsonDictionary.Set(key2, ScriptableObject.CreateInstance<T>());
		}
		return serializableJsonDictionary.GetScriptable<T>(key2);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void OverwriteFromViewData(object obj, string key)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		Debug.Assert(elementPanel != null, "VisualElement.elementPanel is null! Cannot load view data.");
		ISerializableJsonDictionary serializableJsonDictionary = ((elementPanel == null || elementPanel.getViewDataDictionary == null) ? null : elementPanel.getViewDataDictionary());
		if (serializableJsonDictionary != null && !string.IsNullOrEmpty(viewDataKey) && enableViewDataPersistence)
		{
			string key2 = key + "__" + obj.GetType();
			if (!serializableJsonDictionary.ContainsKey(key2))
			{
				serializableJsonDictionary.Set(key2, obj);
			}
			else
			{
				serializableJsonDictionary.Overwrite(obj, key2);
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SaveViewData()
	{
		if (elementPanel != null && elementPanel.saveViewData != null && !string.IsNullOrEmpty(viewDataKey) && enableViewDataPersistence)
		{
			elementPanel.saveViewData();
		}
	}

	internal bool IsViewDataPersitenceSupportedOnChildren(bool existingState)
	{
		bool result = existingState;
		if (string.IsNullOrEmpty(viewDataKey) && this != contentContainer)
		{
			result = false;
		}
		if (parent != null && this == parent.contentContainer)
		{
			result = true;
		}
		return result;
	}

	internal void OnViewDataReady(bool enablePersistence)
	{
		enableViewDataPersistence = enablePersistence;
		OnViewDataReady();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal virtual void OnViewDataReady()
	{
	}

	public virtual bool ContainsPoint(Vector2 localPoint)
	{
		return rect.Contains(localPoint);
	}

	public virtual bool Overlaps(Rect rectangle)
	{
		return rect.Overlaps(rectangle, allowInverse: true);
	}

	private void AssignMeasureFunction()
	{
		layoutNode.SetOwner(this);
		layoutNode.UsesMeasure = true;
	}

	private void RemoveMeasureFunction()
	{
		layoutNode.UsesMeasure = false;
		layoutNode.SetOwner(null);
	}

	protected internal virtual Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode)
	{
		return new Vector2(float.NaN, float.NaN);
	}

	internal static void Measure(VisualElement ve, ref LayoutNode node, float width, LayoutMeasureMode widthMode, float height, LayoutMeasureMode heightMode, out LayoutSize result)
	{
		result = default(LayoutSize);
		Debug.Assert(node.Equals(ve.layoutNode), "LayoutNode instance mismatch");
		Vector2 vector = ve.DoMeasure(width, (MeasureMode)widthMode, height, (MeasureMode)heightMode);
		float pixelsPerPoint = ve.scaledPixelsPerPoint;
		result = new LayoutSize(AlignmentUtils.RoundToPixelGrid(vector.x, pixelsPerPoint), AlignmentUtils.RoundToPixelGrid(vector.y, pixelsPerPoint));
	}

	internal void SetSize(Vector2 size)
	{
		Rect rect = layout;
		rect.width = size.x;
		rect.height = size.y;
		layout = rect;
	}

	private void FinalizeLayout()
	{
		layoutNode.CopyFromComputedStyle(computedStyle);
	}

	internal void SetInlineRule(StyleSheet sheet, StyleRule rule)
	{
		if (inlineStyleAccess == null)
		{
			inlineStyleAccess = new InlineStyleAccess(this);
		}
		inlineStyleAccess.SetInlineRule(sheet, rule);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void UpdateInlineRule(StyleSheet sheet, StyleRule rule)
	{
		ComputedStyle x = computedStyle.Acquire();
		long matchingRulesHash = computedStyle.matchingRulesHash;
		if (!StyleCache.TryGetValue(matchingRulesHash, out var data))
		{
			data = InitialStyle.Get();
		}
		m_Style.CopyFrom(ref data);
		SetInlineRule(sheet, rule);
		FinalizeLayout();
		VersionChangeType changeType = ComputedStyle.CompareChanges(ref x, ref computedStyle);
		x.Release();
		IncrementVersion(changeType);
	}

	internal void SetComputedStyle(ref ComputedStyle newStyle)
	{
		if (m_Style.matchingRulesHash != newStyle.matchingRulesHash)
		{
			VersionChangeType changeType = ComputedStyle.CompareChanges(ref m_Style, ref newStyle);
			m_Style.CopyFrom(ref newStyle);
			FinalizeLayout();
			if (elementPanel?.GetTopElementUnderPointer(PointerId.mousePointerId) == this)
			{
				elementPanel.cursorManager.SetCursor(m_Style.cursor);
			}
			IncrementVersion(changeType);
		}
	}

	internal void ResetPositionProperties()
	{
		if (hasInlineStyle)
		{
			style.position = StyleKeyword.Null;
			style.marginLeft = StyleKeyword.Null;
			style.marginRight = StyleKeyword.Null;
			style.marginBottom = StyleKeyword.Null;
			style.marginTop = StyleKeyword.Null;
			style.left = StyleKeyword.Null;
			style.top = StyleKeyword.Null;
			style.right = StyleKeyword.Null;
			style.bottom = StyleKeyword.Null;
			style.width = StyleKeyword.Null;
			style.height = StyleKeyword.Null;
		}
	}

	public override string ToString()
	{
		return GetType().Name + " " + name + " " + layout.ToString() + " world rect: " + worldBound.ToString();
	}

	public IEnumerable<string> GetClasses()
	{
		return m_ClassList;
	}

	internal List<string> GetClassesForIteration()
	{
		return m_ClassList;
	}

	public void ClearClassList()
	{
		if (m_ClassList.Count > 0)
		{
			ObjectListPool<string>.Release(m_ClassList);
			m_ClassList = s_EmptyClassList;
			IncrementVersion(VersionChangeType.StyleSheet);
		}
	}

	public void AddToClassList(string className)
	{
		if (string.IsNullOrEmpty(className))
		{
			return;
		}
		if (m_ClassList == s_EmptyClassList)
		{
			m_ClassList = ObjectListPool<string>.Get();
		}
		else
		{
			if (m_ClassList.Contains(className))
			{
				return;
			}
			if (m_ClassList.Capacity == m_ClassList.Count)
			{
				m_ClassList.Capacity++;
			}
		}
		m_ClassList.Add(className);
		IncrementVersion(VersionChangeType.StyleSheet);
	}

	public void RemoveFromClassList(string className)
	{
		if (m_ClassList.Remove(className))
		{
			if (m_ClassList.Count == 0)
			{
				ObjectListPool<string>.Release(m_ClassList);
				m_ClassList = s_EmptyClassList;
			}
			IncrementVersion(VersionChangeType.StyleSheet);
		}
	}

	public void ToggleInClassList(string className)
	{
		if (ClassListContains(className))
		{
			RemoveFromClassList(className);
		}
		else
		{
			AddToClassList(className);
		}
	}

	public void EnableInClassList(string className, bool enable)
	{
		if (enable)
		{
			AddToClassList(className);
		}
		else
		{
			RemoveFromClassList(className);
		}
	}

	public bool ClassListContains(string cls)
	{
		for (int i = 0; i < m_ClassList.Count; i++)
		{
			if (m_ClassList[i].Equals(cls, StringComparison.Ordinal))
			{
				return true;
			}
		}
		return false;
	}

	public object FindAncestorUserData()
	{
		for (VisualElement visualElement = parent; visualElement != null; visualElement = visualElement.parent)
		{
			if (visualElement.userData != null)
			{
				return visualElement.userData;
			}
		}
		return null;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal object GetProperty(PropertyName key)
	{
		CheckUserKeyArgument(key);
		if (m_PropertyBag != null)
		{
			m_PropertyBag.TryGetValue(key, out var value);
			return value;
		}
		return null;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetProperty(PropertyName key, object value)
	{
		CheckUserKeyArgument(key);
		SetPropertyInternal(key, value);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool HasProperty(PropertyName key)
	{
		CheckUserKeyArgument(key);
		return m_PropertyBag?.ContainsKey(key) ?? false;
	}

	internal bool ClearProperty(PropertyName key)
	{
		CheckUserKeyArgument(key);
		return m_PropertyBag?.Remove(key) ?? false;
	}

	private static void CheckUserKeyArgument(PropertyName key)
	{
		if (PropertyName.IsNullOrEmpty(key))
		{
			throw new ArgumentNullException("key");
		}
		if (key == userDataPropertyKey)
		{
			throw new InvalidOperationException($"The {userDataPropertyKey} key is reserved by the system");
		}
	}

	private void SetPropertyInternal(PropertyName key, object value)
	{
		if (m_PropertyBag == null)
		{
			m_PropertyBag = new Dictionary<PropertyName, object>();
		}
		m_PropertyBag[key] = value;
	}

	internal void UpdateCursorStyle(long eventType)
	{
		if (elementPanel == null)
		{
			return;
		}
		if (eventType == EventBase<MouseCaptureOutEvent>.TypeId())
		{
			VisualElement topElementUnderPointer = elementPanel.GetTopElementUnderPointer(PointerId.mousePointerId);
			if (topElementUnderPointer != null)
			{
				elementPanel.cursorManager.SetCursor(topElementUnderPointer.computedStyle.cursor);
			}
			else
			{
				elementPanel.cursorManager.ResetCursor();
			}
			return;
		}
		IEventHandler capturingElement = elementPanel.GetCapturingElement(PointerId.mousePointerId);
		if (capturingElement == null || capturingElement == this)
		{
			if (eventType == EventBase<MouseOverEvent>.TypeId() && elementPanel.GetTopElementUnderPointer(PointerId.mousePointerId) == this)
			{
				elementPanel.cursorManager.SetCursor(computedStyle.cursor);
			}
			else if (eventType == EventBase<MouseOutEvent>.TypeId() && capturingElement == null)
			{
				elementPanel.cursorManager.ResetCursor();
			}
		}
	}

	private Material getRuntimeMaterial()
	{
		if (s_runtimeMaterial != null)
		{
			return s_runtimeMaterial;
		}
		Shader shader = Shader.Find(UIRUtility.k_DefaultShaderName);
		Debug.Assert(shader != null, "Failed to load UIElements default shader");
		if (shader != null)
		{
			shader.hideFlags |= HideFlags.DontSaveInEditor;
			Material material = new Material(shader);
			material.hideFlags |= HideFlags.DontSaveInEditor;
			return s_runtimeMaterial = material;
		}
		return null;
	}

	private VisualElementAnimationSystem GetAnimationSystem()
	{
		if (elementPanel != null)
		{
			return elementPanel.GetUpdater(VisualTreeUpdatePhase.Animation) as VisualElementAnimationSystem;
		}
		return null;
	}

	internal void RegisterAnimation(IValueAnimationUpdate anim)
	{
		if (m_RunningAnimations == null)
		{
			m_RunningAnimations = new List<IValueAnimationUpdate>();
		}
		m_RunningAnimations.Add(anim);
		GetAnimationSystem()?.RegisterAnimation(anim);
	}

	internal void UnregisterAnimation(IValueAnimationUpdate anim)
	{
		if (m_RunningAnimations != null)
		{
			m_RunningAnimations.Remove(anim);
		}
		GetAnimationSystem()?.UnregisterAnimation(anim);
	}

	private void UnregisterRunningAnimations()
	{
		if (m_RunningAnimations != null && m_RunningAnimations.Count > 0)
		{
			GetAnimationSystem()?.UnregisterAnimations(m_RunningAnimations);
		}
		styleAnimation.CancelAllAnimations();
	}

	private void RegisterRunningAnimations()
	{
		if (m_RunningAnimations != null && m_RunningAnimations.Count > 0)
		{
			GetAnimationSystem()?.RegisterAnimations(m_RunningAnimations);
		}
	}

	ValueAnimation<float> ITransitionAnimations.Start(float from, float to, int durationMs, Action<VisualElement, float> onValueChanged)
	{
		return experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
	}

	ValueAnimation<Rect> ITransitionAnimations.Start(Rect from, Rect to, int durationMs, Action<VisualElement, Rect> onValueChanged)
	{
		return experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
	}

	ValueAnimation<Color> ITransitionAnimations.Start(Color from, Color to, int durationMs, Action<VisualElement, Color> onValueChanged)
	{
		return experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
	}

	ValueAnimation<Vector3> ITransitionAnimations.Start(Vector3 from, Vector3 to, int durationMs, Action<VisualElement, Vector3> onValueChanged)
	{
		return experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
	}

	ValueAnimation<Vector2> ITransitionAnimations.Start(Vector2 from, Vector2 to, int durationMs, Action<VisualElement, Vector2> onValueChanged)
	{
		return experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
	}

	ValueAnimation<Quaternion> ITransitionAnimations.Start(Quaternion from, Quaternion to, int durationMs, Action<VisualElement, Quaternion> onValueChanged)
	{
		return experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
	}

	ValueAnimation<StyleValues> ITransitionAnimations.Start(StyleValues from, StyleValues to, int durationMs)
	{
		if (from.m_StyleValues == null)
		{
			from.Values();
		}
		if (to.m_StyleValues == null)
		{
			to.Values();
		}
		return Start((VisualElement e) => from, to, durationMs);
	}

	ValueAnimation<float> ITransitionAnimations.Start(Func<VisualElement, float> fromValueGetter, float to, int durationMs, Action<VisualElement, float> onValueChanged)
	{
		return StartAnimation(ValueAnimation<float>.Create(this, Lerp.Interpolate), fromValueGetter, to, durationMs, onValueChanged);
	}

	ValueAnimation<Rect> ITransitionAnimations.Start(Func<VisualElement, Rect> fromValueGetter, Rect to, int durationMs, Action<VisualElement, Rect> onValueChanged)
	{
		return StartAnimation(ValueAnimation<Rect>.Create(this, Lerp.Interpolate), fromValueGetter, to, durationMs, onValueChanged);
	}

	ValueAnimation<Color> ITransitionAnimations.Start(Func<VisualElement, Color> fromValueGetter, Color to, int durationMs, Action<VisualElement, Color> onValueChanged)
	{
		return StartAnimation(ValueAnimation<Color>.Create(this, Lerp.Interpolate), fromValueGetter, to, durationMs, onValueChanged);
	}

	ValueAnimation<Vector3> ITransitionAnimations.Start(Func<VisualElement, Vector3> fromValueGetter, Vector3 to, int durationMs, Action<VisualElement, Vector3> onValueChanged)
	{
		return StartAnimation(ValueAnimation<Vector3>.Create(this, Lerp.Interpolate), fromValueGetter, to, durationMs, onValueChanged);
	}

	ValueAnimation<Vector2> ITransitionAnimations.Start(Func<VisualElement, Vector2> fromValueGetter, Vector2 to, int durationMs, Action<VisualElement, Vector2> onValueChanged)
	{
		return StartAnimation(ValueAnimation<Vector2>.Create(this, Lerp.Interpolate), fromValueGetter, to, durationMs, onValueChanged);
	}

	ValueAnimation<Quaternion> ITransitionAnimations.Start(Func<VisualElement, Quaternion> fromValueGetter, Quaternion to, int durationMs, Action<VisualElement, Quaternion> onValueChanged)
	{
		return StartAnimation(ValueAnimation<Quaternion>.Create(this, Lerp.Interpolate), fromValueGetter, to, durationMs, onValueChanged);
	}

	private static ValueAnimation<T> StartAnimation<T>(ValueAnimation<T> anim, Func<VisualElement, T> fromValueGetter, T to, int durationMs, Action<VisualElement, T> onValueChanged)
	{
		anim.initialValue = fromValueGetter;
		anim.to = to;
		anim.durationMs = durationMs;
		anim.valueUpdated = onValueChanged;
		anim.Start();
		return anim;
	}

	private static void AssignStyleValues(VisualElement ve, StyleValues src)
	{
		IStyle style = ve.style;
		if (src.m_StyleValues == null)
		{
			return;
		}
		foreach (StyleValue value in src.m_StyleValues.m_Values)
		{
			switch (value.id)
			{
			case StylePropertyId.MarginLeft:
				style.marginLeft = value.number;
				break;
			case StylePropertyId.MarginTop:
				style.marginTop = value.number;
				break;
			case StylePropertyId.MarginRight:
				style.marginRight = value.number;
				break;
			case StylePropertyId.MarginBottom:
				style.marginBottom = value.number;
				break;
			case StylePropertyId.PaddingLeft:
				style.paddingLeft = value.number;
				break;
			case StylePropertyId.PaddingTop:
				style.paddingTop = value.number;
				break;
			case StylePropertyId.PaddingRight:
				style.paddingRight = value.number;
				break;
			case StylePropertyId.PaddingBottom:
				style.paddingBottom = value.number;
				break;
			case StylePropertyId.Left:
				style.left = value.number;
				break;
			case StylePropertyId.Top:
				style.top = value.number;
				break;
			case StylePropertyId.Right:
				style.right = value.number;
				break;
			case StylePropertyId.Bottom:
				style.bottom = value.number;
				break;
			case StylePropertyId.Width:
				style.width = value.number;
				break;
			case StylePropertyId.Height:
				style.height = value.number;
				break;
			case StylePropertyId.FlexGrow:
				style.flexGrow = value.number;
				break;
			case StylePropertyId.FlexShrink:
				style.flexShrink = value.number;
				break;
			case StylePropertyId.BorderLeftWidth:
				style.borderLeftWidth = value.number;
				break;
			case StylePropertyId.BorderTopWidth:
				style.borderTopWidth = value.number;
				break;
			case StylePropertyId.BorderRightWidth:
				style.borderRightWidth = value.number;
				break;
			case StylePropertyId.BorderBottomWidth:
				style.borderBottomWidth = value.number;
				break;
			case StylePropertyId.BorderTopLeftRadius:
				style.borderTopLeftRadius = value.number;
				break;
			case StylePropertyId.BorderTopRightRadius:
				style.borderTopRightRadius = value.number;
				break;
			case StylePropertyId.BorderBottomRightRadius:
				style.borderBottomRightRadius = value.number;
				break;
			case StylePropertyId.BorderBottomLeftRadius:
				style.borderBottomLeftRadius = value.number;
				break;
			case StylePropertyId.FontSize:
				style.fontSize = value.number;
				break;
			case StylePropertyId.Color:
				style.color = value.color;
				break;
			case StylePropertyId.BackgroundColor:
				style.backgroundColor = value.color;
				break;
			case StylePropertyId.BorderColor:
				style.borderLeftColor = value.color;
				style.borderTopColor = value.color;
				style.borderRightColor = value.color;
				style.borderBottomColor = value.color;
				break;
			case StylePropertyId.UnityBackgroundImageTintColor:
				style.unityBackgroundImageTintColor = value.color;
				break;
			case StylePropertyId.Opacity:
				style.opacity = value.number;
				break;
			}
		}
	}

	private StyleValues ReadCurrentValues(VisualElement ve, StyleValues targetValuesToRead)
	{
		StyleValues result = default(StyleValues);
		IResolvedStyle resolvedStyle = ve.resolvedStyle;
		if (targetValuesToRead.m_StyleValues != null)
		{
			using List<StyleValue>.Enumerator enumerator = targetValuesToRead.m_StyleValues.m_Values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current.id)
				{
				case StylePropertyId.MarginLeft:
					result.marginLeft = resolvedStyle.marginLeft;
					break;
				case StylePropertyId.MarginTop:
					result.marginTop = resolvedStyle.marginTop;
					break;
				case StylePropertyId.MarginRight:
					result.marginRight = resolvedStyle.marginRight;
					break;
				case StylePropertyId.MarginBottom:
					result.marginBottom = resolvedStyle.marginBottom;
					break;
				case StylePropertyId.PaddingLeft:
					result.paddingLeft = resolvedStyle.paddingLeft;
					break;
				case StylePropertyId.PaddingTop:
					result.paddingTop = resolvedStyle.paddingTop;
					break;
				case StylePropertyId.PaddingRight:
					result.paddingRight = resolvedStyle.paddingRight;
					break;
				case StylePropertyId.PaddingBottom:
					result.paddingBottom = resolvedStyle.paddingBottom;
					break;
				case StylePropertyId.Left:
					result.left = resolvedStyle.left;
					break;
				case StylePropertyId.Top:
					result.top = resolvedStyle.top;
					break;
				case StylePropertyId.Right:
					result.right = resolvedStyle.right;
					break;
				case StylePropertyId.Bottom:
					result.bottom = resolvedStyle.bottom;
					break;
				case StylePropertyId.Width:
					result.width = resolvedStyle.width;
					break;
				case StylePropertyId.Height:
					result.height = resolvedStyle.height;
					break;
				case StylePropertyId.FlexGrow:
					result.flexGrow = resolvedStyle.flexGrow;
					break;
				case StylePropertyId.FlexShrink:
					result.flexShrink = resolvedStyle.flexShrink;
					break;
				case StylePropertyId.BorderLeftWidth:
					result.borderLeftWidth = resolvedStyle.borderLeftWidth;
					break;
				case StylePropertyId.BorderTopWidth:
					result.borderTopWidth = resolvedStyle.borderTopWidth;
					break;
				case StylePropertyId.BorderRightWidth:
					result.borderRightWidth = resolvedStyle.borderRightWidth;
					break;
				case StylePropertyId.BorderBottomWidth:
					result.borderBottomWidth = resolvedStyle.borderBottomWidth;
					break;
				case StylePropertyId.BorderTopLeftRadius:
					result.borderTopLeftRadius = resolvedStyle.borderTopLeftRadius;
					break;
				case StylePropertyId.BorderTopRightRadius:
					result.borderTopRightRadius = resolvedStyle.borderTopRightRadius;
					break;
				case StylePropertyId.BorderBottomRightRadius:
					result.borderBottomRightRadius = resolvedStyle.borderBottomRightRadius;
					break;
				case StylePropertyId.BorderBottomLeftRadius:
					result.borderBottomLeftRadius = resolvedStyle.borderBottomLeftRadius;
					break;
				case StylePropertyId.Color:
					result.color = resolvedStyle.color;
					break;
				case StylePropertyId.BackgroundColor:
					result.backgroundColor = resolvedStyle.backgroundColor;
					break;
				case StylePropertyId.BorderColor:
					result.borderColor = resolvedStyle.borderLeftColor;
					break;
				case StylePropertyId.UnityBackgroundImageTintColor:
					result.unityBackgroundImageTintColor = resolvedStyle.unityBackgroundImageTintColor;
					break;
				case StylePropertyId.Opacity:
					result.opacity = resolvedStyle.opacity;
					break;
				}
			}
		}
		return result;
	}

	ValueAnimation<StyleValues> ITransitionAnimations.Start(StyleValues to, int durationMs)
	{
		if (to.m_StyleValues == null)
		{
			to.Values();
		}
		return Start((VisualElement e) => ReadCurrentValues(e, to), to, durationMs);
	}

	private ValueAnimation<StyleValues> Start(Func<VisualElement, StyleValues> fromValueGetter, StyleValues to, int durationMs)
	{
		return StartAnimation(ValueAnimation<StyleValues>.Create(this, Lerp.Interpolate), fromValueGetter, to, durationMs, AssignStyleValues);
	}

	ValueAnimation<Rect> ITransitionAnimations.Layout(Rect to, int durationMs)
	{
		return experimental.animation.Start((VisualElement e) => new Rect(e.resolvedStyle.left, e.resolvedStyle.top, e.resolvedStyle.width, e.resolvedStyle.height), to, durationMs, delegate(VisualElement e, Rect c)
		{
			e.style.left = c.x;
			e.style.top = c.y;
			e.style.width = c.width;
			e.style.height = c.height;
		});
	}

	ValueAnimation<Vector2> ITransitionAnimations.TopLeft(Vector2 to, int durationMs)
	{
		return experimental.animation.Start((VisualElement e) => new Vector2(e.resolvedStyle.left, e.resolvedStyle.top), to, durationMs, delegate(VisualElement e, Vector2 c)
		{
			e.style.left = c.x;
			e.style.top = c.y;
		});
	}

	ValueAnimation<Vector2> ITransitionAnimations.Size(Vector2 to, int durationMs)
	{
		return experimental.animation.Start((VisualElement e) => e.layout.size, to, durationMs, delegate(VisualElement e, Vector2 c)
		{
			e.style.width = c.x;
			e.style.height = c.y;
		});
	}

	ValueAnimation<float> ITransitionAnimations.Scale(float to, int durationMs)
	{
		return experimental.animation.Start((VisualElement e) => e.transform.scale.x, to, durationMs, delegate(VisualElement e, float c)
		{
			e.transform.scale = new Vector3(c, c, c);
		});
	}

	ValueAnimation<Vector3> ITransitionAnimations.Position(Vector3 to, int durationMs)
	{
		return experimental.animation.Start((VisualElement e) => e.transform.position, to, durationMs, delegate(VisualElement e, Vector3 c)
		{
			e.transform.position = c;
		});
	}

	ValueAnimation<Quaternion> ITransitionAnimations.Rotation(Quaternion to, int durationMs)
	{
		return experimental.animation.Start((VisualElement e) => e.transform.rotation, to, durationMs, delegate(VisualElement e, Quaternion c)
		{
			e.transform.rotation = c;
		});
	}

	public void SetBinding(BindingId bindingId, Binding binding)
	{
		RegisterBinding(bindingId, binding);
	}

	public Binding GetBinding(BindingId bindingId)
	{
		Binding binding;
		return TryGetBinding(bindingId, out binding) ? binding : null;
	}

	public bool TryGetBinding(BindingId bindingId, out Binding binding)
	{
		if (DataBindingUtility.TryGetBinding(this, in bindingId, out var bindingInfo))
		{
			binding = bindingInfo.binding;
			return true;
		}
		binding = null;
		return false;
	}

	public IEnumerable<BindingInfo> GetBindingInfos()
	{
		List<BindingInfo> bindingInfos;
		using (CollectionPool<List<BindingInfo>, BindingInfo>.Get(out bindingInfos))
		{
			DataBindingUtility.GetBindingsForElement(this, bindingInfos);
			foreach (BindingInfo item in bindingInfos)
			{
				yield return item;
			}
		}
	}

	public void GetBindingInfos(List<BindingInfo> bindingInfos)
	{
		DataBindingUtility.GetBindingsForElement(this, bindingInfos);
	}

	public bool HasBinding(BindingId bindingId)
	{
		Binding binding;
		return TryGetBinding(bindingId, out binding);
	}

	public void ClearBinding(BindingId bindingId)
	{
		SetBinding(bindingId, null);
		bindings?.RemoveAll((Binding b) => (BindingId)b.property == bindingId);
	}

	public void ClearBindings()
	{
		DataBindingManager.CreateClearAllBindingsRequest(this);
		bindings?.Clear();
		if (panel != null)
		{
			ProcessBindingRequests();
		}
	}

	public DataSourceContext GetHierarchicalDataSourceContext()
	{
		VisualElement visualElement = this;
		PropertyPath pathToAppend = default(PropertyPath);
		while (visualElement != null)
		{
			if (!visualElement.isDataSourcePathEmpty)
			{
				pathToAppend = PropertyPath.Combine(visualElement.dataSourcePath, in pathToAppend);
			}
			if (visualElement.dataSource != null)
			{
				object obj = visualElement.dataSource;
				return new DataSourceContext(obj, in pathToAppend);
			}
			visualElement = visualElement.hierarchy.parent;
		}
		return new DataSourceContext(null, in pathToAppend);
	}

	public DataSourceContext GetDataSourceContext(BindingId bindingId)
	{
		if (TryGetDataSourceContext(bindingId, out var context))
		{
			return context;
		}
		throw new ArgumentOutOfRangeException("bindingId", $"[UI Toolkit] could not get binding with id '{bindingId}' on the element.");
	}

	public bool TryGetDataSourceContext(BindingId bindingId, out DataSourceContext context)
	{
		Binding binding = GetBinding(bindingId);
		Binding binding2 = binding;
		Binding binding3 = binding2;
		if (binding3 != null)
		{
			if (!(binding3 is IDataSourceProvider { dataSource: var obj } dataSourceProvider))
			{
				goto IL_00ae;
			}
			if (obj == null)
			{
				if (dataSourceProvider.dataSourcePath.IsEmpty)
				{
					goto IL_00ae;
				}
				IDataSourceProvider dataSourceProvider2 = dataSourceProvider;
				DataSourceContext hierarchicalDataSourceContext = GetHierarchicalDataSourceContext();
				context = new DataSourceContext(hierarchicalDataSourceContext.dataSource, PropertyPath.Combine(hierarchicalDataSourceContext.dataSourcePath, dataSourceProvider2.dataSourcePath));
			}
			else
			{
				context = new DataSourceContext(dataSourceProvider.dataSource, dataSourceProvider.dataSourcePath);
			}
			goto IL_00bd;
		}
		context = default(DataSourceContext);
		return false;
		IL_00ae:
		context = GetHierarchicalDataSourceContext();
		goto IL_00bd;
		IL_00bd:
		return true;
	}

	public bool TryGetLastBindingToUIResult(in BindingId bindingId, out BindingResult result)
	{
		if (elementPanel == null)
		{
			result = default(BindingResult);
			return false;
		}
		DataBindingManager dataBindingManager = elementPanel.dataBindingManager;
		if (dataBindingManager.TryGetBindingData(this, in bindingId, out var bindingData) && dataBindingManager.TryGetLastUIBindingResult(bindingData, out result))
		{
			return true;
		}
		result = default(BindingResult);
		return false;
	}

	public bool TryGetLastBindingToSourceResult(in BindingId bindingId, out BindingResult result)
	{
		if (elementPanel == null)
		{
			result = default(BindingResult);
			return false;
		}
		DataBindingManager dataBindingManager = elementPanel.dataBindingManager;
		if (dataBindingManager.TryGetBindingData(this, in bindingId, out var bindingData) && dataBindingManager.TryGetLastSourceBindingResult(bindingData, out result))
		{
			return true;
		}
		result = default(BindingResult);
		return false;
	}

	private void RegisterBinding(BindingId bindingId, Binding binding)
	{
		AddBindingRequest(bindingId, binding);
		if (panel != null)
		{
			ProcessBindingRequests();
		}
	}

	internal void AddBindingRequest(BindingId bindingId, Binding binding)
	{
		DataBindingManager.CreateBindingRequest(this, in bindingId, binding);
	}

	private void ProcessBindingRequests()
	{
		Assert.IsFalse(elementPanel == null, null);
		if (DataBindingManager.AnyPendingBindingRequests(this))
		{
			IncrementVersion(VersionChangeType.BindingRegistration);
		}
	}

	private void CreateBindingRequests()
	{
		BaseVisualElementPanel baseVisualElementPanel = elementPanel;
		Assert.IsFalse(baseVisualElementPanel == null, null);
		baseVisualElementPanel.dataBindingManager.TransferBindingRequests(this);
	}

	private void TrackSource(object previous, object current)
	{
		DataBindingManager dataBindingManager = elementPanel?.dataBindingManager;
		if (dataBindingManager != null && (m_Flags & VisualElementFlags.DetachedDataSource) != VisualElementFlags.DetachedDataSource)
		{
			elementPanel?.dataBindingManager.TrackDataSource(previous, current);
		}
	}

	private void DetachDataSource()
	{
		TrackSource(dataSource, null);
		m_Flags |= VisualElementFlags.DetachedDataSource;
	}

	private void AttachDataSource()
	{
		m_Flags &= ~VisualElementFlags.DetachedDataSource;
		TrackSource(null, dataSource);
	}

	private void DirtyNextParentWithEventInterests()
	{
		if (m_CachedNextParentWithEventInterests != null && m_NextParentCachedVersion == m_CachedNextParentWithEventInterests.m_NextParentRequiredVersion)
		{
			m_CachedNextParentWithEventInterests.m_NextParentRequiredVersion = ++s_NextParentVersion;
		}
	}

	internal void SetAsNextParentWithEventInterests()
	{
		if (m_NextParentRequiredVersion == 0)
		{
			m_NextParentRequiredVersion = ++s_NextParentVersion;
			if (m_CachedNextParentWithEventInterests != null && m_NextParentCachedVersion == m_CachedNextParentWithEventInterests.m_NextParentRequiredVersion)
			{
				m_CachedNextParentWithEventInterests.m_NextParentRequiredVersion = ++s_NextParentVersion;
			}
		}
	}

	internal bool GetCachedNextParentWithEventInterests(out VisualElement nextParent)
	{
		nextParent = m_CachedNextParentWithEventInterests;
		return nextParent != null && nextParent.m_NextParentRequiredVersion == m_NextParentCachedVersion;
	}

	private void PropagateCachedNextParentWithEventInterests(VisualElement nextParent, VisualElement stopParent)
	{
		for (VisualElement visualElement = this; visualElement != stopParent; visualElement = visualElement.hierarchy.parent)
		{
			visualElement.m_CachedNextParentWithEventInterests = nextParent;
			visualElement.m_NextParentCachedVersion = nextParent.m_NextParentRequiredVersion;
		}
	}

	internal void AddEventCallbackCategories(int eventCategories, TrickleDown trickleDown)
	{
		if (trickleDown == TrickleDown.TrickleDown)
		{
			m_TrickleDownEventCallbackCategories |= eventCategories;
		}
		else
		{
			m_BubbleUpEventCallbackCategories |= eventCategories;
		}
		UpdateEventInterestSelfCategories();
	}

	internal void RemoveEventCallbackCategories(int eventCategories, TrickleDown trickleDown)
	{
		if (trickleDown == TrickleDown.TrickleDown)
		{
			m_TrickleDownEventCallbackCategories &= ~eventCategories;
		}
		else
		{
			m_BubbleUpEventCallbackCategories &= ~eventCategories;
		}
		UpdateEventInterestSelfCategories();
	}

	private void UpdateEventInterestSelfCategories()
	{
		int num = m_TrickleDownHandleEventCategories | m_BubbleUpHandleEventCategories | m_TrickleDownEventCallbackCategories | m_BubbleUpEventCallbackCategories;
		if (m_EventInterestSelfCategories != num)
		{
			int num2 = m_EventInterestSelfCategories ^ num;
			if ((num2 & -5537) != 0)
			{
				SetAsNextParentWithEventInterests();
				IncrementVersion(VersionChangeType.EventCallbackCategories);
			}
			else
			{
				m_CachedEventInterestParentCategories |= num;
			}
			m_EventInterestSelfCategories = num;
		}
	}

	private void UpdateEventInterestParentCategories()
	{
		m_CachedEventInterestParentCategories = m_EventInterestSelfCategories;
		VisualElement visualElement = nextParentWithEventInterests;
		if (visualElement == null)
		{
			return;
		}
		m_CachedEventInterestParentCategories |= visualElement.eventInterestParentCategories;
		if (hierarchy.parent != null)
		{
			for (VisualElement visualElement2 = hierarchy.parent; visualElement2 != visualElement; visualElement2 = visualElement2.hierarchy.parent)
			{
				visualElement2.m_CachedEventInterestParentCategories = m_CachedEventInterestParentCategories;
				visualElement2.isEventInterestParentCategoriesDirty = false;
			}
		}
	}

	internal bool HasParentEventInterests(EventCategory eventCategory)
	{
		return (eventInterestParentCategories & (1 << (int)eventCategory)) != 0;
	}

	internal bool HasParentEventInterests(int eventCategories)
	{
		return (eventInterestParentCategories & eventCategories) != 0;
	}

	internal bool HasSelfEventInterests(EventCategory eventCategory)
	{
		return (m_EventInterestSelfCategories & (1 << (int)eventCategory)) != 0;
	}

	internal bool HasSelfEventInterests(int eventCategories)
	{
		return (m_EventInterestSelfCategories & eventCategories) != 0;
	}

	internal bool HasTrickleDownEventInterests(int eventCategories)
	{
		return ((m_TrickleDownHandleEventCategories | m_TrickleDownEventCallbackCategories) & eventCategories) != 0;
	}

	internal bool HasBubbleUpEventInterests(int eventCategories)
	{
		return ((m_BubbleUpHandleEventCategories | m_BubbleUpEventCallbackCategories) & eventCategories) != 0;
	}

	internal bool HasTrickleDownEventCallbacks(int eventCategories)
	{
		return (m_TrickleDownEventCallbackCategories & eventCategories) != 0;
	}

	internal bool HasBubbleUpEventCallbacks(int eventCategories)
	{
		return (m_BubbleUpEventCallbackCategories & eventCategories) != 0;
	}

	internal bool HasTrickleDownHandleEvent(EventCategory eventCategory)
	{
		return (m_TrickleDownHandleEventCategories & (1 << (int)eventCategory)) != 0;
	}

	internal bool HasTrickleDownHandleEvent(int eventCategories)
	{
		return (m_TrickleDownHandleEventCategories & eventCategories) != 0;
	}

	internal bool HasBubbleUpHandleEvent(EventCategory eventCategory)
	{
		return (m_BubbleUpHandleEventCategories & (1 << (int)eventCategory)) != 0;
	}

	internal bool HasBubbleUpHandleEvent(int eventCategories)
	{
		return (m_BubbleUpHandleEventCategories & eventCategories) != 0;
	}

	internal bool ShouldClip()
	{
		return computedStyle.overflow != OverflowInternal.Visible && !disableClipping;
	}

	public void Add(VisualElement child)
	{
		if (child != null)
		{
			VisualElement visualElement = contentContainer;
			if (visualElement == null)
			{
				throw new InvalidOperationException("You can't add directly to this VisualElement. Use hierarchy.Add() if you know what you're doing.");
			}
			if (visualElement == this)
			{
				hierarchy.Add(child);
			}
			else
			{
				visualElement?.Add(child);
			}
			child.m_LogicalParent = this;
		}
	}

	internal void Add(VisualElement child, bool ignoreContentContainer)
	{
		if (ignoreContentContainer)
		{
			hierarchy.Add(child);
		}
		else
		{
			Add(child);
		}
	}

	public void Insert(int index, VisualElement element)
	{
		if (element != null)
		{
			if (contentContainer == this)
			{
				hierarchy.Insert(index, element);
			}
			else
			{
				contentContainer?.Insert(index, element);
			}
			element.m_LogicalParent = this;
		}
	}

	internal void Insert(int index, VisualElement element, bool ignoreContentContainer)
	{
		if (ignoreContentContainer)
		{
			hierarchy.Insert(index, element);
		}
		else
		{
			Insert(index, element);
		}
	}

	public void Remove(VisualElement element)
	{
		if (contentContainer == this)
		{
			hierarchy.Remove(element);
		}
		else
		{
			contentContainer?.Remove(element);
		}
	}

	public void RemoveAt(int index)
	{
		if (contentContainer == this)
		{
			hierarchy.RemoveAt(index);
		}
		else
		{
			contentContainer?.RemoveAt(index);
		}
	}

	public void Clear()
	{
		if (contentContainer == this)
		{
			hierarchy.Clear();
		}
		else
		{
			contentContainer?.Clear();
		}
	}

	public VisualElement ElementAt(int index)
	{
		return this[index];
	}

	internal int ChildCount(bool ignoreContentContainer)
	{
		if (ignoreContentContainer)
		{
			return hierarchy.childCount;
		}
		return childCount;
	}

	public int IndexOf(VisualElement element)
	{
		if (contentContainer == this)
		{
			return hierarchy.IndexOf(element);
		}
		return contentContainer?.IndexOf(element) ?? (-1);
	}

	internal int IndexOf(VisualElement element, bool ignoreContentContainer)
	{
		if (ignoreContentContainer)
		{
			return hierarchy.IndexOf(element);
		}
		return IndexOf(element);
	}

	internal VisualElement ElementAtTreePath(List<int> childIndexes)
	{
		VisualElement visualElement = this;
		foreach (int childIndex in childIndexes)
		{
			if (childIndex >= 0 && childIndex < visualElement.hierarchy.childCount)
			{
				visualElement = visualElement.hierarchy[childIndex];
				continue;
			}
			return null;
		}
		return visualElement;
	}

	internal bool FindElementInTree(VisualElement element, List<int> outChildIndexes)
	{
		VisualElement visualElement = element;
		for (VisualElement visualElement2 = visualElement.hierarchy.parent; visualElement2 != null; visualElement2 = visualElement2.hierarchy.parent)
		{
			outChildIndexes.Insert(0, visualElement2.hierarchy.IndexOf(visualElement));
			if (visualElement2 == this)
			{
				return true;
			}
			visualElement = visualElement2;
		}
		outChildIndexes.Clear();
		return false;
	}

	public IEnumerable<VisualElement> Children()
	{
		if (contentContainer == this)
		{
			return hierarchy.Children();
		}
		return contentContainer?.Children() ?? s_EmptyList;
	}

	public void Sort(Comparison<VisualElement> comp)
	{
		if (contentContainer == this)
		{
			hierarchy.Sort(comp);
		}
		else
		{
			contentContainer?.Sort(comp);
		}
	}

	public void BringToFront()
	{
		if (hierarchy.parent != null)
		{
			hierarchy.parent.hierarchy.BringToFront(this);
		}
	}

	public void SendToBack()
	{
		if (hierarchy.parent != null)
		{
			hierarchy.parent.hierarchy.SendToBack(this);
		}
	}

	public void PlaceBehind(VisualElement sibling)
	{
		if (sibling == null)
		{
			throw new ArgumentNullException("sibling");
		}
		if (hierarchy.parent == null || sibling.hierarchy.parent != hierarchy.parent)
		{
			throw new ArgumentException("VisualElements are not siblings");
		}
		hierarchy.parent.hierarchy.PlaceBehind(this, sibling);
	}

	public void PlaceInFront(VisualElement sibling)
	{
		if (sibling == null)
		{
			throw new ArgumentNullException("sibling");
		}
		if (hierarchy.parent == null || sibling.hierarchy.parent != hierarchy.parent)
		{
			throw new ArgumentException("VisualElements are not siblings");
		}
		hierarchy.parent.hierarchy.PlaceInFront(this, sibling);
	}

	public void RemoveFromHierarchy()
	{
		if (hierarchy.parent != null)
		{
			hierarchy.parent.hierarchy.Remove(this);
		}
	}

	public T GetFirstOfType<T>() where T : class
	{
		if (this is T result)
		{
			return result;
		}
		return GetFirstAncestorOfType<T>();
	}

	public T GetFirstAncestorOfType<T>() where T : class
	{
		for (VisualElement visualElement = hierarchy.parent; visualElement != null; visualElement = visualElement.hierarchy.parent)
		{
			if (visualElement is T result)
			{
				return result;
			}
		}
		return null;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal VisualElement GetFirstAncestorWhere(Predicate<VisualElement> predicate)
	{
		for (VisualElement visualElement = hierarchy.parent; visualElement != null; visualElement = visualElement.hierarchy.parent)
		{
			if (predicate(visualElement))
			{
				return visualElement;
			}
		}
		return null;
	}

	public bool Contains(VisualElement child)
	{
		while (child != null)
		{
			if (child.hierarchy.parent == this)
			{
				return true;
			}
			child = child.hierarchy.parent;
		}
		return false;
	}

	private void GatherAllChildren(List<VisualElement> elements)
	{
		if (m_Children.Count > 0)
		{
			int i = elements.Count;
			elements.AddRange(m_Children);
			for (; i < elements.Count; i++)
			{
				VisualElement visualElement = elements[i];
				elements.AddRange(visualElement.m_Children);
			}
		}
	}

	public VisualElement FindCommonAncestor(VisualElement other)
	{
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		if (panel != other.panel)
		{
			return null;
		}
		VisualElement visualElement = this;
		int num = 0;
		while (visualElement != null)
		{
			num++;
			visualElement = visualElement.hierarchy.parent;
		}
		VisualElement visualElement2 = other;
		int num2 = 0;
		while (visualElement2 != null)
		{
			num2++;
			visualElement2 = visualElement2.hierarchy.parent;
		}
		visualElement = this;
		visualElement2 = other;
		while (num > num2)
		{
			num--;
			visualElement = visualElement.hierarchy.parent;
		}
		while (num2 > num)
		{
			num2--;
			visualElement2 = visualElement2.hierarchy.parent;
		}
		while (visualElement != visualElement2)
		{
			visualElement = visualElement.hierarchy.parent;
			visualElement2 = visualElement2.hierarchy.parent;
		}
		return visualElement;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal VisualElement GetRoot()
	{
		if (panel != null)
		{
			return panel.visualTree;
		}
		VisualElement visualElement = this;
		while (visualElement.m_PhysicalParent != null)
		{
			visualElement = visualElement.m_PhysicalParent;
		}
		return visualElement;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal VisualElement GetRootVisualContainer()
	{
		VisualElement result = null;
		for (VisualElement visualElement = this; visualElement != null; visualElement = visualElement.hierarchy.parent)
		{
			if (visualElement.isRootVisualContainer)
			{
				result = visualElement;
			}
		}
		return result;
	}

	internal VisualElement GetNextElementDepthFirst()
	{
		if (m_Children.Count > 0)
		{
			return m_Children[0];
		}
		VisualElement physicalParent = m_PhysicalParent;
		VisualElement visualElement = this;
		while (physicalParent != null)
		{
			int i;
			for (i = 0; i < physicalParent.m_Children.Count && physicalParent.m_Children[i] != visualElement; i++)
			{
			}
			if (i < physicalParent.m_Children.Count - 1)
			{
				return physicalParent.m_Children[i + 1];
			}
			visualElement = physicalParent;
			physicalParent = physicalParent.m_PhysicalParent;
		}
		return null;
	}

	internal VisualElement GetPreviousElementDepthFirst()
	{
		if (m_PhysicalParent != null)
		{
			int i;
			for (i = 0; i < m_PhysicalParent.m_Children.Count && m_PhysicalParent.m_Children[i] != this; i++)
			{
			}
			if (i > 0)
			{
				VisualElement visualElement = m_PhysicalParent.m_Children[i - 1];
				while (visualElement.m_Children.Count > 0)
				{
					visualElement = visualElement.m_Children[visualElement.m_Children.Count - 1];
				}
				return visualElement;
			}
			return m_PhysicalParent;
		}
		return null;
	}

	internal VisualElement RetargetElement(VisualElement retargetAgainst)
	{
		if (retargetAgainst == null)
		{
			return this;
		}
		VisualElement visualElement = retargetAgainst.m_PhysicalParent ?? retargetAgainst;
		while (visualElement.m_PhysicalParent != null && !visualElement.isCompositeRoot)
		{
			visualElement = visualElement.m_PhysicalParent;
		}
		VisualElement result = this;
		VisualElement physicalParent = m_PhysicalParent;
		while (physicalParent != null)
		{
			physicalParent = physicalParent.m_PhysicalParent;
			if (physicalParent == visualElement)
			{
				return result;
			}
			if (physicalParent != null && physicalParent.isCompositeRoot)
			{
				result = physicalParent;
			}
		}
		return this;
	}

	internal void GetPivotedMatrixWithLayout(out Matrix4x4 result)
	{
		Vector3 vector = ResolveTransformOrigin();
		result = Matrix4x4.TRS(positionWithLayout + vector, ResolveRotation(), ResolveScale());
		TranslateMatrix34InPlace(ref result, -vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static float Min(float a, float b, float c, float d)
	{
		return Mathf.Min(Mathf.Min(a, b), Mathf.Min(c, d));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static float Max(float a, float b, float c, float d)
	{
		return Mathf.Max(Mathf.Max(a, b), Mathf.Max(c, d));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void TransformAlignedBoundsToParentSpace(ref Bounds bounds)
	{
		if (hasDefaultRotationAndScale)
		{
			bounds.center += positionWithLayout;
			return;
		}
		GetPivotedMatrixWithLayout(out var result);
		bounds = CalculateConservativeBounds(ref result, bounds);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void TransformAlignedRectToParentSpace(ref Rect rect)
	{
		if (hasDefaultRotationAndScale)
		{
			rect.position += (Vector2)positionWithLayout;
			return;
		}
		GetPivotedMatrixWithLayout(out var result);
		rect = CalculateConservativeRect(ref result, rect);
	}

	internal static Rect CalculateConservativeRect(ref Matrix4x4 matrix, Rect rect)
	{
		if (float.IsNaN(rect.height) | float.IsNaN(rect.width) | float.IsNaN(rect.x) | float.IsNaN(rect.y))
		{
			rect = new Rect(MultiplyMatrix44Point2(ref matrix, rect.position), MultiplyVector2(ref matrix, rect.size));
			OrderMinMaxRect(ref rect);
			return rect;
		}
		Vector2 vector = new Vector2(rect.xMin, rect.yMin);
		Vector2 vector2 = new Vector2(rect.xMax, rect.yMax);
		Vector2 vector3 = new Vector2(rect.xMax, rect.yMin);
		Vector2 vector4 = new Vector2(rect.xMin, rect.yMax);
		Vector3 vector5 = matrix.MultiplyPoint3x4(vector);
		Vector3 vector6 = matrix.MultiplyPoint3x4(vector2);
		Vector3 vector7 = matrix.MultiplyPoint3x4(vector3);
		Vector3 vector8 = matrix.MultiplyPoint3x4(vector4);
		Vector2 vector9 = new Vector2(Min(vector5.x, vector6.x, vector7.x, vector8.x), Min(vector5.y, vector6.y, vector7.y, vector8.y));
		Vector2 vector10 = new Vector2(Max(vector5.x, vector6.x, vector7.x, vector8.x), Max(vector5.y, vector6.y, vector7.y, vector8.y));
		return new Rect(vector9.x, vector9.y, vector10.x - vector9.x, vector10.y - vector9.y);
	}

	internal static Bounds CalculateConservativeBounds(ref Matrix4x4 matrix, Bounds bounds)
	{
		if (IsNaN(bounds.center) | IsNaN(bounds.extents))
		{
			bounds = new Bounds(matrix.MultiplyPoint3x4(bounds.center), matrix.MultiplyVector(bounds.size));
			OrderMinMaxBounds(ref bounds);
			return bounds;
		}
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		for (int i = 0; i < 8; i++)
		{
			Vector3 vector3 = new Vector3(((i & 1) != 0) ? max.x : min.x, ((i & 2) != 0) ? max.y : min.y, ((i & 4) != 0) ? max.z : min.z);
			vector3 = matrix.MultiplyPoint3x4(vector3);
			vector = ((i == 0) ? vector3 : Vector3.Min(vector, vector3));
			vector2 = ((i == 0) ? vector3 : Vector3.Max(vector2, vector3));
		}
		bounds.SetMinMax(vector, vector2);
		return bounds;
		static bool IsNaN(Vector3 v)
		{
			return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void TransformAlignedRect(ref Matrix4x4 matrix, ref Rect rect)
	{
		rect = CalculateConservativeRect(ref matrix, rect);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void TransformAlignedBounds(ref Matrix4x4 matrix, ref Bounds bounds)
	{
		bounds = CalculateConservativeBounds(ref matrix, bounds);
	}

	internal static void OrderMinMaxRect(ref Rect rect)
	{
		if (rect.width < 0f)
		{
			rect.x += rect.width;
			rect.width = 0f - rect.width;
		}
		if (rect.height < 0f)
		{
			rect.y += rect.height;
			rect.height = 0f - rect.height;
		}
	}

	internal static void OrderMinMaxBounds(ref Bounds bounds)
	{
		Vector3 extents = bounds.extents;
		bounds.extents = new Vector3(Mathf.Abs(extents.x), Mathf.Abs(extents.y), Mathf.Abs(extents.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector2 MultiplyMatrix44Point2(ref Matrix4x4 lhs, Vector2 point)
	{
		Vector2 result = default(Vector2);
		result.x = lhs.m00 * point.x + lhs.m01 * point.y + lhs.m03;
		result.y = lhs.m10 * point.x + lhs.m11 * point.y + lhs.m13;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector3 MultiplyMatrix44Point2ToPoint3(ref Matrix4x4 lhs, Vector2 point)
	{
		Vector3 result = default(Vector3);
		result.x = lhs.m00 * point.x + lhs.m01 * point.y + lhs.m03;
		result.y = lhs.m10 * point.x + lhs.m11 * point.y + lhs.m13;
		result.z = lhs.m20 * point.x + lhs.m21 * point.y + lhs.m23;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector2 MultiplyMatrix44Point3ToPoint2(ref Matrix4x4 lhs, Vector3 point)
	{
		Vector2 result = default(Vector2);
		result.x = lhs.m00 * point.x + lhs.m01 * point.y + lhs.m02 * point.z + lhs.m03;
		result.y = lhs.m10 * point.x + lhs.m11 * point.y + lhs.m12 * point.z + lhs.m13;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector2 MultiplyVector2(ref Matrix4x4 lhs, Vector2 vector)
	{
		Vector2 result = default(Vector2);
		result.x = lhs.m00 * vector.x + lhs.m01 * vector.y;
		result.y = lhs.m10 * vector.x + lhs.m11 * vector.y;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Rect MultiplyMatrix44Rect2(ref Matrix4x4 lhs, Rect r)
	{
		r.position = MultiplyMatrix44Point2(ref lhs, r.position);
		r.size = MultiplyVector2(ref lhs, r.size);
		return r;
	}

	internal static void MultiplyMatrix34(ref Matrix4x4 lhs, ref Matrix4x4 rhs, out Matrix4x4 res)
	{
		res.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20;
		res.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21;
		res.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22;
		res.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03;
		res.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20;
		res.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21;
		res.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22;
		res.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13;
		res.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20;
		res.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21;
		res.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22;
		res.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23;
		res.m30 = 0f;
		res.m31 = 0f;
		res.m32 = 0f;
		res.m33 = 1f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void TranslateMatrix34(ref Matrix4x4 lhs, Vector3 rhs, out Matrix4x4 res)
	{
		res = lhs;
		TranslateMatrix34InPlace(ref res, rhs);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void TranslateMatrix34InPlace(ref Matrix4x4 lhs, Vector3 rhs)
	{
		lhs.m03 += lhs.m00 * rhs.x + lhs.m01 * rhs.y + lhs.m02 * rhs.z;
		lhs.m13 += lhs.m10 * rhs.x + lhs.m11 * rhs.y + lhs.m12 * rhs.z;
		lhs.m23 += lhs.m20 * rhs.x + lhs.m21 * rhs.y + lhs.m22 * rhs.z;
	}

	IVisualElementScheduledItem IVisualElementScheduler.Execute(Action<TimerState> timerUpdateEvent)
	{
		TimerStateScheduledItem timerStateScheduledItem = new TimerStateScheduledItem(this, timerUpdateEvent)
		{
			timerUpdateStopCondition = ScheduledItem.OnceCondition
		};
		timerStateScheduledItem.Resume();
		return timerStateScheduledItem;
	}

	IVisualElementScheduledItem IVisualElementScheduler.Execute(Action updateEvent)
	{
		SimpleScheduledItem simpleScheduledItem = new SimpleScheduledItem(this, updateEvent)
		{
			timerUpdateStopCondition = ScheduledItem.OnceCondition
		};
		simpleScheduledItem.Resume();
		return simpleScheduledItem;
	}

	internal void AddStyleSheetPath(string sheetPath)
	{
		StyleSheet styleSheet = Panel.LoadResource(sheetPath, typeof(StyleSheet), scaledPixelsPerPoint_noChecks) as StyleSheet;
		if (styleSheet == null)
		{
			if (!s_InternalStyleSheetPath.IsMatch(sheetPath))
			{
				Debug.LogWarning($"Style sheet not found for path \"{sheetPath}\"");
			}
		}
		else
		{
			styleSheets.Add(styleSheet);
		}
	}

	internal bool HasStyleSheetPath(string sheetPath)
	{
		StyleSheet styleSheet = Panel.LoadResource(sheetPath, typeof(StyleSheet), scaledPixelsPerPoint_noChecks) as StyleSheet;
		if (styleSheet == null)
		{
			Debug.LogWarning($"Style sheet not found for path \"{sheetPath}\"");
			return false;
		}
		return styleSheets.Contains(styleSheet);
	}

	internal void RemoveStyleSheetPath(string sheetPath)
	{
		StyleSheet styleSheet = Panel.LoadResource(sheetPath, typeof(StyleSheet), scaledPixelsPerPoint_noChecks) as StyleSheet;
		if (styleSheet == null)
		{
			Debug.LogWarning($"Style sheet not found for path \"{sheetPath}\"");
		}
		else
		{
			styleSheets.Remove(styleSheet);
		}
	}

	internal StyleFloat ResolveLengthValue(Length length, bool isRow)
	{
		if (length.IsAuto())
		{
			return new StyleFloat(StyleKeyword.Auto);
		}
		if (length.IsNone())
		{
			return new StyleFloat(StyleKeyword.None);
		}
		if (length.unit != LengthUnit.Percent)
		{
			return new StyleFloat(length.value);
		}
		VisualElement visualElement = hierarchy.parent;
		if (visualElement == null)
		{
			return 0f;
		}
		float num = (isRow ? visualElement.resolvedStyle.width : visualElement.resolvedStyle.height);
		return length.value * num / 100f;
	}

	internal Vector3 ResolveTranslate()
	{
		Translate translate = computedStyle.translate;
		Length x = translate.x;
		float x2;
		if (x.unit == LengthUnit.Percent)
		{
			float width = resolvedStyle.width;
			x2 = (float.IsNaN(width) ? 0f : (width * x.value / 100f));
		}
		else
		{
			x2 = x.value;
			x2 = (float.IsNaN(x2) ? 0f : x2);
		}
		Length y = translate.y;
		float y2;
		if (y.unit == LengthUnit.Percent)
		{
			float height = resolvedStyle.height;
			y2 = (float.IsNaN(height) ? 0f : (height * y.value / 100f));
		}
		else
		{
			y2 = y.value;
			y2 = (float.IsNaN(y2) ? 0f : y2);
		}
		float z = translate.z;
		z = (float.IsNaN(z) ? 0f : z);
		return new Vector3(x2, y2, z);
	}

	internal Vector3 ResolveTransformOrigin()
	{
		TransformOrigin transformOrigin = computedStyle.transformOrigin;
		float num = float.NaN;
		Length x = transformOrigin.x;
		if (x.IsNone())
		{
			float width = resolvedStyle.width;
			num = (float.IsNaN(width) ? 0f : (width / 2f));
		}
		else if (x.unit == LengthUnit.Percent)
		{
			float width2 = resolvedStyle.width;
			num = (float.IsNaN(width2) ? 0f : (width2 * x.value / 100f));
		}
		else
		{
			num = x.value;
		}
		float num2 = float.NaN;
		Length y = transformOrigin.y;
		if (y.IsNone())
		{
			float height = resolvedStyle.height;
			num2 = (float.IsNaN(height) ? 0f : (height / 2f));
		}
		else if (y.unit == LengthUnit.Percent)
		{
			float height2 = resolvedStyle.height;
			num2 = (float.IsNaN(height2) ? 0f : (height2 * y.value / 100f));
		}
		else
		{
			num2 = y.value;
		}
		float z = transformOrigin.z;
		return new Vector3(num, num2, z);
	}

	private Quaternion ResolveRotation()
	{
		Rotate rotate = computedStyle.rotate;
		Vector3 axis = rotate.axis;
		if (float.IsNaN(rotate.angle.value) || float.IsNaN(axis.x) || float.IsNaN(axis.y) || float.IsNaN(axis.z))
		{
			rotate = Rotate.Initial();
		}
		return rotate.ToQuaternion();
	}

	private Vector3 ResolveScale()
	{
		Vector3 value = computedStyle.scale.value;
		BaseVisualElementPanel baseVisualElementPanel = elementPanel;
		if (baseVisualElementPanel != null && baseVisualElementPanel.isFlat)
		{
			value.z = 1f;
		}
		return (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z)) ? Vector3.one : value;
	}

	internal static TypeData GetOrCreateTypeData(Type t)
	{
		if (!s_TypeData.TryGetValue(t, out var value))
		{
			value = new TypeData(t);
			s_TypeData.Add(t, value);
		}
		return value;
	}
}
