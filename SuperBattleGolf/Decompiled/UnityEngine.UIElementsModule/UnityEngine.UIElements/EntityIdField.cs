namespace UnityEngine.UIElements;

internal class EntityIdField : BaseField<EntityId>
{
	private readonly IntegerField m_IntegerField = new IntegerField();

	public new static readonly string ussClassName = "unity-entityId-field";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	public EntityIdField()
		: this(null)
	{
	}

	public EntityIdField(string label)
		: base(label, (VisualElement)null)
	{
		AddToClassList(ussClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		base.visualInput.Add(m_IntegerField);
		m_IntegerField.RegisterValueChangedCallback(delegate(ChangeEvent<int> evt)
		{
			value = EntityId.From(evt.newValue);
		});
	}

	public override void SetValueWithoutNotify(EntityId newValue)
	{
		base.SetValueWithoutNotify(newValue);
		m_IntegerField.SetValueWithoutNotify(newValue.GetRawData());
	}

	protected override void UpdateMixedValueContent()
	{
		m_IntegerField.showMixedValue = base.showMixedValue;
	}
}
