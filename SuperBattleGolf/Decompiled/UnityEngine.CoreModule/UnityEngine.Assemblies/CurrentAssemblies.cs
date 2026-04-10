using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.Scripting.LifecycleManagement;

namespace UnityEngine.Assemblies;

internal static class CurrentAssemblies
{
	private struct AssemblyLoadContextStateHelper
	{
		public MethodInfo GetAssemblyLoadContextMethod;

		public FieldInfo AssemblyLoadContextStateField;
	}

	[NoAutoStaticsCleanup]
	private static readonly AssemblyLoadContextStateHelper k_AssemblyLoadContextStateHelper = GetAssemblyLoadContextStateHelperImpl();

	private static AssemblyLoadContextStateHelper GetAssemblyLoadContextStateHelperImpl()
	{
		MethodInfo methodInfo = Type.GetType("System.Runtime.Loader.AssemblyLoadContext")?.GetMethod("GetLoadContext", BindingFlags.Static | BindingFlags.Public);
		if (methodInfo == null)
		{
			return default(AssemblyLoadContextStateHelper);
		}
		FieldInfo assemblyLoadContextStateField = methodInfo.DeclaringType?.GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic);
		return new AssemblyLoadContextStateHelper
		{
			GetAssemblyLoadContextMethod = methodInfo,
			AssemblyLoadContextStateField = assemblyLoadContextStateField
		};
	}

	private static bool IsFromLiveAssemblyLoadContext(Assembly assembly)
	{
		object obj = k_AssemblyLoadContextStateHelper.GetAssemblyLoadContextMethod.Invoke(null, new object[1] { assembly });
		if (obj == null)
		{
			return true;
		}
		object value = k_AssemblyLoadContextStateHelper.AssemblyLoadContextStateField?.GetValue(obj);
		return Convert.ToInt32(value) == 0;
	}

	internal static IReadOnlyList<Assembly> GetLoadedAssemblies()
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		if (k_AssemblyLoadContextStateHelper.GetAssemblyLoadContextMethod == null)
		{
			return assemblies;
		}
		List<Assembly> list = new List<Assembly>();
		Assembly[] array = assemblies;
		foreach (Assembly assembly in array)
		{
			if (IsFromLiveAssemblyLoadContext(assembly))
			{
				list.Add(assembly);
			}
		}
		return list;
	}

	internal static Assembly LoadFromPath(string assemblyPath)
	{
		if (!Path.IsPathFullyQualified(assemblyPath))
		{
			throw new ArgumentException("Assembly path must be fully qualified", "assemblyPath");
		}
		return Assembly.LoadFrom(assemblyPath);
	}

	internal static Assembly LoadFromBytes(byte[] rawAssembly)
	{
		return LoadFromBytes(rawAssembly, null);
	}

	internal static Assembly LoadFromBytes(byte[] rawAssembly, byte[] rawSymbolStore)
	{
		if (rawAssembly == null)
		{
			throw new ArgumentNullException("rawAssembly");
		}
		if (rawAssembly.Length == 0)
		{
			throw new BadImageFormatException("Empty raw assembly byte array");
		}
		if (rawSymbolStore != null && rawSymbolStore.Length == 0)
		{
			throw new BadImageFormatException("Empty raw assembly symbols byte array");
		}
		return Assembly.Load(rawAssembly, rawSymbolStore);
	}
}
