using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables;

[NativeHeader("Runtime/Graphics/Director/TexturePlayableOutput.h")]
[RequiredByNativeCode]
[NativeHeader("Runtime/Graphics/RenderTexture.h")]
[StaticAccessor("TexturePlayableOutputBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Runtime/Export/Director/TexturePlayableOutput.bindings.h")]
public struct TexturePlayableOutput : IPlayableOutput
{
	private PlayableOutputHandle m_Handle;

	public static TexturePlayableOutput Null => new TexturePlayableOutput(PlayableOutputHandle.Null);

	public static TexturePlayableOutput Create(PlayableGraph graph, string name, RenderTexture target)
	{
		if (!TexturePlayableGraphExtensions.InternalCreateTextureOutput(ref graph, name, out var handle))
		{
			return Null;
		}
		TexturePlayableOutput result = new TexturePlayableOutput(handle);
		result.SetTarget(target);
		return result;
	}

	internal TexturePlayableOutput(PlayableOutputHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOutputOfType<TexturePlayableOutput>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an TexturePlayableOutput.");
		}
		m_Handle = handle;
	}

	public PlayableOutputHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator PlayableOutput(TexturePlayableOutput output)
	{
		return new PlayableOutput(output.GetHandle());
	}

	public static explicit operator TexturePlayableOutput(PlayableOutput output)
	{
		return new TexturePlayableOutput(output.GetHandle());
	}

	public RenderTexture GetTarget()
	{
		return InternalGetTarget(ref m_Handle);
	}

	public void SetTarget(RenderTexture value)
	{
		InternalSetTarget(ref m_Handle, value);
	}

	[NativeThrows]
	private static RenderTexture InternalGetTarget(ref PlayableOutputHandle output)
	{
		return Unmarshal.UnmarshalUnityObject<RenderTexture>(InternalGetTarget_Injected(ref output));
	}

	[NativeThrows]
	private static void InternalSetTarget(ref PlayableOutputHandle output, RenderTexture target)
	{
		InternalSetTarget_Injected(ref output, Object.MarshalledUnityObject.Marshal(target));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr InternalGetTarget_Injected(ref PlayableOutputHandle output);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetTarget_Injected(ref PlayableOutputHandle output, IntPtr target);
}
