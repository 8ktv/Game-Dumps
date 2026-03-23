using System;

namespace UnityEngine.UIElements;

[Serializable]
internal sealed class FilterFunctionDefinition : ScriptableObject
{
	[SerializeField]
	private string m_FilterName;

	[SerializeField]
	private FilterParameterDeclaration[] m_Parameters;

	[SerializeField]
	private PostProcessingPass[] m_Passes;

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
