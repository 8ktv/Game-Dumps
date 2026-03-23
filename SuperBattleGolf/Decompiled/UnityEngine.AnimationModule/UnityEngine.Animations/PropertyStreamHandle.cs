using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations;

[MovedFrom("UnityEngine.Experimental.Animations")]
[NativeHeader("Modules/Animation/Director/AnimationStreamHandles.h")]
public struct PropertyStreamHandle
{
	private uint m_AnimatorBindingsVersion;

	private int handleIndex;

	private int valueArrayIndex;

	private int bindType;

	private bool createdByNative => animatorBindingsVersion != 0;

	private bool hasHandleIndex => handleIndex != -1;

	private bool hasValueArrayIndex => valueArrayIndex != -1;

	private bool hasBindType => bindType != 0;

	internal uint animatorBindingsVersion
	{
		get
		{
			return m_AnimatorBindingsVersion;
		}
		private set
		{
			m_AnimatorBindingsVersion = value;
		}
	}

	public bool IsValid(AnimationStream stream)
	{
		return IsValidInternal(ref stream);
	}

	private bool IsValidInternal(ref AnimationStream stream)
	{
		return stream.isValid && createdByNative && hasHandleIndex && hasBindType;
	}

	private bool IsSameVersionAsStream(ref AnimationStream stream)
	{
		return animatorBindingsVersion == stream.animatorBindingsVersion;
	}

	public void Resolve(AnimationStream stream)
	{
		CheckIsValidAndResolve(ref stream);
	}

	public bool IsResolved(AnimationStream stream)
	{
		return IsResolvedInternal(ref stream);
	}

	private bool IsResolvedInternal(ref AnimationStream stream)
	{
		return IsValidInternal(ref stream) && IsSameVersionAsStream(ref stream) && hasValueArrayIndex;
	}

	private void CheckIsValidAndResolve(ref AnimationStream stream)
	{
		stream.CheckIsValid();
		if (!IsResolvedInternal(ref stream))
		{
			if (!createdByNative || !hasHandleIndex || !hasBindType)
			{
				throw new InvalidOperationException("The PropertyStreamHandle is invalid. Please use proper function to create the handle.");
			}
			if (!IsSameVersionAsStream(ref stream) || (hasHandleIndex && !hasValueArrayIndex))
			{
				ResolveInternal(ref stream);
			}
			if (hasHandleIndex && !hasValueArrayIndex)
			{
				throw new InvalidOperationException("The PropertyStreamHandle cannot be resolved.");
			}
		}
	}

	public float GetFloat(AnimationStream stream)
	{
		CheckIsValidAndResolve(ref stream);
		if (bindType != 5)
		{
			throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
		}
		return GetFloatInternal(ref stream);
	}

	public void SetFloat(AnimationStream stream, float value)
	{
		CheckIsValidAndResolve(ref stream);
		if (bindType != 5)
		{
			throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
		}
		SetFloatInternal(ref stream, value);
	}

	public int GetInt(AnimationStream stream)
	{
		CheckIsValidAndResolve(ref stream);
		if (bindType == 9)
		{
			Debug.LogWarning("Please Use GetEntityId directly to get the value of an ObjectReference PropertyStreamHandle.");
			return GetEntityId(stream);
		}
		if (bindType != 10 && bindType != 11)
		{
			throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
		}
		return GetIntInternal(ref stream);
	}

	public void SetInt(AnimationStream stream, int value)
	{
		CheckIsValidAndResolve(ref stream);
		if (bindType == 9)
		{
			Debug.LogWarning("Please Use SetEntityId directly to set the value of an ObjectReference PropertyStreamHandle.");
			SetEntityId(stream, value);
			return;
		}
		if (bindType != 10 && bindType != 11)
		{
			throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
		}
		SetIntInternal(ref stream, value);
	}

	public EntityId GetEntityId(AnimationStream stream)
	{
		CheckIsValidAndResolve(ref stream);
		if (bindType != 9)
		{
			throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
		}
		return GetEntityIdInternal(ref stream);
	}

	public void SetEntityId(AnimationStream stream, EntityId value)
	{
		CheckIsValidAndResolve(ref stream);
		if (bindType != 9)
		{
			throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
		}
		SetEntityIdInternal(ref stream, value);
	}

	public bool GetBool(AnimationStream stream)
	{
		CheckIsValidAndResolve(ref stream);
		if (bindType != 6 && bindType != 7)
		{
			throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
		}
		return GetBoolInternal(ref stream);
	}

	public void SetBool(AnimationStream stream, bool value)
	{
		CheckIsValidAndResolve(ref stream);
		if (bindType != 6 && bindType != 7)
		{
			throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
		}
		SetBoolInternal(ref stream, value);
	}

	public bool GetReadMask(AnimationStream stream)
	{
		CheckIsValidAndResolve(ref stream);
		return GetReadMaskInternal(ref stream);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "Resolve", IsThreadSafe = true)]
	private extern void ResolveInternal(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetFloat", IsThreadSafe = true)]
	private extern float GetFloatInternal(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetFloat", IsThreadSafe = true)]
	private extern void SetFloatInternal(ref AnimationStream stream, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetInt", IsThreadSafe = true)]
	private extern int GetIntInternal(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetInt", IsThreadSafe = true)]
	private extern void SetIntInternal(ref AnimationStream stream, int value);

	[NativeMethod(Name = "GetEntityId", IsThreadSafe = true)]
	private EntityId GetEntityIdInternal(ref AnimationStream stream)
	{
		GetEntityIdInternal_Injected(ref this, ref stream, out var ret);
		return ret;
	}

	[NativeMethod(Name = "SetEntityId", IsThreadSafe = true)]
	private void SetEntityIdInternal(ref AnimationStream stream, EntityId value)
	{
		SetEntityIdInternal_Injected(ref this, ref stream, ref value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetBool", IsThreadSafe = true)]
	private extern bool GetBoolInternal(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetBool", IsThreadSafe = true)]
	private extern void SetBoolInternal(ref AnimationStream stream, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetReadMask", IsThreadSafe = true)]
	private extern bool GetReadMaskInternal(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetEntityIdInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, out EntityId ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetEntityIdInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, [In] ref EntityId value);
}
