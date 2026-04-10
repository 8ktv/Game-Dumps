using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
[UxmlObject]
internal abstract class StylePropertyValidation : INotifyBindablePropertyChanged
{
	[Serializable]
	[ExcludeFromDocs]
	public abstract class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
	{
	}

	public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

	protected void NotifyPropertyChanged(in BindingId bindingId)
	{
		this.propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(in bindingId));
	}
}
