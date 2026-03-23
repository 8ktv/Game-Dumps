using UnityEngine;

public class NameAttribute : PropertyAttribute
{
	public string Name { get; private set; } = string.Empty;

	public NameAttribute(string name)
	{
		Name = name;
	}
}
