using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements;

[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
public class Vector2IntField : BaseCompositeField<Vector2Int, IntegerField, int>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseCompositeField<Vector2Int, IntegerField, int>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
	{
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
		}

		public override object CreateInstance()
		{
			return new Vector2IntField();
		}

		void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
		{
			int foundAttributeCounter = 0;
			int x = UxmlUtility.TryParseIntAttribute("x", bag, ref foundAttributeCounter);
			int y = UxmlUtility.TryParseIntAttribute("y", bag, ref foundAttributeCounter);
			if (foundAttributeCounter > 0)
			{
				base.Value = new Vector2Int(x, y);
				handledAttributes.Add("value");
				if (bag is UxmlAsset uxmlAsset)
				{
					uxmlAsset.RemoveAttribute("x");
					uxmlAsset.RemoveAttribute("y");
					uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
				}
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<Vector2IntField, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseField<Vector2Int>.UxmlTraits
	{
		private UxmlIntAttributeDescription m_XValue = new UxmlIntAttributeDescription
		{
			name = "x"
		};

		private UxmlIntAttributeDescription m_YValue = new UxmlIntAttributeDescription
		{
			name = "y"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			Vector2IntField vector2IntField = (Vector2IntField)ve;
			vector2IntField.SetValueWithoutNotify(new Vector2Int(m_XValue.GetValueFromBag(bag, cc), m_YValue.GetValueFromBag(bag, cc)));
		}
	}

	public new static readonly string ussClassName = "unity-vector2-int-field";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	internal override FieldDescription[] DescribeFields()
	{
		return new FieldDescription[2]
		{
			new FieldDescription("X", "unity-x-input", (Vector2Int r) => r.x, delegate(ref Vector2Int r, int v)
			{
				r.x = v;
			}),
			new FieldDescription("Y", "unity-y-input", (Vector2Int r) => r.y, delegate(ref Vector2Int r, int v)
			{
				r.y = v;
			})
		};
	}

	public Vector2IntField()
		: this(null)
	{
	}

	public Vector2IntField(string label)
		: base(label, 2)
	{
		AddToClassList(ussClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
	}
}
