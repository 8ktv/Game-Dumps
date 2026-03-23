using System;

namespace UnityEngine.UIElements;

[Serializable]
internal struct FilterParameterDeclaration
{
	[SerializeField]
	private string m_Name;

	[SerializeField]
	private FilterParameter m_InterpolationDefaultValue;

	internal FilterParameter defaultValue;

	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			m_Name = value;
		}
	}

	public FilterParameter interpolationDefaultValue
	{
		get
		{
			return m_InterpolationDefaultValue;
		}
		set
		{
			m_InterpolationDefaultValue = value;
		}
	}
}
