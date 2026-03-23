using System;
using System.Diagnostics.CodeAnalysis;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
internal static class ThrowHelper
{
	[DoesNotReturn]
	public static void ThrowArgumentNullException(object obj, string parameterName)
	{
		if (obj is Object unityObj)
		{
			Object.MarshalledUnityObject.TryThrowEditorNullExceptionObject(unityObj, parameterName);
		}
		throw new ArgumentNullException(parameterName);
	}

	[DoesNotReturn]
	public static void ThrowNullReferenceException(object obj)
	{
		if (obj is Object unityObj)
		{
			Object.MarshalledUnityObject.TryThrowEditorNullExceptionObject(unityObj, null);
		}
		throw new NullReferenceException();
	}
}
