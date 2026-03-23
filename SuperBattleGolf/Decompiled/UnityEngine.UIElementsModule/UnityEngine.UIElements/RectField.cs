using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements;

[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
public class RectField : BaseCompositeField<Rect, FloatField, float>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseCompositeField<Rect, FloatField, float>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
	{
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseCompositeField<Rect, FloatField, float>.UxmlSerializedData.Register();
		}

		public override object CreateInstance()
		{
			return new RectField();
		}

		void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
		{
			int foundAttributeCounter = 0;
			float x = UxmlUtility.TryParseFloatAttribute("x", bag, ref foundAttributeCounter);
			float y = UxmlUtility.TryParseFloatAttribute("y", bag, ref foundAttributeCounter);
			float width = UxmlUtility.TryParseFloatAttribute("w", bag, ref foundAttributeCounter);
			float height = UxmlUtility.TryParseFloatAttribute("h", bag, ref foundAttributeCounter);
			if (foundAttributeCounter > 0)
			{
				base.Value = new Rect(x, y, width, height);
				handledAttributes.Add("value");
				if (bag is UxmlAsset uxmlAsset)
				{
					uxmlAsset.RemoveAttribute("x");
					uxmlAsset.RemoveAttribute("y");
					uxmlAsset.RemoveAttribute("w");
					uxmlAsset.RemoveAttribute("h");
					uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
				}
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<RectField, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseField<Rect>.UxmlTraits
	{
		private UxmlFloatAttributeDescription m_XValue = new UxmlFloatAttributeDescription
		{
			name = "x"
		};

		private UxmlFloatAttributeDescription m_YValue = new UxmlFloatAttributeDescription
		{
			name = "y"
		};

		private UxmlFloatAttributeDescription m_WValue = new UxmlFloatAttributeDescription
		{
			name = "w"
		};

		private UxmlFloatAttributeDescription m_HValue = new UxmlFloatAttributeDescription
		{
			name = "h"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			RectField rectField = (RectField)ve;
			rectField.SetValueWithoutNotify(new Rect(m_XValue.GetValueFromBag(bag, cc), m_YValue.GetValueFromBag(bag, cc), m_WValue.GetValueFromBag(bag, cc), m_HValue.GetValueFromBag(bag, cc)));
		}
	}

	public new static readonly string ussClassName = "unity-rect-field";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	internal override FieldDescription[] DescribeFields()
	{
		return new FieldDescription[4]
		{
			new FieldDescription("X", "unity-x-input", (Rect r) => r.x, delegate(ref Rect r, float v)
			{
				r.x = v;
			}),
			new FieldDescription("Y", "unity-y-input", (Rect r) => r.y, delegate(ref Rect r, float v)
			{
				r.y = v;
			}),
			new FieldDescription("W", "unity-width-input", (Rect r) => r.width, delegate(ref Rect r, float v)
			{
				r.width = v;
			}),
			new FieldDescription("H", "unity-height-input", (Rect r) => r.height, delegate(ref Rect r, float v)
			{
				r.height = v;
			})
		};
	}

	public RectField()
		: this(null)
	{
	}

	public RectField(string label)
		: base(label, 2)
	{
		AddToClassList(ussClassName);
		AddToClassList(BaseCompositeField<Rect, FloatField, float>.twoLinesVariantUssClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
	}
}
