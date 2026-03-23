using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeHeader("Runtime/Scripting/ApiRestrictions.h")]
[ExtensionOfNativeClass]
[StaticAccessor("GetApiRestrictions()", StaticAccessorType.Arrow)]
internal class ApiRestrictions
{
	internal enum GlobalRestrictions
	{
		OBJECT_DESTROYIMMEDIATE,
		OBJECT_SENDMESSAGE,
		OBJECT_RENDERING,
		GLOBALCOUNT
	}

	internal enum ContextRestrictions
	{
		RENDERERSCENE_ADDREMOVE,
		OBJECT_ADDCOMPONENTTRANSFORM,
		CONTEXTCOUNT
	}

	internal static void PushDisableApiInternal(ContextRestrictions contextApi, Object context, GlobalRestrictions globalApi)
	{
		PushDisableApiInternal_Injected(contextApi, Object.MarshalledUnityObject.Marshal(context), globalApi);
	}

	internal static void PopDisableApiInternal(ContextRestrictions contextApi, Object context, GlobalRestrictions globalApi)
	{
		PopDisableApiInternal_Injected(contextApi, Object.MarshalledUnityObject.Marshal(context), globalApi);
	}

	internal static bool TryApiInternal(ContextRestrictions contextApi, Object context, GlobalRestrictions globalApi, bool allowErrorLogging)
	{
		return TryApiInternal_Injected(contextApi, Object.MarshalledUnityObject.Marshal(context), globalApi, allowErrorLogging);
	}

	internal static void PushDisableApi(ContextRestrictions api, Object owner)
	{
		PushDisableApiInternal(api, owner, GlobalRestrictions.GLOBALCOUNT);
	}

	internal static void PushDisableApi(GlobalRestrictions api)
	{
		PushDisableApiInternal(ContextRestrictions.CONTEXTCOUNT, null, api);
	}

	internal static void PopDisableApi(ContextRestrictions api, Object context)
	{
		PopDisableApiInternal(api, context, GlobalRestrictions.GLOBALCOUNT);
	}

	internal static void PopDisableApi(GlobalRestrictions api)
	{
		PopDisableApiInternal(ContextRestrictions.CONTEXTCOUNT, null, api);
	}

	internal static bool TryApi(ContextRestrictions api, Object context, bool allowErrorLogging = true)
	{
		return TryApiInternal(api, context, GlobalRestrictions.GLOBALCOUNT, allowErrorLogging);
	}

	internal static bool TryApi(GlobalRestrictions api, bool allowErrorLogging = true)
	{
		return TryApiInternal(ContextRestrictions.CONTEXTCOUNT, null, api, allowErrorLogging);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PushDisableApiInternal_Injected(ContextRestrictions contextApi, IntPtr context, GlobalRestrictions globalApi);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PopDisableApiInternal_Injected(ContextRestrictions contextApi, IntPtr context, GlobalRestrictions globalApi);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool TryApiInternal_Injected(ContextRestrictions contextApi, IntPtr context, GlobalRestrictions globalApi, bool allowErrorLogging);
}
