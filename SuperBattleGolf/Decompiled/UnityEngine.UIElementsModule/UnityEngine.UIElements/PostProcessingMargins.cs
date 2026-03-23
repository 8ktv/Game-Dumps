using System;

namespace UnityEngine.UIElements;

[Serializable]
internal struct PostProcessingMargins
{
	[SerializeField]
	private float m_Left;

	[SerializeField]
	private float m_Top;

	[SerializeField]
	private float m_Right;

	[SerializeField]
	private float m_Bottom;

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
