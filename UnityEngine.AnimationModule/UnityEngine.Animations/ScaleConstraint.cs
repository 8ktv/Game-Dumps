using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
[NativeHeader("Modules/Animation/Constraints/ScaleConstraint.h")]
[RequireComponent(typeof(Transform))]
[UsedByNativeCode]
public sealed class ScaleConstraint : Behaviour, IConstraint, IConstraintInternal
{
	public float weight
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_weight_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_weight_Injected(intPtr, value);
		}
	}

	public Vector3 scaleAtRest
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_scaleAtRest_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_scaleAtRest_Injected(intPtr, ref value);
		}
	}

	public Vector3 scaleOffset
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_scaleOffset_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_scaleOffset_Injected(intPtr, ref value);
		}
	}

	public Axis scalingAxis
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_scalingAxis_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_scalingAxis_Injected(intPtr, value);
		}
	}

	public bool constraintActive
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_constraintActive_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_constraintActive_Injected(intPtr, value);
		}
	}

	public bool locked
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_locked_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_locked_Injected(intPtr, value);
		}
	}

	public int sourceCount => GetSourceCountInternal(this);

	private ScaleConstraint()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create([Writable] ScaleConstraint self);

	[FreeFunction("ConstraintBindings::GetSourceCount")]
	private static int GetSourceCountInternal([NotNull] ScaleConstraint self)
	{
		if ((object)self == null)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(self);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		return GetSourceCountInternal_Injected(intPtr);
	}

	[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
	public void GetSources([NotNull] List<ConstraintSource> sources)
	{
		if (sources == null)
		{
			ThrowHelper.ThrowArgumentNullException(sources, "sources");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetSources_Injected(intPtr, sources);
	}

	public void SetSources(List<ConstraintSource> sources)
	{
		if (sources == null)
		{
			throw new ArgumentNullException("sources");
		}
		SetSourcesInternal(this, sources);
	}

	[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
	private static void SetSourcesInternal([NotNull] ScaleConstraint self, List<ConstraintSource> sources)
	{
		if ((object)self == null)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(self);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		SetSourcesInternal_Injected(intPtr, sources);
	}

	public int AddSource(ConstraintSource source)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddSource_Injected(intPtr, ref source);
	}

	public void RemoveSource(int index)
	{
		ValidateSourceIndex(index);
		RemoveSourceInternal(index);
	}

	[NativeName("RemoveSource")]
	private void RemoveSourceInternal(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveSourceInternal_Injected(intPtr, index);
	}

	public ConstraintSource GetSource(int index)
	{
		ValidateSourceIndex(index);
		return GetSourceInternal(index);
	}

	[NativeName("GetSource")]
	private ConstraintSource GetSourceInternal(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetSourceInternal_Injected(intPtr, index, out var ret);
		return ret;
	}

	public void SetSource(int index, ConstraintSource source)
	{
		ValidateSourceIndex(index);
		SetSourceInternal(index, source);
	}

	[NativeName("SetSource")]
	private void SetSourceInternal(int index, ConstraintSource source)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetSourceInternal_Injected(intPtr, index, ref source);
	}

	private void ValidateSourceIndex(int index)
	{
		if (sourceCount == 0)
		{
			throw new InvalidOperationException("The ScaleConstraint component has no sources.");
		}
		if (index < 0 || index >= sourceCount)
		{
			throw new ArgumentOutOfRangeException("index", $"Constraint source index {index} is out of bounds (0-{sourceCount}).");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_weight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_weight_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_scaleAtRest_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_scaleAtRest_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_scaleOffset_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_scaleOffset_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Axis get_scalingAxis_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_scalingAxis_Injected(IntPtr _unity_self, Axis value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_constraintActive_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_constraintActive_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_locked_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_locked_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSourceCountInternal_Injected(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSources_Injected(IntPtr _unity_self, List<ConstraintSource> sources);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSourcesInternal_Injected(IntPtr self, List<ConstraintSource> sources);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int AddSource_Injected(IntPtr _unity_self, [In] ref ConstraintSource source);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveSourceInternal_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSourceInternal_Injected(IntPtr _unity_self, int index, out ConstraintSource ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSourceInternal_Injected(IntPtr _unity_self, int index, [In] ref ConstraintSource source);
}
