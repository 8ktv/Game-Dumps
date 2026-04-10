using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class RotateField : BaseField<Rotate>, IValueField<Rotate>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<Rotate>.UxmlSerializedData
	{
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<Rotate>.UxmlSerializedData.Register();
		}

		public override object CreateInstance()
		{
			return new RotateField();
		}
	}

	public static readonly string styleFieldUssClassName = "unity-style-field";

	private AngleField m_AngleField;

	private Vector3Field m_AxisField;

	private BaseFieldMouseDragger m_Dragger;

	public AngleField angleField => m_AngleField;

	public Vector3Field axisField => m_AxisField;

	public RotateField()
		: this(null)
	{
	}

	public RotateField(string label)
		: base(label, (VisualElement)null)
	{
		m_AngleField = new AngleField();
		m_AxisField = new Vector3Field();
		base.visualInput.Add(m_AngleField);
		base.visualInput.Add(m_AxisField);
		m_AngleField.AddToClassList(styleFieldUssClassName);
		m_AngleField.RegisterValueChangedCallback(delegate(ChangeEvent<Angle> e)
		{
			if (e.newValue != value.angle)
			{
				Rotate rotate = value;
				rotate.angle = e.newValue;
				value = rotate;
			}
		});
		m_AxisField.RegisterValueChangedCallback(delegate(ChangeEvent<Vector3> e)
		{
			if (e.newValue != value.axis)
			{
				Rotate rotate = value;
				rotate.axis = e.newValue;
				value = rotate;
			}
		});
		AddLabelDragger();
		SetValueWithoutNotify(Rotate.Initial());
	}

	public override void SetValueWithoutNotify(Rotate rotate)
	{
		base.SetValueWithoutNotify(rotate);
		m_AngleField.SetValueWithoutNotify(value.angle);
		m_AxisField.SetValueWithoutNotify(value.axis);
	}

	protected void AddLabelDragger()
	{
		m_Dragger = new FieldMouseDragger<Rotate>(this);
		EnableLabelDragger(!m_AngleField.isReadOnly);
	}

	private void EnableLabelDragger(bool enable)
	{
		if (m_Dragger != null)
		{
			m_Dragger.SetDragZone(enable ? base.labelElement : null);
			base.labelElement.EnableInClassList(BaseField<Rotate>.labelDraggerVariantUssClassName, enable);
		}
	}

	public void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Rotate startValue)
	{
		m_AngleField.ApplyInputDeviceDelta(delta, speed, startValue.angle);
	}

	public void StartDragging()
	{
		m_AngleField.StartDragging();
	}

	public void StopDragging()
	{
		m_AngleField.StopDragging();
	}
}
