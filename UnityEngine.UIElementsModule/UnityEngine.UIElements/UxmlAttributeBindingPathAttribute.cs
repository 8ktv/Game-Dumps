using System;

namespace UnityEngine.UIElements;

internal class UxmlAttributeBindingPathAttribute : Attribute
{
	public string path { get; private set; }

	public UxmlAttributeBindingPathAttribute(string bindingPath)
	{
		path = bindingPath;
	}
}
