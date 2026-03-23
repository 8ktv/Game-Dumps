using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[StaticAccessor("GetQualitySettings()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Graphics/QualitySettings.h")]
public static class TextureMipmapLimitGroups
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetTextureMipmapLimitGroupNames")]
	public static extern string[] GetGroups();

	[NativeName("HasTextureMipmapLimitGroup")]
	public unsafe static bool HasGroup([NotNull] string groupName)
	{
		//The blocks IL_0038 are reachable both inside and outside the pinned region starting at IL_0027. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if (groupName == null)
		{
			ThrowHelper.ThrowArgumentNullException(groupName, "groupName");
		}
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(groupName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(groupName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return HasGroup_Injected(ref managedSpanWrapper);
				}
			}
			return HasGroup_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasGroup_Injected(ref ManagedSpanWrapper groupName);
}
