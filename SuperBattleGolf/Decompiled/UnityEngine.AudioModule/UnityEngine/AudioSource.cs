using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Audio;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[StaticAccessor("AudioSourceBindings", StaticAccessorType.DoubleColon)]
[RequireComponent(typeof(Transform))]
public sealed class AudioSource : AudioBehaviour
{
	public float volume
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_volume_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_volume_Injected(intPtr, value);
		}
	}

	public float pitch
	{
		get
		{
			return GetPitch(this);
		}
		set
		{
			SetPitch(this, value);
		}
	}

	[NativeProperty("SecPosition")]
	public float time
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_time_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_time_Injected(intPtr, value);
		}
	}

	[NativeProperty("SamplePosition")]
	public int timeSamples
	{
		[NativeMethod(IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_timeSamples_Injected(intPtr);
		}
		[NativeMethod(IsThreadSafe = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_timeSamples_Injected(intPtr, value);
		}
	}

	public AudioClip clip
	{
		get
		{
			return generatorObject as AudioClip;
		}
		set
		{
			generatorObject = value;
		}
	}

	public AudioResource resource
	{
		get
		{
			return generatorObject as AudioResource;
		}
		set
		{
			generatorObject = value;
		}
	}

	public IAudioGenerator generator
	{
		get
		{
			return (IAudioGenerator)generatorObject;
		}
		set
		{
			generatorObject = (Object)value;
		}
	}

	public unsafe ProcessorInstance generatorInstance
	{
		get
		{
			GeneratorInstance.GeneratorHeader* ptr = (GeneratorInstance.GeneratorHeader*)generatorHeader;
			if (ptr != null)
			{
				return new GeneratorInstance(ptr);
			}
			return default(ProcessorInstance);
		}
	}

	internal unsafe void* generatorHeader
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_generatorHeader_Injected(intPtr);
		}
	}

	internal Object generatorObject
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Object>(get_generatorObject_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_generatorObject_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public AudioMixerGroup outputAudioMixerGroup
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<AudioMixerGroup>(get_outputAudioMixerGroup_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_outputAudioMixerGroup_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public bool isPlaying
	{
		[NativeName("IsPlayingScripting")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isPlaying_Injected(intPtr);
		}
	}

	internal bool isContainerPlaying
	{
		[NativeName("IsContainerPlaying")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isContainerPlaying_Injected(intPtr);
		}
	}

	internal ActivePlayable[] containerActivePlayables
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_containerActivePlayables_Injected(intPtr);
		}
	}

	public bool isVirtual
	{
		[NativeName("GetLastVirtualState")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isVirtual_Injected(intPtr);
		}
	}

	public bool loop
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_loop_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_loop_Injected(intPtr, value);
		}
	}

	public bool ignoreListenerVolume
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_ignoreListenerVolume_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_ignoreListenerVolume_Injected(intPtr, value);
		}
	}

	public bool playOnAwake
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_playOnAwake_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_playOnAwake_Injected(intPtr, value);
		}
	}

	public bool ignoreListenerPause
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_ignoreListenerPause_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_ignoreListenerPause_Injected(intPtr, value);
		}
	}

	public AudioVelocityUpdateMode velocityUpdateMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_velocityUpdateMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_velocityUpdateMode_Injected(intPtr, value);
		}
	}

	[NativeProperty("StereoPan")]
	public float panStereo
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_panStereo_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_panStereo_Injected(intPtr, value);
		}
	}

	[NativeProperty("SpatialBlendMix")]
	public float spatialBlend
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_spatialBlend_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_spatialBlend_Injected(intPtr, value);
		}
	}

	public bool spatialize
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_spatialize_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_spatialize_Injected(intPtr, value);
		}
	}

	public bool spatializePostEffects
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_spatializePostEffects_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_spatializePostEffects_Injected(intPtr, value);
		}
	}

	public float reverbZoneMix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_reverbZoneMix_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_reverbZoneMix_Injected(intPtr, value);
		}
	}

	public bool bypassEffects
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bypassEffects_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bypassEffects_Injected(intPtr, value);
		}
	}

	public bool bypassListenerEffects
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bypassListenerEffects_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bypassListenerEffects_Injected(intPtr, value);
		}
	}

	public bool bypassReverbZones
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bypassReverbZones_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bypassReverbZones_Injected(intPtr, value);
		}
	}

	public float dopplerLevel
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_dopplerLevel_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_dopplerLevel_Injected(intPtr, value);
		}
	}

	public float spread
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_spread_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_spread_Injected(intPtr, value);
		}
	}

	public int priority
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_priority_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_priority_Injected(intPtr, value);
		}
	}

	public bool mute
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_mute_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_mute_Injected(intPtr, value);
		}
	}

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

	public AudioRolloffMode rolloffMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rolloffMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rolloffMode_Injected(intPtr, value);
		}
	}

	[Obsolete("minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
	public float minVolume
	{
		get
		{
			Debug.LogError("minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
			return 0f;
		}
		set
		{
			Debug.LogError("minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
		}
	}

	[Obsolete("maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
	public float maxVolume
	{
		get
		{
			Debug.LogError("maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
			return 0f;
		}
		set
		{
			Debug.LogError("maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
		}
	}

	[Obsolete("rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
	public float rolloffFactor
	{
		get
		{
			Debug.LogError("rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
			return 0f;
		}
		set
		{
			Debug.LogError("rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
		}
	}

	[Obsolete("AudioSource.generatorDefinition has been deprecated. Use AudioSource.generator instead. (UnityUpgradable) -> generator", true)]
	public IAudioGenerator generatorDefinition
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	[Obsolete("AudioSource.generatorHandle has been deprecated. Use AudioSource.generatorInstance instead. (UnityUpgradable) -> generatorInstance", true)]
	public ProcessorInstance generatorHandle
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	private static float GetPitch([NotNull] AudioSource source)
	{
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		return GetPitch_Injected(intPtr);
	}

	private static void SetPitch([NotNull] AudioSource source, float pitch)
	{
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		SetPitch_Injected(intPtr, pitch);
	}

	private static void PlayHelper([NotNull] AudioSource source, ulong delay)
	{
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		PlayHelper_Injected(intPtr, delay);
	}

	private void Play(double delay)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Play_Injected(intPtr, delay);
	}

	private static void PlayOneShotHelper([NotNull] AudioSource source, [NotNull] AudioClip clip, float volumeScale)
	{
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		if ((object)clip == null)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(clip);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		PlayOneShotHelper_Injected(intPtr, intPtr2, volumeScale);
	}

	private void Stop(bool stopOneShots)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Stop_Injected(intPtr, stopOneShots);
	}

	[NativeThrows]
	private static void SetCustomCurveHelper([NotNull] AudioSource source, AudioSourceCurveType type, AnimationCurve curve)
	{
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		SetCustomCurveHelper_Injected(intPtr, type, (curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
	}

	private static AnimationCurve GetCustomCurveHelper([NotNull] AudioSource source, AudioSourceCurveType type)
	{
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr customCurveHelper_Injected = GetCustomCurveHelper_Injected(intPtr, type);
		return (customCurveHelper_Injected == (IntPtr)0) ? null : AnimationCurve.BindingsMarshaller.ConvertToManaged(customCurveHelper_Injected);
	}

	private unsafe static void GetOutputDataHelper([NotNull] AudioSource source, [Out] float[] samples, int channel)
	{
		//The blocks IL_003f are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		BlittableArrayWrapper samples2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			if (samples != null)
			{
				fixed (float[] array = samples)
				{
					if (array.Length != 0)
					{
						samples2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetOutputDataHelper_Injected(intPtr, out samples2, channel);
					return;
				}
			}
			GetOutputDataHelper_Injected(intPtr, out samples2, channel);
		}
		finally
		{
			samples2.Unmarshal(ref array);
		}
	}

	[NativeThrows]
	private unsafe static void GetSpectrumDataHelper([NotNull] AudioSource source, [Out] float[] samples, int channel, FFTWindow window)
	{
		//The blocks IL_003f are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		BlittableArrayWrapper samples2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			if (samples != null)
			{
				fixed (float[] array = samples)
				{
					if (array.Length != 0)
					{
						samples2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetSpectrumDataHelper_Injected(intPtr, out samples2, channel, window);
					return;
				}
			}
			GetSpectrumDataHelper_Injected(intPtr, out samples2, channel, window);
		}
		finally
		{
			samples2.Unmarshal(ref array);
		}
	}

	[ExcludeFromDocs]
	public void Play()
	{
		PlayHelper(this, 0uL);
	}

	public void Play([DefaultValue("0")] ulong delay)
	{
		PlayHelper(this, delay);
	}

	public void PlayDelayed(float delay)
	{
		Play((delay < 0f) ? 0.0 : (0.0 - (double)delay));
	}

	public void PlayScheduled(double time)
	{
		Play((time < 0.0) ? 0.0 : time);
	}

	[ExcludeFromDocs]
	public void PlayOneShot(AudioClip clip)
	{
		PlayOneShot(clip, 1f);
	}

	public void PlayOneShot(AudioClip clip, [DefaultValue("1.0F")] float volumeScale)
	{
		if (clip == null)
		{
			Debug.LogWarning("PlayOneShot was called with a null AudioClip.");
		}
		else
		{
			PlayOneShotHelper(this, clip, volumeScale);
		}
	}

	public void SetScheduledStartTime(double time)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetScheduledStartTime_Injected(intPtr, time);
	}

	public void SetScheduledEndTime(double time)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetScheduledEndTime_Injected(intPtr, time);
	}

	public void Stop()
	{
		Stop(stopOneShots: true);
	}

	public void Pause()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Pause_Injected(intPtr);
	}

	public void UnPause()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UnPause_Injected(intPtr);
	}

	internal void SkipToNextElementIfHasContainer()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SkipToNextElementIfHasContainer_Injected(intPtr);
	}

	[ExcludeFromDocs]
	public static void PlayClipAtPoint(AudioClip clip, Vector3 position)
	{
		PlayClipAtPoint(clip, position, 1f);
	}

	public static void PlayClipAtPoint(AudioClip clip, Vector3 position, [DefaultValue("1.0F")] float volume)
	{
		GameObject gameObject = new GameObject("One shot audio");
		gameObject.transform.position = position;
		AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		audioSource.clip = clip;
		audioSource.spatialBlend = 1f;
		audioSource.volume = volume;
		audioSource.Play();
		Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
	}

	public void SetCustomCurve(AudioSourceCurveType type, AnimationCurve curve)
	{
		SetCustomCurveHelper(this, type, curve);
	}

	public AnimationCurve GetCustomCurve(AudioSourceCurveType type)
	{
		return GetCustomCurveHelper(this, type);
	}

	[Obsolete("GetOutputData returning a float[] is deprecated, use GetOutputData and pass a pre allocated array instead.")]
	public float[] GetOutputData(int numSamples, int channel)
	{
		float[] array = new float[numSamples];
		GetOutputDataHelper(this, array, channel);
		return array;
	}

	public void GetOutputData(float[] samples, int channel)
	{
		GetOutputDataHelper(this, samples, channel);
	}

	[Obsolete("GetSpectrumData returning a float[] is deprecated, use GetSpectrumData and pass a pre allocated array instead.")]
	public float[] GetSpectrumData(int numSamples, int channel, FFTWindow window)
	{
		float[] array = new float[numSamples];
		GetSpectrumDataHelper(this, array, channel, window);
		return array;
	}

	public void GetSpectrumData(float[] samples, int channel, FFTWindow window)
	{
		GetSpectrumDataHelper(this, samples, channel, window);
	}

	public bool SetSpatializerFloat(int index, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetSpatializerFloat_Injected(intPtr, index, value);
	}

	public bool GetSpatializerFloat(int index, out float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetSpatializerFloat_Injected(intPtr, index, out value);
	}

	public bool GetAmbisonicDecoderFloat(int index, out float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAmbisonicDecoderFloat_Injected(intPtr, index, out value);
	}

	public bool SetAmbisonicDecoderFloat(int index, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetAmbisonicDecoderFloat_Injected(intPtr, index, value);
	}

	internal float GetAudioRandomContainerRuntimeMeterValue()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAudioRandomContainerRuntimeMeterValue_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetPitch_Injected(IntPtr source);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPitch_Injected(IntPtr source, float pitch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PlayHelper_Injected(IntPtr source, ulong delay);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Play_Injected(IntPtr _unity_self, double delay);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PlayOneShotHelper_Injected(IntPtr source, IntPtr clip, float volumeScale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Stop_Injected(IntPtr _unity_self, bool stopOneShots);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCustomCurveHelper_Injected(IntPtr source, AudioSourceCurveType type, IntPtr curve);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetCustomCurveHelper_Injected(IntPtr source, AudioSourceCurveType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetOutputDataHelper_Injected(IntPtr source, out BlittableArrayWrapper samples, int channel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSpectrumDataHelper_Injected(IntPtr source, out BlittableArrayWrapper samples, int channel, FFTWindow window);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_volume_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_volume_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_time_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_time_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_timeSamples_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_timeSamples_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* get_generatorHeader_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_generatorObject_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_generatorObject_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_outputAudioMixerGroup_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_outputAudioMixerGroup_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetScheduledStartTime_Injected(IntPtr _unity_self, double time);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetScheduledEndTime_Injected(IntPtr _unity_self, double time);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Pause_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UnPause_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SkipToNextElementIfHasContainer_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isPlaying_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isContainerPlaying_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ActivePlayable[] get_containerActivePlayables_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isVirtual_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_loop_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_loop_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_ignoreListenerVolume_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_ignoreListenerVolume_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_playOnAwake_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_playOnAwake_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_ignoreListenerPause_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_ignoreListenerPause_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioVelocityUpdateMode get_velocityUpdateMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_velocityUpdateMode_Injected(IntPtr _unity_self, AudioVelocityUpdateMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_panStereo_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_panStereo_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_spatialBlend_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_spatialBlend_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_spatialize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_spatialize_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_spatializePostEffects_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_spatializePostEffects_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_reverbZoneMix_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_reverbZoneMix_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_bypassEffects_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bypassEffects_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_bypassListenerEffects_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bypassListenerEffects_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_bypassReverbZones_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bypassReverbZones_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_dopplerLevel_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_dopplerLevel_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_spread_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_spread_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_priority_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_priority_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_mute_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mute_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_minDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_minDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioRolloffMode get_rolloffMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rolloffMode_Injected(IntPtr _unity_self, AudioRolloffMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetSpatializerFloat_Injected(IntPtr _unity_self, int index, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetSpatializerFloat_Injected(IntPtr _unity_self, int index, out float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetAmbisonicDecoderFloat_Injected(IntPtr _unity_self, int index, out float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetAmbisonicDecoderFloat_Injected(IntPtr _unity_self, int index, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetAudioRandomContainerRuntimeMeterValue_Injected(IntPtr _unity_self);
}
