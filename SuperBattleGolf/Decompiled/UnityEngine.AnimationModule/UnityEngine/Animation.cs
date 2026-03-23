using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Modules/Animation/Animation.h")]
public sealed class Animation : Behaviour, IEnumerable
{
	private sealed class Enumerator : IEnumerator
	{
		private Animation m_Outer;

		private int m_CurrentIndex = -1;

		public object Current => m_Outer.GetStateAtIndex(m_CurrentIndex);

		internal Enumerator(Animation outer)
		{
			m_Outer = outer;
		}

		public bool MoveNext()
		{
			int stateCount = m_Outer.GetStateCount();
			m_CurrentIndex++;
			return m_CurrentIndex < stateCount;
		}

		public void Reset()
		{
			m_CurrentIndex = -1;
		}
	}

	public AnimationClip clip
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<AnimationClip>(get_clip_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_clip_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public bool playAutomatically
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_playAutomatically_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_playAutomatically_Injected(intPtr, value);
		}
	}

	public WrapMode wrapMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wrapMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wrapMode_Injected(intPtr, value);
		}
	}

	public bool isPlaying
	{
		[NativeName("IsPlaying")]
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

	public AnimationState this[string name] => GetState(name);

	public bool animatePhysics
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_animatePhysics_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_animatePhysics_Injected(intPtr, value);
		}
	}

	public AnimationUpdateMode updateMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_updateMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_updateMode_Injected(intPtr, value);
		}
	}

	[Obsolete("Use cullingType instead")]
	public bool animateOnlyIfVisible
	{
		[FreeFunction("AnimationBindings::GetAnimateOnlyIfVisible", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_animateOnlyIfVisible_Injected(intPtr);
		}
		[FreeFunction("AnimationBindings::SetAnimateOnlyIfVisible", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_animateOnlyIfVisible_Injected(intPtr, value);
		}
	}

	public AnimationCullingType cullingType
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cullingType_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cullingType_Injected(intPtr, value);
		}
	}

	public Bounds localBounds
	{
		[NativeName("GetLocalAABB")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localBounds_Injected(intPtr, out var ret);
			return ret;
		}
		[NativeName("SetLocalAABB")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_localBounds_Injected(intPtr, ref value);
		}
	}

	public void Stop()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Stop_Injected(intPtr);
	}

	public void Stop(string name)
	{
		StopNamed(name);
	}

	[NativeName("Stop")]
	private unsafe void StopNamed(string name)
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
					StopNamed_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			StopNamed_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public void Rewind()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Rewind_Injected(intPtr);
	}

	public void Rewind(string name)
	{
		RewindNamed(name);
	}

	[NativeName("Rewind")]
	private unsafe void RewindNamed(string name)
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
					RewindNamed_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			RewindNamed_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public void Sample()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Sample_Injected(intPtr);
	}

	public unsafe bool IsPlaying(string name)
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
					return IsPlaying_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return IsPlaying_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[ExcludeFromDocs]
	public bool Play()
	{
		return Play(PlayMode.StopSameLayer);
	}

	public bool Play([DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
	{
		return PlayDefaultAnimation(mode);
	}

	[NativeName("Play")]
	private bool PlayDefaultAnimation(PlayMode mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return PlayDefaultAnimation_Injected(intPtr, mode);
	}

	[ExcludeFromDocs]
	public bool Play(string animation)
	{
		return Play(animation, PlayMode.StopSameLayer);
	}

	public unsafe bool Play(string animation, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(animation);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return Play_Injected(intPtr, ref managedSpanWrapper, mode);
				}
			}
			return Play_Injected(intPtr, ref managedSpanWrapper, mode);
		}
		finally
		{
		}
	}

	[ExcludeFromDocs]
	public void CrossFade(string animation)
	{
		CrossFade(animation, 0.3f);
	}

	[ExcludeFromDocs]
	public void CrossFade(string animation, float fadeLength)
	{
		CrossFade(animation, fadeLength, PlayMode.StopSameLayer);
	}

	public unsafe void CrossFade(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(animation);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					CrossFade_Injected(intPtr, ref managedSpanWrapper, fadeLength, mode);
					return;
				}
			}
			CrossFade_Injected(intPtr, ref managedSpanWrapper, fadeLength, mode);
		}
		finally
		{
		}
	}

	[ExcludeFromDocs]
	public void Blend(string animation)
	{
		Blend(animation, 1f);
	}

	[ExcludeFromDocs]
	public void Blend(string animation, float targetWeight)
	{
		Blend(animation, targetWeight, 0.3f);
	}

	public unsafe void Blend(string animation, [DefaultValue("1.0F")] float targetWeight, [DefaultValue("0.3F")] float fadeLength)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(animation);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Blend_Injected(intPtr, ref managedSpanWrapper, targetWeight, fadeLength);
					return;
				}
			}
			Blend_Injected(intPtr, ref managedSpanWrapper, targetWeight, fadeLength);
		}
		finally
		{
		}
	}

	[ExcludeFromDocs]
	public AnimationState CrossFadeQueued(string animation)
	{
		return CrossFadeQueued(animation, 0.3f);
	}

	[ExcludeFromDocs]
	public AnimationState CrossFadeQueued(string animation, float fadeLength)
	{
		return CrossFadeQueued(animation, fadeLength, QueueMode.CompleteOthers);
	}

	[ExcludeFromDocs]
	public AnimationState CrossFadeQueued(string animation, float fadeLength, QueueMode queue)
	{
		return CrossFadeQueued(animation, fadeLength, queue, PlayMode.StopSameLayer);
	}

	[FreeFunction("AnimationBindings::CrossFadeQueuedImpl", HasExplicitThis = true)]
	[return: Unmarshalled]
	public unsafe AnimationState CrossFadeQueued(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(animation);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return CrossFadeQueued_Injected(intPtr, ref managedSpanWrapper, fadeLength, queue, mode);
				}
			}
			return CrossFadeQueued_Injected(intPtr, ref managedSpanWrapper, fadeLength, queue, mode);
		}
		finally
		{
		}
	}

	[ExcludeFromDocs]
	public AnimationState PlayQueued(string animation)
	{
		return PlayQueued(animation, QueueMode.CompleteOthers);
	}

	[ExcludeFromDocs]
	public AnimationState PlayQueued(string animation, QueueMode queue)
	{
		return PlayQueued(animation, queue, PlayMode.StopSameLayer);
	}

	[FreeFunction("AnimationBindings::PlayQueuedImpl", HasExplicitThis = true)]
	[return: Unmarshalled]
	public unsafe AnimationState PlayQueued(string animation, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(animation);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return PlayQueued_Injected(intPtr, ref managedSpanWrapper, queue, mode);
				}
			}
			return PlayQueued_Injected(intPtr, ref managedSpanWrapper, queue, mode);
		}
		finally
		{
		}
	}

	public void AddClip(AnimationClip clip, string newName)
	{
		AddClip(clip, newName, int.MinValue, int.MaxValue);
	}

	[ExcludeFromDocs]
	public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
	{
		AddClip(clip, newName, firstFrame, lastFrame, addLoopFrame: false);
	}

	public unsafe void AddClip([NotNull] AnimationClip clip, string newName, int firstFrame, int lastFrame, [DefaultValue("false")] bool addLoopFrame)
	{
		//The blocks IL_005d are reachable both inside and outside the pinned region starting at IL_004c. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)clip == null)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(clip);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(newName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(newName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					AddClip_Injected(intPtr, intPtr2, ref managedSpanWrapper, firstFrame, lastFrame, addLoopFrame);
					return;
				}
			}
			AddClip_Injected(intPtr, intPtr2, ref managedSpanWrapper, firstFrame, lastFrame, addLoopFrame);
		}
		finally
		{
		}
	}

	public void RemoveClip([NotNull] AnimationClip clip)
	{
		if ((object)clip == null)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(clip);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		RemoveClip_Injected(intPtr, intPtr2);
	}

	public void RemoveClip(string clipName)
	{
		RemoveClipNamed(clipName);
	}

	[NativeName("RemoveClip")]
	private unsafe void RemoveClipNamed(string clipName)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(clipName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(clipName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					RemoveClipNamed_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			RemoveClipNamed_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public int GetClipCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetClipCount_Injected(intPtr);
	}

	[Obsolete("use PlayMode instead of AnimationPlayMode.")]
	public bool Play(AnimationPlayMode mode)
	{
		return PlayDefaultAnimation((PlayMode)mode);
	}

	[Obsolete("use PlayMode instead of AnimationPlayMode.")]
	public bool Play(string animation, AnimationPlayMode mode)
	{
		return Play(animation, (PlayMode)mode);
	}

	public void SyncLayer(int layer)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SyncLayer_Injected(intPtr, layer);
	}

	public IEnumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	[FreeFunction("AnimationBindings::GetState", HasExplicitThis = true)]
	[return: Unmarshalled]
	internal unsafe AnimationState GetState(string name)
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
					return GetState_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return GetState_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("AnimationBindings::GetStateAtIndex", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	internal AnimationState GetStateAtIndex(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetStateAtIndex_Injected(intPtr, index);
	}

	[NativeName("GetAnimationStateCount")]
	internal int GetStateCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetStateCount_Injected(intPtr);
	}

	public AnimationClip GetClip(string name)
	{
		AnimationState state = GetState(name);
		if ((bool)state)
		{
			return state.clip;
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_clip_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_clip_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_playAutomatically_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_playAutomatically_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern WrapMode get_wrapMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wrapMode_Injected(IntPtr _unity_self, WrapMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Stop_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopNamed_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Rewind_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RewindNamed_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Sample_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isPlaying_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsPlaying_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PlayDefaultAnimation_Injected(IntPtr _unity_self, PlayMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Play_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CrossFade_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Blend_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("1.0F")] float targetWeight, [DefaultValue("0.3F")] float fadeLength);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimationState CrossFadeQueued_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimationState PlayQueued_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddClip_Injected(IntPtr _unity_self, IntPtr clip, ref ManagedSpanWrapper newName, int firstFrame, int lastFrame, [DefaultValue("false")] bool addLoopFrame);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveClip_Injected(IntPtr _unity_self, IntPtr clip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveClipNamed_Injected(IntPtr _unity_self, ref ManagedSpanWrapper clipName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetClipCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SyncLayer_Injected(IntPtr _unity_self, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimationState GetState_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimationState GetStateAtIndex_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetStateCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_animatePhysics_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_animatePhysics_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimationUpdateMode get_updateMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_updateMode_Injected(IntPtr _unity_self, AnimationUpdateMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_animateOnlyIfVisible_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_animateOnlyIfVisible_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimationCullingType get_cullingType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cullingType_Injected(IntPtr _unity_self, AnimationCullingType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localBounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_localBounds_Injected(IntPtr _unity_self, [In] ref Bounds value);
}
