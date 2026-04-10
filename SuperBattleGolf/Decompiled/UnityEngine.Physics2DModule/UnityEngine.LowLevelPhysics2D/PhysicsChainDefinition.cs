using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsChainDefinition
{
	[SerializeField]
	private PhysicsShape.SurfaceMaterial m_SurfaceMaterial;

	[SerializeField]
	private PhysicsShape.ContactFilter m_ContactFilter;

	[SerializeField]
	private bool m_IsLoop;

	[SerializeField]
	private bool m_TriggerEvents;

	public static PhysicsChainDefinition defaultDefinition => PhysicsLowLevelScripting2D.PhysicsChain_GetDefaultDefinition(useSettings: true);

	public PhysicsShape.SurfaceMaterial surfaceMaterial
	{
		readonly get
		{
			return m_SurfaceMaterial;
		}
		set
		{
			m_SurfaceMaterial = value;
		}
	}

	public PhysicsShape.ContactFilter contactFilter
	{
		readonly get
		{
			return m_ContactFilter;
		}
		set
		{
			m_ContactFilter = value;
		}
	}

	public bool isLoop
	{
		readonly get
		{
			return m_IsLoop;
		}
		set
		{
			m_IsLoop = value;
		}
	}

	public bool triggerEvents
	{
		readonly get
		{
			return m_TriggerEvents;
		}
		set
		{
			m_TriggerEvents = value;
		}
	}

	public PhysicsChainDefinition()
	{
		this = defaultDefinition;
	}

	public PhysicsChainDefinition(bool useSettings)
	{
		this = PhysicsLowLevelScripting2D.PhysicsChain_GetDefaultDefinition(useSettings);
	}
}
