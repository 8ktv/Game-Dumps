using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("TerrainScriptingClasses.h")]
[NativeHeader("Modules/Terrain/Public/TerrainDataScriptingInterface.h")]
[UsedByNativeCode]
[NativeAsStruct]
public sealed class DetailPrototype
{
	internal static readonly Color DefaultHealthColor = new Color(0.2627451f, 83f / 85f, 14f / 85f, 1f);

	internal static readonly Color DefaultDryColor = new Color(41f / 51f, 0.7372549f, 0.101960786f, 1f);

	[NativeName("prototype")]
	internal GameObject m_Prototype = null;

	[NativeName("prototypeTexture")]
	internal Texture2D m_PrototypeTexture = null;

	[NativeName("healthyColor")]
	internal Color m_HealthyColor = DefaultHealthColor;

	[NativeName("dryColor")]
	internal Color m_DryColor = DefaultDryColor;

	[NativeName("minWidth")]
	internal float m_MinWidth = 1f;

	[NativeName("maxWidth")]
	internal float m_MaxWidth = 2f;

	[NativeName("minHeight")]
	internal float m_MinHeight = 1f;

	[NativeName("maxHeight")]
	internal float m_MaxHeight = 2f;

	[NativeName("noiseSeed")]
	internal int m_NoiseSeed = 0;

	[NativeName("noiseSpread")]
	internal float m_NoiseSpread = 0.1f;

	[NativeName("density")]
	internal float m_Density = 1f;

	[NativeName("holeTestRadius")]
	internal float m_HoleEdgePadding = 0f;

	[NativeName("renderMode")]
	internal int m_RenderMode = 2;

	[NativeName("usePrototypeMesh")]
	internal int m_UsePrototypeMesh = 0;

	[NativeName("useInstancing")]
	internal int m_UseInstancing = 0;

	[NativeName("useDensityScaling")]
	internal int m_UseDensityScaling = 0;

	[NativeName("alignToGround")]
	internal float m_AlignToGround = 0f;

	[NativeName("positionJitter")]
	internal float m_PositionJitter = 0f;

	[NativeName("targetCoverage")]
	internal float m_TargetCoverage = 1f;

	public GameObject prototype
	{
		get
		{
			return m_Prototype;
		}
		set
		{
			m_Prototype = value;
		}
	}

	public Texture2D prototypeTexture
	{
		get
		{
			return m_PrototypeTexture;
		}
		set
		{
			m_PrototypeTexture = value;
		}
	}

	public float minWidth
	{
		get
		{
			return m_MinWidth;
		}
		set
		{
			m_MinWidth = value;
		}
	}

	public float maxWidth
	{
		get
		{
			return m_MaxWidth;
		}
		set
		{
			m_MaxWidth = value;
		}
	}

	public float minHeight
	{
		get
		{
			return m_MinHeight;
		}
		set
		{
			m_MinHeight = value;
		}
	}

	public float maxHeight
	{
		get
		{
			return m_MaxHeight;
		}
		set
		{
			m_MaxHeight = value;
		}
	}

	public int noiseSeed
	{
		get
		{
			return m_NoiseSeed;
		}
		set
		{
			m_NoiseSeed = value;
		}
	}

	public float noiseSpread
	{
		get
		{
			return m_NoiseSpread;
		}
		set
		{
			m_NoiseSpread = value;
		}
	}

	public float density
	{
		get
		{
			return m_Density;
		}
		set
		{
			m_Density = value;
		}
	}

	[Obsolete("bendFactor has no effect and is deprecated.", false)]
	public float bendFactor
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public float holeEdgePadding
	{
		get
		{
			return m_HoleEdgePadding;
		}
		set
		{
			m_HoleEdgePadding = value;
		}
	}

	public Color healthyColor
	{
		get
		{
			return m_HealthyColor;
		}
		set
		{
			m_HealthyColor = value;
		}
	}

	public Color dryColor
	{
		get
		{
			return m_DryColor;
		}
		set
		{
			m_DryColor = value;
		}
	}

	public DetailRenderMode renderMode
	{
		get
		{
			return (DetailRenderMode)m_RenderMode;
		}
		set
		{
			m_RenderMode = (int)value;
		}
	}

	public bool usePrototypeMesh
	{
		get
		{
			return m_UsePrototypeMesh != 0;
		}
		set
		{
			m_UsePrototypeMesh = (value ? 1 : 0);
		}
	}

	public bool useInstancing
	{
		get
		{
			return m_UseInstancing != 0;
		}
		set
		{
			m_UseInstancing = (value ? 1 : 0);
		}
	}

	public float targetCoverage
	{
		get
		{
			return m_TargetCoverage;
		}
		set
		{
			m_TargetCoverage = value;
		}
	}

	public bool useDensityScaling
	{
		get
		{
			return m_UseDensityScaling != 0;
		}
		set
		{
			m_UseDensityScaling = (value ? 1 : 0);
		}
	}

	public float alignToGround
	{
		get
		{
			return m_AlignToGround;
		}
		set
		{
			m_AlignToGround = value;
		}
	}

	public float positionJitter
	{
		get
		{
			return m_PositionJitter;
		}
		set
		{
			m_PositionJitter = value;
		}
	}

	public DetailPrototype()
	{
	}

	public DetailPrototype(DetailPrototype other)
	{
		m_Prototype = other.m_Prototype;
		m_PrototypeTexture = other.m_PrototypeTexture;
		m_HealthyColor = other.m_HealthyColor;
		m_DryColor = other.m_DryColor;
		m_MinWidth = other.m_MinWidth;
		m_MaxWidth = other.m_MaxWidth;
		m_MinHeight = other.m_MinHeight;
		m_MaxHeight = other.m_MaxHeight;
		m_NoiseSeed = other.m_NoiseSeed;
		m_NoiseSpread = other.m_NoiseSpread;
		m_Density = other.m_Density;
		m_HoleEdgePadding = other.m_HoleEdgePadding;
		m_RenderMode = other.m_RenderMode;
		m_UsePrototypeMesh = other.m_UsePrototypeMesh;
		m_UseInstancing = other.m_UseInstancing;
		m_UseDensityScaling = other.m_UseDensityScaling;
		m_AlignToGround = other.m_AlignToGround;
		m_PositionJitter = other.m_PositionJitter;
		m_TargetCoverage = other.m_TargetCoverage;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as DetailPrototype);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	private bool Equals(DetailPrototype other)
	{
		if (other == null)
		{
			return false;
		}
		if (other == this)
		{
			return true;
		}
		if (GetType() != other.GetType())
		{
			return false;
		}
		return m_Prototype == other.m_Prototype && m_PrototypeTexture == other.m_PrototypeTexture && m_HealthyColor == other.m_HealthyColor && m_DryColor == other.m_DryColor && m_MinWidth == other.m_MinWidth && m_MaxWidth == other.m_MaxWidth && m_MinHeight == other.m_MinHeight && m_MaxHeight == other.m_MaxHeight && m_NoiseSeed == other.m_NoiseSeed && m_NoiseSpread == other.m_NoiseSpread && m_Density == other.m_Density && m_HoleEdgePadding == other.m_HoleEdgePadding && m_RenderMode == other.m_RenderMode && m_UsePrototypeMesh == other.m_UsePrototypeMesh && m_UseInstancing == other.m_UseInstancing && m_TargetCoverage == other.m_TargetCoverage && m_UseDensityScaling == other.m_UseDensityScaling;
	}

	public bool Validate()
	{
		string errorMessage;
		return ValidateDetailPrototype(this, out errorMessage);
	}

	public bool Validate(out string errorMessage)
	{
		return ValidateDetailPrototype(this, out errorMessage);
	}

	[FreeFunction("TerrainDataScriptingInterface::ValidateDetailPrototype")]
	internal static bool ValidateDetailPrototype([NotNull] DetailPrototype prototype, out string errorMessage)
	{
		if (prototype == null)
		{
			ThrowHelper.ThrowArgumentNullException(prototype, "prototype");
		}
		ManagedSpanWrapper errorMessage2 = default(ManagedSpanWrapper);
		try
		{
			return ValidateDetailPrototype_Injected(prototype, out errorMessage2);
		}
		finally
		{
			errorMessage = OutStringMarshaller.GetStringAndDispose(errorMessage2);
		}
	}

	internal static bool IsModeSupportedByRenderPipeline(DetailRenderMode renderMode, bool useInstancing, out string errorMessage)
	{
		if (GraphicsSettings.currentRenderPipeline != null)
		{
			if (renderMode == DetailRenderMode.GrassBillboard && GraphicsSettings.GetDefaultShader(DefaultShaderType.TerrainDetailGrassBillboard) == null)
			{
				errorMessage = "The current render pipeline does not support Billboard details. Details will not be rendered.";
				return false;
			}
			if (renderMode == DetailRenderMode.VertexLit && !useInstancing && GraphicsSettings.GetDefaultShader(DefaultShaderType.TerrainDetailLit) == null)
			{
				errorMessage = "The current render pipeline does not support VertexLit details. Details will be rendered using the default shader.";
				return false;
			}
			if (renderMode == DetailRenderMode.Grass && GraphicsSettings.GetDefaultShader(DefaultShaderType.TerrainDetailGrass) == null)
			{
				errorMessage = "The current render pipeline does not support Grass details. Details will be rendered using the default shader without alpha test and animation.";
				return false;
			}
		}
		errorMessage = string.Empty;
		return true;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ValidateDetailPrototype_Injected(DetailPrototype prototype, out ManagedSpanWrapper errorMessage);
}
