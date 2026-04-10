using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsShapeDefinition
{
	public PhysicsShape.ContactFilter contactFilter;

	public PhysicsShape.SurfaceMaterial surfaceMaterial;

	[SerializeField]
	[Min(0f)]
	private float m_Density;

	[SerializeField]
	private bool m_IsTrigger;

	[SerializeField]
	private bool m_TriggerEvents;

	[SerializeField]
	private bool m_ContactEvents;

	[SerializeField]
	private bool m_HitEvents;

	[SerializeField]
	private bool m_ContactFilterCallbacks;

	[SerializeField]
	private bool m_PreSolveCallbacks;

	[SerializeField]
	private bool m_StartStaticContacts;

	[SerializeField]
	private bool m_StartMassUpdate;

	public PhysicsShape.MoverData moverData;

	public static PhysicsShapeDefinition defaultDefinition => PhysicsLowLevelScripting2D.PhysicsShape_GetDefaultDefinition(useSettings: true);

	public float density
	{
		readonly get
		{
			return m_Density;
		}
		set
		{
			m_Density = Mathf.Max(0f, value);
		}
	}

	public bool isTrigger
	{
		readonly get
		{
			return m_IsTrigger;
		}
		set
		{
			m_IsTrigger = value;
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

	public bool contactEvents
	{
		readonly get
		{
			return m_ContactEvents;
		}
		set
		{
			m_ContactEvents = value;
		}
	}

	public bool hitEvents
	{
		readonly get
		{
			return m_HitEvents;
		}
		set
		{
			m_HitEvents = value;
		}
	}

	public bool contactFilterCallbacks
	{
		readonly get
		{
			return m_ContactFilterCallbacks;
		}
		set
		{
			m_ContactFilterCallbacks = value;
		}
	}

	public bool preSolveCallbacks
	{
		readonly get
		{
			return m_PreSolveCallbacks;
		}
		set
		{
			m_PreSolveCallbacks = value;
		}
	}

	public bool startStaticContacts
	{
		readonly get
		{
			return m_StartStaticContacts;
		}
		set
		{
			m_StartStaticContacts = value;
		}
	}

	public bool startMassUpdate
	{
		readonly get
		{
			return m_StartMassUpdate;
		}
		set
		{
			m_StartMassUpdate = value;
		}
	}

	public PhysicsShapeDefinition()
	{
		this = defaultDefinition;
	}

	public PhysicsShapeDefinition(bool useSettings)
	{
		this = PhysicsLowLevelScripting2D.PhysicsShape_GetDefaultDefinition(useSettings);
	}
}
