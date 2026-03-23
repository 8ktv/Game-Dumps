namespace UnityEngine;

public struct SoftJointLimit
{
	private float m_Limit;

	private float m_Bounciness;

	private float m_ContactDistance;

	public float limit
	{
		get
		{
			return m_Limit;
		}
		set
		{
			m_Limit = value;
		}
	}

	public float bounciness
	{
		get
		{
			return m_Bounciness;
		}
		set
		{
			m_Bounciness = value;
		}
	}

	public float contactDistance
	{
		get
		{
			return m_ContactDistance;
		}
		set
		{
			m_ContactDistance = value;
		}
	}
}
