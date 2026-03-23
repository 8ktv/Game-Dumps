using System;

namespace UnityEngine.UIElements;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class UxmlAttributeAttribute : Attribute
{
	public string name;

	public string[] obsoleteNames;

	public UxmlAttributeAttribute()
		: this((string)null, (string[])null)
	{
	}

	public UxmlAttributeAttribute(string name)
		: this(name, (string[])null)
	{
	}

	public UxmlAttributeAttribute(string name, params string[] obsoleteNames)
	{
		this.name = name;
		this.obsoleteNames = obsoleteNames;
	}
}
