using System;

namespace UnityEngine.UIElements;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public class UxmlObjectReferenceAttribute : Attribute
{
	public string name;

	public Type[] types;

	public UxmlObjectReferenceAttribute()
		: this((string)null, (Type[])null)
	{
	}

	public UxmlObjectReferenceAttribute(string uxmlName)
		: this(uxmlName, (Type[])null)
	{
		if (uxmlName == "null")
		{
			throw new ArgumentException("UxmlObjectReferenceAttribute name cannot be \"null\".");
		}
	}

	public UxmlObjectReferenceAttribute(string uxmlName, params Type[] acceptedTypes)
	{
		name = uxmlName;
		types = acceptedTypes;
	}
}
