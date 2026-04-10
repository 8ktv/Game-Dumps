using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables;

[NativeHeader("Runtime/Export/Director/CameraPlayable.bindings.h")]
[NativeHeader("Runtime/Camera//Director/CameraPlayable.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[StaticAccessor("CameraPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
public struct CameraPlayable : IPlayable, IEquatable<CameraPlayable>
{
	private PlayableHandle m_Handle;

	public static CameraPlayable Create(PlayableGraph graph, Camera camera)
	{
		PlayableHandle handle = CreateHandle(graph, camera);
		return new CameraPlayable(handle);
	}

	private static PlayableHandle CreateHandle(PlayableGraph graph, Camera camera)
	{
		PlayableHandle handle = PlayableHandle.Null;
		if (!InternalCreateCameraPlayable(ref graph, camera, ref handle))
		{
			return PlayableHandle.Null;
		}
		return handle;
	}

	internal CameraPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<CameraPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an CameraPlayable.");
		}
		m_Handle = handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator Playable(CameraPlayable playable)
	{
		return new Playable(playable.GetHandle());
	}

	public static explicit operator CameraPlayable(Playable playable)
	{
		return new CameraPlayable(playable.GetHandle());
	}

	public bool Equals(CameraPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}

	public Camera GetCamera()
	{
		return GetCameraInternal(ref m_Handle);
	}

	public void SetCamera(Camera value)
	{
		SetCameraInternal(ref m_Handle, value);
	}

	[NativeThrows]
	private static Camera GetCameraInternal(ref PlayableHandle hdl)
	{
		return Unmarshal.UnmarshalUnityObject<Camera>(GetCameraInternal_Injected(ref hdl));
	}

	[NativeThrows]
	private static void SetCameraInternal(ref PlayableHandle hdl, Camera camera)
	{
		SetCameraInternal_Injected(ref hdl, Object.MarshalledUnityObject.Marshal(camera));
	}

	[NativeThrows]
	private static bool InternalCreateCameraPlayable(ref PlayableGraph graph, Camera camera, ref PlayableHandle handle)
	{
		return InternalCreateCameraPlayable_Injected(ref graph, Object.MarshalledUnityObject.Marshal(camera), ref handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern bool ValidateType(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetCameraInternal_Injected(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCameraInternal_Injected(ref PlayableHandle hdl, IntPtr camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InternalCreateCameraPlayable_Injected(ref PlayableGraph graph, IntPtr camera, ref PlayableHandle handle);
}
