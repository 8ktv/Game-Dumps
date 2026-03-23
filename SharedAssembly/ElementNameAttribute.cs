using UnityEngine;

public class ElementNameAttribute : PropertyAttribute
{
	public string Name { get; private set; } = "Element";

	public ElementNameAttribute(string name)
	{
		Name = name;
	}
}
