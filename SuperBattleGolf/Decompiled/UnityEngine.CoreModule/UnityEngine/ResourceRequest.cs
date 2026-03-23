using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
public class ResourceRequest : AsyncOperation
{
	internal new static class BindingsMarshaller
	{
		public static ResourceRequest ConvertToManaged(IntPtr ptr)
		{
			return new ResourceRequest(ptr);
		}
	}

	internal string m_Path;

	internal Type m_Type;

	public Object asset => GetResult();

	protected virtual Object GetResult()
	{
		return Resources.Load(m_Path, m_Type);
	}

	public ResourceRequest()
	{
	}

	protected ResourceRequest(IntPtr ptr)
		: base(ptr)
	{
	}
}
