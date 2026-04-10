using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequiredByNativeCode]
[NativeHeader("Runtime/Transform/ScriptBindings/TransformScriptBindings.h")]
[NativeHeader("Runtime/Transform/Transform.h")]
[NativeHeader("Configuration/UnityConfigure.h")]
public class Transform : Component, IEnumerable
{
	private class Enumerator : IEnumerator
	{
		private Transform outer;

		private int currentIndex = -1;

		public object Current => outer.GetChild(currentIndex);

		internal Enumerator(Transform outer)
		{
			this.outer = outer;
		}

		public bool MoveNext()
		{
			int childCount = outer.childCount;
			return ++currentIndex < childCount;
		}

		public void Reset()
		{
			currentIndex = -1;
		}
	}

	public Vector3 position
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_position_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_position_Injected(intPtr, ref value);
		}
	}

	public Vector3 localPosition
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localPosition_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_localPosition_Injected(intPtr, ref value);
		}
	}

	public Vector3 eulerAngles
	{
		get
		{
			return rotation.eulerAngles;
		}
		set
		{
			rotation = Quaternion.Euler(value);
		}
	}

	public Vector3 localEulerAngles
	{
		get
		{
			return localRotation.eulerAngles;
		}
		set
		{
			localRotation = Quaternion.Euler(value);
		}
	}

	public Vector3 right
	{
		get
		{
			return rotation * Vector3.right;
		}
		set
		{
			rotation = Quaternion.FromToRotation(Vector3.right, value);
		}
	}

	public Vector3 up
	{
		get
		{
			return rotation * Vector3.up;
		}
		set
		{
			rotation = Quaternion.FromToRotation(Vector3.up, value);
		}
	}

	public Vector3 forward
	{
		get
		{
			return rotation * Vector3.forward;
		}
		set
		{
			rotation = Quaternion.LookRotation(value);
		}
	}

	public Quaternion rotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_rotation_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rotation_Injected(intPtr, ref value);
		}
	}

	public Quaternion localRotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localRotation_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_localRotation_Injected(intPtr, ref value);
		}
	}

	[NativeConditional("UNITY_EDITOR")]
	internal RotationOrder rotationOrder
	{
		get
		{
			return (RotationOrder)GetRotationOrderInternal();
		}
		set
		{
			SetRotationOrderInternal(value);
		}
	}

	public Vector3 localScale
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localScale_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_localScale_Injected(intPtr, ref value);
		}
	}

	public Transform parent
	{
		get
		{
			return parentInternal;
		}
		set
		{
			if (this is RectTransform)
			{
				Debug.LogWarning("Parent of RectTransform is being set with parent property. Consider using the SetParent method instead, with the worldPositionStays argument set to false. This will retain local orientation and scale rather than world orientation and scale, which can prevent common UI scaling issues.", this);
			}
			parentInternal = value;
		}
	}

	internal Transform parentInternal
	{
		get
		{
			return GetParent();
		}
		set
		{
			SetParent(value);
		}
	}

	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_worldToLocalMatrix_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localToWorldMatrix_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Transform root => GetRoot();

	public int childCount
	{
		[NativeMethod("GetChildrenCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_childCount_Injected(intPtr);
		}
	}

	public Vector3 lossyScale
	{
		[NativeMethod("GetWorldScaleLossy")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_lossyScale_Injected(intPtr, out var ret);
			return ret;
		}
	}

	[NativeProperty("HasChangedDeprecated")]
	public bool hasChanged
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasChanged_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_hasChanged_Injected(intPtr, value);
		}
	}

	public int hierarchyCapacity
	{
		get
		{
			return internal_getHierarchyCapacity();
		}
		set
		{
			internal_setHierarchyCapacity(value);
		}
	}

	public int hierarchyCount => internal_getHierarchyCount();

	[NativeConditional("UNITY_EDITOR")]
	internal bool constrainProportionsScale
	{
		get
		{
			return IsConstrainProportionsScale();
		}
		set
		{
			SetConstrainProportionsScale(value);
		}
	}

	protected Transform()
	{
	}

	internal Vector3 GetLocalEulerAngles(RotationOrder order)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetLocalEulerAngles_Injected(intPtr, order, out var ret);
		return ret;
	}

	internal void SetLocalEulerAngles(Vector3 euler, RotationOrder order)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLocalEulerAngles_Injected(intPtr, ref euler, order);
	}

	[NativeConditional("UNITY_EDITOR")]
	internal void SetLocalEulerHint(Vector3 euler)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLocalEulerHint_Injected(intPtr, ref euler);
	}

	[NativeConditional("UNITY_EDITOR")]
	[NativeMethod("GetRotationOrder")]
	internal int GetRotationOrderInternal()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetRotationOrderInternal_Injected(intPtr);
	}

	[NativeMethod("SetRotationOrder")]
	[NativeConditional("UNITY_EDITOR")]
	internal void SetRotationOrderInternal(RotationOrder rotationOrder)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetRotationOrderInternal_Injected(intPtr, rotationOrder);
	}

	private Transform GetParent()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Transform>(GetParent_Injected(intPtr));
	}

	public void SetParent(Transform p)
	{
		SetParent(p, worldPositionStays: true);
	}

	[FreeFunction("SetParent", HasExplicitThis = true)]
	public void SetParent(Transform parent, bool worldPositionStays)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetParent_Injected(intPtr, MarshalledUnityObject.Marshal(parent), worldPositionStays);
	}

	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPositionAndRotation_Injected(intPtr, ref position, ref rotation);
	}

	public void SetLocalPositionAndRotation(Vector3 localPosition, Quaternion localRotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLocalPositionAndRotation_Injected(intPtr, ref localPosition, ref localRotation);
	}

	public void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPositionAndRotation_Injected(intPtr, out position, out rotation);
	}

	public void GetLocalPositionAndRotation(out Vector3 localPosition, out Quaternion localRotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetLocalPositionAndRotation_Injected(intPtr, out localPosition, out localRotation);
	}

	public void Translate(Vector3 translation, [DefaultValue("Space.Self")] Space relativeTo)
	{
		if (relativeTo == Space.World)
		{
			position += translation;
		}
		else
		{
			position += TransformDirection(translation);
		}
	}

	public void Translate(Vector3 translation)
	{
		Translate(translation, Space.Self);
	}

	public void Translate(float x, float y, float z, [DefaultValue("Space.Self")] Space relativeTo)
	{
		Translate(new Vector3(x, y, z), relativeTo);
	}

	public void Translate(float x, float y, float z)
	{
		Translate(new Vector3(x, y, z), Space.Self);
	}

	public void Translate(Vector3 translation, Transform relativeTo)
	{
		if ((bool)relativeTo)
		{
			position += relativeTo.TransformDirection(translation);
		}
		else
		{
			position += translation;
		}
	}

	public void Translate(float x, float y, float z, Transform relativeTo)
	{
		Translate(new Vector3(x, y, z), relativeTo);
	}

	public void Rotate(Vector3 eulers, [DefaultValue("Space.Self")] Space relativeTo)
	{
		Quaternion quaternion = Quaternion.Euler(eulers.x, eulers.y, eulers.z);
		if (relativeTo == Space.Self)
		{
			localRotation *= quaternion;
		}
		else
		{
			rotation *= Quaternion.Inverse(rotation) * quaternion * rotation;
		}
	}

	public void Rotate(Vector3 eulers)
	{
		Rotate(eulers, Space.Self);
	}

	public void Rotate(float xAngle, float yAngle, float zAngle, [DefaultValue("Space.Self")] Space relativeTo)
	{
		Rotate(new Vector3(xAngle, yAngle, zAngle), relativeTo);
	}

	public void Rotate(float xAngle, float yAngle, float zAngle)
	{
		Rotate(new Vector3(xAngle, yAngle, zAngle), Space.Self);
	}

	[NativeMethod("RotateAround")]
	internal void RotateAroundInternal(Vector3 axis, float angle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RotateAroundInternal_Injected(intPtr, ref axis, angle);
	}

	public void Rotate(Vector3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
	{
		if (relativeTo == Space.Self)
		{
			RotateAroundInternal(base.transform.TransformDirection(axis), angle * (MathF.PI / 180f));
		}
		else
		{
			RotateAroundInternal(axis, angle * (MathF.PI / 180f));
		}
	}

	public void Rotate(Vector3 axis, float angle)
	{
		Rotate(axis, angle, Space.Self);
	}

	public void RotateAround(Vector3 point, Vector3 axis, float angle)
	{
		Vector3 vector = position;
		Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
		Vector3 vector2 = vector - point;
		vector2 = quaternion * vector2;
		vector = point + vector2;
		position = vector;
		RotateAroundInternal(axis, angle * (MathF.PI / 180f));
	}

	public void LookAt(Transform target, [DefaultValue("Vector3.up")] Vector3 worldUp)
	{
		if ((bool)target)
		{
			LookAt(target.position, worldUp);
		}
	}

	public void LookAt(Transform target)
	{
		if ((bool)target)
		{
			LookAt(target.position, Vector3.up);
		}
	}

	public void LookAt(Vector3 worldPosition, [DefaultValue("Vector3.up")] Vector3 worldUp)
	{
		Internal_LookAt(worldPosition, worldUp);
	}

	public void LookAt(Vector3 worldPosition)
	{
		Internal_LookAt(worldPosition, Vector3.up);
	}

	[FreeFunction("Internal_LookAt", HasExplicitThis = true)]
	private void Internal_LookAt(Vector3 worldPosition, Vector3 worldUp)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_LookAt_Injected(intPtr, ref worldPosition, ref worldUp);
	}

	public Vector3 TransformDirection(Vector3 direction)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TransformDirection_Injected(intPtr, ref direction, out var ret);
		return ret;
	}

	public Vector3 TransformDirection(float x, float y, float z)
	{
		return TransformDirection(new Vector3(x, y, z));
	}

	[NativeMethod(Name = "TransformDirections")]
	internal unsafe void TransformDirectionsInternal(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<Vector3> readOnlySpan = directions;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper directions2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedDirections;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedDirections2 = new ManagedSpanWrapper(begin2, span.Length);
				TransformDirectionsInternal_Injected(intPtr, ref directions2, ref transformedDirections2);
			}
		}
	}

	public void TransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
	{
		if (directions.Length != transformedDirections.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.TransformDirections() must be the same length");
		}
		TransformDirectionsInternal(directions, transformedDirections);
	}

	public void TransformDirections(Span<Vector3> directions)
	{
		TransformDirectionsInternal(directions, directions);
	}

	public Vector3 InverseTransformDirection(Vector3 direction)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InverseTransformDirection_Injected(intPtr, ref direction, out var ret);
		return ret;
	}

	public Vector3 InverseTransformDirection(float x, float y, float z)
	{
		return InverseTransformDirection(new Vector3(x, y, z));
	}

	[NativeMethod(Name = "InverseTransformDirections")]
	internal unsafe void InverseTransformDirectionsInternal(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<Vector3> readOnlySpan = directions;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper directions2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedDirections;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedDirections2 = new ManagedSpanWrapper(begin2, span.Length);
				InverseTransformDirectionsInternal_Injected(intPtr, ref directions2, ref transformedDirections2);
			}
		}
	}

	public void InverseTransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
	{
		if (directions.Length != transformedDirections.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.InverseTransformDirections() must be the same length");
		}
		InverseTransformDirectionsInternal(directions, transformedDirections);
	}

	public void InverseTransformDirections(Span<Vector3> directions)
	{
		InverseTransformDirectionsInternal(directions, directions);
	}

	public Vector3 TransformVector(Vector3 vector)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TransformVector_Injected(intPtr, ref vector, out var ret);
		return ret;
	}

	public Vector3 TransformVector(float x, float y, float z)
	{
		return TransformVector(new Vector3(x, y, z));
	}

	[NativeMethod(Name = "TransformVectors")]
	internal unsafe void TransformVectorsInternal(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<Vector3> readOnlySpan = vectors;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper vectors2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedVectors;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedVectors2 = new ManagedSpanWrapper(begin2, span.Length);
				TransformVectorsInternal_Injected(intPtr, ref vectors2, ref transformedVectors2);
			}
		}
	}

	public void TransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
	{
		if (vectors.Length != transformedVectors.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.TransformVectors() must be the same length");
		}
		TransformVectorsInternal(vectors, transformedVectors);
	}

	public void TransformVectors(Span<Vector3> vectors)
	{
		TransformVectorsInternal(vectors, vectors);
	}

	public Vector3 InverseTransformVector(Vector3 vector)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InverseTransformVector_Injected(intPtr, ref vector, out var ret);
		return ret;
	}

	public Vector3 InverseTransformVector(float x, float y, float z)
	{
		return InverseTransformVector(new Vector3(x, y, z));
	}

	[NativeMethod(Name = "InverseTransformVectors")]
	internal unsafe void InverseTransformVectorsInternal(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<Vector3> readOnlySpan = vectors;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper vectors2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedVectors;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedVectors2 = new ManagedSpanWrapper(begin2, span.Length);
				InverseTransformVectorsInternal_Injected(intPtr, ref vectors2, ref transformedVectors2);
			}
		}
	}

	public void InverseTransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
	{
		if (vectors.Length != transformedVectors.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.InverseTransformVectors() must be the same length");
		}
		InverseTransformVectorsInternal(vectors, transformedVectors);
	}

	public void InverseTransformVectors(Span<Vector3> vectors)
	{
		InverseTransformVectorsInternal(vectors, vectors);
	}

	public Vector3 TransformPoint(Vector3 position)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TransformPoint_Injected(intPtr, ref position, out var ret);
		return ret;
	}

	public Vector3 TransformPoint(float x, float y, float z)
	{
		return TransformPoint(new Vector3(x, y, z));
	}

	[NativeMethod(Name = "TransformPoints")]
	internal unsafe void TransformPointsInternal(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<Vector3> readOnlySpan = positions;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper positions2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedPositions;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedPositions2 = new ManagedSpanWrapper(begin2, span.Length);
				TransformPointsInternal_Injected(intPtr, ref positions2, ref transformedPositions2);
			}
		}
	}

	public void TransformPoints(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
	{
		if (positions.Length != transformedPositions.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.TransformPoints() must be the same length");
		}
		TransformPointsInternal(positions, transformedPositions);
	}

	public void TransformPoints(Span<Vector3> positions)
	{
		TransformPointsInternal(positions, positions);
	}

	public Vector3 InverseTransformPoint(Vector3 position)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InverseTransformPoint_Injected(intPtr, ref position, out var ret);
		return ret;
	}

	public Vector3 InverseTransformPoint(float x, float y, float z)
	{
		return InverseTransformPoint(new Vector3(x, y, z));
	}

	[NativeMethod(Name = "InverseTransformPoints")]
	internal unsafe void InverseTransformPointsInternal(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<Vector3> readOnlySpan = positions;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper positions2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedPositions;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedPositions2 = new ManagedSpanWrapper(begin2, span.Length);
				InverseTransformPointsInternal_Injected(intPtr, ref positions2, ref transformedPositions2);
			}
		}
	}

	public void InverseTransformPoints(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
	{
		if (positions.Length != transformedPositions.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.InverseTransformPoints() must be the same length");
		}
		InverseTransformPointsInternal(positions, transformedPositions);
	}

	public void InverseTransformPoints(Span<Vector3> positions)
	{
		InverseTransformPoints(positions, positions);
	}

	private Transform GetRoot()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Transform>(GetRoot_Injected(intPtr));
	}

	[FreeFunction("DetachChildren", HasExplicitThis = true)]
	public void DetachChildren()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		DetachChildren_Injected(intPtr);
	}

	public void SetAsFirstSibling()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetAsFirstSibling_Injected(intPtr);
	}

	public void SetAsLastSibling()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetAsLastSibling_Injected(intPtr);
	}

	public void SetSiblingIndex(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetSiblingIndex_Injected(intPtr, index);
	}

	[NativeMethod("MoveAfterSiblingInternal")]
	internal void MoveAfterSibling(Transform transform, bool notifyEditorAndMarkDirty)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MoveAfterSibling_Injected(intPtr, MarshalledUnityObject.Marshal(transform), notifyEditorAndMarkDirty);
	}

	public int GetSiblingIndex()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetSiblingIndex_Injected(intPtr);
	}

	[FreeFunction(HasExplicitThis = true)]
	private unsafe Transform FindRelativeTransformWithPath(string path, [DefaultValue("false")] bool isActiveOnly)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		Transform result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = FindRelativeTransformWithPath_Injected(intPtr, ref managedSpanWrapper, isActiveOnly);
				}
			}
			else
			{
				gcHandlePtr = FindRelativeTransformWithPath_Injected(intPtr, ref managedSpanWrapper, isActiveOnly);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Transform>(gcHandlePtr);
		}
		return result;
	}

	public Transform Find(string n)
	{
		if (n == null)
		{
			throw new ArgumentNullException("Name cannot be null");
		}
		return FindRelativeTransformWithPath(n, isActiveOnly: false);
	}

	[NativeConditional("UNITY_EDITOR")]
	internal void SendTransformChangedScale()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SendTransformChangedScale_Injected(intPtr);
	}

	[FreeFunction("Internal_IsChildOrSameAsOtherTransform", HasExplicitThis = true)]
	public bool IsChildOf([NotNull] Transform parent)
	{
		if ((object)parent == null)
		{
			ThrowHelper.ThrowArgumentNullException(parent, "parent");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(parent);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(parent, "parent");
		}
		return IsChildOf_Injected(intPtr, intPtr2);
	}

	[Obsolete("FindChild has been deprecated. Use Find instead (UnityUpgradable) -> Find([mscorlib] System.String)", false)]
	public Transform FindChild(string n)
	{
		return Find(n);
	}

	public IEnumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	[Obsolete("warning use Transform.Rotate instead.")]
	public void RotateAround(Vector3 axis, float angle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RotateAround_Injected(intPtr, ref axis, angle);
	}

	[Obsolete("warning use Transform.Rotate instead.")]
	public void RotateAroundLocal(Vector3 axis, float angle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RotateAroundLocal_Injected(intPtr, ref axis, angle);
	}

	[FreeFunction("GetChild", HasExplicitThis = true)]
	[NativeThrows]
	public Transform GetChild(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Transform>(GetChild_Injected(intPtr, index));
	}

	[Obsolete("warning use Transform.childCount instead (UnityUpgradable) -> Transform.childCount", false)]
	[NativeMethod("GetChildrenCount")]
	public int GetChildCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetChildCount_Injected(intPtr);
	}

	[FreeFunction("GetHierarchyCapacity", HasExplicitThis = true)]
	private int internal_getHierarchyCapacity()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return internal_getHierarchyCapacity_Injected(intPtr);
	}

	[FreeFunction("SetHierarchyCapacity", HasExplicitThis = true)]
	private void internal_setHierarchyCapacity(int value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		internal_setHierarchyCapacity_Injected(intPtr, value);
	}

	[FreeFunction("GetHierarchyCount", HasExplicitThis = true)]
	private int internal_getHierarchyCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return internal_getHierarchyCount_Injected(intPtr);
	}

	[NativeConditional("UNITY_EDITOR")]
	[FreeFunction("IsNonUniformScaleTransform", HasExplicitThis = true)]
	internal bool IsNonUniformScaleTransform()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsNonUniformScaleTransform_Injected(intPtr);
	}

	[NativeConditional("UNITY_EDITOR")]
	private void SetConstrainProportionsScale(bool isLinked)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetConstrainProportionsScale_Injected(intPtr, isLinked);
	}

	[NativeConditional("UNITY_EDITOR")]
	private bool IsConstrainProportionsScale()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsConstrainProportionsScale_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_position_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_position_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localPosition_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_localPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLocalEulerAngles_Injected(IntPtr _unity_self, RotationOrder order, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLocalEulerAngles_Injected(IntPtr _unity_self, [In] ref Vector3 euler, RotationOrder order);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLocalEulerHint_Injected(IntPtr _unity_self, [In] ref Vector3 euler);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localRotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_localRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRotationOrderInternal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRotationOrderInternal_Injected(IntPtr _unity_self, RotationOrder rotationOrder);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localScale_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_localScale_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetParent_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetParent_Injected(IntPtr _unity_self, IntPtr parent, bool worldPositionStays);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_worldToLocalMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPositionAndRotation_Injected(IntPtr _unity_self, [In] ref Vector3 position, [In] ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLocalPositionAndRotation_Injected(IntPtr _unity_self, [In] ref Vector3 localPosition, [In] ref Quaternion localRotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPositionAndRotation_Injected(IntPtr _unity_self, out Vector3 position, out Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLocalPositionAndRotation_Injected(IntPtr _unity_self, out Vector3 localPosition, out Quaternion localRotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RotateAroundInternal_Injected(IntPtr _unity_self, [In] ref Vector3 axis, float angle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_LookAt_Injected(IntPtr _unity_self, [In] ref Vector3 worldPosition, [In] ref Vector3 worldUp);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TransformDirection_Injected(IntPtr _unity_self, [In] ref Vector3 direction, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TransformDirectionsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper directions, ref ManagedSpanWrapper transformedDirections);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InverseTransformDirection_Injected(IntPtr _unity_self, [In] ref Vector3 direction, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InverseTransformDirectionsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper directions, ref ManagedSpanWrapper transformedDirections);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TransformVector_Injected(IntPtr _unity_self, [In] ref Vector3 vector, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TransformVectorsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper vectors, ref ManagedSpanWrapper transformedVectors);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InverseTransformVector_Injected(IntPtr _unity_self, [In] ref Vector3 vector, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InverseTransformVectorsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper vectors, ref ManagedSpanWrapper transformedVectors);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TransformPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TransformPointsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions, ref ManagedSpanWrapper transformedPositions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InverseTransformPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InverseTransformPointsInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions, ref ManagedSpanWrapper transformedPositions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetRoot_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_childCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DetachChildren_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetAsFirstSibling_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetAsLastSibling_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSiblingIndex_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveAfterSibling_Injected(IntPtr _unity_self, IntPtr transform, bool notifyEditorAndMarkDirty);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSiblingIndex_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr FindRelativeTransformWithPath_Injected(IntPtr _unity_self, ref ManagedSpanWrapper path, [DefaultValue("false")] bool isActiveOnly);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendTransformChangedScale_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_lossyScale_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsChildOf_Injected(IntPtr _unity_self, IntPtr parent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasChanged_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_hasChanged_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RotateAround_Injected(IntPtr _unity_self, [In] ref Vector3 axis, float angle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RotateAroundLocal_Injected(IntPtr _unity_self, [In] ref Vector3 axis, float angle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetChild_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetChildCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int internal_getHierarchyCapacity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void internal_setHierarchyCapacity_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int internal_getHierarchyCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsNonUniformScaleTransform_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetConstrainProportionsScale_Injected(IntPtr _unity_self, bool isLinked);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsConstrainProportionsScale_Injected(IntPtr _unity_self);
}
