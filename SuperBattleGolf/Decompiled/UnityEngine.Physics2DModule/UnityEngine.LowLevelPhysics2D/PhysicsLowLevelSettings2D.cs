using System;
using UnityEngine.Scripting;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
[RequiredByNativeCode]
public sealed class PhysicsLowLevelSettings2D : ScriptableObject
{
	[SerializeField]
	[Header("Layers")]
	private PhysicsLayers.LayerNames m_PhysicsLayerNames;

	[SerializeField]
	private bool m_UseFullLayers;

	[SerializeField]
	[Header("Default Definitions")]
	private PhysicsWorldDefinition m_PhysicsWorldDefinition;

	[SerializeField]
	private PhysicsBodyDefinition m_PhysicsBodyDefinition;

	[SerializeField]
	private PhysicsShapeDefinition m_PhysicsShapeDefinition;

	[SerializeField]
	private PhysicsChainDefinition m_PhysicsChainDefinition;

	[SerializeField]
	private PhysicsDistanceJointDefinition m_PhysicsDistanceJointDefinition;

	[SerializeField]
	private PhysicsFixedJointDefinition m_PhysicsFixedJointDefinition;

	[SerializeField]
	private PhysicsHingeJointDefinition m_PhysicsHingeJointDefinition;

	[SerializeField]
	private PhysicsRelativeJointDefinition m_PhysicsRelativeJointDefinition;

	[SerializeField]
	private PhysicsSliderJointDefinition m_PhysicsSliderJointDefinition;

	[SerializeField]
	private PhysicsWheelJointDefinition m_PhysicsWheelJointDefinition;

	[Range(1f, 64f)]
	[SerializeField]
	[Header("Globals")]
	private int m_ConcurrentSimulations;

	[Min(1E-05f)]
	[SerializeField]
	private float m_LengthUnitsPerMeter;

	[SerializeField]
	private bool m_DrawInBuild;

	[SerializeField]
	private bool m_BypassLowLevel;

	public PhysicsLayers.LayerNames physicsLayerNames
	{
		get
		{
			return m_PhysicsLayerNames;
		}
		set
		{
			m_PhysicsLayerNames = value;
		}
	}

	public bool useFullLayers
	{
		get
		{
			return m_UseFullLayers;
		}
		set
		{
			m_UseFullLayers = value;
		}
	}

	public PhysicsWorldDefinition physicsWorldDefinition
	{
		get
		{
			return m_PhysicsWorldDefinition;
		}
		set
		{
			m_PhysicsWorldDefinition = value;
		}
	}

	public PhysicsBodyDefinition physicsBodyDefinition
	{
		get
		{
			return m_PhysicsBodyDefinition;
		}
		set
		{
			m_PhysicsBodyDefinition = value;
		}
	}

	public PhysicsShapeDefinition physicsShapeDefinition
	{
		get
		{
			return m_PhysicsShapeDefinition;
		}
		set
		{
			m_PhysicsShapeDefinition = value;
		}
	}

	public PhysicsChainDefinition physicsChainDefinition
	{
		get
		{
			return m_PhysicsChainDefinition;
		}
		set
		{
			m_PhysicsChainDefinition = value;
		}
	}

	[Range(1f, 64f)]
	public int concurrentSimulations
	{
		get
		{
			return m_ConcurrentSimulations;
		}
		set
		{
			m_ConcurrentSimulations = Mathf.Clamp(value, 1, 64);
		}
	}

	public float lengthUnitsPerMeter
	{
		get
		{
			return m_LengthUnitsPerMeter;
		}
		set
		{
			m_LengthUnitsPerMeter = Mathf.Max(1E-05f, value);
		}
	}

	public bool drawInBuild
	{
		get
		{
			return m_DrawInBuild;
		}
		set
		{
			m_DrawInBuild = value;
		}
	}

	public bool bypassLowLevel
	{
		get
		{
			return m_BypassLowLevel;
		}
		set
		{
			m_BypassLowLevel = value;
		}
	}

	public PhysicsLowLevelSettings2D()
	{
		Reset();
	}

	private void Reset()
	{
		m_PhysicsWorldDefinition = new PhysicsWorldDefinition(useSettings: false);
		m_PhysicsBodyDefinition = new PhysicsBodyDefinition(useSettings: false);
		m_PhysicsShapeDefinition = new PhysicsShapeDefinition(useSettings: false);
		m_PhysicsChainDefinition = new PhysicsChainDefinition(useSettings: false);
		m_PhysicsDistanceJointDefinition = new PhysicsDistanceJointDefinition(useSettings: false);
		m_PhysicsFixedJointDefinition = new PhysicsFixedJointDefinition(useSettings: false);
		m_PhysicsHingeJointDefinition = new PhysicsHingeJointDefinition(useSettings: false);
		m_PhysicsRelativeJointDefinition = new PhysicsRelativeJointDefinition(useSettings: false);
		m_PhysicsSliderJointDefinition = new PhysicsSliderJointDefinition(useSettings: false);
		m_PhysicsWheelJointDefinition = new PhysicsWheelJointDefinition(useSettings: false);
		m_PhysicsLayerNames = PhysicsLayers.LayerNames.DefaultLayerNames;
		m_ConcurrentSimulations = 2;
		m_LengthUnitsPerMeter = 1f;
		m_DrawInBuild = false;
		m_BypassLowLevel = false;
	}

	[RequiredByNativeCode]
	private void GetPhysicsLayerNames(out PhysicsLayers.LayerNames layerNames)
	{
		layerNames = m_PhysicsLayerNames;
	}

	[RequiredByNativeCode]
	private void GetPhysicsWorldDefinition(out PhysicsWorldDefinition definition)
	{
		definition = m_PhysicsWorldDefinition;
	}

	[RequiredByNativeCode]
	private void GetPhysicsBodyDefinition(out PhysicsBodyDefinition definition)
	{
		definition = m_PhysicsBodyDefinition;
	}

	[RequiredByNativeCode]
	private void GetPhysicsShapeDefinition(out PhysicsShapeDefinition definition)
	{
		definition = m_PhysicsShapeDefinition;
	}

	[RequiredByNativeCode]
	private void GetPhysicsChainDefinition(out PhysicsChainDefinition definition)
	{
		definition = m_PhysicsChainDefinition;
	}

	[RequiredByNativeCode]
	private void GetPhysicsDistanceJointDefinition(out PhysicsDistanceJointDefinition definition)
	{
		definition = m_PhysicsDistanceJointDefinition;
	}

	[RequiredByNativeCode]
	private void GetPhysicsFixedJointDefinition(out PhysicsFixedJointDefinition definition)
	{
		definition = m_PhysicsFixedJointDefinition;
	}

	[RequiredByNativeCode]
	private void GetPhysicsHingeJointDefinition(out PhysicsHingeJointDefinition definition)
	{
		definition = m_PhysicsHingeJointDefinition;
	}

	[RequiredByNativeCode]
	private void GetPhysicsRelativeJointDefinition(out PhysicsRelativeJointDefinition definition)
	{
		definition = m_PhysicsRelativeJointDefinition;
	}

	[RequiredByNativeCode]
	private void GetPhysicsSliderJointDefinition(out PhysicsSliderJointDefinition definition)
	{
		definition = m_PhysicsSliderJointDefinition;
	}

	[RequiredByNativeCode]
	private void GetPhysicsWheelJointDefinition(out PhysicsWheelJointDefinition definition)
	{
		definition = m_PhysicsWheelJointDefinition;
	}

	[RequiredByNativeCode]
	private int GetConcurrentSimulations()
	{
		return m_ConcurrentSimulations;
	}

	[RequiredByNativeCode]
	private float GetLengthUnitsPerMeter()
	{
		return m_LengthUnitsPerMeter;
	}

	[RequiredByNativeCode]
	private bool GetDrawInBuild()
	{
		return m_DrawInBuild;
	}

	[RequiredByNativeCode]
	private bool GetBypassLowLevel()
	{
		return m_BypassLowLevel;
	}

	[RequiredByNativeCode]
	private bool GetUseFullLayers()
	{
		return m_UseFullLayers;
	}
}
