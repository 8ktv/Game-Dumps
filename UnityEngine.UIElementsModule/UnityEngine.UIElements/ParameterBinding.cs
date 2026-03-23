using System;

namespace UnityEngine.UIElements;

[Serializable]
internal struct ParameterBinding
{
	[SerializeField]
	private int m_Index;

	[SerializeField]
	private string m_Name;

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
