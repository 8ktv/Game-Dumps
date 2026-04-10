using System;
using Unity.Properties;

namespace UnityEngine.UIElements;

[Serializable]
[HelpURL("ui-systems/custom-filters")]
public sealed class FilterFunctionDefinition : ScriptableObject
{
	[SerializeField]
	[DontCreateProperty]
	private string m_FilterName;

	[DontCreateProperty]
	[SerializeField]
	private FilterParameterDeclaration[] m_Parameters;

	[SerializeField]
	[DontCreateProperty]
	private PostProcessingPass[] m_Passes;

	[CreateProperty]
	public string filterName
	{
		get
		{
			return m_FilterName;
		}
		set
		{
			m_FilterName = value;
		}
	}

	[CreateProperty]
	public FilterParameterDeclaration[] parameters
	{
		get
		{
			return m_Parameters;
		}
		set
		{
			m_Parameters = value;
		}
	}

	[CreateProperty]
	public PostProcessingPass[] passes
	{
		get
		{
			return m_Passes;
		}
		set
		{
			m_Passes = value;
		}
	}
}
