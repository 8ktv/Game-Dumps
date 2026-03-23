using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine.Bindings;

namespace Unity.Content;

[StaticAccessor("GetContentNamespaceManager()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Misc/ContentNamespace.h")]
public struct ContentNamespace
{
	internal ulong Id;

	private static bool s_defaultInitialized = false;

	private static ContentNamespace s_Default;

	private static Regex s_ValidName = new Regex("^[a-zA-Z0-9]{1,16}$", RegexOptions.Compiled);

	public bool IsValid => IsNamespaceHandleValid(this);

	public static ContentNamespace Default
	{
		get
		{
			if (!s_defaultInitialized)
			{
				s_defaultInitialized = true;
				s_Default = GetOrCreateNamespace("default");
			}
			return s_Default;
		}
	}

	public string GetName()
	{
		ThrowIfInvalidNamespace();
		return GetNamespaceName(this);
	}

	public void Delete()
	{
		if (Id == s_Default.Id)
		{
			throw new InvalidOperationException("Cannot delete the default namespace.");
		}
		ThrowIfInvalidNamespace();
		RemoveNamespace(this);
	}

	private void ThrowIfInvalidNamespace()
	{
		if (!IsValid)
		{
			throw new InvalidOperationException("The provided namespace is invalid. Did you already delete it?");
		}
	}

	public static ContentNamespace GetOrCreateNamespace(string name)
	{
		if (s_ValidName.IsMatch(name))
		{
			return GetOrCreate(name);
		}
		throw new InvalidOperationException("Namespace name can only contain alphanumeric characters and a maximum length of 16 characters.");
	}

	public static ContentNamespace[] GetAll()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		ContentNamespace[] result;
		try
		{
			GetAll_Injected(out ret);
		}
		finally
		{
			ContentNamespace[] array = default(ContentNamespace[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	internal unsafe static ContentNamespace GetOrCreate(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ContentNamespace ret = default(ContentNamespace);
		ContentNamespace result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetOrCreate_Injected(ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				GetOrCreate_Injected(ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	internal static void RemoveNamespace(ContentNamespace ns)
	{
		RemoveNamespace_Injected(ref ns);
	}

	internal static string GetNamespaceName(ContentNamespace ns)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetNamespaceName_Injected(ref ns, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	internal static bool IsNamespaceHandleValid(ContentNamespace ns)
	{
		return IsNamespaceHandleValid_Injected(ref ns);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAll_Injected(out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetOrCreate_Injected(ref ManagedSpanWrapper name, out ContentNamespace ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveNamespace_Injected([In] ref ContentNamespace ns);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetNamespaceName_Injected([In] ref ContentNamespace ns, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsNamespaceHandleValid_Injected([In] ref ContentNamespace ns);
}
