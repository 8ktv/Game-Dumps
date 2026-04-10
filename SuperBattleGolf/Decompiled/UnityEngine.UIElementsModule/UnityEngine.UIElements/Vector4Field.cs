using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements;

[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
public class Vector4Field : BaseCompositeField<Vector4, FloatField, float>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseCompositeField<Vector4, FloatField, float>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
	{
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
		}

		public override object CreateInstance()
		{
			return new Vector4Field();
		}

		void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
		{
			int foundAttributeCounter = 0;
			float x = UxmlUtility.TryParseFloatAttribute("x", bag, ref foundAttributeCounter);
			float y = UxmlUtility.TryParseFloatAttribute("y", bag, ref foundAttributeCounter);
			float z = UxmlUtility.TryParseFloatAttribute("z", bag, ref foundAttributeCounter);
			float w = UxmlUtility.TryParseFloatAttribute("w", bag, ref foundAttributeCounter);
			if (foundAttributeCounter > 0)
			{
				base.Value = new Vector4(x, y, z, w);
				handledAttributes.Add("value");
				if (bag is UxmlAsset uxmlAsset)
				{
					uxmlAsset.RemoveAttribute("x");
					uxmlAsset.RemoveAttribute("y");
					uxmlAsset.RemoveAttribute("z");
					uxmlAsset.RemoveAttribute("w");
					uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
				}
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<Vector4Field, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseField<Vector4>.UxmlTraits
	{
		private UxmlFloatAttributeDescription m_XValue = new UxmlFloatAttributeDescription
		{
			name = "x"
		};

		private UxmlFloatAttributeDescription m_YValue = new UxmlFloatAttributeDescription
		{
			name = "y"
		};

		private UxmlFloatAttributeDescription m_ZValue = new UxmlFloatAttributeDescription
		{
			name = "z"
		};

		private UxmlFloatAttributeDescription m_WValue = new UxmlFloatAttributeDescription
		{
			name = "w"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			Vector4Field vector4Field = (Vector4Field)ve;
			vector4Field.SetValueWithoutNotify(new Vector4(m_XValue.GetValueFromBag(bag, cc), m_YValue.GetValueFromBag(bag, cc), m_ZValue.GetValueFromBag(bag, cc), m_WValue.GetValueFromBag(bag, cc)));
		}
	}

	public new static readonly string ussClassName = "unity-vector4-field";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	internal override FieldDescription[] DescribeFields()
	{
		return new FieldDescription[4]
		{
			new FieldDescription("X", "unity-x-input", (Vector4 r) => r.x, delegate(ref Vector4 r, float v)
			{
				r.x = v;
			}),
			new FieldDescription("Y", "unity-y-input", (Vector4 r) => r.y, delegate(ref Vector4 r, float v)
			{
				r.y = v;
			}),
			new FieldDescription("Z", "unity-z-input", (Vector4 r) => r.z, delegate(ref Vector4 r, float v)
			{
				r.z = v;
			}),
			new FieldDescription("W", "unity-w-input", (Vector4 r) => r.w, delegate(ref Vector4 r, float v)
			{
				r.w = v;
			})
		};
	}

	public Vector4Field()
		: this(null)
	{
	}

	public Vector4Field(string label)
		: base(label, 4)
	{
		AddToClassList(ussClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
	}
}
