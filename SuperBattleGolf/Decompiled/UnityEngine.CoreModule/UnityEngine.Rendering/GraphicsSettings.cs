using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[StaticAccessor("GetGraphicsSettings()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Camera/GraphicsSettings.h")]
public sealed class GraphicsSettings : Object
{
	private static Lazy<RenderPipelineGlobalSettings> s_CurrentRenderPipelineGlobalSettings = new Lazy<RenderPipelineGlobalSettings>(Internal_GetCurrentRenderPipelineGlobalSettings);

	public static extern TransparencySortMode transparencySortMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static Vector3 transparencySortAxis
	{
		get
		{
			get_transparencySortAxis_Injected(out var ret);
			return ret;
		}
		set
		{
			set_transparencySortAxis_Injected(ref value);
		}
	}

	public static extern bool realtimeDirectRectangularAreaLights
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool lightsUseLinearIntensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool lightsUseColorTemperature
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[Obsolete("This property is obsolete. Use RenderingLayerMask API and Tags & Layers project settings instead. #from(23.3)")]
	public static extern uint defaultRenderingLayerMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern Camera.GateFitMode defaultGateFitMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool useScriptableRenderPipelineBatching
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool logWhenShaderIsCompiled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool disableBuiltinCustomRenderTextureUpdate
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern VideoShadersIncludeMode videoShadersIncludeMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern LightProbeOutsideHullStrategy lightProbeOutsideHullStrategy
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeName("CurrentRenderPipeline")]
	private static ScriptableObject INTERNAL_currentRenderPipeline => Unmarshal.UnmarshalUnityObject<ScriptableObject>(get_INTERNAL_currentRenderPipeline_Injected());

	public static RenderPipelineAsset currentRenderPipeline => INTERNAL_currentRenderPipeline as RenderPipelineAsset;

	public static bool isScriptableRenderPipelineEnabled => INTERNAL_currentRenderPipeline != null;

	public static Type currentRenderPipelineAssetType => isScriptableRenderPipelineEnabled ? INTERNAL_currentRenderPipeline.GetType() : null;

	[Obsolete("renderPipelineAsset has been deprecated. Use defaultRenderPipeline instead (UnityUpgradable) -> defaultRenderPipeline", false)]
	public static RenderPipelineAsset renderPipelineAsset
	{
		get
		{
			return defaultRenderPipeline;
		}
		set
		{
			defaultRenderPipeline = value;
		}
	}

	[NativeName("DefaultRenderPipeline")]
	private static ScriptableObject INTERNAL_defaultRenderPipeline
	{
		get
		{
			return Unmarshal.UnmarshalUnityObject<ScriptableObject>(get_INTERNAL_defaultRenderPipeline_Injected());
		}
		set
		{
			set_INTERNAL_defaultRenderPipeline_Injected(MarshalledUnityObject.Marshal(value));
		}
	}

	public static RenderPipelineAsset defaultRenderPipeline
	{
		get
		{
			return INTERNAL_defaultRenderPipeline as RenderPipelineAsset;
		}
		set
		{
			ValidateSetRenderPipelineAsset(value);
			INTERNAL_defaultRenderPipeline = value;
		}
	}

	public static RenderPipelineAsset[] allConfiguredRenderPipelines => GetAllConfiguredRenderPipelines().Cast<RenderPipelineAsset>().ToArray();

	public static extern bool cameraRelativeLightCulling
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool cameraRelativeShadowCulling
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeName("CurrentRenderPipelineGlobalSettings")]
	private static Object INTERNAL_currentRenderPipelineGlobalSettings
	{
		get
		{
			return Unmarshal.UnmarshalUnityObject<Object>(get_INTERNAL_currentRenderPipelineGlobalSettings_Injected());
		}
		set
		{
			set_INTERNAL_currentRenderPipelineGlobalSettings_Injected(MarshalledUnityObject.Marshal(value));
		}
	}

	internal static RenderPipelineGlobalSettings currentRenderPipelineGlobalSettings
	{
		get
		{
			return INTERNAL_currentRenderPipelineGlobalSettings as RenderPipelineGlobalSettings;
		}
		set
		{
			INTERNAL_currentRenderPipelineGlobalSettings = value;
		}
	}

	private GraphicsSettings()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool HasShaderDefine(GraphicsTier tier, BuiltinShaderDefine defineHash);

	public static bool HasShaderDefine(BuiltinShaderDefine defineHash)
	{
		return HasShaderDefine(Graphics.activeTier, defineHash);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetAllConfiguredRenderPipelinesForScript")]
	private static extern ScriptableObject[] GetAllConfiguredRenderPipelines();

	[FreeFunction]
	public static Object GetGraphicsSettings()
	{
		return Unmarshal.UnmarshalUnityObject<Object>(GetGraphicsSettings_Injected());
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetShaderModeScript")]
	public static extern void SetShaderMode(BuiltinShaderType type, BuiltinShaderMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetShaderModeScript")]
	public static extern BuiltinShaderMode GetShaderMode(BuiltinShaderType type);

	[NativeName("SetCustomShaderScript")]
	public static void SetCustomShader(BuiltinShaderType type, Shader shader)
	{
		SetCustomShader_Injected(type, MarshalledUnityObject.Marshal(shader));
	}

	[NativeName("GetCustomShaderScript")]
	public static Shader GetCustomShader(BuiltinShaderType type)
	{
		return Unmarshal.UnmarshalUnityObject<Shader>(GetCustomShader_Injected(type));
	}

	[RequiredByNativeCode]
	[VisibleToOtherModules]
	internal static Shader GetDefaultShader(DefaultShaderType type)
	{
		RenderPipelineAsset renderPipelineAsset = currentRenderPipeline;
		if (currentRenderPipeline == null)
		{
			return null;
		}
		if (1 == 0)
		{
		}
		Shader result = type switch
		{
			DefaultShaderType.Default => renderPipelineAsset.defaultShader, 
			DefaultShaderType.AutodeskInteractive => renderPipelineAsset.autodeskInteractiveShader, 
			DefaultShaderType.AutodeskInteractiveTransparent => renderPipelineAsset.autodeskInteractiveTransparentShader, 
			DefaultShaderType.AutodeskInteractiveMasked => renderPipelineAsset.autodeskInteractiveMaskedShader, 
			DefaultShaderType.TerrainDetailLit => renderPipelineAsset.terrainDetailLitShader, 
			DefaultShaderType.TerrainDetailGrass => renderPipelineAsset.terrainDetailGrassShader, 
			DefaultShaderType.TerrainDetailGrassBillboard => renderPipelineAsset.terrainDetailGrassBillboardShader, 
			DefaultShaderType.SpeedTree7 => renderPipelineAsset.defaultSpeedTree7Shader, 
			DefaultShaderType.SpeedTree8 => renderPipelineAsset.defaultSpeedTree8Shader, 
			DefaultShaderType.SpeedTree9 => renderPipelineAsset.defaultSpeedTree9Shader, 
			_ => throw new NotImplementedException($"DefaultShaderType {type} not implemented"), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[VisibleToOtherModules]
	[RequiredByNativeCode]
	internal static Material GetDefaultMaterial(DefaultMaterialType type)
	{
		RenderPipelineAsset renderPipelineAsset = currentRenderPipeline;
		if (currentRenderPipeline == null)
		{
			return null;
		}
		if (1 == 0)
		{
		}
		Material result = type switch
		{
			DefaultMaterialType.Default => renderPipelineAsset.defaultMaterial, 
			DefaultMaterialType.Particle => renderPipelineAsset.defaultParticleMaterial, 
			DefaultMaterialType.Line => renderPipelineAsset.defaultLineMaterial, 
			DefaultMaterialType.Terrain => renderPipelineAsset.defaultTerrainMaterial, 
			DefaultMaterialType.Sprite => renderPipelineAsset.default2DMaterial, 
			DefaultMaterialType.SpriteMask => renderPipelineAsset.default2DMaskMaterial, 
			DefaultMaterialType.UGUI => renderPipelineAsset.defaultUIMaterial, 
			DefaultMaterialType.UGUI_Overdraw => renderPipelineAsset.defaultUIOverdrawMaterial, 
			DefaultMaterialType.UGUI_ETC1Supported => renderPipelineAsset.defaultUIETC1SupportedMaterial, 
			_ => throw new NotImplementedException($"DefaultMaterialType {type} not implemented"), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[NativeName("RegisterRenderPipelineSettings")]
	private unsafe static void Internal_RegisterRenderPipeline(string renderpipelineName, Object settings)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(renderpipelineName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(renderpipelineName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Internal_RegisterRenderPipeline_Injected(ref managedSpanWrapper, MarshalledUnityObject.Marshal(settings));
					return;
				}
			}
			Internal_RegisterRenderPipeline_Injected(ref managedSpanWrapper, MarshalledUnityObject.Marshal(settings));
		}
		finally
		{
		}
	}

	[NativeName("UnregisterRenderPipelineSettings")]
	private unsafe static void Internal_UnregisterRenderPipeline(string renderpipelineName)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(renderpipelineName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(renderpipelineName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Internal_UnregisterRenderPipeline_Injected(ref managedSpanWrapper);
					return;
				}
			}
			Internal_UnregisterRenderPipeline_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeName("GetSettingsForRenderPipeline")]
	private unsafe static Object Internal_GetSettingsForRenderPipeline(string renderpipelineName)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		Object result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(renderpipelineName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(renderpipelineName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = Internal_GetSettingsForRenderPipeline_Injected(ref managedSpanWrapper);
				}
			}
			else
			{
				gcHandlePtr = Internal_GetSettingsForRenderPipeline_Injected(ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Object>(gcHandlePtr);
		}
		return result;
	}

	private static void CheckRenderPipelineType(Type renderPipelineType)
	{
		if (renderPipelineType == null)
		{
			throw new ArgumentNullException("renderPipelineType");
		}
		if (!typeof(RenderPipeline).IsAssignableFrom(renderPipelineType))
		{
			throw new ArgumentException(string.Format("{0} must be a valid {1}", renderPipelineType, "RenderPipeline"));
		}
	}

	[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset(renderPipelineType, newSettings). #from(23.2)", false)]
	public static void UpdateGraphicsSettings(RenderPipelineGlobalSettings newSettings, Type renderPipelineType)
	{
		CheckRenderPipelineType(renderPipelineType);
		if (newSettings != null)
		{
			Internal_RegisterRenderPipeline(renderPipelineType.FullName, newSettings);
		}
		else
		{
			Internal_UnregisterRenderPipeline(renderPipelineType.FullName);
		}
	}

	[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset(renderPipelineType, settings). #from(23.2)", false)]
	public static void RegisterRenderPipelineSettings(Type renderPipelineType, RenderPipelineGlobalSettings settings)
	{
		CheckRenderPipelineType(renderPipelineType);
		Internal_RegisterRenderPipeline(renderPipelineType.FullName, settings);
	}

	[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset<TRenderPipelineType>(settings). #from(23.2)", false)]
	public static void RegisterRenderPipelineSettings<T>(RenderPipelineGlobalSettings settings) where T : RenderPipeline
	{
		Internal_RegisterRenderPipeline(typeof(T).FullName, settings);
	}

	[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset<TRenderPipelineType>(null). #from(23.2)", false)]
	public static void UnregisterRenderPipelineSettings<T>() where T : RenderPipeline
	{
		Internal_UnregisterRenderPipeline(typeof(T).FullName);
	}

	[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset(renderPipelineType, null). #from(23.2)", false)]
	public static void UnregisterRenderPipelineSettings(Type renderPipelineType)
	{
		CheckRenderPipelineType(renderPipelineType);
		Internal_UnregisterRenderPipeline(renderPipelineType.FullName);
	}

	public static RenderPipelineGlobalSettings GetSettingsForRenderPipeline<T>() where T : RenderPipeline
	{
		return Internal_GetSettingsForRenderPipeline(typeof(T).FullName) as RenderPipelineGlobalSettings;
	}

	public static RenderPipelineGlobalSettings GetSettingsForRenderPipeline(Type renderPipelineType)
	{
		CheckRenderPipelineType(renderPipelineType);
		return Internal_GetSettingsForRenderPipeline(renderPipelineType.FullName) as RenderPipelineGlobalSettings;
	}

	private static RenderPipelineGlobalSettings Internal_GetCurrentRenderPipelineGlobalSettings()
	{
		RenderPipelineGlobalSettings result = null;
		if (currentRenderPipeline != null)
		{
			result = Internal_GetSettingsForRenderPipeline(currentRenderPipeline.pipelineTypeFullName) as RenderPipelineGlobalSettings;
		}
		return result;
	}

	internal static void ValidateSetRenderPipelineAsset(RenderPipelineAsset newRenderPipelineAsset)
	{
		if (!(newRenderPipelineAsset == null) && newRenderPipelineAsset.requiresCompatibleRenderPipelineGlobalSettings)
		{
			if (!TryGetCurrentRenderPipelineGlobalSettings(out var asset))
			{
				throw new InvalidOperationException("Cannot set " + newRenderPipelineAsset.name + " Render Pipeline Asset when there is no current RenderPipelineGlobalSettings set in GraphicsSettings. Make sure your project has one when you build or use TrySetCurrentRenderPipelineGlobalSettings to set it in Player before setting Render Pipeline asset.");
			}
			Type type = newRenderPipelineAsset.GetType();
			if (!SupportedOnRenderPipelineAttribute.IsTypeSupportedOnRenderPipeline(asset.GetType(), type))
			{
				throw new InvalidOperationException("Cannot set " + newRenderPipelineAsset.name + " Render Pipeline Asset of type " + type.Name + " when the current RenderPipelineGlobalSettings is of type " + asset.GetType().Name + ". Make sure your project has Render Pipeline Global Settings for " + newRenderPipelineAsset.name + " render pipeline when you build or use TrySetCurrentRenderPipelineGlobalSettings to set it in Player before setting Render Pipeline asset.");
			}
		}
	}

	public static bool TrySetCurrentRenderPipelineGlobalSettings(RenderPipelineGlobalSettings asset)
	{
		if (asset == null)
		{
			return false;
		}
		currentRenderPipelineGlobalSettings = asset;
		s_CurrentRenderPipelineGlobalSettings = new Lazy<RenderPipelineGlobalSettings>(currentRenderPipelineGlobalSettings);
		RenderPipelineManager.CleanupRenderPipeline();
		return true;
	}

	public static bool TryGetCurrentRenderPipelineGlobalSettings(out RenderPipelineGlobalSettings asset)
	{
		asset = s_CurrentRenderPipelineGlobalSettings.Value;
		return asset != null;
	}

	public static T GetRenderPipelineSettings<T>() where T : class, IRenderPipelineGraphicsSettings
	{
		TryGetRenderPipelineSettings<T>(out var settings);
		return settings;
	}

	public static bool TryGetRenderPipelineSettings<T>(out T settings) where T : class, IRenderPipelineGraphicsSettings
	{
		settings = null;
		if (!TryGetCurrentRenderPipelineGlobalSettings(out var asset))
		{
			return false;
		}
		if (asset.TryGet(typeof(T), out var settings2))
		{
			settings = settings2 as T;
		}
		return settings != null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_transparencySortAxis_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_transparencySortAxis_Injected([In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_INTERNAL_currentRenderPipeline_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_INTERNAL_defaultRenderPipeline_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_INTERNAL_defaultRenderPipeline_Injected(IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetGraphicsSettings_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCustomShader_Injected(BuiltinShaderType type, IntPtr shader);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetCustomShader_Injected(BuiltinShaderType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RegisterRenderPipeline_Injected(ref ManagedSpanWrapper renderpipelineName, IntPtr settings);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_UnregisterRenderPipeline_Injected(ref ManagedSpanWrapper renderpipelineName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_GetSettingsForRenderPipeline_Injected(ref ManagedSpanWrapper renderpipelineName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_INTERNAL_currentRenderPipelineGlobalSettings_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_INTERNAL_currentRenderPipelineGlobalSettings_Injected(IntPtr value);
}
