using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;

namespace UnityEngine.Animations;

[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[StaticAccessor("AnimationPlayableGraphExtensionsBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
[NativeHeader("Modules/Animation/Animator.h")]
[NativeHeader("Modules/Animation/ScriptBindings/AnimationPlayableGraphExtensions.bindings.h")]
internal static class AnimationPlayableGraphExtensions
{
	internal static void SyncUpdateAndTimeMode(this PlayableGraph graph, Animator animator)
	{
		InternalSyncUpdateAndTimeMode(ref graph, animator);
	}

	internal static void DestroyOutput(this PlayableGraph graph, PlayableOutputHandle handle)
	{
		InternalDestroyOutput(ref graph, ref handle);
	}

	[NativeThrows]
	internal unsafe static bool InternalCreateAnimationOutput(ref PlayableGraph graph, string name, out PlayableOutputHandle handle)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return InternalCreateAnimationOutput_Injected(ref graph, ref managedSpanWrapper, out handle);
				}
			}
			return InternalCreateAnimationOutput_Injected(ref graph, ref managedSpanWrapper, out handle);
		}
		finally
		{
		}
	}

	[NativeThrows]
	internal static void InternalSyncUpdateAndTimeMode(ref PlayableGraph graph, [NotNull] Animator animator)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		InternalSyncUpdateAndTimeMode_Injected(ref graph, intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern void InternalDestroyOutput(ref PlayableGraph graph, ref PlayableOutputHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern int InternalAnimationOutputCount(ref PlayableGraph graph);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern bool InternalGetAnimationOutput(ref PlayableGraph graph, int index, out PlayableOutputHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InternalCreateAnimationOutput_Injected(ref PlayableGraph graph, ref ManagedSpanWrapper name, out PlayableOutputHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSyncUpdateAndTimeMode_Injected(ref PlayableGraph graph, IntPtr animator);
}
