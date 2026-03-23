using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequiredByNativeCode]
[NativeHeader("Runtime/Scripting/DelayedCallUtility.h")]
[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
[ExtensionOfNativeClass]
public class MonoBehaviour : Behaviour
{
	private CancellationTokenSource m_CancellationTokenSource;

	public CancellationToken destroyCancellationToken
	{
		get
		{
			if (this == null)
			{
				throw new MissingReferenceException("DestroyCancellation token should be called atleast once before destroying the monobehaviour object");
			}
			if (m_CancellationTokenSource == null)
			{
				m_CancellationTokenSource = new CancellationTokenSource();
				OnCancellationTokenCreated();
			}
			return m_CancellationTokenSource.Token;
		}
	}

	public bool useGUILayout
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useGUILayout_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useGUILayout_Injected(intPtr, value);
		}
	}

	public bool didStart
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_didStart_Injected(intPtr);
		}
	}

	public bool didAwake
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_didAwake_Injected(intPtr);
		}
	}

	[RequiredByNativeCode]
	private void RaiseCancellation()
	{
		m_CancellationTokenSource?.Cancel();
	}

	public bool IsInvoking()
	{
		return Internal_IsInvokingAll(this);
	}

	public void CancelInvoke()
	{
		Internal_CancelInvokeAll(this);
	}

	public void Invoke(string methodName, float time)
	{
		InvokeDelayed(this, methodName, time, 0f);
	}

	public void InvokeRepeating(string methodName, float time, float repeatRate)
	{
		if (repeatRate <= 1E-05f && repeatRate != 0f)
		{
			throw new UnityException("Invoke repeat rate has to be larger than 0.00001F");
		}
		InvokeDelayed(this, methodName, time, repeatRate);
	}

	public void CancelInvoke(string methodName)
	{
		CancelInvoke(this, methodName);
	}

	public bool IsInvoking(string methodName)
	{
		return IsInvoking(this, methodName);
	}

	[ExcludeFromDocs]
	public Coroutine StartCoroutine(string methodName)
	{
		object value = null;
		return StartCoroutine(methodName, value);
	}

	public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
	{
		if (string.IsNullOrEmpty(methodName))
		{
			throw new NullReferenceException("methodName is null or empty");
		}
		if (!IsObjectMonoBehaviour(this))
		{
			throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
		}
		return StartCoroutineManaged(methodName, value);
	}

	public Coroutine StartCoroutine(IEnumerator routine)
	{
		if (routine == null)
		{
			throw new NullReferenceException("routine is null");
		}
		if (!IsObjectMonoBehaviour(this))
		{
			throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
		}
		return StartCoroutineManaged2(routine);
	}

	[Obsolete("StartCoroutine_Auto has been deprecated. Use StartCoroutine instead (UnityUpgradable) -> StartCoroutine([mscorlib] System.Collections.IEnumerator)", false)]
	public Coroutine StartCoroutine_Auto(IEnumerator routine)
	{
		return StartCoroutine(routine);
	}

	public void StopCoroutine(IEnumerator routine)
	{
		if (routine == null)
		{
			throw new NullReferenceException("routine is null");
		}
		if (!IsObjectMonoBehaviour(this))
		{
			throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
		}
		StopCoroutineFromEnumeratorManaged(routine);
	}

	public void StopCoroutine(Coroutine routine)
	{
		if (routine == null)
		{
			throw new NullReferenceException("routine is null");
		}
		if (!IsObjectMonoBehaviour(this))
		{
			throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
		}
		StopCoroutineManaged(routine);
	}

	public unsafe void StopCoroutine(string methodName)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(methodName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					StopCoroutine_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			StopCoroutine_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public void StopAllCoroutines()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StopAllCoroutines_Injected(intPtr);
	}

	public static void print(object message)
	{
		Debug.Log(message);
	}

	[FreeFunction("CancelInvoke")]
	private static void Internal_CancelInvokeAll([NotNull] MonoBehaviour self)
	{
		if ((object)self == null)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(self);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		Internal_CancelInvokeAll_Injected(intPtr);
	}

	[FreeFunction("IsInvoking")]
	private static bool Internal_IsInvokingAll([NotNull] MonoBehaviour self)
	{
		if ((object)self == null)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(self);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		return Internal_IsInvokingAll_Injected(intPtr);
	}

	[FreeFunction]
	private unsafe static void InvokeDelayed([NotNull] MonoBehaviour self, string methodName, float time, float repeatRate)
	{
		//The blocks IL_004d are reachable both inside and outside the pinned region starting at IL_003c. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)self == null)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(self);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(methodName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					InvokeDelayed_Injected(intPtr, ref managedSpanWrapper, time, repeatRate);
					return;
				}
			}
			InvokeDelayed_Injected(intPtr, ref managedSpanWrapper, time, repeatRate);
		}
		finally
		{
		}
	}

	[FreeFunction]
	private unsafe static void CancelInvoke([NotNull] MonoBehaviour self, string methodName)
	{
		//The blocks IL_004d are reachable both inside and outside the pinned region starting at IL_003c. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)self == null)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(self);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(methodName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					CancelInvoke_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			CancelInvoke_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction]
	private unsafe static bool IsInvoking([NotNull] MonoBehaviour self, string methodName)
	{
		//The blocks IL_004d are reachable both inside and outside the pinned region starting at IL_003c. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)self == null)
		{
			ThrowHelper.ThrowArgumentNullException(self, "self");
		}
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(self);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(methodName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return IsInvoking_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return IsInvoking_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction]
	private static bool IsObjectMonoBehaviour([NotNull] Object obj)
	{
		if ((object)obj == null)
		{
			ThrowHelper.ThrowArgumentNullException(obj, "obj");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(obj);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(obj, "obj");
		}
		return IsObjectMonoBehaviour_Injected(intPtr);
	}

	[return: Unmarshalled]
	private unsafe Coroutine StartCoroutineManaged(string methodName, object value)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(methodName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return StartCoroutineManaged_Injected(intPtr, ref managedSpanWrapper, value);
				}
			}
			return StartCoroutineManaged_Injected(intPtr, ref managedSpanWrapper, value);
		}
		finally
		{
		}
	}

	[return: Unmarshalled]
	private Coroutine StartCoroutineManaged2(IEnumerator enumerator)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return StartCoroutineManaged2_Injected(intPtr, enumerator);
	}

	private void StopCoroutineManaged(Coroutine routine)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StopCoroutineManaged_Injected(intPtr, (routine == null) ? ((IntPtr)0) : Coroutine.BindingsMarshaller.ConvertToNative(routine));
	}

	private void StopCoroutineFromEnumeratorManaged(IEnumerator routine)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StopCoroutineFromEnumeratorManaged_Injected(intPtr, routine);
	}

	internal string GetScriptClassName()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetScriptClassName_Injected(intPtr, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	private void OnCancellationTokenCreated()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		OnCancellationTokenCreated_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopCoroutine_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopAllCoroutines_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useGUILayout_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useGUILayout_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_didStart_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_didAwake_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CancelInvokeAll_Injected(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_IsInvokingAll_Injected(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InvokeDelayed_Injected(IntPtr self, ref ManagedSpanWrapper methodName, float time, float repeatRate);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CancelInvoke_Injected(IntPtr self, ref ManagedSpanWrapper methodName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsInvoking_Injected(IntPtr self, ref ManagedSpanWrapper methodName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsObjectMonoBehaviour_Injected(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Coroutine StartCoroutineManaged_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, object value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Coroutine StartCoroutineManaged2_Injected(IntPtr _unity_self, IEnumerator enumerator);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopCoroutineManaged_Injected(IntPtr _unity_self, IntPtr routine);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopCoroutineFromEnumeratorManaged_Injected(IntPtr _unity_self, IEnumerator routine);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetScriptClassName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OnCancellationTokenCreated_Injected(IntPtr _unity_self);
}
