using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Export/Director/PlayableOutputHandle.bindings.h")]
public struct PlayableOutputHandle : IEquatable<PlayableOutputHandle>
{
	internal IntPtr m_Handle;

	internal uint m_Version;

	private static readonly PlayableOutputHandle m_Null = default(PlayableOutputHandle);

	public static PlayableOutputHandle Null => m_Null;

	[VisibleToOtherModules]
	internal bool IsPlayableOutputOfType<T>()
	{
		return GetPlayableOutputType() == typeof(T);
	}

	public override int GetHashCode()
	{
		return m_Handle.GetHashCode() ^ m_Version.GetHashCode();
	}

	public static bool operator ==(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
	{
		return CompareVersion(lhs, rhs);
	}

	public static bool operator !=(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
	{
		return !CompareVersion(lhs, rhs);
	}

	public override bool Equals(object p)
	{
		return p is PlayableOutputHandle && Equals((PlayableOutputHandle)p);
	}

	public bool Equals(PlayableOutputHandle other)
	{
		return CompareVersion(this, other);
	}

	internal static bool CompareVersion(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
	{
		return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules]
	internal extern bool IsNull();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules]
	internal extern bool IsValid();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableOutputHandleBindings::GetPlayableOutputType", HasExplicitThis = true, ThrowsException = true)]
	internal extern Type GetPlayableOutputType();

	[FreeFunction("PlayableOutputHandleBindings::GetReferenceObject", HasExplicitThis = true, ThrowsException = true)]
	internal Object GetReferenceObject()
	{
		return Unmarshal.UnmarshalUnityObject<Object>(GetReferenceObject_Injected(ref this));
	}

	[FreeFunction("PlayableOutputHandleBindings::SetReferenceObject", HasExplicitThis = true, ThrowsException = true)]
	internal void SetReferenceObject(Object target)
	{
		SetReferenceObject_Injected(ref this, Object.MarshalledUnityObject.Marshal(target));
	}

	[FreeFunction("PlayableOutputHandleBindings::GetUserData", HasExplicitThis = true, ThrowsException = true)]
	internal Object GetUserData()
	{
		return Unmarshal.UnmarshalUnityObject<Object>(GetUserData_Injected(ref this));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableOutputHandleBindings::SetUserData", HasExplicitThis = true, ThrowsException = true)]
	internal extern void SetUserData([Writable] Object target);

	[FreeFunction("PlayableOutputHandleBindings::GetSourcePlayable", HasExplicitThis = true, ThrowsException = true)]
	internal PlayableHandle GetSourcePlayable()
	{
		GetSourcePlayable_Injected(ref this, out var ret);
		return ret;
	}

	[FreeFunction("PlayableOutputHandleBindings::SetSourcePlayable", HasExplicitThis = true, ThrowsException = true)]
	internal void SetSourcePlayable(PlayableHandle target, int port)
	{
		SetSourcePlayable_Injected(ref this, ref target, port);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableOutputHandleBindings::GetSourceOutputPort", HasExplicitThis = true, ThrowsException = true)]
	internal extern int GetSourceOutputPort();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableOutputHandleBindings::GetWeight", HasExplicitThis = true, ThrowsException = true)]
	internal extern float GetWeight();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableOutputHandleBindings::SetWeight", HasExplicitThis = true, ThrowsException = true)]
	internal extern void SetWeight(float weight);

	[FreeFunction("PlayableOutputHandleBindings::PushNotification", HasExplicitThis = true, ThrowsException = true)]
	internal void PushNotification(PlayableHandle origin, INotification notification, object context)
	{
		PushNotification_Injected(ref this, ref origin, notification, context);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableOutputHandleBindings::GetNotificationReceivers", HasExplicitThis = true, ThrowsException = true)]
	internal extern INotificationReceiver[] GetNotificationReceivers();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableOutputHandleBindings::AddNotificationReceiver", HasExplicitThis = true, ThrowsException = true)]
	internal extern void AddNotificationReceiver(INotificationReceiver receiver);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableOutputHandleBindings::RemoveNotificationReceiver", HasExplicitThis = true, ThrowsException = true)]
	internal extern void RemoveNotificationReceiver(INotificationReceiver receiver);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetReferenceObject_Injected(ref PlayableOutputHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetReferenceObject_Injected(ref PlayableOutputHandle _unity_self, IntPtr target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetUserData_Injected(ref PlayableOutputHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSourcePlayable_Injected(ref PlayableOutputHandle _unity_self, out PlayableHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSourcePlayable_Injected(ref PlayableOutputHandle _unity_self, [In] ref PlayableHandle target, int port);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PushNotification_Injected(ref PlayableOutputHandle _unity_self, [In] ref PlayableHandle origin, INotification notification, object context);
}
