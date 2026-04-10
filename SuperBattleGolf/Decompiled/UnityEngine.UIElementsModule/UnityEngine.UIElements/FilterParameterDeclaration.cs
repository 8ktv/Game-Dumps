using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
public struct FilterParameterDeclaration
{
	[SerializeField]
	[DontCreateProperty]
	private string m_Name;

	[SerializeField]
	[DontCreateProperty]
	private FilterParameter m_InterpolationDefaultValue;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal FilterParameter defaultValue;

	[CreateProperty]
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

	[CreateProperty]
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
