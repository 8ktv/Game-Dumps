using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class ImageFieldValueDecoratorAttribute : PropertyAttribute
{
	public string name;

	public ImageFieldValueDecoratorAttribute(string fieldName)
	{
		name = fieldName;
	}
}
