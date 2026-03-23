using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemScriptBindings.h")]
[NativeHeader("ParticleSystemScriptingClasses.h")]
[NativeHeader("Modules/ParticleSystem/ParticleSystem.h")]
[NativeHeader("Modules/ParticleSystem/ParticleSystemForceField.h")]
[NativeHeader("Modules/ParticleSystem/ParticleSystemForceFieldManager.h")]
public class ParticleSystemForceField : Behaviour
{
	[NativeName("ForceShape")]
	public ParticleSystemForceFieldShape shape
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shape_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shape_Injected(intPtr, value);
		}
	}

	public float startRange
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_startRange_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_startRange_Injected(intPtr, value);
		}
	}

	public float endRange
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_endRange_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_endRange_Injected(intPtr, value);
		}
	}

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
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_length_Injected(intPtr, value);
		}
	}

	public float gravityFocus
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_gravityFocus_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_gravityFocus_Injected(intPtr, value);
		}
	}

	public Vector2 rotationRandomness
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_rotationRandomness_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rotationRandomness_Injected(intPtr, ref value);
		}
	}

	public bool multiplyDragByParticleSize
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_multiplyDragByParticleSize_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_multiplyDragByParticleSize_Injected(intPtr, value);
		}
	}

	public bool multiplyDragByParticleVelocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_multiplyDragByParticleVelocity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_multiplyDragByParticleVelocity_Injected(intPtr, value);
		}
	}

	public Texture3D vectorField
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture3D>(get_vectorField_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_vectorField_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public ParticleSystem.MinMaxCurve directionX
	{
		get
		{
			return directionXBlittable;
		}
		set
		{
			directionXBlittable = value;
		}
	}

	[NativeName("DirectionX")]
	private ParticleSystem.MinMaxCurveBlittable directionXBlittable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_directionXBlittable_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_directionXBlittable_Injected(intPtr, ref value);
		}
	}

	public ParticleSystem.MinMaxCurve directionY
	{
		get
		{
			return directionYBlittable;
		}
		set
		{
			directionYBlittable = value;
		}
	}

	[NativeName("DirectionY")]
	private ParticleSystem.MinMaxCurveBlittable directionYBlittable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_directionYBlittable_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_directionYBlittable_Injected(intPtr, ref value);
		}
	}

	public ParticleSystem.MinMaxCurve directionZ
	{
		get
		{
			return directionZBlittable;
		}
		set
		{
			directionZBlittable = value;
		}
	}

	[NativeName("DirectionZ")]
	private ParticleSystem.MinMaxCurveBlittable directionZBlittable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_directionZBlittable_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_directionZBlittable_Injected(intPtr, ref value);
		}
	}

	public ParticleSystem.MinMaxCurve gravity
	{
		get
		{
			return gravityBlittable;
		}
		set
		{
			gravityBlittable = value;
		}
	}

	[NativeName("Gravity")]
	private ParticleSystem.MinMaxCurveBlittable gravityBlittable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_gravityBlittable_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_gravityBlittable_Injected(intPtr, ref value);
		}
	}

	public ParticleSystem.MinMaxCurve rotationSpeed
	{
		get
		{
			return rotationSpeedBlittable;
		}
		set
		{
			rotationSpeedBlittable = value;
		}
	}

	[NativeName("RotationSpeed")]
	private ParticleSystem.MinMaxCurveBlittable rotationSpeedBlittable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_rotationSpeedBlittable_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rotationSpeedBlittable_Injected(intPtr, ref value);
		}
	}

	public ParticleSystem.MinMaxCurve rotationAttraction
	{
		get
		{
			return rotationAttractionBlittable;
		}
		set
		{
			rotationAttractionBlittable = value;
		}
	}

	[NativeName("RotationAttraction")]
	private ParticleSystem.MinMaxCurveBlittable rotationAttractionBlittable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_rotationAttractionBlittable_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rotationAttractionBlittable_Injected(intPtr, ref value);
		}
	}

	public ParticleSystem.MinMaxCurve drag
	{
		get
		{
			return dragBlittable;
		}
		set
		{
			dragBlittable = value;
		}
	}

	[NativeName("Drag")]
	private ParticleSystem.MinMaxCurveBlittable dragBlittable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_dragBlittable_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_dragBlittable_Injected(intPtr, ref value);
		}
	}

	public ParticleSystem.MinMaxCurve vectorFieldSpeed
	{
		get
		{
			return vectorFieldSpeedBlittable;
		}
		set
		{
			vectorFieldSpeedBlittable = value;
		}
	}

	[NativeName("VectorFieldSpeed")]
	private ParticleSystem.MinMaxCurveBlittable vectorFieldSpeedBlittable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_vectorFieldSpeedBlittable_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_vectorFieldSpeedBlittable_Injected(intPtr, ref value);
		}
	}

	public ParticleSystem.MinMaxCurve vectorFieldAttraction
	{
		get
		{
			return vectorFieldAttractionBlittable;
		}
		set
		{
			vectorFieldAttractionBlittable = value;
		}
	}

	[NativeName("VectorFieldAttraction")]
	private ParticleSystem.MinMaxCurveBlittable vectorFieldAttractionBlittable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_vectorFieldAttractionBlittable_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_vectorFieldAttractionBlittable_Injected(intPtr, ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ParticleSystemForceFieldShape get_shape_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shape_Injected(IntPtr _unity_self, ParticleSystemForceFieldShape value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_startRange_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_startRange_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_endRange_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_endRange_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_length_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_length_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_gravityFocus_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_gravityFocus_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rotationRandomness_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rotationRandomness_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_multiplyDragByParticleSize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_multiplyDragByParticleSize_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_multiplyDragByParticleVelocity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_multiplyDragByParticleVelocity_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_vectorField_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_vectorField_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_directionXBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_directionXBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_directionYBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_directionYBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_directionZBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_directionZBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_gravityBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_gravityBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rotationSpeedBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rotationSpeedBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rotationAttractionBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rotationAttractionBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_dragBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_dragBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_vectorFieldSpeedBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_vectorFieldSpeedBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_vectorFieldAttractionBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_vectorFieldAttractionBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);
}
