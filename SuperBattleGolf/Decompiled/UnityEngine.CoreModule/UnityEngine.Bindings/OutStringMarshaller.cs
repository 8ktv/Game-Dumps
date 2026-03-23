using System.Runtime.InteropServices;

namespace UnityEngine.Bindings;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[VisibleToOtherModules]
internal ref struct OutStringMarshaller
{
	public unsafe static string GetStringAndDispose(ManagedSpanWrapper managedSpan)
	{
		if (managedSpan.length == 0)
		{
			return (managedSpan.begin == null) ? null : string.Empty;
		}
		string result = new string((char*)managedSpan.begin, 0, managedSpan.length);
		BindingsAllocator.Free(managedSpan.begin);
		return result;
	}

	public unsafe static void UpdateStringAndDispose(ManagedSpanWrapper inSpanWrapper, ManagedSpanWrapper outSpanWrapper, ref string outString)
	{
		if (inSpanWrapper.begin != outSpanWrapper.begin)
		{
			outString = GetStringAndDispose(outSpanWrapper);
		}
	}
}
