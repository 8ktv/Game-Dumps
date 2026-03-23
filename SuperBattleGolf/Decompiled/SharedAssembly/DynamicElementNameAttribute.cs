using UnityEngine;

public class DynamicElementNameAttribute : PropertyAttribute
{
	public string NameProperty { get; private set; }

	public DynamicElementNameAttribute(string nameProperty)
	{
		NameProperty = nameProperty;
	}
}
