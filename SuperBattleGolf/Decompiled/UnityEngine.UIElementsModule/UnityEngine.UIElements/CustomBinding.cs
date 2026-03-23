using System;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[UxmlObject]
public abstract class CustomBinding : Binding
{
	[Serializable]
	[ExcludeFromDocs]
	public new abstract class UxmlSerializedData : Binding.UxmlSerializedData
	{
	}

	protected CustomBinding()
	{
		base.updateTrigger = BindingUpdateTrigger.EveryUpdate;
	}

	protected internal virtual BindingResult Update(in BindingContext context)
	{
		return new BindingResult(BindingStatus.Success);
	}
}
