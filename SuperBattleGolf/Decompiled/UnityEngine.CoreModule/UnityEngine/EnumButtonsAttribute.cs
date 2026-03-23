namespace UnityEngine;

public class EnumButtonsAttribute : PropertyAttribute
{
	public bool includeObsolete;

	public EnumButtonsAttribute(bool includeObsolete = false)
	{
		this.includeObsolete = includeObsolete;
	}
}
