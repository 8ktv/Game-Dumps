using System;

namespace UnityEngine.UIElements;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UxmlElementAttribute : Attribute
{
	public readonly string name;

	public LibraryVisibility visibility = LibraryVisibility.Default;

	public string libraryPath;

	public UxmlElementAttribute()
		: this(null)
	{
	}

	public UxmlElementAttribute(string uxmlName)
	{
		name = uxmlName;
	}
}
