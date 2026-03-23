using System;

[AttributeUsage(AttributeTargets.Method)]
public class CCommandAttribute : Attribute
{
	public string name;

	public string description;

	public bool serverOnly;

	public bool hidden;

	public CCommandAttribute(string name, string description = "", bool serverOnly = false, bool hidden = false)
	{
		this.name = name;
		this.description = description;
		this.serverOnly = serverOnly;
		this.hidden = hidden;
	}
}
