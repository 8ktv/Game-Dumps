using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[UsedByNativeCode]
[NativeClass("TransformHandle")]
public struct TransformHandle : IEquatable<TransformHandle>, IComparable<TransformHandle>
{
	public struct DirectChildrenEnumerable : IEnumerable<TransformHandle>, IEnumerable
	{
		private TransformHandle Root;

		public DirectChildrenEnumerable(TransformHandle root)
		{
			Root = root;
		}

		public IEnumerator<TransformHandle> GetEnumerator()
		{
			return new DirectChildrenEnumerator(Root);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public struct DirectChildrenEnumerator : IEnumerator<TransformHandle>, IEnumerator, IDisposable
	{
		private TransformHandle parent;

		private int currentIndex;

		object IEnumerator.Current => Current;

		public TransformHandle Current => parent.GetChild(currentIndex);

		internal DirectChildrenEnumerator(TransformHandle parent)
		{
			this.parent = parent;
			currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++currentIndex < parent.childCount;
		}

		public void Reset()
		{
			currentIndex = -1;
		}

		public void Dispose()
		{
		}
	}

	internal IntPtr pTransformData;

	[SerializeField]
	internal EntityId id;

	public static TransformHandle None => default(TransformHandle);

	public DirectChildrenEnumerable DirectChildren => new DirectChildrenEnumerable(this);

	public Vector3 position
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_GetPosition(out var p);
			return p;
		}
		set
		{
			AssertHandleIsValid(this);
			Internal_SetPosition(value);
		}
	}

	public Quaternion rotation
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_GetRotation(out var r);
			return r;
		}
		set
		{
			AssertHandleIsValid(this);
			Internal_SetRotation(value);
		}
	}

	public Vector3 lossyScale
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_GetWorldScaleLossy(out var s);
			return s;
		}
	}

	public Vector3 localPosition
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_GetLocalPosition(out var r);
			return r;
		}
		set
		{
			AssertHandleIsValid(this);
			Internal_SetLocalPosition(value);
		}
	}

	public Quaternion localRotation
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_GetLocalRotation(out var r);
			return r;
		}
		set
		{
			AssertHandleIsValid(this);
			Internal_SetLocalRotation(value);
		}
	}

	public Vector3 localScale
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_GetLocalScale(out var r);
			return r;
		}
		set
		{
			AssertHandleIsValid(this);
			Internal_SetLocalScale(value);
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

	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_GetWorldToLocalMatrix(out var m);
			return m;
		}
	}

	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_GetLocalToWorldMatrix(out var m);
			return m;
		}
	}

	public TransformHandle root
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_GetRoot(out var outRootHandle);
			return outRootHandle;
		}
	}

	public TransformHandle parent
	{
		get
		{
			AssertHandleIsValid(this);
			Internal_TryGetParent(out var parentHandle);
			return parentHandle;
		}
		set
		{
			AssertHandleIsValid(this);
			TransformHandle transformHandle = value;
			if (transformHandle != None && !transformHandle.IsValid())
			{
				transformHandle = None;
			}
			Internal_SetParent(transformHandle, worldPositionStays: true);
		}
	}

	public int childCount
	{
		get
		{
			AssertHandleIsValid(this);
			return Internal_GetChildrenCount();
		}
	}

	public int hierarchyCapacity
	{
		get
		{
			AssertHandleIsValid(this);
			return Internal_GetHierarchyCapacity();
		}
		set
		{
			AssertHandleIsValid(this);
			Internal_SetHierarchyCapacity(value);
		}
	}

	public int hierarchyCount
	{
		get
		{
			AssertHandleIsValid(this);
			return Internal_GetHierarchyCount();
		}
	}

	public override bool Equals(object obj)
	{
		return obj is TransformHandle other && Equals(other);
	}

	public bool Equals(TransformHandle other)
	{
		return id == other.id;
	}

	public int CompareTo(TransformHandle other)
	{
		return id.CompareTo(other.id);
	}

	public static bool operator ==(TransformHandle lhs, TransformHandle rhs)
	{
		return lhs.id == rhs.id && lhs.pTransformData == rhs.pTransformData;
	}

	public static bool operator !=(TransformHandle lhs, TransformHandle rhs)
	{
		return lhs.id != rhs.id || lhs.pTransformData != rhs.pTransformData;
	}

	public DirectChildrenEnumerator GetDirectChildrenEnumerator()
	{
		return new DirectChildrenEnumerator(this);
	}

	public override int GetHashCode()
	{
		return id.GetHashCode();
	}

	private static void AssertHandleIsValid(TransformHandle handle)
	{
		if (!Resources.EntityIdIsValid(handle.id))
		{
			if (handle.id == EntityId.None)
			{
				throw new NullReferenceException("The TransformHandle object is null. It may not have been properly initialized, or may refer to an object which has been destroyed. TransformHandles should only be obtained through a valid GameObject or Component.");
			}
			throw new MissingReferenceException($"The target of this TransformHandle (id='{handle.id}') is not a valid object. The corresponding object may have been destroyed.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetPosition")]
	private extern void Internal_GetPosition(out Vector3 p);

	[NativeMethod(Name = "SetPosition")]
	private void Internal_SetPosition(Vector3 p)
	{
		Internal_SetPosition_Injected(ref this, ref p);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetRotation")]
	private extern void Internal_GetRotation(out Quaternion r);

	[NativeMethod(Name = "SetRotation")]
	private void Internal_SetRotation(Quaternion r)
	{
		Internal_SetRotation_Injected(ref this, ref r);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetWorldScaleLossy")]
	private extern void Internal_GetWorldScaleLossy(out Vector3 s);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetLocalPosition")]
	private extern void Internal_GetLocalPosition(out Vector3 r);

	[NativeMethod(Name = "SetLocalPosition")]
	private void Internal_SetLocalPosition(Vector3 r)
	{
		Internal_SetLocalPosition_Injected(ref this, ref r);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetLocalRotation")]
	private extern void Internal_GetLocalRotation(out Quaternion r);

	[NativeMethod(Name = "SetLocalRotation")]
	private void Internal_SetLocalRotation(Quaternion r)
	{
		Internal_SetLocalRotation_Injected(ref this, ref r);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetLocalScale")]
	private extern void Internal_GetLocalScale(out Vector3 r);

	[NativeMethod(Name = "SetLocalScale")]
	private void Internal_SetLocalScale(Vector3 r)
	{
		Internal_SetLocalScale_Injected(ref this, ref r);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetWorldToLocalMatrix")]
	private extern void Internal_GetWorldToLocalMatrix(out Matrix4x4 m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetLocalToWorldMatrix")]
	private extern void Internal_GetLocalToWorldMatrix(out Matrix4x4 m);

	public bool IsValid()
	{
		return Internal_IsValid();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "IsValid")]
	private extern bool Internal_IsValid();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "Internal_GetRoot")]
	private extern void Internal_GetRoot(out TransformHandle outRootHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TryGetParent")]
	private extern bool Internal_TryGetParent(out TransformHandle parentHandle);

	public void SetParent(TransformHandle p)
	{
		AssertHandleIsValid(this);
		if (p != None && !p.IsValid())
		{
			p = None;
		}
		Internal_SetParent(p, worldPositionStays: true);
	}

	public void SetParent(TransformHandle parent, bool worldPositionStays)
	{
		AssertHandleIsValid(this);
		if (parent != None && !parent.IsValid())
		{
			parent = None;
		}
		Internal_SetParent(parent, worldPositionStays);
	}

	[NativeMethod(Name = "SetParent_Internal")]
	private void Internal_SetParent(TransformHandle parent, bool worldPositionStays)
	{
		Internal_SetParent_Injected(ref this, ref parent, worldPositionStays);
	}

	public TransformHandle GetChild(int index)
	{
		AssertHandleIsValid(this);
		Internal_GetChild(index, out var outChildHandle);
		return outChildHandle;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "Internal_GetChild")]
	private extern void Internal_GetChild(int index, out TransformHandle outChildHandle);

	public bool HasParent()
	{
		AssertHandleIsValid(this);
		return Internal_HasParent();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "HasParent")]
	private extern bool Internal_HasParent();

	public bool IsChildOf(TransformHandle parent)
	{
		AssertHandleIsValid(this);
		if (parent != None)
		{
			AssertHandleIsValid(parent);
		}
		return Internal_IsChildOf(parent);
	}

	[NativeMethod(Name = "IsChildOrSameAsOther")]
	private bool Internal_IsChildOf(TransformHandle parent)
	{
		return Internal_IsChildOf_Injected(ref this, ref parent);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetChildrenCount")]
	private extern int Internal_GetChildrenCount();

	public void DetachChildren()
	{
		AssertHandleIsValid(this);
		Internal_DetachChildren();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "DetachChildren")]
	private extern void Internal_DetachChildren();

	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		AssertHandleIsValid(this);
		Internal_SetPositionAndRotation(position, rotation);
	}

	[NativeMethod(Name = "SetPositionAndRotation")]
	private void Internal_SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		Internal_SetPositionAndRotation_Injected(ref this, ref position, ref rotation);
	}

	public void SetLocalPositionAndRotation(Vector3 localPosition, Quaternion localRotation)
	{
		AssertHandleIsValid(this);
		Internal_SetLocalPositionAndRotation(localPosition, localRotation);
	}

	[NativeMethod(Name = "SetLocalPositionAndRotation")]
	private void Internal_SetLocalPositionAndRotation(Vector3 localPosition, Quaternion localRotation)
	{
		Internal_SetLocalPositionAndRotation_Injected(ref this, ref localPosition, ref localRotation);
	}

	public void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
	{
		AssertHandleIsValid(this);
		Internal_GetPositionAndRotation(out position, out rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetPositionAndRotation")]
	private extern void Internal_GetPositionAndRotation(out Vector3 position, out Quaternion rotation);

	public void GetLocalPositionAndRotation(out Vector3 localPosition, out Quaternion localRotation)
	{
		AssertHandleIsValid(this);
		Internal_GetLocalPositionAndRotation(out localPosition, out localRotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetLocalPositionAndRotation")]
	private extern void Internal_GetLocalPositionAndRotation(out Vector3 localPosition, out Quaternion localRotation);

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

	public void Translate(Vector3 translation, TransformHandle relativeTo)
	{
		if (relativeTo != None)
		{
			position += relativeTo.TransformDirection(translation);
		}
		else
		{
			position += translation;
		}
	}

	public void Translate(float x, float y, float z, TransformHandle relativeTo)
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

	public void Rotate(Vector3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
	{
		AssertHandleIsValid(this);
		if (relativeTo == Space.Self)
		{
			Internal_RotateAround(TransformDirection(axis), angle * (MathF.PI / 180f));
		}
		else
		{
			Internal_RotateAround(axis, angle * (MathF.PI / 180f));
		}
	}

	public void RotateAround(Vector3 point, Vector3 axis, float angle)
	{
		Vector3 vector = position;
		Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
		Vector3 vector2 = vector - point;
		vector2 = quaternion * vector2;
		vector = point + vector2;
		position = vector;
		Internal_RotateAround(axis, angle * (MathF.PI / 180f));
	}

	[NativeMethod(Name = "RotateAround")]
	private void Internal_RotateAround(Vector3 worldAxis, float rad)
	{
		Internal_RotateAround_Injected(ref this, ref worldAxis, rad);
	}

	public void Rotate(Vector3 axis, float angle)
	{
		Rotate(axis, angle, Space.Self);
	}

	public void LookAt(TransformHandle target, [DefaultValue("Vector3.up")] Vector3 worldUp)
	{
		if (target != None)
		{
			AssertHandleIsValid(this);
			AssertHandleIsValid(target);
			Internal_LookAt(target.position, worldUp);
		}
	}

	public void LookAt(TransformHandle target)
	{
		if (target != None)
		{
			AssertHandleIsValid(this);
			AssertHandleIsValid(target);
			Internal_LookAt(target.position, Vector3.up);
		}
	}

	public void LookAt(Vector3 worldPosition, [DefaultValue("Vector3.up")] Vector3 worldUp)
	{
		AssertHandleIsValid(this);
		Internal_LookAt(worldPosition, worldUp);
	}

	public void LookAt(Vector3 worldPosition)
	{
		AssertHandleIsValid(this);
		Internal_LookAt(worldPosition, Vector3.up);
	}

	[NativeMethod(Name = "LookAt")]
	private void Internal_LookAt(Vector3 worldPosition, Vector3 worldUp)
	{
		Internal_LookAt_Injected(ref this, ref worldPosition, ref worldUp);
	}

	public Vector3 TransformPoint(float x, float y, float z)
	{
		AssertHandleIsValid(this);
		return Internal_TransformPoint(new Vector3(x, y, z));
	}

	public Vector3 TransformPoint(Vector3 point)
	{
		AssertHandleIsValid(this);
		return Internal_TransformPoint(point);
	}

	[NativeMethod(Name = "TransformPoint")]
	private Vector3 Internal_TransformPoint(Vector3 point)
	{
		Internal_TransformPoint_Injected(ref this, ref point, out var ret);
		return ret;
	}

	public void TransformPoints(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
	{
		if (positions.Length != transformedPositions.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.TransformPoints() must be the same length");
		}
		AssertHandleIsValid(this);
		Internal_TransformPoints(positions, transformedPositions);
	}

	public void TransformPoints(Span<Vector3> positions)
	{
		AssertHandleIsValid(this);
		Internal_TransformPoints(positions, positions);
	}

	[NativeMethod(Name = "TransformPoints")]
	private unsafe void Internal_TransformPoints(ReadOnlySpan<Vector3> points, Span<Vector3> transformedPoints)
	{
		ReadOnlySpan<Vector3> readOnlySpan = points;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper points2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedPoints;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedPoints2 = new ManagedSpanWrapper(begin2, span.Length);
				Internal_TransformPoints_Injected(ref this, ref points2, ref transformedPoints2);
			}
		}
	}

	public Vector3 TransformDirection(float x, float y, float z)
	{
		AssertHandleIsValid(this);
		return Internal_TransformDirection(new Vector3(x, y, z));
	}

	public Vector3 TransformDirection(Vector3 direction)
	{
		AssertHandleIsValid(this);
		return Internal_TransformDirection(direction);
	}

	[NativeMethod(Name = "TransformDirection")]
	private Vector3 Internal_TransformDirection(Vector3 direction)
	{
		Internal_TransformDirection_Injected(ref this, ref direction, out var ret);
		return ret;
	}

	public void TransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
	{
		if (directions.Length != transformedDirections.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.TransformDirections() must be the same length");
		}
		AssertHandleIsValid(this);
		Internal_TransformDirections(directions, transformedDirections);
	}

	public void TransformDirections(Span<Vector3> directions)
	{
		AssertHandleIsValid(this);
		Internal_TransformDirections(directions, directions);
	}

	[NativeMethod(Name = "TransformDirections")]
	private unsafe void Internal_TransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
	{
		ReadOnlySpan<Vector3> readOnlySpan = directions;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper directions2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedDirections;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedDirections2 = new ManagedSpanWrapper(begin2, span.Length);
				Internal_TransformDirections_Injected(ref this, ref directions2, ref transformedDirections2);
			}
		}
	}

	public Vector3 TransformVector(float x, float y, float z)
	{
		return Internal_TransformVector(new Vector3(x, y, z));
	}

	public Vector3 TransformVector(Vector3 vector)
	{
		return Internal_TransformVector(vector);
	}

	[NativeMethod(Name = "TransformVector")]
	private Vector3 Internal_TransformVector(Vector3 vector)
	{
		Internal_TransformVector_Injected(ref this, ref vector, out var ret);
		return ret;
	}

	public void TransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
	{
		if (vectors.Length != transformedVectors.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.TransformVectors() must be the same length");
		}
		AssertHandleIsValid(this);
		Internal_TransformVectors(vectors, transformedVectors);
	}

	public void TransformVectors(Span<Vector3> vectors)
	{
		AssertHandleIsValid(this);
		Internal_TransformVectors(vectors, vectors);
	}

	[NativeMethod(Name = "TransformVectors")]
	private unsafe void Internal_TransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
	{
		ReadOnlySpan<Vector3> readOnlySpan = vectors;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper vectors2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedVectors;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedVectors2 = new ManagedSpanWrapper(begin2, span.Length);
				Internal_TransformVectors_Injected(ref this, ref vectors2, ref transformedVectors2);
			}
		}
	}

	public Vector3 InverseTransformPoint(float x, float y, float z)
	{
		AssertHandleIsValid(this);
		return Internal_InverseTransformPoint(new Vector3(x, y, z));
	}

	public Vector3 InverseTransformPoint(Vector3 point)
	{
		AssertHandleIsValid(this);
		return Internal_InverseTransformPoint(point);
	}

	[NativeMethod(Name = "InverseTransformPoint")]
	private Vector3 Internal_InverseTransformPoint(Vector3 point)
	{
		Internal_InverseTransformPoint_Injected(ref this, ref point, out var ret);
		return ret;
	}

	public void InverseTransformPoints(ReadOnlySpan<Vector3> positions, Span<Vector3> transformedPositions)
	{
		if (positions.Length != transformedPositions.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.InverseTransformPoints() must be the same length");
		}
		AssertHandleIsValid(this);
		Internal_InverseTransformPoints(positions, transformedPositions);
	}

	public void InverseTransformPoints(Span<Vector3> positions)
	{
		AssertHandleIsValid(this);
		Internal_InverseTransformPoints(positions, positions);
	}

	[NativeMethod(Name = "InverseTransformPoints")]
	private unsafe void Internal_InverseTransformPoints(ReadOnlySpan<Vector3> points, Span<Vector3> transformedPoints)
	{
		ReadOnlySpan<Vector3> readOnlySpan = points;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper points2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedPoints;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedPoints2 = new ManagedSpanWrapper(begin2, span.Length);
				Internal_InverseTransformPoints_Injected(ref this, ref points2, ref transformedPoints2);
			}
		}
	}

	public Vector3 InverseTransformDirection(float x, float y, float z)
	{
		AssertHandleIsValid(this);
		return Internal_InverseTransformDirection(new Vector3(x, y, z));
	}

	public Vector3 InverseTransformDirection(Vector3 direction)
	{
		AssertHandleIsValid(this);
		return Internal_InverseTransformDirection(direction);
	}

	[NativeMethod(Name = "InverseTransformDirection")]
	private Vector3 Internal_InverseTransformDirection(Vector3 direction)
	{
		Internal_InverseTransformDirection_Injected(ref this, ref direction, out var ret);
		return ret;
	}

	public void InverseTransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
	{
		if (directions.Length != transformedDirections.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.InverseTransformDirections() must be the same length");
		}
		AssertHandleIsValid(this);
		Internal_InverseTransformDirections(directions, transformedDirections);
	}

	public void InverseTransformDirections(Span<Vector3> directions)
	{
		AssertHandleIsValid(this);
		Internal_InverseTransformDirections(directions, directions);
	}

	[NativeMethod(Name = "InverseTransformDirections")]
	private unsafe void Internal_InverseTransformDirections(ReadOnlySpan<Vector3> directions, Span<Vector3> transformedDirections)
	{
		ReadOnlySpan<Vector3> readOnlySpan = directions;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper directions2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedDirections;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedDirections2 = new ManagedSpanWrapper(begin2, span.Length);
				Internal_InverseTransformDirections_Injected(ref this, ref directions2, ref transformedDirections2);
			}
		}
	}

	public Vector3 InverseTransformVector(float x, float y, float z)
	{
		AssertHandleIsValid(this);
		return Internal_InverseTransformVector(new Vector3(x, y, z));
	}

	public Vector3 InverseTransformVector(Vector3 vector)
	{
		AssertHandleIsValid(this);
		return Internal_InverseTransformVector(vector);
	}

	[NativeMethod(Name = "InverseTransformVector")]
	private Vector3 Internal_InverseTransformVector(Vector3 vector)
	{
		Internal_InverseTransformVector_Injected(ref this, ref vector, out var ret);
		return ret;
	}

	public void InverseTransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
	{
		if (vectors.Length != transformedVectors.Length)
		{
			throw new InvalidOperationException("Both spans passed to Transform.InverseTransformVectors() must be the same length");
		}
		AssertHandleIsValid(this);
		Internal_InverseTransformVectors(vectors, transformedVectors);
	}

	public void InverseTransformVectors(Span<Vector3> vectors)
	{
		AssertHandleIsValid(this);
		Internal_InverseTransformVectors(vectors, vectors);
	}

	[NativeMethod(Name = "InverseTransformVectors")]
	private unsafe void Internal_InverseTransformVectors(ReadOnlySpan<Vector3> vectors, Span<Vector3> transformedVectors)
	{
		ReadOnlySpan<Vector3> readOnlySpan = vectors;
		fixed (Vector3* begin = readOnlySpan)
		{
			ManagedSpanWrapper vectors2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<Vector3> span = transformedVectors;
			fixed (Vector3* begin2 = span)
			{
				ManagedSpanWrapper transformedVectors2 = new ManagedSpanWrapper(begin2, span.Length);
				Internal_InverseTransformVectors_Injected(ref this, ref vectors2, ref transformedVectors2);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetHierarchyCapacity")]
	private extern int Internal_GetHierarchyCapacity();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("SetHierarchyCapacity")]
	private extern void Internal_SetHierarchyCapacity(int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetHierarchyCount")]
	private extern int Internal_GetHierarchyCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetPosition_Injected(ref TransformHandle _unity_self, [In] ref Vector3 p);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetRotation_Injected(ref TransformHandle _unity_self, [In] ref Quaternion r);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetLocalPosition_Injected(ref TransformHandle _unity_self, [In] ref Vector3 r);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetLocalRotation_Injected(ref TransformHandle _unity_self, [In] ref Quaternion r);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetLocalScale_Injected(ref TransformHandle _unity_self, [In] ref Vector3 r);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetParent_Injected(ref TransformHandle _unity_self, [In] ref TransformHandle parent, bool worldPositionStays);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_IsChildOf_Injected(ref TransformHandle _unity_self, [In] ref TransformHandle parent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetPositionAndRotation_Injected(ref TransformHandle _unity_self, [In] ref Vector3 position, [In] ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetLocalPositionAndRotation_Injected(ref TransformHandle _unity_self, [In] ref Vector3 localPosition, [In] ref Quaternion localRotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RotateAround_Injected(ref TransformHandle _unity_self, [In] ref Vector3 worldAxis, float rad);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_LookAt_Injected(ref TransformHandle _unity_self, [In] ref Vector3 worldPosition, [In] ref Vector3 worldUp);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_TransformPoint_Injected(ref TransformHandle _unity_self, [In] ref Vector3 point, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_TransformPoints_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper points, ref ManagedSpanWrapper transformedPoints);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_TransformDirection_Injected(ref TransformHandle _unity_self, [In] ref Vector3 direction, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_TransformDirections_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper directions, ref ManagedSpanWrapper transformedDirections);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_TransformVector_Injected(ref TransformHandle _unity_self, [In] ref Vector3 vector, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_TransformVectors_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper vectors, ref ManagedSpanWrapper transformedVectors);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_InverseTransformPoint_Injected(ref TransformHandle _unity_self, [In] ref Vector3 point, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_InverseTransformPoints_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper points, ref ManagedSpanWrapper transformedPoints);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_InverseTransformDirection_Injected(ref TransformHandle _unity_self, [In] ref Vector3 direction, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_InverseTransformDirections_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper directions, ref ManagedSpanWrapper transformedDirections);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_InverseTransformVector_Injected(ref TransformHandle _unity_self, [In] ref Vector3 vector, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_InverseTransformVectors_Injected(ref TransformHandle _unity_self, ref ManagedSpanWrapper vectors, ref ManagedSpanWrapper transformedVectors);
}
