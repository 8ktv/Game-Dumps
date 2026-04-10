using System;
using System.Runtime.CompilerServices;
using Unity.IntegerTime;
using UnityEngine.Audio;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/Audio/Public/ScriptBindings/Audio.bindings.h")]
[StaticAccessor("AudioClipBindings", StaticAccessorType.DoubleColon)]
public sealed class AudioClip : AudioResource, IAudioGenerator, GeneratorInstance.ICapabilities
{
	public delegate void PCMReaderCallback(float[] data);

	public delegate void PCMSetPositionCallback(int position);

	[NativeProperty("LengthSec")]
	public float length
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_length_Injected(intPtr);
		}
	}

	[NativeProperty("SampleCount")]
	public int samples
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_samples_Injected(intPtr);
		}
	}

	[NativeProperty("ChannelCount")]
	public int channels
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_channels_Injected(intPtr);
		}
	}

	public int frequency
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_frequency_Injected(intPtr);
		}
	}

	[Obsolete("Use AudioClip.loadState instead to get more detailed information about the loading process.")]
	public bool isReadyToPlay
	{
		[NativeName("ReadyToPlay")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isReadyToPlay_Injected(intPtr);
		}
	}

	public AudioClipLoadType loadType
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_loadType_Injected(intPtr);
		}
	}

	public bool preloadAudioData
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_preloadAudioData_Injected(intPtr);
		}
	}

	public bool ambisonic
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_ambisonic_Injected(intPtr);
		}
	}

	public bool loadInBackground
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_loadInBackground_Injected(intPtr);
		}
	}

	public AudioDataLoadState loadState
	{
		[NativeMethod(Name = "AudioClipBindings::GetLoadState", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_loadState_Injected(intPtr);
		}
	}

	bool GeneratorInstance.ICapabilities.isRealtime
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	bool GeneratorInstance.ICapabilities.isFinite
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	DiscreteTime? GeneratorInstance.ICapabilities.length
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	private event PCMReaderCallback m_PCMReaderCallback = null;

	private event PCMSetPositionCallback m_PCMSetPositionCallback = null;

	private AudioClip()
	{
	}

	private unsafe static bool GetData([NotNull] AudioClip clip, Span<float> data, int samplesOffset)
	{
		if ((object)clip == null)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(clip);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		Span<float> span = data;
		bool data_Injected;
		fixed (float* begin = span)
		{
			ManagedSpanWrapper data2 = new ManagedSpanWrapper(begin, span.Length);
			data_Injected = GetData_Injected(intPtr, ref data2, samplesOffset);
		}
		return data_Injected;
	}

	private unsafe static bool SetData([NotNull] AudioClip clip, ReadOnlySpan<float> data, int samplesOffset)
	{
		if ((object)clip == null)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(clip);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		ReadOnlySpan<float> readOnlySpan = data;
		bool result;
		fixed (float* begin = readOnlySpan)
		{
			ManagedSpanWrapper data2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			result = SetData_Injected(intPtr, ref data2, samplesOffset);
		}
		return result;
	}

	private static AudioClip Construct_Internal()
	{
		return Unmarshal.UnmarshalUnityObject<AudioClip>(Construct_Internal_Injected());
	}

	private string GetName()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetName_Injected(intPtr, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	private unsafe void CreateUserSound(string name, int lengthSamples, int channels, int frequency, bool stream)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					CreateUserSound_Injected(intPtr, ref managedSpanWrapper, lengthSamples, channels, frequency, stream);
					return;
				}
			}
			CreateUserSound_Injected(intPtr, ref managedSpanWrapper, lengthSamples, channels, frequency, stream);
		}
		finally
		{
		}
	}

	public bool LoadAudioData()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return LoadAudioData_Injected(intPtr);
	}

	public bool UnloadAudioData()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return UnloadAudioData_Injected(intPtr);
	}

	public bool GetData(Span<float> data, int offsetSamples)
	{
		if (channels <= 0)
		{
			Debug.Log("AudioClip.GetData failed; AudioClip " + GetName() + " contains no data");
			return false;
		}
		return GetData(this, data, offsetSamples);
	}

	public bool GetData(float[] data, int offsetSamples)
	{
		if (channels <= 0)
		{
			Debug.Log("AudioClip.GetData failed; AudioClip " + GetName() + " contains no data");
			return false;
		}
		return GetData(this, MemoryExtensions.AsSpan(data), offsetSamples);
	}

	public bool SetData(float[] data, int offsetSamples)
	{
		if (channels <= 0)
		{
			Debug.Log("AudioClip.SetData failed; AudioClip " + GetName() + " contains no data");
			return false;
		}
		if (offsetSamples < 0 || offsetSamples >= samples)
		{
			throw new ArgumentException("AudioClip.SetData failed; invalid offsetSamples");
		}
		if (data == null || data.Length == 0)
		{
			throw new ArgumentException("AudioClip.SetData failed; invalid data");
		}
		return SetData(this, MemoryExtensions.AsSpan(data), offsetSamples);
	}

	public bool SetData(ReadOnlySpan<float> data, int offsetSamples)
	{
		if (channels <= 0)
		{
			Debug.Log("AudioClip.SetData failed; AudioClip " + GetName() + " contains no data");
			return false;
		}
		if (offsetSamples < 0 || offsetSamples >= samples)
		{
			throw new ArgumentException("AudioClip.SetData failed; invalid offsetSamples");
		}
		if (data.Length == 0)
		{
			throw new ArgumentException("AudioClip.SetData failed; invalid data");
		}
		return SetData(this, data, offsetSamples);
	}

	[Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
	public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream)
	{
		return Create(name, lengthSamples, channels, frequency, stream);
	}

	[Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
	public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, PCMReaderCallback pcmreadercallback)
	{
		return Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, null);
	}

	[Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
	public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, PCMReaderCallback pcmreadercallback, PCMSetPositionCallback pcmsetpositioncallback)
	{
		return Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, pcmsetpositioncallback);
	}

	public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream)
	{
		return Create(name, lengthSamples, channels, frequency, stream, null, null);
	}

	public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream, PCMReaderCallback pcmreadercallback)
	{
		return Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, null);
	}

	public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream, PCMReaderCallback pcmreadercallback, PCMSetPositionCallback pcmsetpositioncallback)
	{
		if (name == null)
		{
			throw new NullReferenceException();
		}
		if (lengthSamples <= 0)
		{
			throw new ArgumentException("Length of created clip must be larger than 0");
		}
		if (channels <= 0)
		{
			throw new ArgumentException("Number of channels in created clip must be greater than 0");
		}
		if (frequency <= 0)
		{
			throw new ArgumentException("Frequency in created clip must be greater than 0");
		}
		AudioClip audioClip = Construct_Internal();
		if (pcmreadercallback != null)
		{
			audioClip.m_PCMReaderCallback += pcmreadercallback;
		}
		if (pcmsetpositioncallback != null)
		{
			audioClip.m_PCMSetPositionCallback += pcmsetpositioncallback;
		}
		audioClip.CreateUserSound(name, lengthSamples, channels, frequency, stream);
		return audioClip;
	}

	[RequiredByNativeCode]
	private void InvokePCMReaderCallback_Internal(float[] data)
	{
		if (this.m_PCMReaderCallback != null)
		{
			this.m_PCMReaderCallback(data);
		}
	}

	[RequiredByNativeCode]
	private void InvokePCMSetPositionCallback_Internal(int position)
	{
		if (this.m_PCMSetPositionCallback != null)
		{
			this.m_PCMSetPositionCallback(position);
		}
	}

	GeneratorInstance IAudioGenerator.CreateInstance(ControlContext context, AudioFormat? nestedFormat, ProcessorInstance.CreationParameters parameters)
	{
		throw new NotImplementedException();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetData_Injected(IntPtr clip, ref ManagedSpanWrapper data, int samplesOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetData_Injected(IntPtr clip, ref ManagedSpanWrapper data, int samplesOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Construct_Internal_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateUserSound_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, int lengthSamples, int channels, int frequency, bool stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_length_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_samples_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_channels_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_frequency_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isReadyToPlay_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioClipLoadType get_loadType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool LoadAudioData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UnloadAudioData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_preloadAudioData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_ambisonic_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_loadInBackground_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioDataLoadState get_loadState_Injected(IntPtr _unity_self);
}
