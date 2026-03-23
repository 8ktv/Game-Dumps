using System;

[AttributeUsage(AttributeTargets.Field)]
public class CVarAttribute : Attribute
{
	public string name;

	public string description;

	public string callback;

	public bool hidden;

	public bool resetOnSceneChangeOrCheatsDisabled;

	public CVarAttribute(string name, string description = "", string callback = "", bool hidden = false, bool resetOnSceneChangeOrCheatsDisabled = true)
	{
		this.name = name;
		this.description = description;
		this.description = callback;
		this.hidden = hidden;
		this.resetOnSceneChangeOrCheatsDisabled = resetOnSceneChangeOrCheatsDisabled;
	}
}
