using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class ScaleField : BaseField<Scale>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<Scale>.UxmlSerializedData
	{
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<Scale>.UxmlSerializedData.Register();
		}

		public override object CreateInstance()
		{
			return new ScaleField();
		}
	}

	private Vector3Field m_VectorField;

	public Vector3Field vectorField => m_VectorField;

	public ScaleField()
		: this(null)
	{
	}

	public ScaleField(string label)
		: base(label, (VisualElement)null)
	{
		m_VectorField = new Vector3Field();
		base.visualInput.Add(m_VectorField);
		m_VectorField.RegisterValueChangedCallback(delegate(ChangeEvent<Vector3> e)
		{
			if (e.newValue != value.value)
			{
				Scale scale = value;
				scale.value = e.newValue;
				value = scale;
			}
		});
		SetValueWithoutNotify(Scale.Initial());
	}

	public override void SetValueWithoutNotify(Scale scale)
	{
		base.SetValueWithoutNotify(scale);
		m_VectorField.SetValueWithoutNotify(value.value);
	}
}
