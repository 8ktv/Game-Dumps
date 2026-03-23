using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements;

[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
public class BoundsIntField : BaseField<BoundsInt>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<BoundsInt>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
	{
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<BoundsInt>.UxmlSerializedData.Register();
		}

		public override object CreateInstance()
		{
			return new BoundsIntField();
		}

		void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
		{
			int foundAttributeCounter = 0;
			int x = UxmlUtility.TryParseIntAttribute("px", bag, ref foundAttributeCounter);
			int y = UxmlUtility.TryParseIntAttribute("py", bag, ref foundAttributeCounter);
			int z = UxmlUtility.TryParseIntAttribute("pz", bag, ref foundAttributeCounter);
			int x2 = UxmlUtility.TryParseIntAttribute("sx", bag, ref foundAttributeCounter);
			int y2 = UxmlUtility.TryParseIntAttribute("sy", bag, ref foundAttributeCounter);
			int z2 = UxmlUtility.TryParseIntAttribute("sz", bag, ref foundAttributeCounter);
			if (foundAttributeCounter > 0)
			{
				base.Value = new BoundsInt(new Vector3Int(x, y, z), new Vector3Int(x2, y2, z2));
				handledAttributes.Add("value");
				if (bag is UxmlAsset uxmlAsset)
				{
					uxmlAsset.RemoveAttribute("px");
					uxmlAsset.RemoveAttribute("py");
					uxmlAsset.RemoveAttribute("pz");
					uxmlAsset.RemoveAttribute("sx");
					uxmlAsset.RemoveAttribute("sy");
					uxmlAsset.RemoveAttribute("sz");
					uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
				}
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<BoundsIntField, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseField<BoundsInt>.UxmlTraits
	{
		private UxmlIntAttributeDescription m_PositionXValue = new UxmlIntAttributeDescription
		{
			name = "px"
		};

		private UxmlIntAttributeDescription m_PositionYValue = new UxmlIntAttributeDescription
		{
			name = "py"
		};

		private UxmlIntAttributeDescription m_PositionZValue = new UxmlIntAttributeDescription
		{
			name = "pz"
		};

		private UxmlIntAttributeDescription m_SizeXValue = new UxmlIntAttributeDescription
		{
			name = "sx"
		};

		private UxmlIntAttributeDescription m_SizeYValue = new UxmlIntAttributeDescription
		{
			name = "sy"
		};

		private UxmlIntAttributeDescription m_SizeZValue = new UxmlIntAttributeDescription
		{
			name = "sz"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			BoundsIntField boundsIntField = (BoundsIntField)ve;
			boundsIntField.SetValueWithoutNotify(new BoundsInt(new Vector3Int(m_PositionXValue.GetValueFromBag(bag, cc), m_PositionYValue.GetValueFromBag(bag, cc), m_PositionZValue.GetValueFromBag(bag, cc)), new Vector3Int(m_SizeXValue.GetValueFromBag(bag, cc), m_SizeYValue.GetValueFromBag(bag, cc), m_SizeZValue.GetValueFromBag(bag, cc))));
		}
	}

	private Vector3IntField m_PositionField;

	private Vector3IntField m_SizeField;

	public new static readonly string ussClassName = "unity-bounds-int-field";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	public static readonly string positionUssClassName = ussClassName + "__position-field";

	public static readonly string sizeUssClassName = ussClassName + "__size-field";

	public BoundsIntField()
		: this(null)
	{
	}

	public BoundsIntField(string label)
		: base(label, (VisualElement)null)
	{
		base.delegatesFocus = false;
		base.visualInput.focusable = false;
		AddToClassList(ussClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		m_PositionField = new Vector3IntField("Position");
		m_PositionField.name = "unity-m_Position-input";
		m_PositionField.delegatesFocus = true;
		m_PositionField.AddToClassList(positionUssClassName);
		m_PositionField.RegisterValueChangedCallback(delegate(ChangeEvent<Vector3Int> e)
		{
			BoundsInt boundsInt = value;
			boundsInt.position = e.newValue;
			value = boundsInt;
		});
		base.visualInput.hierarchy.Add(m_PositionField);
		m_SizeField = new Vector3IntField("Size");
		m_SizeField.name = "unity-m_Size-input";
		m_SizeField.delegatesFocus = true;
		m_SizeField.AddToClassList(sizeUssClassName);
		m_SizeField.RegisterValueChangedCallback(delegate(ChangeEvent<Vector3Int> e)
		{
			BoundsInt boundsInt = value;
			boundsInt.size = e.newValue;
			value = boundsInt;
		});
		base.visualInput.hierarchy.Add(m_SizeField);
	}

	public override void SetValueWithoutNotify(BoundsInt newValue)
	{
		base.SetValueWithoutNotify(newValue);
		m_PositionField.SetValueWithoutNotify(base.rawValue.position);
		m_SizeField.SetValueWithoutNotify(base.rawValue.size);
	}

	protected override void UpdateMixedValueContent()
	{
		m_PositionField.showMixedValue = base.showMixedValue;
		m_SizeField.showMixedValue = base.showMixedValue;
	}
}
