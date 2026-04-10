using System;
using System.ComponentModel;
using UnityEngine.Serialization;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsWorldDefinition
{
	[SerializeField]
	private Vector2 m_Gravity;

	[FormerlySerializedAs("m_SimulationMode")]
	[SerializeField]
	private PhysicsWorld.SimulationType m_SimulationType;

	[SerializeField]
	[Min(1f)]
	private int m_SimulationSubSteps;

	[SerializeField]
	[Range(0f, 64f)]
	private int m_SimulationWorkers;

	[SerializeField]
	private PhysicsWorld.TransformWriteMode m_TransformWriteMode;

	[SerializeField]
	private PhysicsWorld.TransformPlane m_TransformPlane;

	[SerializeField]
	private bool m_TransformTweening;

	[SerializeField]
	private bool m_SleepingAllowed;

	[SerializeField]
	private bool m_ContinuousAllowed;

	[SerializeField]
	private bool m_ContactFilterCallbacks;

	[SerializeField]
	private bool m_PreSolveCallbacks;

	[SerializeField]
	private bool m_AutoBodyUpdateCallbacks;

	[SerializeField]
	private bool m_AutoContactCallbacks;

	[SerializeField]
	private bool m_AutoTriggerCallbacks;

	[SerializeField]
	private bool m_AutoJointThresholdCallbacks;

	[SerializeField]
	[Min(0f)]
	private float m_BounceThreshold;

	[Min(0f)]
	[SerializeField]
	private float m_ContactHitEventThreshold;

	[SerializeField]
	[Min(0f)]
	private float m_ContactFrequency;

	[SerializeField]
	[Min(0f)]
	private float m_ContactDamping;

	[SerializeField]
	[Min(0f)]
	private float m_ContactSpeed;

	[Min(0f)]
	[SerializeField]
	private float m_MaximumLinearSpeed;

	[SerializeField]
	private PhysicsWorld.DrawOptions m_DrawOptions;

	[SerializeField]
	private PhysicsWorld.DrawFillOptions m_DrawFillOptions;

	[SerializeField]
	[Range(1f, 5f)]
	private float m_DrawThickness;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_DrawFillAlpha;

	[SerializeField]
	[Range(0.001f, 10f)]
	private float m_DrawPointScale;

	[SerializeField]
	[Range(0.001f, 10f)]
	private float m_DrawNormalScale;

	[Range(0.001f, 10f)]
	[SerializeField]
	private float m_DrawImpulseScale;

	[Min(0f)]
	[SerializeField]
	private int m_DrawCapacity;

	[SerializeField]
	private PhysicsWorld.DrawColors m_DrawColors;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("PhysicsWorldDefinition.simulationMode has been deprecated. Please use PhysicsWorldDefinition.simulateType instead.", false)]
	public SimulationMode2D simulationMode
	{
		readonly get
		{
			return (SimulationMode2D)simulateType;
		}
		set
		{
			simulateType = (PhysicsWorld.SimulationType)value;
		}
	}

	public static PhysicsWorldDefinition defaultDefinition => PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultDefinition(useSettings: true);

	public Vector2 gravity
	{
		readonly get
		{
			return m_Gravity;
		}
		set
		{
			m_Gravity = value;
		}
	}

	public PhysicsWorld.SimulationType simulateType
	{
		readonly get
		{
			return m_SimulationType;
		}
		set
		{
			m_SimulationType = value;
		}
	}

	public int simulationSubSteps
	{
		readonly get
		{
			return m_SimulationSubSteps;
		}
		set
		{
			m_SimulationSubSteps = Mathf.Max(1, value);
		}
	}

	public int simulationWorkers
	{
		readonly get
		{
			return m_SimulationWorkers;
		}
		set
		{
			m_SimulationWorkers = Mathf.Clamp(value, 0, 64);
		}
	}

	public PhysicsWorld.TransformWriteMode transformWriteMode
	{
		readonly get
		{
			return m_TransformWriteMode;
		}
		set
		{
			m_TransformWriteMode = value;
		}
	}

	public PhysicsWorld.TransformPlane transformPlane
	{
		readonly get
		{
			return m_TransformPlane;
		}
		set
		{
			m_TransformPlane = value;
		}
	}

	public bool transformTweening
	{
		readonly get
		{
			return m_TransformTweening;
		}
		set
		{
			m_TransformTweening = value;
		}
	}

	public bool sleepingAllowed
	{
		readonly get
		{
			return m_SleepingAllowed;
		}
		set
		{
			m_SleepingAllowed = value;
		}
	}

	public bool continuousAllowed
	{
		readonly get
		{
			return m_ContinuousAllowed;
		}
		set
		{
			m_ContinuousAllowed = value;
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

	public bool autoBodyUpdateCallbacks
	{
		readonly get
		{
			return m_AutoBodyUpdateCallbacks;
		}
		set
		{
			m_AutoBodyUpdateCallbacks = value;
		}
	}

	public bool autoContactCallbacks
	{
		readonly get
		{
			return m_AutoContactCallbacks;
		}
		set
		{
			m_AutoContactCallbacks = value;
		}
	}

	public bool autoTriggerCallbacks
	{
		readonly get
		{
			return m_AutoTriggerCallbacks;
		}
		set
		{
			m_AutoTriggerCallbacks = value;
		}
	}

	public bool autoJointThresholdCallbacks
	{
		readonly get
		{
			return m_AutoJointThresholdCallbacks;
		}
		set
		{
			m_AutoJointThresholdCallbacks = value;
		}
	}

	public float bounceThreshold
	{
		readonly get
		{
			return m_BounceThreshold;
		}
		set
		{
			m_BounceThreshold = Mathf.Max(0f, value);
		}
	}

	public float contactHitEventThreshold
	{
		readonly get
		{
			return m_ContactHitEventThreshold;
		}
		set
		{
			m_ContactHitEventThreshold = Mathf.Max(0f, value);
		}
	}

	public float contactFrequency
	{
		readonly get
		{
			return m_ContactFrequency;
		}
		set
		{
			m_ContactFrequency = Mathf.Max(0f, value);
		}
	}

	public float contactDamping
	{
		readonly get
		{
			return m_ContactDamping;
		}
		set
		{
			m_ContactDamping = Mathf.Max(0f, value);
		}
	}

	public float contactSpeed
	{
		readonly get
		{
			return m_ContactSpeed;
		}
		set
		{
			m_ContactSpeed = Mathf.Max(0f, value);
		}
	}

	public float maximumLinearSpeed
	{
		readonly get
		{
			return m_MaximumLinearSpeed;
		}
		set
		{
			m_MaximumLinearSpeed = Mathf.Max(0f, value);
		}
	}

	public PhysicsWorld.DrawOptions drawOptions
	{
		readonly get
		{
			return m_DrawOptions;
		}
		set
		{
			m_DrawOptions = value;
		}
	}

	public PhysicsWorld.DrawFillOptions drawFillOptions
	{
		readonly get
		{
			return m_DrawFillOptions;
		}
		set
		{
			m_DrawFillOptions = value;
		}
	}

	public float drawThickness
	{
		readonly get
		{
			return m_DrawThickness;
		}
		set
		{
			m_DrawThickness = Mathf.Clamp(value, 1f, 5f);
		}
	}

	public float drawFillAlpha
	{
		readonly get
		{
			return m_DrawFillAlpha;
		}
		set
		{
			m_DrawFillAlpha = Mathf.Clamp01(value);
		}
	}

	public float drawPointScale
	{
		readonly get
		{
			return m_DrawPointScale;
		}
		set
		{
			m_DrawPointScale = Mathf.Clamp(value, 0.001f, 10f);
		}
	}

	public float drawNormalScale
	{
		readonly get
		{
			return m_DrawNormalScale;
		}
		set
		{
			m_DrawNormalScale = Mathf.Clamp(value, 0.001f, 10f);
		}
	}

	public float drawImpulseScale
	{
		readonly get
		{
			return m_DrawImpulseScale;
		}
		set
		{
			m_DrawImpulseScale = Mathf.Clamp(value, 0.001f, 10f);
		}
	}

	public int drawCapacity
	{
		readonly get
		{
			return m_DrawCapacity;
		}
		set
		{
			m_DrawCapacity = Mathf.Max(0, value);
		}
	}

	public PhysicsWorld.DrawColors drawColors
	{
		readonly get
		{
			return m_DrawColors;
		}
		set
		{
			m_DrawColors = value;
		}
	}

	public PhysicsWorldDefinition()
	{
		this = defaultDefinition;
	}

	public PhysicsWorldDefinition(bool useSettings)
	{
		this = PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultDefinition(useSettings);
	}
}
