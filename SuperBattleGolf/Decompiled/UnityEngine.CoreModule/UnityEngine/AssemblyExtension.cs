using System;
using System.Reflection;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Scripting/ScriptingRuntime.h")]
internal static class AssemblyExtension
{
	public static string GetLoadedAssemblyPath(this Assembly assembly)
	{
		if (assembly == null)
		{
			throw new ArgumentNullException("assembly");
		}
		return assembly.Location;
	}
}
