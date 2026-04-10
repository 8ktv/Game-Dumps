using System;
using Unity.Properties;

namespace UnityEngine.UIElements;

[Serializable]
public struct ParameterBinding
{
	[SerializeField]
	[DontCreateProperty]
	private int m_Index;

	[DontCreateProperty]
	[SerializeField]
	private string m_Name;

	[CreateProperty]
	public int index
	{
		get
		{
			return m_Index;
		}
		set
		{
			m_Index = value;
		}
	}

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
}
