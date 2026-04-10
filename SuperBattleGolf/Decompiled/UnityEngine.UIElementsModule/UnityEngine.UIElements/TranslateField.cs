using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class TranslateField : BaseField<Translate>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<Translate>.UxmlSerializedData
	{
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<Translate>.UxmlSerializedData.Register();
		}

		public override object CreateInstance()
		{
			return new TranslateField();
		}
	}

	public new static readonly string ussClassName = "unity-translate-field";

	private static readonly string compositeUssClassName = "unity-composite-field";

	private static readonly string compositeFieldUssClassName = compositeUssClassName + "__field";

	private LengthField m_XField;

	private LengthField m_YField;

	private FloatField m_ZField;

	public LengthField xField => m_XField;

	public LengthField yField => m_YField;

	public FloatField zField => m_ZField;

	public TranslateField()
		: this(null)
	{
	}

	public TranslateField(string label)
		: base(label, (VisualElement)null)
	{
		AddToClassList(ussClassName);
		m_XField = new LengthField("X")
		{
			classList = { compositeFieldUssClassName }
		};
		m_YField = new LengthField("Y")
		{
			classList = { compositeFieldUssClassName }
		};
		m_ZField = new FloatField("Z")
		{
			classList = { compositeFieldUssClassName }
		};
		base.visualInput.Add(m_XField);
		base.visualInput.Add(m_YField);
		base.visualInput.Add(m_ZField);
		m_XField.RegisterValueChangedCallback(delegate(ChangeEvent<Length> e)
		{
			if (e.newValue != value.x)
			{
				Translate translate = value;
				translate.x = e.newValue;
				value = translate;
			}
		});
		m_YField.RegisterValueChangedCallback(delegate(ChangeEvent<Length> e)
		{
			if (e.newValue != value.y)
			{
				Translate translate = value;
				translate.y = e.newValue;
				value = translate;
			}
		});
		m_ZField.RegisterValueChangedCallback(delegate(ChangeEvent<float> e)
		{
			if (e.newValue != value.z)
			{
				Translate translate = value;
				translate.z = e.newValue;
				value = translate;
			}
		});
	}

	public override void SetValueWithoutNotify(Translate t)
	{
		base.SetValueWithoutNotify(t);
		m_XField.SetValueWithoutNotify(value.x);
		m_YField.SetValueWithoutNotify(value.y);
		m_ZField.SetValueWithoutNotify(value.z);
	}

	public override string ToString()
	{
		return $"(x:{m_XField.value}, y:{m_YField.value}, z:{m_ZField.value})";
	}
}
