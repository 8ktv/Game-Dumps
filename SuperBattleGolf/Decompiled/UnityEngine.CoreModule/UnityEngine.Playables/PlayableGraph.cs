using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
[NativeHeader("Runtime/Export/Director/PlayableGraph.bindings.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
public struct PlayableGraph
{
	internal IntPtr m_Handle;

	internal uint m_Version;

	public Playable GetRootPlayable(int index)
	{
		PlayableHandle rootPlayableInternal = GetRootPlayableInternal(index);
		return new Playable(rootPlayableInternal);
	}

	public bool Connect<U, V>(U source, int sourceOutputPort, V destination, int destinationInputPort) where U : struct, IPlayable where V : struct, IPlayable
	{
		return ConnectInternal(source.GetHandle(), sourceOutputPort, destination.GetHandle(), destinationInputPort);
	}

	public void Disconnect<U>(U input, int inputPort) where U : struct, IPlayable
	{
		DisconnectInternal(input.GetHandle(), inputPort);
	}

	public void DestroyPlayable<U>(U playable) where U : struct, IPlayable
	{
		DestroyPlayableInternal(playable.GetHandle());
	}

	public void DestroySubgraph<U>(U playable) where U : struct, IPlayable
	{
		DestroySubgraphInternal(playable.GetHandle());
	}

	public void DestroyOutput<U>(U output) where U : struct, IPlayableOutput
	{
		DestroyOutputInternal(output.GetHandle());
	}

	public int GetOutputCountByType<T>() where T : struct, IPlayableOutput
	{
		return GetOutputCountByTypeInternal(typeof(T));
	}

	public PlayableOutput GetOutput(int index)
	{
		if (!GetOutputInternal(index, out var handle))
		{
			return PlayableOutput.Null;
		}
		return new PlayableOutput(handle);
	}

	public PlayableOutput GetOutputByType<T>(int index) where T : struct, IPlayableOutput
	{
		if (!GetOutputByTypeInternal(typeof(T), index, out var handle))
		{
			return PlayableOutput.Null;
		}
		return new PlayableOutput(handle);
	}

	public void Evaluate()
	{
		Evaluate(0f);
	}

	public static PlayableGraph Create()
	{
		return Create(null);
	}

	public unsafe static PlayableGraph Create(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		PlayableGraph ret = default(PlayableGraph);
		PlayableGraph result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Create_Injected(ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				Create_Injected(ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::Destroy", HasExplicitThis = true, ThrowsException = true)]
	public extern void Destroy();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern bool IsValid();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::IsPlaying", HasExplicitThis = true, ThrowsException = true)]
	public extern bool IsPlaying();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::IsDone", HasExplicitThis = true, ThrowsException = true)]
	public extern bool IsDone();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::Play", HasExplicitThis = true, ThrowsException = true)]
	public extern void Play();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::Stop", HasExplicitThis = true, ThrowsException = true)]
	public extern void Stop();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::Evaluate", HasExplicitThis = true, ThrowsException = true)]
	public extern void Evaluate([DefaultValue("0")] float deltaTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::GetTimeUpdateMode", HasExplicitThis = true, ThrowsException = true)]
	public extern DirectorUpdateMode GetTimeUpdateMode();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::SetTimeUpdateMode", HasExplicitThis = true, ThrowsException = true)]
	public extern void SetTimeUpdateMode(DirectorUpdateMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::GetResolver", HasExplicitThis = true, ThrowsException = true)]
	public extern IExposedPropertyTable GetResolver();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::SetResolver", HasExplicitThis = true, ThrowsException = true)]
	public extern void SetResolver(IExposedPropertyTable value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::GetPlayableCount", HasExplicitThis = true, ThrowsException = true)]
	public extern int GetPlayableCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::GetRootPlayableCount", HasExplicitThis = true, ThrowsException = true)]
	public extern int GetRootPlayableCount();

	[FreeFunction("PlayableGraphBindings::SynchronizeEvaluation", HasExplicitThis = true, ThrowsException = true)]
	internal void SynchronizeEvaluation(PlayableGraph playable)
	{
		SynchronizeEvaluation_Injected(ref this, ref playable);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::GetOutputCount", HasExplicitThis = true, ThrowsException = true)]
	public extern int GetOutputCount();

	[FreeFunction("PlayableGraphBindings::CreatePlayableHandle", HasExplicitThis = true, ThrowsException = true)]
	internal PlayableHandle CreatePlayableHandle()
	{
		CreatePlayableHandle_Injected(ref this, out var ret);
		return ret;
	}

	[FreeFunction("PlayableGraphBindings::CreateScriptOutputInternal", HasExplicitThis = true, ThrowsException = true)]
	internal unsafe bool CreateScriptOutputInternal(string name, out PlayableOutputHandle handle)
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
					return CreateScriptOutputInternal_Injected(ref this, ref managedSpanWrapper, out handle);
				}
			}
			return CreateScriptOutputInternal_Injected(ref this, ref managedSpanWrapper, out handle);
		}
		finally
		{
		}
	}

	[FreeFunction("PlayableGraphBindings::GetRootPlayableInternal", HasExplicitThis = true, ThrowsException = true)]
	internal PlayableHandle GetRootPlayableInternal(int index)
	{
		GetRootPlayableInternal_Injected(ref this, index, out var ret);
		return ret;
	}

	[FreeFunction("PlayableGraphBindings::DestroyOutputInternal", HasExplicitThis = true, ThrowsException = true)]
	internal void DestroyOutputInternal(PlayableOutputHandle handle)
	{
		DestroyOutputInternal_Injected(ref this, ref handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::IsMatchFrameRateEnabled", HasExplicitThis = true, ThrowsException = true)]
	internal extern bool IsMatchFrameRateEnabled();

	[FreeFunction("PlayableGraphBindings::EnableMatchFrameRate", HasExplicitThis = true, ThrowsException = true)]
	internal void EnableMatchFrameRate(FrameRate frameRate)
	{
		EnableMatchFrameRate_Injected(ref this, ref frameRate);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::DisableMatchFrameRate", HasExplicitThis = true, ThrowsException = true)]
	internal extern void DisableMatchFrameRate();

	[FreeFunction("PlayableGraphBindings::GetFrameRate", HasExplicitThis = true, ThrowsException = true)]
	internal FrameRate GetFrameRate()
	{
		GetFrameRate_Injected(ref this, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::GetOutputInternal", HasExplicitThis = true, ThrowsException = true)]
	private extern bool GetOutputInternal(int index, out PlayableOutputHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::GetOutputCountByTypeInternal", HasExplicitThis = true, ThrowsException = true)]
	private extern int GetOutputCountByTypeInternal(Type outputType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayableGraphBindings::GetOutputByTypeInternal", HasExplicitThis = true, ThrowsException = true)]
	private extern bool GetOutputByTypeInternal(Type outputType, int index, out PlayableOutputHandle handle);

	[FreeFunction("PlayableGraphBindings::ConnectInternal", HasExplicitThis = true, ThrowsException = true)]
	private bool ConnectInternal(PlayableHandle source, int sourceOutputPort, PlayableHandle destination, int destinationInputPort)
	{
		return ConnectInternal_Injected(ref this, ref source, sourceOutputPort, ref destination, destinationInputPort);
	}

	[FreeFunction("PlayableGraphBindings::DisconnectInternal", HasExplicitThis = true, ThrowsException = true)]
	private void DisconnectInternal(PlayableHandle playable, int inputPort)
	{
		DisconnectInternal_Injected(ref this, ref playable, inputPort);
	}

	[FreeFunction("PlayableGraphBindings::DestroyPlayableInternal", HasExplicitThis = true, ThrowsException = true)]
	private void DestroyPlayableInternal(PlayableHandle playable)
	{
		DestroyPlayableInternal_Injected(ref this, ref playable);
	}

	[FreeFunction("PlayableGraphBindings::DestroySubgraphInternal", HasExplicitThis = true, ThrowsException = true)]
	private void DestroySubgraphInternal(PlayableHandle playable)
	{
		DestroySubgraphInternal_Injected(ref this, ref playable);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Create_Injected(ref ManagedSpanWrapper name, out PlayableGraph ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SynchronizeEvaluation_Injected(ref PlayableGraph _unity_self, [In] ref PlayableGraph playable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreatePlayableHandle_Injected(ref PlayableGraph _unity_self, out PlayableHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateScriptOutputInternal_Injected(ref PlayableGraph _unity_self, ref ManagedSpanWrapper name, out PlayableOutputHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRootPlayableInternal_Injected(ref PlayableGraph _unity_self, int index, out PlayableHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DestroyOutputInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableOutputHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableMatchFrameRate_Injected(ref PlayableGraph _unity_self, [In] ref FrameRate frameRate);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetFrameRate_Injected(ref PlayableGraph _unity_self, out FrameRate ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ConnectInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableHandle source, int sourceOutputPort, [In] ref PlayableHandle destination, int destinationInputPort);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisconnectInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableHandle playable, int inputPort);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DestroyPlayableInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableHandle playable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DestroySubgraphInternal_Injected(ref PlayableGraph _unity_self, [In] ref PlayableHandle playable);
}
