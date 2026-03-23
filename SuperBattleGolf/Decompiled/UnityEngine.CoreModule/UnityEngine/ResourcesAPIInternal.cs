using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

[NativeHeader("Runtime/Misc/ResourceManagerUtility.h")]
[NativeHeader("Runtime/Export/Resources/Resources.bindings.h")]
internal static class ResourcesAPIInternal
{
	internal static class EntitiesAssetGC
	{
		internal delegate void AdditionalRootsHandlerDelegate(IntPtr state);

		internal static AdditionalRootsHandlerDelegate AdditionalRootsHandler;

		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("Resources_Bindings::MarkInstanceIDsAsRoot")]
		internal static extern void MarkInstanceIDsAsRoot(IntPtr instanceIDs, int count, IntPtr state);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("Resources_Bindings::EnableEntitiesAssetGCCallback")]
		internal static extern void EnableEntitiesAssetGCCallback();

		internal static void RegisterAdditionalRootsHandler(AdditionalRootsHandlerDelegate newAdditionalRootsHandler)
		{
			if (AdditionalRootsHandler == null)
			{
				EnableEntitiesAssetGCCallback();
				AdditionalRootsHandler = newAdditionalRootsHandler;
			}
			else
			{
				Debug.LogWarning("Attempting to register more than one AdditionalRootsHandlerDelegate! Only one may be registered at a time.");
			}
		}

		[UsedByNativeCode]
		private static void GetAdditionalRoots(IntPtr state)
		{
			if (AdditionalRootsHandler != null)
			{
				AdditionalRootsHandler(state);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Resources_Bindings::FindObjectsOfTypeAll")]
	[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
	public static extern Object[] FindObjectsOfTypeAll(Type type);

	[FreeFunction("GetShaderNameRegistry().FindShader")]
	public unsafe static Shader FindShaderByName(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		Shader result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = FindShaderByName_Injected(ref managedSpanWrapper);
				}
			}
			else
			{
				gcHandlePtr = FindShaderByName_Injected(ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Shader>(gcHandlePtr);
		}
		return result;
	}

	[FreeFunction("Resources_Bindings::Load")]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
	[NativeThrows]
	public unsafe static Object Load(string path, [NotNull] Type systemTypeInstance)
	{
		//The blocks IL_0038 are reachable both inside and outside the pinned region starting at IL_0027. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)systemTypeInstance == null)
		{
			ThrowHelper.ThrowArgumentNullException(systemTypeInstance, "systemTypeInstance");
		}
		IntPtr gcHandlePtr = default(IntPtr);
		Object result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = Load_Injected(ref managedSpanWrapper, systemTypeInstance);
				}
			}
			else
			{
				gcHandlePtr = Load_Injected(ref managedSpanWrapper, systemTypeInstance);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Object>(gcHandlePtr);
		}
		return result;
	}

	[NativeThrows]
	[FreeFunction("Resources_Bindings::LoadAll")]
	public unsafe static Object[] LoadAll([NotNull] string path, [NotNull] Type systemTypeInstance)
	{
		//The blocks IL_0047 are reachable both inside and outside the pinned region starting at IL_0036. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if (path == null)
		{
			ThrowHelper.ThrowArgumentNullException(path, "path");
		}
		if ((object)systemTypeInstance == null)
		{
			ThrowHelper.ThrowArgumentNullException(systemTypeInstance, "systemTypeInstance");
		}
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return LoadAll_Injected(ref managedSpanWrapper, systemTypeInstance);
				}
			}
			return LoadAll_Injected(ref managedSpanWrapper, systemTypeInstance);
		}
		finally
		{
		}
	}

	[FreeFunction("Resources_Bindings::LoadAsyncInternal")]
	internal unsafe static ResourceRequest LoadAsyncInternal(string path, Type type)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr intPtr = default(IntPtr);
		ResourceRequest result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					intPtr = LoadAsyncInternal_Injected(ref managedSpanWrapper, type);
				}
			}
			else
			{
				intPtr = LoadAsyncInternal_Injected(ref managedSpanWrapper, type);
			}
		}
		finally
		{
			IntPtr intPtr2 = intPtr;
			result = ((intPtr2 == (IntPtr)0) ? null : ResourceRequest.BindingsMarshaller.ConvertToManaged(intPtr2));
		}
		return result;
	}

	[FreeFunction("Scripting::UnloadAssetFromScripting")]
	public static void UnloadAsset(Object assetToUnload)
	{
		UnloadAsset_Injected(Object.MarshalledUnityObject.Marshal(assetToUnload));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr FindShaderByName_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Load_Injected(ref ManagedSpanWrapper path, Type systemTypeInstance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Object[] LoadAll_Injected(ref ManagedSpanWrapper path, Type systemTypeInstance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr LoadAsyncInternal_Injected(ref ManagedSpanWrapper path, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UnloadAsset_Injected(IntPtr assetToUnload);
}
