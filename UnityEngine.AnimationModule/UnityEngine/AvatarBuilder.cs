using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Animation/ScriptBindings/AvatarBuilder.bindings.h")]
public class AvatarBuilder
{
	public static Avatar BuildHumanAvatar(GameObject go, HumanDescription humanDescription)
	{
		if (go == null)
		{
			throw new NullReferenceException();
		}
		return BuildHumanAvatarInternal(go, humanDescription);
	}

	[FreeFunction("AvatarBuilderBindings::BuildHumanAvatar")]
	private static Avatar BuildHumanAvatarInternal(GameObject go, HumanDescription humanDescription)
	{
		return Unmarshal.UnmarshalUnityObject<Avatar>(BuildHumanAvatarInternal_Injected(Object.MarshalledUnityObject.Marshal(go), ref humanDescription));
	}

	[FreeFunction("AvatarBuilderBindings::BuildGenericAvatar")]
	public unsafe static Avatar BuildGenericAvatar([NotNull] GameObject go, [NotNull] string rootMotionTransformName)
	{
		//The blocks IL_005c are reachable both inside and outside the pinned region starting at IL_004b. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)go == null)
		{
			ThrowHelper.ThrowArgumentNullException(go, "go");
		}
		if (rootMotionTransformName == null)
		{
			ThrowHelper.ThrowArgumentNullException(rootMotionTransformName, "rootMotionTransformName");
		}
		IntPtr gcHandlePtr = default(IntPtr);
		Avatar result;
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(go);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(rootMotionTransformName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(rootMotionTransformName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = BuildGenericAvatar_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			else
			{
				gcHandlePtr = BuildGenericAvatar_Injected(intPtr, ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Avatar>(gcHandlePtr);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr BuildHumanAvatarInternal_Injected(IntPtr go, [In] ref HumanDescription humanDescription);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr BuildGenericAvatar_Injected(IntPtr go, ref ManagedSpanWrapper rootMotionTransformName);
}
