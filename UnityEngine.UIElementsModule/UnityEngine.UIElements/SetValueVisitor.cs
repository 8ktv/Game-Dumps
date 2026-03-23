using Unity.Properties;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

internal class SetValueVisitor<TSrcValue> : PathVisitor
{
	public static readonly UnityEngine.Pool.ObjectPool<SetValueVisitor<TSrcValue>> Pool = new UnityEngine.Pool.ObjectPool<SetValueVisitor<TSrcValue>>(() => new SetValueVisitor<TSrcValue>(), delegate(SetValueVisitor<TSrcValue> v)
	{
		v.Reset();
	});

	public TSrcValue Value;

	public ConverterGroup group { get; set; }

	public override void Reset()
	{
		base.Reset();
		Value = default(TSrcValue);
		group = null;
	}

	protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
	{
		TValue destination;
		TValue destination2;
		if (property.IsReadOnly)
		{
			base.ReturnCode = VisitReturnCode.AccessViolation;
		}
		else if (group != null && group.TryConvert<TSrcValue, TValue>(ref Value, out destination))
		{
			property.SetValue(ref container, destination);
		}
		else if (ConverterGroups.TryConvert<TSrcValue, TValue>(ref Value, out destination2))
		{
			property.SetValue(ref container, destination2);
		}
		else
		{
			base.ReturnCode = VisitReturnCode.InvalidCast;
		}
	}
}
