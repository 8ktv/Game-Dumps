using System;
using System.Runtime.InteropServices;

namespace UnityEngine;

internal struct SpeedTreeWindConfig9
{
	public float strengthResponse;

	public float directionResponse;

	public float gustFrequency;

	public float gustStrengthMin;

	public float gustStrengthMax;

	public float gustDurationMin;

	public float gustDurationMax;

	public float gustRiseScalar;

	public float gustFallScalar;

	public float branch1StretchLimit;

	public float branch2StretchLimit;

	public float sharedHeightStart;

	public unsafe fixed float bendShared[20];

	public unsafe fixed float oscillationShared[20];

	public unsafe fixed float speedShared[20];

	public unsafe fixed float turbulenceShared[20];

	public unsafe fixed float flexibilityShared[20];

	public float independenceShared;

	public unsafe fixed float bendBranch1[20];

	public unsafe fixed float oscillationBranch1[20];

	public unsafe fixed float speedBranch1[20];

	public unsafe fixed float turbulenceBranch1[20];

	public unsafe fixed float flexibilityBranch1[20];

	public float independenceBranch1;

	public unsafe fixed float bendBranch2[20];

	public unsafe fixed float oscillationBranch2[20];

	public unsafe fixed float speedBranch2[20];

	public unsafe fixed float turbulenceBranch2[20];

	public unsafe fixed float flexibilityBranch2[20];

	public float independenceBranch2;

	public unsafe fixed float planarRipple[20];

	public unsafe fixed float directionalRipple[20];

	public unsafe fixed float speedRipple[20];

	public unsafe fixed float flexibilityRipple[20];

	public float independenceRipple;

	public float shimmerRipple;

	public float treeExtentX;

	public float treeExtentY;

	public float treeExtentZ;

	public float windIndependence;

	public int doShared;

	public int doBranch1;

	public int doBranch2;

	public int doRipple;

	public int doShimmer;

	public int lodFade;

	public float importScale;

	public readonly bool IsWindEnabled => doShared != 0 || doBranch1 != 0 || doBranch2 != 0 || doRipple != 0;

	public SpeedTreeWindConfig9()
	{
		strengthResponse = 5f;
		directionResponse = 2.5f;
		gustFrequency = 0f;
		gustStrengthMin = 0.5f;
		gustStrengthMax = 1f;
		gustDurationMin = 1f;
		gustDurationMax = 4f;
		gustRiseScalar = 1f;
		gustFallScalar = 1f;
		branch1StretchLimit = 1f;
		branch2StretchLimit = 1f;
		sharedHeightStart = 0f;
		independenceShared = 0f;
		independenceBranch1 = 0f;
		independenceBranch2 = 0f;
		independenceRipple = 0f;
		shimmerRipple = 0f;
		windIndependence = 0f;
		treeExtentX = 0f;
		treeExtentY = 0f;
		treeExtentZ = 0f;
		doShared = 0;
		doBranch1 = 0;
		doBranch2 = 0;
		doRipple = 0;
		doShimmer = 0;
		lodFade = 0;
		importScale = 1f;
	}

	public static byte[] Serialize(SpeedTreeWindConfig9 config)
	{
		int num = Marshal.SizeOf(config);
		byte[] array = new byte[num];
		GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
		try
		{
			IntPtr ptr = gCHandle.AddrOfPinnedObject();
			Marshal.StructureToPtr(config, ptr, fDeleteOld: false);
		}
		finally
		{
			gCHandle.Free();
		}
		return array;
	}
}
