using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Audio/Public/AudioReverbZone.h")]
[RequireComponent(typeof(Transform))]
public sealed class AudioReverbZone : Behaviour
{
	public float minDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_minDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_minDistance_Injected(intPtr, value);
		}
	}

	public float maxDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maxDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maxDistance_Injected(intPtr, value);
		}
	}

	public AudioReverbPreset reverbPreset
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_reverbPreset_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_reverbPreset_Injected(intPtr, value);
		}
	}

	public int room
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_room_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_room_Injected(intPtr, value);
		}
	}

	public int roomHF
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_roomHF_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_roomHF_Injected(intPtr, value);
		}
	}

	public int roomLF
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_roomLF_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_roomLF_Injected(intPtr, value);
		}
	}

	public float decayTime
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_decayTime_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_decayTime_Injected(intPtr, value);
		}
	}

	public float decayHFRatio
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_decayHFRatio_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_decayHFRatio_Injected(intPtr, value);
		}
	}

	public int reflections
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_reflections_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_reflections_Injected(intPtr, value);
		}
	}

	public float reflectionsDelay
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_reflectionsDelay_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_reflectionsDelay_Injected(intPtr, value);
		}
	}

	public int reverb
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_reverb_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_reverb_Injected(intPtr, value);
		}
	}

	public float reverbDelay
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_reverbDelay_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_reverbDelay_Injected(intPtr, value);
		}
	}

	public float HFReference
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_HFReference_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_HFReference_Injected(intPtr, value);
		}
	}

	public float LFReference
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_LFReference_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_LFReference_Injected(intPtr, value);
		}
	}

	[Obsolete("Warning! roomRolloffFactor is no longer supported.")]
	public float roomRolloffFactor
	{
		get
		{
			Debug.LogWarning("Warning! roomRolloffFactor is no longer supported.");
			return 10f;
		}
		set
		{
			Debug.LogWarning("Warning! roomRolloffFactor is no longer supported.");
		}
	}

	public float diffusion
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_diffusion_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_diffusion_Injected(intPtr, value);
		}
	}

	public float density
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_density_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_density_Injected(intPtr, value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_minDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_minDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioReverbPreset get_reverbPreset_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_reverbPreset_Injected(IntPtr _unity_self, AudioReverbPreset value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_room_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_room_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_roomHF_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_roomHF_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_roomLF_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_roomLF_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_decayTime_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_decayTime_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_decayHFRatio_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_decayHFRatio_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_reflections_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_reflections_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_reflectionsDelay_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_reflectionsDelay_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_reverb_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_reverb_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_reverbDelay_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_reverbDelay_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_HFReference_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_HFReference_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_LFReference_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_LFReference_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_diffusion_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_diffusion_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_density_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_density_Injected(IntPtr _unity_self, float value);
}
