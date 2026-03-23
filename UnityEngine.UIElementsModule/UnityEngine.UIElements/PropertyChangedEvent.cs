using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class PropertyChangedEvent : EventBase<PropertyChangedEvent>, IChangeEvent
{
	public BindingId property { get; set; }

	static PropertyChangedEvent()
	{
		EventBase<PropertyChangedEvent>.SetCreateFunction(() => new PropertyChangedEvent());
	}

	public PropertyChangedEvent()
	{
		base.bubbles = false;
		base.tricklesDown = false;
	}

	public static PropertyChangedEvent GetPooled(in BindingId property)
	{
		PropertyChangedEvent propertyChangedEvent = EventBase<PropertyChangedEvent>.GetPooled();
		propertyChangedEvent.property = property;
		return propertyChangedEvent;
	}
}
