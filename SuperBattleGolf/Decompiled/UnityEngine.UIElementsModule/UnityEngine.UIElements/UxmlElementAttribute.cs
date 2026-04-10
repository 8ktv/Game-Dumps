using System;

namespace UnityEngine.UIElements;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UxmlElementAttribute : Attribute
{
	public readonly string name;

	public LibraryVisibility visibility = LibraryVisibility.Default;

	public string libraryPath;

	internal readonly Type[] supportedChildTypes;

	public UxmlElementAttribute()
		: this(null)
	{
	}

	public UxmlElementAttribute(string uxmlName)
		: this(uxmlName, (Type[])null)
	{
	}

	public UxmlElementAttribute(string uxmlName, params Type[] supportedTypes)
	{
		name = uxmlName;
		supportedChildTypes = supportedTypes;
	}
}
