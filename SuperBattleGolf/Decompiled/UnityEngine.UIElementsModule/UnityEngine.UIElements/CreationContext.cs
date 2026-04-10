using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public struct CreationContext : IEquatable<CreationContext>
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct AttributeOverrideRange
	{
		internal readonly VisualTreeAsset sourceAsset;

		internal readonly List<TemplateAsset.AttributeOverride> attributeOverrides;

		public AttributeOverrideRange(VisualTreeAsset sourceAsset, List<TemplateAsset.AttributeOverride> attributeOverrides)
		{
			this.sourceAsset = sourceAsset;
			this.attributeOverrides = attributeOverrides;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct SerializedDataOverrideRange
	{
		internal readonly VisualTreeAsset sourceAsset;

		internal readonly int templateId;

		internal readonly List<TemplateAsset.UxmlSerializedDataOverride> attributeOverrides;

		public SerializedDataOverrideRange(VisualTreeAsset sourceAsset, List<TemplateAsset.UxmlSerializedDataOverride> attributeOverrides, int templateId)
		{
			this.sourceAsset = sourceAsset;
			this.attributeOverrides = attributeOverrides;
			this.templateId = templateId;
		}
	}

	public static readonly CreationContext Default = default(CreationContext);

	public VisualElement target { get; private set; }

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal List<int> veaIdsPath
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get;
		private set; }

	internal TemplateAsset templateAsset { get; private set; }

	public VisualTreeAsset visualTreeAsset { get; private set; }

	public Dictionary<string, VisualElement> slotInsertionPoints { get; private set; }

	internal List<AttributeOverrideRange> attributeOverrides { get; private set; }

	internal List<SerializedDataOverrideRange> serializedDataOverrides { get; private set; }

	internal List<string> namesPath { get; private set; }

	internal bool hasOverrides
	{
		get
		{
			List<AttributeOverrideRange> list = attributeOverrides;
			int result;
			if (list == null || list.Count <= 0)
			{
				List<SerializedDataOverrideRange> list2 = serializedDataOverrides;
				result = ((list2 != null && list2.Count > 0) ? 1 : 0);
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal CreationContext(VisualTreeAsset vta)
		: this(null, vta, null)
	{
	}

	internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints)
		: this(slotInsertionPoints, null, null, null)
	{
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints, List<AttributeOverrideRange> attributeOverrides)
		: this(slotInsertionPoints, attributeOverrides, null, null)
	{
	}

	internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints, VisualTreeAsset vta, VisualElement target)
		: this(slotInsertionPoints, null, vta, target)
	{
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints, List<AttributeOverrideRange> attributeOverrides, VisualTreeAsset vta, VisualElement target)
		: this(slotInsertionPoints, attributeOverrides, null, vta, target, null, null, null)
	{
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints, List<AttributeOverrideRange> attributeOverrides, List<SerializedDataOverrideRange> serializedDataOverrides, VisualTreeAsset vta, VisualElement target, List<int> veaIdsPath, List<string> namesPath, TemplateAsset ta)
	{
		this.target = target;
		this.slotInsertionPoints = slotInsertionPoints;
		this.attributeOverrides = attributeOverrides;
		this.serializedDataOverrides = serializedDataOverrides;
		visualTreeAsset = vta;
		this.namesPath = namesPath;
		this.veaIdsPath = veaIdsPath;
		templateAsset = ta;
	}

	public override bool Equals(object obj)
	{
		return obj is CreationContext && Equals((CreationContext)obj);
	}

	public bool Equals(CreationContext other)
	{
		return EqualityComparer<VisualElement>.Default.Equals(target, other.target) && EqualityComparer<VisualTreeAsset>.Default.Equals(visualTreeAsset, other.visualTreeAsset) && EqualityComparer<Dictionary<string, VisualElement>>.Default.Equals(slotInsertionPoints, other.slotInsertionPoints);
	}

	public override int GetHashCode()
	{
		int num = -2123482148;
		num = num * -1521134295 + EqualityComparer<VisualElement>.Default.GetHashCode(target);
		num = num * -1521134295 + EqualityComparer<VisualTreeAsset>.Default.GetHashCode(visualTreeAsset);
		return num * -1521134295 + EqualityComparer<Dictionary<string, VisualElement>>.Default.GetHashCode(slotInsertionPoints);
	}

	public static bool operator ==(CreationContext context1, CreationContext context2)
	{
		return context1.Equals(context2);
	}

	public static bool operator !=(CreationContext context1, CreationContext context2)
	{
		return !(context1 == context2);
	}
}
