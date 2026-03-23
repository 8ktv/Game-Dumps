using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;

namespace UnityEngine.Experimental.Playables;

[NativeHeader("Runtime/Export/Director/TexturePlayableGraphExtensions.bindings.h")]
[StaticAccessor("TexturePlayableGraphExtensionsBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
internal static class TexturePlayableGraphExtensions
{
	[NativeThrows]
	internal unsafe static bool InternalCreateTextureOutput(ref PlayableGraph graph, string name, out PlayableOutputHandle handle)
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
					return InternalCreateTextureOutput_Injected(ref graph, ref managedSpanWrapper, out handle);
				}
			}
			return InternalCreateTextureOutput_Injected(ref graph, ref managedSpanWrapper, out handle);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InternalCreateTextureOutput_Injected(ref PlayableGraph graph, ref ManagedSpanWrapper name, out PlayableOutputHandle handle);
}
