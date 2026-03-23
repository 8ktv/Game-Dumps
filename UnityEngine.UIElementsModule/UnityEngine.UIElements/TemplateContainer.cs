using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Serialization;

namespace UnityEngine.UIElements;

[HideInInspector]
[UxmlElement("Instance")]
public class TemplateContainer : BindableElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BindableElement.UxmlSerializedData
	{
		[SerializeField]
		private VisualTreeAsset template;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags template_UxmlAttributeFlags;

		[SerializeField]
		[FormerlySerializedAs("template")]
		[UxmlAttribute("template")]
		[UxmlAttributeBindingPath("templateId")]
		[HideInInspector]
		private string templateId;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags templateId_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[2]
			{
				new UxmlAttributeNames("template", "template", null),
				new UxmlAttributeNames("templateId", "template", null, "template")
			});
		}

		public override object CreateInstance()
		{
			return new TemplateContainer();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			TemplateContainer templateContainer = (TemplateContainer)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(template_UxmlAttributeFlags))
			{
				templateContainer.templateSource = template;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(templateId_UxmlAttributeFlags))
			{
				templateContainer.templateId = templateId;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<TemplateContainer, UxmlTraits>
	{
		internal const string k_ElementName = "Instance";

		public override string uxmlName => "Instance";

		public override string uxmlQualifiedName => uxmlNamespace + "." + uxmlName;
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BindableElement.UxmlTraits
	{
		internal const string k_TemplateAttributeName = "template";

		private UxmlStringAttributeDescription m_Template = new UxmlStringAttributeDescription
		{
			name = "template",
			use = UxmlAttributeDescription.Use.Required
		};

		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield break;
			}
		}

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			TemplateContainer templateContainer = (TemplateContainer)ve;
			templateContainer.templateId = m_Template.GetValueFromBag(bag, cc);
			VisualTreeAsset visualTreeAsset = cc.visualTreeAsset?.ResolveTemplate(templateContainer.templateId);
			if (visualTreeAsset == null)
			{
				templateContainer.Add(new Label($"Unknown Template: '{templateContainer.templateId}'"));
			}
			else
			{
				List<TemplateAsset.AttributeOverride> list = (bag as TemplateAsset)?.attributeOverrides;
				List<CreationContext.AttributeOverrideRange> list2 = cc.attributeOverrides;
				if (list != null)
				{
					if (list2 == null)
					{
						list2 = new List<CreationContext.AttributeOverrideRange>();
					}
					list2.Add(new CreationContext.AttributeOverrideRange(cc.visualTreeAsset, list));
				}
				templateContainer.templateSource = visualTreeAsset;
				visualTreeAsset.CloneTree(ve, new CreationContext(cc.slotInsertionPoints, list2));
			}
			if (visualTreeAsset == null)
			{
				Debug.LogErrorFormat("Could not resolve template with name '{0}'", templateContainer.templateId);
			}
		}
	}

	internal static readonly BindingId templateIdProperty = "templateId";

	internal static readonly BindingId templateSourceProperty = "templateSource";

	private VisualElement m_ContentContainer;

	private VisualTreeAsset m_TemplateSource;

	[CreateProperty(ReadOnly = true)]
	public string templateId { get; private set; }

	[CreateProperty(ReadOnly = true)]
	public VisualTreeAsset templateSource
	{
		get
		{
			return m_TemplateSource;
		}
		internal set
		{
			m_TemplateSource = value;
		}
	}

	public override VisualElement contentContainer => m_ContentContainer;

	public TemplateContainer()
		: this(null)
	{
	}

	public TemplateContainer(string templateId)
		: this(templateId, null)
	{
	}

	internal TemplateContainer(string templateId, VisualTreeAsset templateSource)
	{
		this.templateId = templateId;
		this.templateSource = templateSource;
		m_ContentContainer = this;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetContentContainer(VisualElement content)
	{
		m_ContentContainer = content;
	}
}
