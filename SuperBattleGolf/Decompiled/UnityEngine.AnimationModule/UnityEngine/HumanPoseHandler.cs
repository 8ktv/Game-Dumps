using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Animation/HumanPoseHandler.h")]
[NativeHeader("Modules/Animation/ScriptBindings/Animation.bindings.h")]
public class HumanPoseHandler : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(HumanPoseHandler humanPoseHandler)
		{
			return humanPoseHandler.m_Ptr;
		}
	}

	internal IntPtr m_Ptr;

	[FreeFunction("AnimationBindings::CreateHumanPoseHandler")]
	private static IntPtr Internal_CreateFromRoot(Avatar avatar, Transform root)
	{
		return Internal_CreateFromRoot_Injected(Object.MarshalledUnityObject.Marshal(avatar), Object.MarshalledUnityObject.Marshal(root));
	}

	[FreeFunction("AnimationBindings::CreateHumanPoseHandler", IsThreadSafe = true)]
	private static IntPtr Internal_CreateFromJointPaths(Avatar avatar, string[] jointPaths)
	{
		return Internal_CreateFromJointPaths_Injected(Object.MarshalledUnityObject.Marshal(avatar), jointPaths);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AnimationBindings::DestroyHumanPoseHandler")]
	private static extern void Internal_Destroy(IntPtr ptr);

	private unsafe void GetHumanPose(out Vector3 bodyPosition, out Quaternion bodyRotation, [Out] float[] muscles, [Out] Vector3[] ikGoalPositions, [Out] Quaternion[] ikGoalRotations)
	{
		//The blocks IL_002d, IL_0034, IL_0039, IL_003b, IL_004d, IL_0054, IL_005b, IL_005d, IL_0071 are reachable both inside and outside the pinned region starting at IL_0016. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_004d, IL_0054, IL_005b, IL_005d, IL_0071 are reachable both inside and outside the pinned region starting at IL_0036. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0071 are reachable both inside and outside the pinned region starting at IL_0056. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper blittableArrayWrapper = default(BlittableArrayWrapper);
		BlittableArrayWrapper blittableArrayWrapper2 = default(BlittableArrayWrapper);
		BlittableArrayWrapper ikGoalRotations2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ref BlittableArrayWrapper muscles2;
			ref BlittableArrayWrapper ikGoalPositions2;
			if (muscles != null)
			{
				fixed (float[] array = muscles)
				{
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					muscles2 = ref blittableArrayWrapper;
					if (ikGoalPositions != null)
					{
						fixed (Vector3[] array2 = ikGoalPositions)
						{
							if (array2.Length != 0)
							{
								blittableArrayWrapper2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array2[0]), array2.Length);
							}
							ikGoalPositions2 = ref blittableArrayWrapper2;
							if (ikGoalRotations != null)
							{
								fixed (Quaternion[] array3 = ikGoalRotations)
								{
									if (array3.Length != 0)
									{
										ikGoalRotations2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array3[0]), array3.Length);
									}
									GetHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out muscles2, out ikGoalPositions2, out ikGoalRotations2);
									return;
								}
							}
							GetHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out muscles2, out ikGoalPositions2, out ikGoalRotations2);
							return;
						}
					}
					ikGoalPositions2 = ref blittableArrayWrapper2;
					if (ikGoalRotations != null)
					{
						array3 = ikGoalRotations;
						if (array3.Length != 0)
						{
							ikGoalRotations2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array3[0]), array3.Length);
						}
					}
					GetHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out muscles2, out ikGoalPositions2, out ikGoalRotations2);
					return;
				}
			}
			muscles2 = ref blittableArrayWrapper;
			if (ikGoalPositions != null)
			{
				array2 = ikGoalPositions;
				if (array2.Length != 0)
				{
					blittableArrayWrapper2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array2[0]), array2.Length);
				}
			}
			ikGoalPositions2 = ref blittableArrayWrapper2;
			if (ikGoalRotations != null)
			{
				array3 = ikGoalRotations;
				if (array3.Length != 0)
				{
					ikGoalRotations2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array3[0]), array3.Length);
				}
			}
			GetHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out muscles2, out ikGoalPositions2, out ikGoalRotations2);
		}
		finally
		{
			blittableArrayWrapper.Unmarshal(ref array);
			blittableArrayWrapper2.Unmarshal(ref array2);
			ikGoalRotations2.Unmarshal(ref array3);
		}
	}

	private unsafe void SetHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(muscles);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper muscles2 = new ManagedSpanWrapper(begin, span.Length);
			SetHumanPose_Injected(intPtr, ref bodyPosition, ref bodyRotation, ref muscles2);
		}
	}

	[ThreadSafe]
	private unsafe void GetInternalHumanPose(out Vector3 bodyPosition, out Quaternion bodyRotation, [Out] float[] muscles, [Out] Vector3[] ikGoalPositions, [Out] Quaternion[] ikGoalRotation)
	{
		//The blocks IL_002d, IL_0034, IL_0039, IL_003b, IL_004d, IL_0054, IL_005b, IL_005d, IL_0071 are reachable both inside and outside the pinned region starting at IL_0016. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_004d, IL_0054, IL_005b, IL_005d, IL_0071 are reachable both inside and outside the pinned region starting at IL_0036. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0071 are reachable both inside and outside the pinned region starting at IL_0056. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper blittableArrayWrapper = default(BlittableArrayWrapper);
		BlittableArrayWrapper blittableArrayWrapper2 = default(BlittableArrayWrapper);
		BlittableArrayWrapper ikGoalRotation2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ref BlittableArrayWrapper muscles2;
			ref BlittableArrayWrapper ikGoalPositions2;
			if (muscles != null)
			{
				fixed (float[] array = muscles)
				{
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					muscles2 = ref blittableArrayWrapper;
					if (ikGoalPositions != null)
					{
						fixed (Vector3[] array2 = ikGoalPositions)
						{
							if (array2.Length != 0)
							{
								blittableArrayWrapper2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array2[0]), array2.Length);
							}
							ikGoalPositions2 = ref blittableArrayWrapper2;
							if (ikGoalRotation != null)
							{
								fixed (Quaternion[] array3 = ikGoalRotation)
								{
									if (array3.Length != 0)
									{
										ikGoalRotation2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array3[0]), array3.Length);
									}
									GetInternalHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out muscles2, out ikGoalPositions2, out ikGoalRotation2);
									return;
								}
							}
							GetInternalHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out muscles2, out ikGoalPositions2, out ikGoalRotation2);
							return;
						}
					}
					ikGoalPositions2 = ref blittableArrayWrapper2;
					if (ikGoalRotation != null)
					{
						array3 = ikGoalRotation;
						if (array3.Length != 0)
						{
							ikGoalRotation2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array3[0]), array3.Length);
						}
					}
					GetInternalHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out muscles2, out ikGoalPositions2, out ikGoalRotation2);
					return;
				}
			}
			muscles2 = ref blittableArrayWrapper;
			if (ikGoalPositions != null)
			{
				array2 = ikGoalPositions;
				if (array2.Length != 0)
				{
					blittableArrayWrapper2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array2[0]), array2.Length);
				}
			}
			ikGoalPositions2 = ref blittableArrayWrapper2;
			if (ikGoalRotation != null)
			{
				array3 = ikGoalRotation;
				if (array3.Length != 0)
				{
					ikGoalRotation2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array3[0]), array3.Length);
				}
			}
			GetInternalHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out muscles2, out ikGoalPositions2, out ikGoalRotation2);
		}
		finally
		{
			blittableArrayWrapper.Unmarshal(ref array);
			blittableArrayWrapper2.Unmarshal(ref array2);
			ikGoalRotation2.Unmarshal(ref array3);
		}
	}

	[ThreadSafe]
	private unsafe void SetInternalHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(muscles);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper muscles2 = new ManagedSpanWrapper(begin, span.Length);
			SetInternalHumanPose_Injected(intPtr, ref bodyPosition, ref bodyRotation, ref muscles2);
		}
	}

	[ThreadSafe]
	private unsafe void GetInternalAvatarPose(void* avatarPose, int avatarPoseLength)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetInternalAvatarPose_Injected(intPtr, avatarPose, avatarPoseLength);
	}

	[ThreadSafe]
	private unsafe void SetInternalAvatarPose(void* avatarPose, int avatarPoseLength)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetInternalAvatarPose_Injected(intPtr, avatarPose, avatarPoseLength);
	}

	public void Dispose()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	public HumanPoseHandler(Avatar avatar, Transform root)
	{
		m_Ptr = IntPtr.Zero;
		if (root == null)
		{
			throw new ArgumentNullException("HumanPoseHandler root Transform is null");
		}
		if (avatar == null)
		{
			throw new ArgumentNullException("HumanPoseHandler avatar is null");
		}
		if (!avatar.isValid)
		{
			throw new ArgumentException("HumanPoseHandler avatar is invalid");
		}
		if (!avatar.isHuman)
		{
			throw new ArgumentException("HumanPoseHandler avatar is not human");
		}
		m_Ptr = Internal_CreateFromRoot(avatar, root);
	}

	public HumanPoseHandler(Avatar avatar, string[] jointPaths)
	{
		m_Ptr = IntPtr.Zero;
		if (jointPaths == null)
		{
			throw new ArgumentNullException("HumanPoseHandler jointPaths array is null");
		}
		if (avatar == null)
		{
			throw new ArgumentNullException("HumanPoseHandler avatar is null");
		}
		if (!avatar.isValid)
		{
			throw new ArgumentException("HumanPoseHandler avatar is invalid");
		}
		if (!avatar.isHuman)
		{
			throw new ArgumentException("HumanPoseHandler avatar is not human");
		}
		m_Ptr = Internal_CreateFromJointPaths(avatar, jointPaths);
	}

	private static void CalculateIKOffsets(in Quaternion[] sourceRotations, ref Quaternion[] destRotations)
	{
		for (int i = 0; i < 4; i++)
		{
			destRotations[i] = sourceRotations[i] * HumanPose.s_IKGoalOffsets[i];
		}
	}

	public void GetHumanPose(ref HumanPose humanPose)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new NullReferenceException("HumanPoseHandler is not initialized properly");
		}
		humanPose.Init();
		GetHumanPose(out humanPose.bodyPosition, out humanPose.bodyRotation, humanPose.muscles, humanPose.m_IkGoalPositions, humanPose.m_IkGoalRotations);
		CalculateIKOffsets(in humanPose.m_IkGoalRotations, ref humanPose.m_OffsetIkGoalRotations);
	}

	public void SetHumanPose(ref HumanPose humanPose)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new NullReferenceException("HumanPoseHandler is not initialized properly");
		}
		humanPose.Init();
		SetHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles);
	}

	public void GetInternalHumanPose(ref HumanPose humanPose)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new NullReferenceException("HumanPoseHandler is not initialized properly");
		}
		humanPose.Init();
		GetInternalHumanPose(out humanPose.bodyPosition, out humanPose.bodyRotation, humanPose.muscles, humanPose.m_IkGoalPositions, humanPose.m_IkGoalRotations);
		CalculateIKOffsets(in humanPose.m_IkGoalRotations, ref humanPose.m_OffsetIkGoalRotations);
	}

	public void SetInternalHumanPose(ref HumanPose humanPose)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new NullReferenceException("HumanPoseHandler is not initialized properly");
		}
		humanPose.Init();
		SetInternalHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles);
	}

	public unsafe void GetInternalAvatarPose(NativeArray<float> avatarPose)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new NullReferenceException("HumanPoseHandler is not initialized properly");
		}
		GetInternalAvatarPose(avatarPose.GetUnsafePtr(), avatarPose.Length);
	}

	public unsafe void SetInternalAvatarPose(NativeArray<float> avatarPose)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new NullReferenceException("HumanPoseHandler is not initialized properly");
		}
		SetInternalAvatarPose(avatarPose.GetUnsafeReadOnlyPtr(), avatarPose.Length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_CreateFromRoot_Injected(IntPtr avatar, IntPtr root);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_CreateFromJointPaths_Injected(IntPtr avatar, string[] jointPaths);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetHumanPose_Injected(IntPtr _unity_self, out Vector3 bodyPosition, out Quaternion bodyRotation, out BlittableArrayWrapper muscles, out BlittableArrayWrapper ikGoalPositions, out BlittableArrayWrapper ikGoalRotations);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetHumanPose_Injected(IntPtr _unity_self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, ref ManagedSpanWrapper muscles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetInternalHumanPose_Injected(IntPtr _unity_self, out Vector3 bodyPosition, out Quaternion bodyRotation, out BlittableArrayWrapper muscles, out BlittableArrayWrapper ikGoalPositions, out BlittableArrayWrapper ikGoalRotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetInternalHumanPose_Injected(IntPtr _unity_self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, ref ManagedSpanWrapper muscles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void GetInternalAvatarPose_Injected(IntPtr _unity_self, void* avatarPose, int avatarPoseLength);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void SetInternalAvatarPose_Injected(IntPtr _unity_self, void* avatarPose, int avatarPoseLength);
}
