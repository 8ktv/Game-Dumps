using System;
using Unity.Properties;

namespace UnityEngine.UIElements;

[Serializable]
public struct PostProcessingMargins
{
	[DontCreateProperty]
	[SerializeField]
	private float m_Left;

	[SerializeField]
	[DontCreateProperty]
	private float m_Top;

	[DontCreateProperty]
	[SerializeField]
	private float m_Right;

	[DontCreateProperty]
	[SerializeField]
	private float m_Bottom;

	[CreateProperty]
	public float left
	{
		get
		{
			return m_Left;
		}
		set
		{
			m_Left = value;
		}
	}

	[CreateProperty]
	public float top
	{
		get
		{
			return m_Top;
		}
		set
		{
			m_Top = value;
		}
	}

	[CreateProperty]
	public float right
	{
		get
		{
			return m_Right;
		}
		set
		{
			m_Right = value;
		}
	}

	[CreateProperty]
	public float bottom
	{
		get
		{
			return m_Bottom;
		}
		set
		{
			m_Bottom = value;
		}
	}
}
