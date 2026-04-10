using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class TransformOriginField : BaseField<TransformOrigin>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<TransformOrigin>.UxmlSerializedData
	{
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<TransformOrigin>.UxmlSerializedData.Register();
		}

		public override object CreateInstance()
		{
			return new TransformOriginField();
		}
	}

	public new static readonly string ussClassName = "unity-transform-origin-field";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	private static readonly string compositeUssClassName = "unity-composite-field";

	private static readonly string fieldUssClassName = compositeUssClassName + "__field";

	private LengthField m_XField;

	private LengthField m_YField;

	private FloatField m_ZField;

	public LengthField xField => m_XField;

	public LengthField yField => m_YField;

	public FloatField zField => m_ZField;

	public TransformOriginField()
		: this(null)
	{
	}

	public TransformOriginField(string label)
		: base(label, (VisualElement)null)
	{
		AddToClassList(ussClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		m_XField = new LengthField("X")
		{
			classList = { fieldUssClassName }
		};
		m_YField = new LengthField("Y")
		{
			classList = { fieldUssClassName }
		};
		m_ZField = new FloatField("Z")
		{
			classList = { fieldUssClassName }
		};
		base.visualInput.Add(m_XField);
		base.visualInput.Add(m_YField);
		base.visualInput.Add(m_ZField);
		m_XField.RegisterValueChangedCallback(delegate(ChangeEvent<Length> e)
		{
			if (e.newValue != value.x)
			{
				TransformOrigin transformOrigin = value;
				transformOrigin.x = e.newValue;
				value = transformOrigin;
			}
		});
		m_YField.RegisterValueChangedCallback(delegate(ChangeEvent<Length> e)
		{
			if (e.newValue != value.y)
			{
				TransformOrigin transformOrigin = value;
				transformOrigin.y = e.newValue;
				value = transformOrigin;
			}
		});
		m_ZField.RegisterValueChangedCallback(delegate(ChangeEvent<float> e)
		{
			if (e.newValue != value.z)
			{
				TransformOrigin transformOrigin = value;
				transformOrigin.z = e.newValue;
				value = transformOrigin;
			}
		});
	}

	public override void SetValueWithoutNotify(TransformOrigin transformOrigin)
	{
		base.SetValueWithoutNotify(transformOrigin);
		m_XField.SetValueWithoutNotify(value.x);
		m_YField.SetValueWithoutNotify(value.y);
		m_ZField.SetValueWithoutNotify(value.z);
	}
}
