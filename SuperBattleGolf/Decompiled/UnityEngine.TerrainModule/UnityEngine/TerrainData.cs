using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/Terrain/Public/TerrainDataScriptingInterface.h")]
[NativeHeader("TerrainScriptingClasses.h")]
[UsedByNativeCode]
public sealed class TerrainData : Object
{
	private enum BoundaryValueType
	{
		MaxHeightmapRes,
		MinDetailResPerPatch,
		MaxDetailResPerPatch,
		MaxDetailPatchCount,
		MaxCoveragePerRes,
		MinAlphamapRes,
		MaxAlphamapRes,
		MinBaseMapRes,
		MaxBaseMapRes
	}

	private const string k_ScriptingInterfaceName = "TerrainDataScriptingInterface";

	private const string k_ScriptingInterfacePrefix = "TerrainDataScriptingInterface::";

	private const string k_HeightmapPrefix = "GetHeightmap().";

	private const string k_DetailDatabasePrefix = "GetDetailDatabase().";

	private const string k_TreeDatabasePrefix = "GetTreeDatabase().";

	private const string k_SplatDatabasePrefix = "GetSplatDatabase().";

	internal static readonly int k_MaximumResolution = GetBoundaryValue(BoundaryValueType.MaxHeightmapRes);

	internal static readonly int k_MinimumDetailResolutionPerPatch = GetBoundaryValue(BoundaryValueType.MinDetailResPerPatch);

	internal static readonly int k_MaximumDetailResolutionPerPatch = GetBoundaryValue(BoundaryValueType.MaxDetailResPerPatch);

	internal static readonly int k_MaximumDetailPatchCount = GetBoundaryValue(BoundaryValueType.MaxDetailPatchCount);

	internal static readonly int k_MinimumAlphamapResolution = GetBoundaryValue(BoundaryValueType.MinAlphamapRes);

	internal static readonly int k_MaximumAlphamapResolution = GetBoundaryValue(BoundaryValueType.MaxAlphamapRes);

	internal static readonly int k_MinimumBaseMapResolution = GetBoundaryValue(BoundaryValueType.MinBaseMapRes);

	internal static readonly int k_MaximumBaseMapResolution = GetBoundaryValue(BoundaryValueType.MaxBaseMapRes);

	[Obsolete("Please use heightmapResolution instead. (UnityUpgradable) -> heightmapResolution", false)]
	public int heightmapWidth => heightmapResolution;

	[Obsolete("Please use heightmapResolution instead. (UnityUpgradable) -> heightmapResolution", false)]
	public int heightmapHeight => heightmapResolution;

	public RenderTexture heightmapTexture
	{
		[NativeName("GetHeightmap().GetHeightmapTexture")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(get_heightmapTexture_Injected(intPtr));
		}
	}

	public int heightmapResolution
	{
		get
		{
			return internalHeightmapResolution;
		}
		set
		{
			int num = value;
			if (value < 0 || value > k_MaximumResolution)
			{
				Debug.LogWarning("heightmapResolution is clamped to the range of [0, " + k_MaximumResolution + "].");
				num = Math.Min(k_MaximumResolution, Math.Max(value, 0));
			}
			internalHeightmapResolution = num;
		}
	}

	private int internalHeightmapResolution
	{
		[NativeName("GetHeightmap().GetResolution")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_internalHeightmapResolution_Injected(intPtr);
		}
		[NativeName("GetHeightmap().SetResolution")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_internalHeightmapResolution_Injected(intPtr, value);
		}
	}

	public Vector3 heightmapScale
	{
		[NativeName("GetHeightmap().GetScale")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_heightmapScale_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Texture holesTexture
	{
		get
		{
			if (IsHolesTextureCompressed())
			{
				return GetCompressedHolesTexture();
			}
			return GetHolesTexture();
		}
	}

	public bool enableHolesTextureCompression
	{
		[NativeName("GetHeightmap().GetEnableHolesTextureCompression")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enableHolesTextureCompression_Injected(intPtr);
		}
		[NativeName("GetHeightmap().SetEnableHolesTextureCompression")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enableHolesTextureCompression_Injected(intPtr, value);
		}
	}

	internal RenderTexture holesRenderTexture => GetHolesTexture();

	public int holesResolution => heightmapResolution - 1;

	public Vector3 size
	{
		[NativeName("GetHeightmap().GetSize")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_size_Injected(intPtr, out var ret);
			return ret;
		}
		[NativeName("GetHeightmap().SetSize")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_size_Injected(intPtr, ref value);
		}
	}

	public Bounds bounds
	{
		[NativeName("GetHeightmap().CalculateBounds")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_bounds_Injected(intPtr, out var ret);
			return ret;
		}
	}

	[Obsolete("Terrain thickness is no longer required by the physics engine. Set appropriate continuous collision detection modes to fast moving bodies.")]
	public float thickness
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public float wavingGrassStrength
	{
		[NativeName("GetDetailDatabase().GetWavingGrassStrength")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wavingGrassStrength_Injected(intPtr);
		}
		[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassStrength", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wavingGrassStrength_Injected(intPtr, value);
		}
	}

	public float wavingGrassAmount
	{
		[NativeName("GetDetailDatabase().GetWavingGrassAmount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wavingGrassAmount_Injected(intPtr);
		}
		[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassAmount", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wavingGrassAmount_Injected(intPtr, value);
		}
	}

	public float wavingGrassSpeed
	{
		[NativeName("GetDetailDatabase().GetWavingGrassSpeed")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wavingGrassSpeed_Injected(intPtr);
		}
		[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassSpeed", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wavingGrassSpeed_Injected(intPtr, value);
		}
	}

	public Color wavingGrassTint
	{
		[NativeName("GetDetailDatabase().GetWavingGrassTint")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_wavingGrassTint_Injected(intPtr, out var ret);
			return ret;
		}
		[FreeFunction("TerrainDataScriptingInterface::SetWavingGrassTint", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wavingGrassTint_Injected(intPtr, ref value);
		}
	}

	public int detailWidth
	{
		[NativeName("GetDetailDatabase().GetWidth")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detailWidth_Injected(intPtr);
		}
	}

	public int detailHeight
	{
		[NativeName("GetDetailDatabase().GetHeight")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detailHeight_Injected(intPtr);
		}
	}

	public int maxDetailScatterPerRes
	{
		[NativeName("GetDetailDatabase().GetMaximumScatterPerRes")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maxDetailScatterPerRes_Injected(intPtr);
		}
	}

	public int detailPatchCount
	{
		[NativeName("GetDetailDatabase().GetPatchCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detailPatchCount_Injected(intPtr);
		}
	}

	public int detailResolution
	{
		[NativeName("GetDetailDatabase().GetResolution")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detailResolution_Injected(intPtr);
		}
	}

	public int detailResolutionPerPatch
	{
		[NativeName("GetDetailDatabase().GetResolutionPerPatch")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detailResolutionPerPatch_Injected(intPtr);
		}
	}

	public DetailScatterMode detailScatterMode
	{
		[NativeName("GetDetailDatabase().GetDetailScatterMode")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detailScatterMode_Injected(intPtr);
		}
	}

	public DetailPrototype[] detailPrototypes
	{
		[FreeFunction("TerrainDataScriptingInterface::GetDetailPrototypes", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detailPrototypes_Injected(intPtr);
		}
		[FreeFunction("TerrainDataScriptingInterface::SetDetailPrototypes", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_detailPrototypes_Injected(intPtr, value);
		}
	}

	public TreeInstance[] treeInstances
	{
		get
		{
			return Internal_GetTreeInstances();
		}
		set
		{
			SetTreeInstances(value, snapToHeightmap: false);
		}
	}

	public int treeInstanceCount
	{
		[NativeName("GetTreeDatabase().GetInstances().size")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_treeInstanceCount_Injected(intPtr);
		}
	}

	public TreePrototype[] treePrototypes
	{
		[FreeFunction("TerrainDataScriptingInterface::GetTreePrototypes", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_treePrototypes_Injected(intPtr);
		}
		[FreeFunction("TerrainDataScriptingInterface::SetTreePrototypes", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_treePrototypes_Injected(intPtr, value);
		}
	}

	public int alphamapLayers
	{
		[NativeName("GetSplatDatabase().GetSplatCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_alphamapLayers_Injected(intPtr);
		}
	}

	public int alphamapResolution
	{
		get
		{
			return Internal_alphamapResolution;
		}
		set
		{
			int internal_alphamapResolution = value;
			if (value < k_MinimumAlphamapResolution || value > k_MaximumAlphamapResolution)
			{
				Debug.LogWarning("alphamapResolution is clamped to the range of [" + k_MinimumAlphamapResolution + ", " + k_MaximumAlphamapResolution + "].");
				internal_alphamapResolution = Math.Min(k_MaximumAlphamapResolution, Math.Max(value, k_MinimumAlphamapResolution));
			}
			Internal_alphamapResolution = internal_alphamapResolution;
		}
	}

	private int Internal_alphamapResolution
	{
		[NativeName("GetSplatDatabase().GetAlphamapResolution")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_Internal_alphamapResolution_Injected(intPtr);
		}
		[NativeName("GetSplatDatabase().SetAlphamapResolution")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_Internal_alphamapResolution_Injected(intPtr, value);
		}
	}

	public int alphamapWidth => alphamapResolution;

	public int alphamapHeight => alphamapResolution;

	public int baseMapResolution
	{
		get
		{
			return Internal_baseMapResolution;
		}
		set
		{
			int internal_baseMapResolution = value;
			if (value < k_MinimumBaseMapResolution || value > k_MaximumBaseMapResolution)
			{
				Debug.LogWarning("baseMapResolution is clamped to the range of [" + k_MinimumBaseMapResolution + ", " + k_MaximumBaseMapResolution + "].");
				internal_baseMapResolution = Math.Min(k_MaximumBaseMapResolution, Math.Max(value, k_MinimumBaseMapResolution));
			}
			Internal_baseMapResolution = internal_baseMapResolution;
		}
	}

	private int Internal_baseMapResolution
	{
		[NativeName("GetSplatDatabase().GetBaseMapResolution")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_Internal_baseMapResolution_Injected(intPtr);
		}
		[NativeName("GetSplatDatabase().SetBaseMapResolution")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_Internal_baseMapResolution_Injected(intPtr, value);
		}
	}

	public int alphamapTextureCount
	{
		[NativeName("GetSplatDatabase().GetAlphaTextureCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_alphamapTextureCount_Injected(intPtr);
		}
	}

	public Texture2D[] alphamapTextures
	{
		get
		{
			Texture2D[] array = new Texture2D[alphamapTextureCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GetAlphamapTexture(i);
			}
			return array;
		}
	}

	[Obsolete("TerrainData.splatPrototypes is obsolete. Use TerrainData.terrainLayers instead.", false)]
	public SplatPrototype[] splatPrototypes
	{
		[FreeFunction("TerrainDataScriptingInterface::GetSplatPrototypes", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_splatPrototypes_Injected(intPtr);
		}
		[FreeFunction("TerrainDataScriptingInterface::SetSplatPrototypes", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_splatPrototypes_Injected(intPtr, value);
		}
	}

	public TerrainLayer[] terrainLayers
	{
		[FreeFunction("TerrainDataScriptingInterface::GetTerrainLayers", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_terrainLayers_Injected(intPtr);
		}
		[FreeFunction("TerrainDataScriptingInterface::SetTerrainLayers", HasExplicitThis = true)]
		[param: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_terrainLayers_Injected(intPtr, value);
		}
	}

	internal TextureFormat atlasFormat
	{
		[NativeName("GetDetailDatabase().GetAtlasTextureFormat")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_atlasFormat_Injected(intPtr);
		}
	}

	internal Terrain[] users
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_users_Injected(intPtr);
		}
	}

	private static bool SupportsCopyTextureBetweenRTAndTexture => (SystemInfo.copyTextureSupport & (CopyTextureSupport.TextureToRT | CopyTextureSupport.RTToTexture)) == (CopyTextureSupport.TextureToRT | CopyTextureSupport.RTToTexture);

	public static string AlphamapTextureName => "alphamap";

	public static string HolesTextureName => "holes";

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[StaticAccessor("TerrainDataScriptingInterface", StaticAccessorType.DoubleColon)]
	private static extern int GetBoundaryValue(BoundaryValueType type);

	public TerrainData()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TerrainDataScriptingInterface::Create")]
	private static extern void Internal_Create([Writable] TerrainData terrainData);

	[Obsolete("Please use DirtyHeightmapRegion instead.", false)]
	public void UpdateDirtyRegion(int x, int y, int width, int height, bool syncHeightmapTextureImmediately)
	{
		DirtyHeightmapRegion(new RectInt(x, y, width, height), syncHeightmapTextureImmediately ? TerrainHeightmapSyncControl.HeightOnly : TerrainHeightmapSyncControl.None);
	}

	[NativeName("GetHeightmap().IsHolesTextureCompressed")]
	internal bool IsHolesTextureCompressed()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsHolesTextureCompressed_Injected(intPtr);
	}

	[NativeName("GetHeightmap().GetHolesTexture")]
	internal RenderTexture GetHolesTexture()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<RenderTexture>(GetHolesTexture_Injected(intPtr));
	}

	[NativeName("GetHeightmap().GetCompressedHolesTexture")]
	internal Texture2D GetCompressedHolesTexture()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Texture2D>(GetCompressedHolesTexture_Injected(intPtr));
	}

	[NativeName("GetHeightmap().GetHeight")]
	public float GetHeight(int x, int y)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetHeight_Injected(intPtr, x, y);
	}

	[NativeName("GetHeightmap().GetInterpolatedHeight")]
	public float GetInterpolatedHeight(float x, float y)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetInterpolatedHeight_Injected(intPtr, x, y);
	}

	public float[,] GetInterpolatedHeights(float xBase, float yBase, int xCount, int yCount, float xInterval, float yInterval)
	{
		if (xCount <= 0)
		{
			throw new ArgumentOutOfRangeException("xCount");
		}
		if (yCount <= 0)
		{
			throw new ArgumentOutOfRangeException("yCount");
		}
		float[,] array = new float[yCount, xCount];
		Internal_GetInterpolatedHeights(array, xCount, 0, 0, xBase, yBase, xCount, yCount, xInterval, yInterval);
		return array;
	}

	public void GetInterpolatedHeights(float[,] results, int resultXOffset, int resultYOffset, float xBase, float yBase, int xCount, int yCount, float xInterval, float yInterval)
	{
		if (results == null)
		{
			throw new ArgumentNullException("results");
		}
		if (xCount <= 0)
		{
			throw new ArgumentOutOfRangeException("xCount");
		}
		if (yCount <= 0)
		{
			throw new ArgumentOutOfRangeException("yCount");
		}
		if (resultXOffset < 0 || resultXOffset + xCount > results.GetLength(1))
		{
			throw new ArgumentOutOfRangeException("resultXOffset");
		}
		if (resultYOffset < 0 || resultYOffset + yCount > results.GetLength(0))
		{
			throw new ArgumentOutOfRangeException("resultYOffset");
		}
		Internal_GetInterpolatedHeights(results, results.GetLength(1), resultXOffset, resultYOffset, xBase, yBase, xCount, yCount, xInterval, yInterval);
	}

	[FreeFunction("TerrainDataScriptingInterface::GetInterpolatedHeights", HasExplicitThis = true)]
	private unsafe void Internal_GetInterpolatedHeights(float[,] results, int resultXDimension, int resultXOffset, int resultYOffset, float xBase, float yBase, int xCount, int yCount, float xInterval, float yInterval)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		fixed (float[,] array = results)
		{
			int length;
			nint begin;
			if (results == null || (length = array.Length) == 0)
			{
				length = 0;
				begin = 0;
			}
			else
			{
				begin = (nint)Unsafe.AsPointer(ref array[0, 0]);
			}
			ManagedSpanWrapper results2 = new ManagedSpanWrapper((void*)begin, length);
			Internal_GetInterpolatedHeights_Injected(intPtr, ref results2, resultXDimension, resultXOffset, resultYOffset, xBase, yBase, xCount, yCount, xInterval, yInterval);
		}
	}

	public float[,] GetHeights(int xBase, int yBase, int width, int height)
	{
		if (xBase < 0 || yBase < 0 || xBase + width < 0 || yBase + height < 0 || xBase + width > heightmapResolution || yBase + height > heightmapResolution)
		{
			throw new ArgumentException("Trying to access out-of-bounds terrain height information.");
		}
		return Internal_GetHeights(xBase, yBase, width, height);
	}

	[FreeFunction("TerrainDataScriptingInterface::GetHeights", HasExplicitThis = true)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	private float[,] Internal_GetHeights(int xBase, int yBase, int width, int height)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_GetHeights_Injected(intPtr, xBase, yBase, width, height);
	}

	public void SetHeights(int xBase, int yBase, float[,] heights)
	{
		if (heights == null)
		{
			throw new NullReferenceException();
		}
		if (xBase + heights.GetLength(1) > heightmapResolution || xBase + heights.GetLength(1) < 0 || yBase + heights.GetLength(0) < 0 || xBase < 0 || yBase < 0 || yBase + heights.GetLength(0) > heightmapResolution)
		{
			throw new ArgumentException(string.Format("X or Y base out of bounds. Setting up to {0}x{1} while map size is {2}x{2}", xBase + heights.GetLength(1), yBase + heights.GetLength(0), heightmapResolution));
		}
		Internal_SetHeights(xBase, yBase, heights.GetLength(1), heights.GetLength(0), heights);
	}

	[FreeFunction("TerrainDataScriptingInterface::SetHeights", HasExplicitThis = true)]
	private unsafe void Internal_SetHeights(int xBase, int yBase, int width, int height, float[,] heights)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		fixed (float[,] array = heights)
		{
			int length;
			nint begin;
			if (heights == null || (length = array.Length) == 0)
			{
				length = 0;
				begin = 0;
			}
			else
			{
				begin = (nint)Unsafe.AsPointer(ref array[0, 0]);
			}
			ManagedSpanWrapper heights2 = new ManagedSpanWrapper((void*)begin, length);
			Internal_SetHeights_Injected(intPtr, xBase, yBase, width, height, ref heights2);
		}
	}

	[FreeFunction("TerrainDataScriptingInterface::GetPatchMinMaxHeights", HasExplicitThis = true)]
	public PatchExtents[] GetPatchMinMaxHeights()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		PatchExtents[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetPatchMinMaxHeights_Injected(intPtr, out ret);
		}
		finally
		{
			PatchExtents[] array = default(PatchExtents[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("TerrainDataScriptingInterface::OverrideMinMaxPatchHeights", HasExplicitThis = true)]
	public unsafe void OverrideMinMaxPatchHeights(PatchExtents[] minMaxHeights)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<PatchExtents> span = new Span<PatchExtents>(minMaxHeights);
		fixed (PatchExtents* begin = span)
		{
			ManagedSpanWrapper minMaxHeights2 = new ManagedSpanWrapper(begin, span.Length);
			OverrideMinMaxPatchHeights_Injected(intPtr, ref minMaxHeights2);
		}
	}

	[FreeFunction("TerrainDataScriptingInterface::GetMaximumHeightError", HasExplicitThis = true)]
	public float[] GetMaximumHeightError()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		float[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetMaximumHeightError_Injected(intPtr, out ret);
		}
		finally
		{
			float[] array = default(float[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("TerrainDataScriptingInterface::OverrideMaximumHeightError", HasExplicitThis = true)]
	public unsafe void OverrideMaximumHeightError(float[] maxError)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(maxError);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper maxError2 = new ManagedSpanWrapper(begin, span.Length);
			OverrideMaximumHeightError_Injected(intPtr, ref maxError2);
		}
	}

	public void SetHeightsDelayLOD(int xBase, int yBase, float[,] heights)
	{
		if (heights == null)
		{
			throw new ArgumentNullException("heights");
		}
		int length = heights.GetLength(0);
		int length2 = heights.GetLength(1);
		if (xBase < 0 || xBase + length2 < 0 || xBase + length2 > heightmapResolution)
		{
			throw new ArgumentException($"X out of bounds - trying to set {xBase}-{xBase + length2} but the terrain ranges from 0-{heightmapResolution}");
		}
		if (yBase < 0 || yBase + length < 0 || yBase + length > heightmapResolution)
		{
			throw new ArgumentException($"Y out of bounds - trying to set {yBase}-{yBase + length} but the terrain ranges from 0-{heightmapResolution}");
		}
		Internal_SetHeightsDelayLOD(xBase, yBase, length2, length, heights);
	}

	[FreeFunction("TerrainDataScriptingInterface::SetHeightsDelayLOD", HasExplicitThis = true)]
	private unsafe void Internal_SetHeightsDelayLOD(int xBase, int yBase, int width, int height, float[,] heights)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		fixed (float[,] array = heights)
		{
			int length;
			nint begin;
			if (heights == null || (length = array.Length) == 0)
			{
				length = 0;
				begin = 0;
			}
			else
			{
				begin = (nint)Unsafe.AsPointer(ref array[0, 0]);
			}
			ManagedSpanWrapper heights2 = new ManagedSpanWrapper((void*)begin, length);
			Internal_SetHeightsDelayLOD_Injected(intPtr, xBase, yBase, width, height, ref heights2);
		}
	}

	public bool IsHole(int x, int y)
	{
		if (x < 0 || x >= holesResolution || y < 0 || y >= holesResolution)
		{
			throw new ArgumentException("Trying to access out-of-bounds terrain holes information.");
		}
		return Internal_IsHole(x, y);
	}

	public bool[,] GetHoles(int xBase, int yBase, int width, int height)
	{
		if (xBase < 0 || yBase < 0 || width <= 0 || height <= 0 || xBase + width > holesResolution || yBase + height > holesResolution)
		{
			throw new ArgumentException("Trying to access out-of-bounds terrain holes information.");
		}
		return Internal_GetHoles(xBase, yBase, width, height);
	}

	public void SetHoles(int xBase, int yBase, bool[,] holes)
	{
		if (holes == null)
		{
			throw new ArgumentNullException("holes");
		}
		int length = holes.GetLength(0);
		int length2 = holes.GetLength(1);
		if (xBase < 0 || xBase + length2 > holesResolution)
		{
			throw new ArgumentException($"X out of bounds - trying to set {xBase}-{xBase + length2} but the terrain ranges from 0-{holesResolution}");
		}
		if (yBase < 0 || yBase + length > holesResolution)
		{
			throw new ArgumentException($"Y out of bounds - trying to set {yBase}-{yBase + length} but the terrain ranges from 0-{holesResolution}");
		}
		Internal_SetHoles(xBase, yBase, holes.GetLength(1), holes.GetLength(0), holes);
	}

	public void SetHolesDelayLOD(int xBase, int yBase, bool[,] holes)
	{
		if (holes == null)
		{
			throw new ArgumentNullException("holes");
		}
		int length = holes.GetLength(0);
		int length2 = holes.GetLength(1);
		if (xBase < 0 || xBase + length2 > holesResolution)
		{
			throw new ArgumentException($"X out of bounds - trying to set {xBase}-{xBase + length2} but the terrain ranges from 0-{holesResolution}");
		}
		if (yBase < 0 || yBase + length > holesResolution)
		{
			throw new ArgumentException($"Y out of bounds - trying to set {yBase}-{yBase + length} but the terrain ranges from 0-{holesResolution}");
		}
		Internal_SetHolesDelayLOD(xBase, yBase, length2, length, holes);
	}

	[FreeFunction("TerrainDataScriptingInterface::SetHoles", HasExplicitThis = true)]
	private unsafe void Internal_SetHoles(int xBase, int yBase, int width, int height, bool[,] holes)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		fixed (bool[,] array = holes)
		{
			int length;
			nint begin;
			if (holes == null || (length = array.Length) == 0)
			{
				length = 0;
				begin = 0;
			}
			else
			{
				begin = (nint)Unsafe.AsPointer(ref array[0, 0]);
			}
			ManagedSpanWrapper holes2 = new ManagedSpanWrapper((void*)begin, length);
			Internal_SetHoles_Injected(intPtr, xBase, yBase, width, height, ref holes2);
		}
	}

	[FreeFunction("TerrainDataScriptingInterface::GetHoles", HasExplicitThis = true)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	private bool[,] Internal_GetHoles(int xBase, int yBase, int width, int height)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_GetHoles_Injected(intPtr, xBase, yBase, width, height);
	}

	[FreeFunction("TerrainDataScriptingInterface::IsHole", HasExplicitThis = true)]
	private bool Internal_IsHole(int x, int y)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_IsHole_Injected(intPtr, x, y);
	}

	[FreeFunction("TerrainDataScriptingInterface::SetHolesDelayLOD", HasExplicitThis = true)]
	private unsafe void Internal_SetHolesDelayLOD(int xBase, int yBase, int width, int height, bool[,] holes)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		fixed (bool[,] array = holes)
		{
			int length;
			nint begin;
			if (holes == null || (length = array.Length) == 0)
			{
				length = 0;
				begin = 0;
			}
			else
			{
				begin = (nint)Unsafe.AsPointer(ref array[0, 0]);
			}
			ManagedSpanWrapper holes2 = new ManagedSpanWrapper((void*)begin, length);
			Internal_SetHolesDelayLOD_Injected(intPtr, xBase, yBase, width, height, ref holes2);
		}
	}

	[NativeName("GetHeightmap().GetSteepness")]
	public float GetSteepness(float x, float y)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetSteepness_Injected(intPtr, x, y);
	}

	[NativeName("GetHeightmap().GetInterpolatedNormal")]
	public Vector3 GetInterpolatedNormal(float x, float y)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetInterpolatedNormal_Injected(intPtr, x, y, out var ret);
		return ret;
	}

	[NativeName("GetHeightmap().GetAdjustedSize")]
	internal int GetAdjustedSize(int size)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAdjustedSize_Injected(intPtr, size);
	}

	public void SetDetailResolution(int detailResolution, int resolutionPerPatch)
	{
		if (detailResolution < 0)
		{
			Debug.LogWarning("detailResolution must not be negative.");
			detailResolution = 0;
		}
		if (resolutionPerPatch < k_MinimumDetailResolutionPerPatch || resolutionPerPatch > k_MaximumDetailResolutionPerPatch)
		{
			Debug.LogWarning("resolutionPerPatch is clamped to the range of [" + k_MinimumDetailResolutionPerPatch + ", " + k_MaximumDetailResolutionPerPatch + "].");
			resolutionPerPatch = Math.Min(k_MaximumDetailResolutionPerPatch, Math.Max(resolutionPerPatch, k_MinimumDetailResolutionPerPatch));
		}
		int num = detailResolution / resolutionPerPatch;
		if (num > k_MaximumDetailPatchCount)
		{
			Debug.LogWarning("Patch count (detailResolution / resolutionPerPatch) is clamped to the range of [0, " + k_MaximumDetailPatchCount + "].");
			num = Math.Min(k_MaximumDetailPatchCount, Math.Max(num, 0));
		}
		Internal_SetDetailResolution(num, resolutionPerPatch);
	}

	[NativeName("GetDetailDatabase().SetDetailResolution")]
	private void Internal_SetDetailResolution(int patchCount, int resolutionPerPatch)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_SetDetailResolution_Injected(intPtr, patchCount, resolutionPerPatch);
	}

	public void SetDetailScatterMode(DetailScatterMode scatterMode)
	{
		Internal_SetDetailScatterMode(scatterMode);
	}

	[NativeName("GetDetailDatabase().SetDetailScatterMode")]
	private void Internal_SetDetailScatterMode(DetailScatterMode scatterMode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_SetDetailScatterMode_Injected(intPtr, scatterMode);
	}

	[NativeName("GetDetailDatabase().ResetDirtyDetails")]
	internal void ResetDirtyDetails()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetDirtyDetails_Injected(intPtr);
	}

	[FreeFunction("TerrainDataScriptingInterface::RefreshPrototypes", HasExplicitThis = true)]
	public void RefreshPrototypes()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RefreshPrototypes_Injected(intPtr);
	}

	[FreeFunction("TerrainDataScriptingInterface::GetSupportedLayers", HasExplicitThis = true)]
	public int[] GetSupportedLayers(int xBase, int yBase, int totalWidth, int totalHeight)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		int[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetSupportedLayers_Injected(intPtr, xBase, yBase, totalWidth, totalHeight, out ret);
		}
		finally
		{
			int[] array = default(int[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public int[] GetSupportedLayers(Vector2Int positionBase, Vector2Int size)
	{
		return GetSupportedLayers(positionBase.x, positionBase.y, size.x, size.y);
	}

	[FreeFunction("TerrainDataScriptingInterface::GetDetailLayer", HasExplicitThis = true)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public int[,] GetDetailLayer(int xBase, int yBase, int width, int height, int layer)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDetailLayer_Injected(intPtr, xBase, yBase, width, height, layer);
	}

	public int[,] GetDetailLayer(Vector2Int positionBase, Vector2Int size, int layer)
	{
		return GetDetailLayer(positionBase.x, positionBase.y, size.x, size.y, layer);
	}

	[FreeFunction("TerrainDataScriptingInterface::ComputeDetailInstanceTransforms", HasExplicitThis = true)]
	public DetailInstanceTransform[] ComputeDetailInstanceTransforms(int patchX, int patchY, int layer, float density, out Bounds bounds)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		DetailInstanceTransform[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeDetailInstanceTransforms_Injected(intPtr, patchX, patchY, layer, density, out bounds, out ret);
		}
		finally
		{
			DetailInstanceTransform[] array = default(DetailInstanceTransform[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("TerrainDataScriptingInterface::ComputeDetailCoverage", HasExplicitThis = true)]
	public float ComputeDetailCoverage(int detailPrototypeIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ComputeDetailCoverage_Injected(intPtr, detailPrototypeIndex);
	}

	public void SetDetailLayer(int xBase, int yBase, int layer, int[,] details)
	{
		Internal_SetDetailLayer(xBase, yBase, details.GetLength(1), details.GetLength(0), layer, details);
	}

	public void SetDetailLayer(Vector2Int basePosition, int layer, int[,] details)
	{
		SetDetailLayer(basePosition.x, basePosition.y, layer, details);
	}

	[FreeFunction("TerrainDataScriptingInterface::SetDetailLayer", HasExplicitThis = true)]
	private unsafe void Internal_SetDetailLayer(int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, int[,] data)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		fixed (int[,] array = data)
		{
			int length;
			nint begin;
			if (data == null || (length = array.Length) == 0)
			{
				length = 0;
				begin = 0;
			}
			else
			{
				begin = (nint)Unsafe.AsPointer(ref array[0, 0]);
			}
			ManagedSpanWrapper data2 = new ManagedSpanWrapper((void*)begin, length);
			Internal_SetDetailLayer_Injected(intPtr, xBase, yBase, totalWidth, totalHeight, detailIndex, ref data2);
		}
	}

	[NativeName("GetTreeDatabase().GetInstances")]
	private TreeInstance[] Internal_GetTreeInstances()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		TreeInstance[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Internal_GetTreeInstances_Injected(intPtr, out ret);
		}
		finally
		{
			TreeInstance[] array = default(TreeInstance[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("TerrainDataScriptingInterface::SetTreeInstances", HasExplicitThis = true)]
	public unsafe void SetTreeInstances([NotNull] TreeInstance[] instances, bool snapToHeightmap)
	{
		if (instances == null)
		{
			ThrowHelper.ThrowArgumentNullException(instances, "instances");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<TreeInstance> span = new Span<TreeInstance>(instances);
		fixed (TreeInstance* begin = span)
		{
			ManagedSpanWrapper instances2 = new ManagedSpanWrapper(begin, span.Length);
			SetTreeInstances_Injected(intPtr, ref instances2, snapToHeightmap);
		}
	}

	public TreeInstance GetTreeInstance(int index)
	{
		if (index < 0 || index >= treeInstanceCount)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		return Internal_GetTreeInstance(index);
	}

	[FreeFunction("TerrainDataScriptingInterface::GetTreeInstance", HasExplicitThis = true)]
	private TreeInstance Internal_GetTreeInstance(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_GetTreeInstance_Injected(intPtr, index, out var ret);
		return ret;
	}

	[NativeThrows]
	[FreeFunction("TerrainDataScriptingInterface::SetTreeInstance", HasExplicitThis = true)]
	public void SetTreeInstance(int index, TreeInstance instance)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTreeInstance_Injected(intPtr, index, ref instance);
	}

	[NativeName("GetTreeDatabase().RemoveTreePrototype")]
	internal void RemoveTreePrototype(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveTreePrototype_Injected(intPtr, index);
	}

	[NativeName("GetDetailDatabase().RemoveDetailPrototype")]
	public void RemoveDetailPrototype(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveDetailPrototype_Injected(intPtr, index);
	}

	[NativeName("GetTreeDatabase().NeedUpgradeScaledPrototypes")]
	internal bool NeedUpgradeScaledTreePrototypes()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return NeedUpgradeScaledTreePrototypes_Injected(intPtr);
	}

	[FreeFunction("TerrainDataScriptingInterface::UpgradeScaledTreePrototype", HasExplicitThis = true)]
	internal void UpgradeScaledTreePrototype()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UpgradeScaledTreePrototype_Injected(intPtr);
	}

	public float[,,] GetAlphamaps(int x, int y, int width, int height)
	{
		if (x < 0 || y < 0 || width < 0 || height < 0)
		{
			throw new ArgumentException("Invalid argument for GetAlphaMaps");
		}
		return Internal_GetAlphamaps(x, y, width, height);
	}

	[FreeFunction("TerrainDataScriptingInterface::GetAlphamaps", HasExplicitThis = true)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	private float[,,] Internal_GetAlphamaps(int x, int y, int width, int height)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_GetAlphamaps_Injected(intPtr, x, y, width, height);
	}

	[RequiredByNativeCode]
	[NativeName("GetSplatDatabase().GetAlphamapResolution")]
	internal float GetAlphamapResolutionInternal()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAlphamapResolutionInternal_Injected(intPtr);
	}

	public void SetAlphamaps(int x, int y, float[,,] map)
	{
		if (map.GetLength(2) != alphamapLayers)
		{
			throw new Exception($"Float array size wrong (layers should be {alphamapLayers})");
		}
		Internal_SetAlphamaps(x, y, map.GetLength(1), map.GetLength(0), map);
	}

	[FreeFunction("TerrainDataScriptingInterface::SetAlphamaps", HasExplicitThis = true)]
	private unsafe void Internal_SetAlphamaps(int x, int y, int width, int height, float[,,] map)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		fixed (float[,,] array = map)
		{
			int length;
			nint begin;
			if (map == null || (length = array.Length) == 0)
			{
				length = 0;
				begin = 0;
			}
			else
			{
				begin = (nint)Unsafe.AsPointer(ref array[0, 0, 0]);
			}
			ManagedSpanWrapper map2 = new ManagedSpanWrapper((void*)begin, length);
			Internal_SetAlphamaps_Injected(intPtr, x, y, width, height, ref map2);
		}
	}

	[NativeName("GetSplatDatabase().SetBaseMapsDirty")]
	public void SetBaseMapDirty()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBaseMapDirty_Injected(intPtr);
	}

	[NativeName("GetSplatDatabase().GetAlphaTexture")]
	public Texture2D GetAlphamapTexture(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Texture2D>(GetAlphamapTexture_Injected(intPtr, index));
	}

	[NativeName("GetTreeDatabase().AddTree")]
	internal void AddTree(ref TreeInstance tree)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddTree_Injected(intPtr, ref tree);
	}

	[NativeName("GetTreeDatabase().RemoveTrees")]
	internal int RemoveTrees(Vector2 position, float radius, int prototypeIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveTrees_Injected(intPtr, ref position, radius, prototypeIndex);
	}

	[NativeName("GetHeightmap().CopyHeightmapFromActiveRenderTexture")]
	private void Internal_CopyActiveRenderTextureToHeightmap(RectInt rect, int destX, int destY, TerrainHeightmapSyncControl syncControl)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_CopyActiveRenderTextureToHeightmap_Injected(intPtr, ref rect, destX, destY, syncControl);
	}

	[NativeName("GetHeightmap().DirtyHeightmapRegion")]
	private void Internal_DirtyHeightmapRegion(int x, int y, int width, int height, TerrainHeightmapSyncControl syncControl)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_DirtyHeightmapRegion_Injected(intPtr, x, y, width, height, syncControl);
	}

	[NativeName("GetHeightmap().SyncHeightmapGPUModifications")]
	public void SyncHeightmap()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SyncHeightmap_Injected(intPtr);
	}

	[NativeName("GetHeightmap().CopyHolesFromActiveRenderTexture")]
	private void Internal_CopyActiveRenderTextureToHoles(RectInt rect, int destX, int destY, bool allowDelayedCPUSync)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_CopyActiveRenderTextureToHoles_Injected(intPtr, ref rect, destX, destY, allowDelayedCPUSync);
	}

	[NativeName("GetHeightmap().DirtyHolesRegion")]
	private void Internal_DirtyHolesRegion(int x, int y, int width, int height, bool allowDelayedCPUSync)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_DirtyHolesRegion_Injected(intPtr, x, y, width, height, allowDelayedCPUSync);
	}

	[NativeName("GetHeightmap().SyncHolesGPUModifications")]
	private void Internal_SyncHoles()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_SyncHoles_Injected(intPtr);
	}

	[NativeName("GetSplatDatabase().MarkDirtyRegion")]
	private void Internal_MarkAlphamapDirtyRegion(int alphamapIndex, int x, int y, int width, int height)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_MarkAlphamapDirtyRegion_Injected(intPtr, alphamapIndex, x, y, width, height);
	}

	[NativeName("GetSplatDatabase().ClearDirtyRegion")]
	private void Internal_ClearAlphamapDirtyRegion(int alphamapIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_ClearAlphamapDirtyRegion_Injected(intPtr, alphamapIndex);
	}

	[NativeName("GetSplatDatabase().SyncGPUModifications")]
	private void Internal_SyncAlphamaps()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_SyncAlphamaps_Injected(intPtr);
	}

	public void CopyActiveRenderTextureToHeightmap(RectInt sourceRect, Vector2Int dest, TerrainHeightmapSyncControl syncControl)
	{
		RenderTexture active = RenderTexture.active;
		if (active == null)
		{
			throw new InvalidOperationException("Active RenderTexture is null.");
		}
		if (sourceRect.x < 0 || sourceRect.y < 0 || sourceRect.xMax > active.width || sourceRect.yMax > active.height)
		{
			throw new ArgumentOutOfRangeException("sourceRect");
		}
		if (dest.x < 0 || dest.x + sourceRect.width > heightmapResolution)
		{
			throw new ArgumentOutOfRangeException("dest.x");
		}
		if (dest.y < 0 || dest.y + sourceRect.height > heightmapResolution)
		{
			throw new ArgumentOutOfRangeException("dest.y");
		}
		Internal_CopyActiveRenderTextureToHeightmap(sourceRect, dest.x, dest.y, syncControl);
		TerrainCallbacks.InvokeHeightmapChangedCallback(this, new RectInt(dest.x, dest.y, sourceRect.width, sourceRect.height), syncControl == TerrainHeightmapSyncControl.HeightAndLod);
	}

	public void DirtyHeightmapRegion(RectInt region, TerrainHeightmapSyncControl syncControl)
	{
		int num = heightmapResolution;
		if (region.x < 0 || region.x >= num)
		{
			throw new ArgumentOutOfRangeException("region.x");
		}
		if (region.width <= 0 || region.xMax > num)
		{
			throw new ArgumentOutOfRangeException("region.width");
		}
		if (region.y < 0 || region.y >= num)
		{
			throw new ArgumentOutOfRangeException("region.y");
		}
		if (region.height <= 0 || region.yMax > num)
		{
			throw new ArgumentOutOfRangeException("region.height");
		}
		Internal_DirtyHeightmapRegion(region.x, region.y, region.width, region.height, syncControl);
		TerrainCallbacks.InvokeHeightmapChangedCallback(this, region, syncControl == TerrainHeightmapSyncControl.HeightAndLod);
	}

	public void CopyActiveRenderTextureToTexture(string textureName, int textureIndex, RectInt sourceRect, Vector2Int dest, bool allowDelayedCPUSync)
	{
		if (string.IsNullOrEmpty(textureName))
		{
			throw new ArgumentNullException("textureName");
		}
		RenderTexture active = RenderTexture.active;
		if (active == null)
		{
			throw new InvalidOperationException("Active RenderTexture is null.");
		}
		int num = 0;
		int num2 = 0;
		if (textureName == HolesTextureName)
		{
			if (textureIndex != 0)
			{
				throw new ArgumentOutOfRangeException("textureIndex");
			}
			if (active == holesTexture)
			{
				throw new ArgumentException("source", "Active RenderTexture cannot be holesTexture.");
			}
			num = (num2 = holesResolution);
		}
		else
		{
			if (!(textureName == AlphamapTextureName))
			{
				throw new ArgumentException("Unrecognized terrain texture name: \"" + textureName + "\"");
			}
			if (textureIndex < 0 || textureIndex >= alphamapTextureCount)
			{
				throw new ArgumentOutOfRangeException("textureIndex");
			}
			num = (num2 = alphamapResolution);
		}
		if (sourceRect.x < 0 || sourceRect.y < 0 || sourceRect.xMax > active.width || sourceRect.yMax > active.height)
		{
			throw new ArgumentOutOfRangeException("sourceRect");
		}
		if (dest.x < 0 || dest.x + sourceRect.width > num)
		{
			throw new ArgumentOutOfRangeException("dest.x");
		}
		if (dest.y < 0 || dest.y + sourceRect.height > num2)
		{
			throw new ArgumentOutOfRangeException("dest.y");
		}
		if (textureName == HolesTextureName)
		{
			Internal_CopyActiveRenderTextureToHoles(sourceRect, dest.x, dest.y, allowDelayedCPUSync);
			return;
		}
		Texture2D alphamapTexture = GetAlphamapTexture(textureIndex);
		allowDelayedCPUSync = allowDelayedCPUSync && SupportsCopyTextureBetweenRTAndTexture && QualitySettings.globalTextureMipmapLimit == 0;
		if (allowDelayedCPUSync)
		{
			if (alphamapTexture.mipmapCount > 1)
			{
				RenderTextureDescriptor desc = new RenderTextureDescriptor(alphamapTexture.width, alphamapTexture.height, active.graphicsFormat, active.depthStencilFormat);
				desc.sRGB = false;
				desc.useMipMap = true;
				desc.autoGenerateMips = false;
				RenderTexture temporary = RenderTexture.GetTemporary(desc);
				if (!temporary.IsCreated())
				{
					temporary.Create();
				}
				Graphics.CopyTexture(alphamapTexture, 0, 0, temporary, 0, 0);
				Graphics.CopyTexture(active, 0, 0, sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height, temporary, 0, 0, dest.x, dest.y);
				temporary.GenerateMips();
				Graphics.CopyTexture(temporary, alphamapTexture);
				RenderTexture.ReleaseTemporary(temporary);
			}
			else
			{
				Graphics.CopyTexture(active, 0, 0, sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height, alphamapTexture, 0, 0, dest.x, dest.y);
			}
			Internal_MarkAlphamapDirtyRegion(textureIndex, dest.x, dest.y, sourceRect.width, sourceRect.height);
		}
		else
		{
			alphamapTexture.ReadPixels(new Rect(sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height), dest.x, dest.y);
			alphamapTexture.Apply(updateMipmaps: true);
			Internal_ClearAlphamapDirtyRegion(textureIndex);
		}
		TerrainCallbacks.InvokeTextureChangedCallback(this, textureName, new RectInt(dest.x, dest.y, sourceRect.width, sourceRect.height), !allowDelayedCPUSync);
	}

	public void DirtyTextureRegion(string textureName, RectInt region, bool allowDelayedCPUSync)
	{
		if (string.IsNullOrEmpty(textureName))
		{
			throw new ArgumentNullException("textureName");
		}
		int num = 0;
		if (textureName == AlphamapTextureName)
		{
			num = alphamapResolution;
		}
		else
		{
			if (!(textureName == HolesTextureName))
			{
				throw new ArgumentException("Unrecognized terrain texture name: \"" + textureName + "\"");
			}
			num = holesResolution;
		}
		if (region.x < 0 || region.x >= num)
		{
			throw new ArgumentOutOfRangeException("region.x");
		}
		if (region.width <= 0 || region.xMax > num)
		{
			throw new ArgumentOutOfRangeException("region.width");
		}
		if (region.y < 0 || region.y >= num)
		{
			throw new ArgumentOutOfRangeException("region.y");
		}
		if (region.height <= 0 || region.yMax > num)
		{
			throw new ArgumentOutOfRangeException("region.height");
		}
		if (textureName == HolesTextureName)
		{
			Internal_DirtyHolesRegion(region.x, region.y, region.width, region.height, allowDelayedCPUSync);
			return;
		}
		Internal_MarkAlphamapDirtyRegion(-1, region.x, region.y, region.width, region.height);
		if (!allowDelayedCPUSync)
		{
			SyncTexture(textureName);
		}
		else
		{
			TerrainCallbacks.InvokeTextureChangedCallback(this, textureName, region, synched: false);
		}
	}

	public void SyncTexture(string textureName)
	{
		if (string.IsNullOrEmpty(textureName))
		{
			throw new ArgumentNullException("textureName");
		}
		if (textureName == AlphamapTextureName)
		{
			Internal_SyncAlphamaps();
			return;
		}
		if (textureName == HolesTextureName)
		{
			if (IsHolesTextureCompressed())
			{
				throw new InvalidOperationException("Holes texture is compressed. Compressed holes texture can not be read back from GPU. Use TerrainData.enableHolesTextureCompression to disable holes texture compression.");
			}
			Internal_SyncHoles();
			return;
		}
		throw new ArgumentException("Unrecognized terrain texture name: \"" + textureName + "\"");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_heightmapTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_internalHeightmapResolution_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_internalHeightmapResolution_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_heightmapScale_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enableHolesTextureCompression_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enableHolesTextureCompression_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsHolesTextureCompressed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetHolesTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetCompressedHolesTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_size_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_size_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetHeight_Injected(IntPtr _unity_self, int x, int y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetInterpolatedHeight_Injected(IntPtr _unity_self, float x, float y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetInterpolatedHeights_Injected(IntPtr _unity_self, ref ManagedSpanWrapper results, int resultXDimension, int resultXOffset, int resultYOffset, float xBase, float yBase, int xCount, int yCount, float xInterval, float yInterval);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float[,] Internal_GetHeights_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetHeights_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, ref ManagedSpanWrapper heights);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPatchMinMaxHeights_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OverrideMinMaxPatchHeights_Injected(IntPtr _unity_self, ref ManagedSpanWrapper minMaxHeights);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMaximumHeightError_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OverrideMaximumHeightError_Injected(IntPtr _unity_self, ref ManagedSpanWrapper maxError);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetHeightsDelayLOD_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, ref ManagedSpanWrapper heights);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetHoles_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, ref ManagedSpanWrapper holes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool[,] Internal_GetHoles_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_IsHole_Injected(IntPtr _unity_self, int x, int y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetHolesDelayLOD_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, ref ManagedSpanWrapper holes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetSteepness_Injected(IntPtr _unity_self, float x, float y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetInterpolatedNormal_Injected(IntPtr _unity_self, float x, float y, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetAdjustedSize_Injected(IntPtr _unity_self, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_wavingGrassStrength_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wavingGrassStrength_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_wavingGrassAmount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wavingGrassAmount_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_wavingGrassSpeed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wavingGrassSpeed_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_wavingGrassTint_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wavingGrassTint_Injected(IntPtr _unity_self, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_detailWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_detailHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_maxDetailScatterPerRes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetDetailResolution_Injected(IntPtr _unity_self, int patchCount, int resolutionPerPatch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetDetailScatterMode_Injected(IntPtr _unity_self, DetailScatterMode scatterMode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_detailPatchCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_detailResolution_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_detailResolutionPerPatch_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern DetailScatterMode get_detailScatterMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetDirtyDetails_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RefreshPrototypes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern DetailPrototype[] get_detailPrototypes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_detailPrototypes_Injected(IntPtr _unity_self, DetailPrototype[] value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSupportedLayers_Injected(IntPtr _unity_self, int xBase, int yBase, int totalWidth, int totalHeight, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int[,] GetDetailLayer_Injected(IntPtr _unity_self, int xBase, int yBase, int width, int height, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ComputeDetailInstanceTransforms_Injected(IntPtr _unity_self, int patchX, int patchY, int layer, float density, out Bounds bounds, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float ComputeDetailCoverage_Injected(IntPtr _unity_self, int detailPrototypeIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetDetailLayer_Injected(IntPtr _unity_self, int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, ref ManagedSpanWrapper data);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetTreeInstances_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTreeInstances_Injected(IntPtr _unity_self, ref ManagedSpanWrapper instances, bool snapToHeightmap);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetTreeInstance_Injected(IntPtr _unity_self, int index, out TreeInstance ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTreeInstance_Injected(IntPtr _unity_self, int index, [In] ref TreeInstance instance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_treeInstanceCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TreePrototype[] get_treePrototypes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_treePrototypes_Injected(IntPtr _unity_self, TreePrototype[] value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveTreePrototype_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveDetailPrototype_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool NeedUpgradeScaledTreePrototypes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpgradeScaledTreePrototype_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_alphamapLayers_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float[,,] Internal_GetAlphamaps_Injected(IntPtr _unity_self, int x, int y, int width, int height);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetAlphamapResolutionInternal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_Internal_alphamapResolution_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_Internal_alphamapResolution_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_Internal_baseMapResolution_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_Internal_baseMapResolution_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetAlphamaps_Injected(IntPtr _unity_self, int x, int y, int width, int height, ref ManagedSpanWrapper map);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBaseMapDirty_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetAlphamapTexture_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_alphamapTextureCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern SplatPrototype[] get_splatPrototypes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_splatPrototypes_Injected(IntPtr _unity_self, SplatPrototype[] value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TerrainLayer[] get_terrainLayers_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_terrainLayers_Injected(IntPtr _unity_self, TerrainLayer[] value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddTree_Injected(IntPtr _unity_self, ref TreeInstance tree);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int RemoveTrees_Injected(IntPtr _unity_self, [In] ref Vector2 position, float radius, int prototypeIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CopyActiveRenderTextureToHeightmap_Injected(IntPtr _unity_self, [In] ref RectInt rect, int destX, int destY, TerrainHeightmapSyncControl syncControl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DirtyHeightmapRegion_Injected(IntPtr _unity_self, int x, int y, int width, int height, TerrainHeightmapSyncControl syncControl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SyncHeightmap_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CopyActiveRenderTextureToHoles_Injected(IntPtr _unity_self, [In] ref RectInt rect, int destX, int destY, bool allowDelayedCPUSync);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DirtyHolesRegion_Injected(IntPtr _unity_self, int x, int y, int width, int height, bool allowDelayedCPUSync);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SyncHoles_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_MarkAlphamapDirtyRegion_Injected(IntPtr _unity_self, int alphamapIndex, int x, int y, int width, int height);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_ClearAlphamapDirtyRegion_Injected(IntPtr _unity_self, int alphamapIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SyncAlphamaps_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureFormat get_atlasFormat_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Terrain[] get_users_Injected(IntPtr _unity_self);
}
