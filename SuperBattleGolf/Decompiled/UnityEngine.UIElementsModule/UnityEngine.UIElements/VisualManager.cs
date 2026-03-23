using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using AOT;
using JetBrains.Annotations;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements;

[NativeType(Header = "Modules/UIElements/VisualManager.h")]
internal sealed class VisualManager : IDisposable
{
	[UsedImplicitly]
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(VisualManager store)
		{
			return store.m_Ptr;
		}

		public static VisualManager ConvertToManaged(IntPtr ptr)
		{
			return new VisualManager(ptr, isWrapper: true);
		}
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void NativeHierarchyChangedDelegate(IntPtr instance, in VisualNodeHandle handle, HierarchyChangeType type);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void NativeVersionChangedDelegate(IntPtr instance, in VisualNodeHandle handle, VersionChangeType type);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void NativeVisualNodeDelegate(IntPtr instance, in VisualNodeHandle handle);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void NativeVisualNodeChildDelegate(IntPtr instance, in VisualNodeHandle handle, in VisualNodeHandle child);

	private static bool s_Initialized;

	private static bool s_AppDomainUnloadRegistered;

	private static readonly List<WeakReference<VisualManager>> s_CallbackInstances;

	private static readonly NativeHierarchyChangedDelegate s_HierarchyChanged;

	private static readonly NativeVersionChangedDelegate s_VersionChanged;

	private static readonly NativeVisualNodeDelegate s_Blur;

	private static readonly NativeVisualNodeChildDelegate s_ChildAdded;

	private static readonly NativeVisualNodeChildDelegate s_ChildRemoved;

	private static readonly IntPtr s_HierarchyChangedPtr;

	private static readonly IntPtr s_VersionChangedPtr;

	private static readonly IntPtr s_BlurPtr;

	private static readonly IntPtr s_ChildAddedPtr;

	private static readonly IntPtr s_ChildRemovedPtr;

	[RequiredByNativeCode]
	private IntPtr m_Ptr;

	[RequiredByNativeCode]
	private bool m_IsWrapper;

	private readonly int m_InstanceId;

	private readonly VisualNodePropertyRegistry m_Registry;

	private readonly ChunkAllocatingArray<WeakReference<VisualElement>> m_Elements = new ChunkAllocatingArray<WeakReference<VisualElement>>();

	private readonly ChunkAllocatingArray<WeakReference<BaseVisualElementPanel>> m_Panels = new ChunkAllocatingArray<WeakReference<BaseVisualElementPanel>>();

	private readonly object m_NodeLock = new object();

	private readonly Stack<VisualNodeHandle> m_NodesToRemove = new Stack<VisualNodeHandle>();

	private readonly object m_PanelLock = new object();

	private readonly Stack<VisualPanelHandle> m_PanelsToRemove = new Stack<VisualPanelHandle>();

	public static VisualManager SharedManager { get; private set; }

	public bool IsCreated => m_Ptr != IntPtr.Zero;

	internal VisualNodeClassNameStore ClassNameStore { get; }

	[NativeProperty("Root", TargetType.Field)]
	public VisualNodeHandle Root
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_Root_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public event HierarchyChangedDelegate OnHierarchyChanged;

	public event VersionChangedDelegate OnVersionChanged;

	public event VisualNodeDelegate OnBlur;

	public event VisualNodeChildDelegate OnChildAdded;

	public event VisualNodeChildDelegate OnChildRemoved;

	static VisualManager()
	{
		s_CallbackInstances = new List<WeakReference<VisualManager>>();
		s_HierarchyChanged = InvokeHierarchyChanged;
		s_VersionChanged = InvokeVersionChanged;
		s_Blur = InvokeBlur;
		s_ChildAdded = InvokeChildAdded;
		s_ChildRemoved = InvokeChildRemoved;
		s_HierarchyChangedPtr = Marshal.GetFunctionPointerForDelegate(s_HierarchyChanged);
		s_VersionChangedPtr = Marshal.GetFunctionPointerForDelegate(s_VersionChanged);
		s_BlurPtr = Marshal.GetFunctionPointerForDelegate(s_Blur);
		s_ChildAddedPtr = Marshal.GetFunctionPointerForDelegate(s_ChildAdded);
		s_ChildRemovedPtr = Marshal.GetFunctionPointerForDelegate(s_ChildRemoved);
		Initialize();
	}

	private static void Initialize()
	{
		if (s_Initialized)
		{
			return;
		}
		VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeData>();
		VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodePseudoStateData>();
		VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeClassData>();
		VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeRenderData>();
		VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeTextData>();
		VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeImguiData>();
		s_Initialized = true;
		if (!s_AppDomainUnloadRegistered)
		{
			AppDomain.CurrentDomain.DomainUnload += delegate
			{
				if (s_Initialized)
				{
					Shutdown();
				}
			};
			s_AppDomainUnloadRegistered = true;
		}
		SharedManager = new VisualManager();
	}

	private static void Shutdown()
	{
		if (s_Initialized)
		{
			s_Initialized = false;
			SharedManager.Dispose();
		}
	}

	private int RegisterCallbackInstance(VisualManager instance)
	{
		for (int i = 0; i < s_CallbackInstances.Count; i++)
		{
			if (!s_CallbackInstances[i].TryGetTarget(out var _))
			{
				s_CallbackInstances[i] = new WeakReference<VisualManager>(instance);
				return i + 1;
			}
		}
		s_CallbackInstances.Add(new WeakReference<VisualManager>(this));
		return s_CallbackInstances.Count;
	}

	private void UnregisterCallbackInstance(int id)
	{
		s_CallbackInstances[id - 1] = null;
	}

	[MonoPInvokeCallback(typeof(NativeHierarchyChangedDelegate))]
	private static void InvokeHierarchyChanged(IntPtr instance, in VisualNodeHandle handle, HierarchyChangeType type)
	{
		for (int i = 0; i < s_CallbackInstances.Count; i++)
		{
			if (s_CallbackInstances[i].TryGetTarget(out var target) && target.m_Ptr == instance)
			{
				target.OnHierarchyChanged(target, in handle, type);
			}
		}
	}

	[MonoPInvokeCallback(typeof(NativeVersionChangedDelegate))]
	private static void InvokeVersionChanged(IntPtr instance, in VisualNodeHandle handle, VersionChangeType type)
	{
		for (int i = 0; i < s_CallbackInstances.Count; i++)
		{
			if (s_CallbackInstances[i].TryGetTarget(out var target) && target.m_Ptr == instance)
			{
				target.OnVersionChanged?.Invoke(target, in handle, type);
			}
		}
	}

	[MonoPInvokeCallback(typeof(NativeVisualNodeDelegate))]
	private static void InvokeBlur(IntPtr instance, in VisualNodeHandle handle)
	{
		for (int i = 0; i < s_CallbackInstances.Count; i++)
		{
			if (s_CallbackInstances[i].TryGetTarget(out var target) && target.m_Ptr == instance)
			{
				target.OnBlur?.Invoke(target, in handle);
			}
		}
	}

	[MonoPInvokeCallback(typeof(NativeVisualNodeChildDelegate))]
	private static void InvokeChildAdded(IntPtr instance, in VisualNodeHandle handle, in VisualNodeHandle child)
	{
		for (int i = 0; i < s_CallbackInstances.Count; i++)
		{
			if (s_CallbackInstances[i].TryGetTarget(out var target) && target.m_Ptr == instance)
			{
				target.OnChildAdded?.Invoke(target, in handle, in child);
			}
		}
	}

	[MonoPInvokeCallback(typeof(NativeVisualNodeChildDelegate))]
	private static void InvokeChildRemoved(IntPtr instance, in VisualNodeHandle handle, in VisualNodeHandle child)
	{
		for (int i = 0; i < s_CallbackInstances.Count; i++)
		{
			if (s_CallbackInstances[i].TryGetTarget(out var target) && target.m_Ptr == instance)
			{
				target.OnChildRemoved?.Invoke(target, in handle, in child);
			}
		}
	}

	public VisualManager()
		: this(Internal_Create(), isWrapper: false)
	{
	}

	private VisualManager(IntPtr ptr, bool isWrapper)
	{
		m_InstanceId = RegisterCallbackInstance(this);
		m_Ptr = ptr;
		m_IsWrapper = isWrapper;
		ClassNameStore = GetClassNameStore();
		SetHierarchyChangedCallback(s_HierarchyChangedPtr);
		SetVersionChangedCallback(s_VersionChangedPtr);
		SetBlurCallback(s_BlurPtr);
		SetChildAddedCallback(s_ChildAddedPtr);
		SetChildRemovedCallback(s_ChildRemovedPtr);
		m_Registry = new VisualNodePropertyRegistry(this);
	}

	~VisualManager()
	{
		UnregisterCallbackInstance(m_InstanceId);
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (m_Ptr != IntPtr.Zero)
		{
			if (!m_IsWrapper)
			{
				Internal_Destroy(m_Ptr);
			}
			m_Ptr = IntPtr.Zero;
		}
	}

	public VisualPanel CreatePanel()
	{
		TryFreePanels();
		VisualPanelHandle handle = AddPanel();
		return new VisualPanel(this, handle);
	}

	public void DestroyPanelThreaded(ref VisualPanel panel)
	{
		if (!(panel.Handle == VisualPanelHandle.Null))
		{
			lock (m_PanelLock)
			{
				m_PanelsToRemove.Push(panel.Handle);
			}
			panel = VisualPanel.Null;
		}
	}

	public VisualNode CreateNode()
	{
		TryFreeNodes();
		VisualNodeHandle handle = AddNode();
		return new VisualNode(this, handle);
	}

	public void DestroyNodeThreaded(ref VisualNode node)
	{
		if (!(node.Handle == VisualNodeHandle.Null))
		{
			lock (m_NodeLock)
			{
				m_NodesToRemove.Push(node.Handle);
			}
			node = VisualNode.Null;
		}
	}

	private void TryFreePanels()
	{
		bool lockTaken = false;
		try
		{
			Monitor.TryEnter(m_PanelLock, ref lockTaken);
			if (lockTaken)
			{
				while (m_PanelsToRemove.Count > 0)
				{
					RemovePanel(m_PanelsToRemove.Pop());
				}
			}
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(m_PanelLock);
			}
		}
	}

	private void TryFreeNodes()
	{
		bool lockTaken = false;
		try
		{
			Monitor.TryEnter(m_NodeLock, ref lockTaken);
			if (lockTaken)
			{
				while (m_NodesToRemove.Count > 0)
				{
					RemoveNode(m_NodesToRemove.Pop());
				}
			}
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(m_NodeLock);
			}
		}
	}

	public void SetOwner(in VisualNodeHandle handle, VisualElement element)
	{
		m_Elements[handle.Id] = ((element != null) ? new WeakReference<VisualElement>(element) : null);
	}

	public VisualElement GetOwner(in VisualNodeHandle handle)
	{
		m_Elements[handle.Id].TryGetTarget(out var target);
		return target;
	}

	public void SetOwner(in VisualPanelHandle handle, BaseVisualElementPanel panel)
	{
		m_Panels[handle.Id] = ((panel != null) ? new WeakReference<BaseVisualElementPanel>(panel) : null);
	}

	public BaseVisualElementPanel GetOwner(in VisualPanelHandle handle)
	{
		m_Panels[handle.Id].TryGetTarget(out var target);
		return target;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ref T GetProperty<T>(VisualNodeHandle handle) where T : unmanaged
	{
		return ref m_Registry.GetPropertyRef<T>(handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("VisualManager::Create")]
	private static extern IntPtr Internal_Create();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("VisualManager::Destroy")]
	private static extern void Internal_Destroy(IntPtr ptr);

	[NativeThrows]
	private void SetHierarchyChangedCallback(IntPtr callback)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetHierarchyChangedCallback_Injected(intPtr, callback);
	}

	[NativeThrows]
	private void SetVersionChangedCallback(IntPtr callback)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetVersionChangedCallback_Injected(intPtr, callback);
	}

	[NativeThrows]
	private void SetBlurCallback(IntPtr callback)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBlurCallback_Injected(intPtr, callback);
	}

	[NativeThrows]
	private void SetChildAddedCallback(IntPtr callback)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetChildAddedCallback_Injected(intPtr, callback);
	}

	[NativeThrows]
	private void SetChildRemovedCallback(IntPtr callback)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetChildRemovedCallback_Injected(intPtr, callback);
	}

	[NativeThrows]
	internal IntPtr GetPropertyPtr(int index)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPropertyPtr_Injected(intPtr, index);
	}

	internal int PanelCount()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return PanelCount_Injected(intPtr);
	}

	[NativeThrows]
	internal VisualPanelHandle AddPanel()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddPanel_Injected(intPtr, out var ret);
		return ret;
	}

	[NativeThrows]
	internal bool RemovePanel(in VisualPanelHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemovePanel_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal bool ContainsPanel(in VisualPanelHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ContainsPanel_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal void ClearPanels()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearPanels_Injected(intPtr);
	}

	[NativeThrows]
	internal unsafe void* GetPanelDataPtr(in VisualPanelHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPanelDataPtr_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal VisualNodeHandle GetRootContainer(in VisualPanelHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetRootContainer_Injected(intPtr, in handle, out var ret);
		return ret;
	}

	[NativeThrows]
	internal bool SetRootContainer(in VisualPanelHandle handle, in VisualNodeHandle container)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetRootContainer_Injected(intPtr, in handle, in container);
	}

	internal int NodeCount()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return NodeCount_Injected(intPtr);
	}

	[NativeThrows]
	internal VisualNodeHandle AddNode()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddNode_Injected(intPtr, out var ret);
		return ret;
	}

	[NativeThrows]
	internal bool RemoveNode(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveNode_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal bool ContainsNode(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ContainsNode_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal void ClearNodes()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearNodes_Injected(intPtr);
	}

	[NativeThrows]
	internal unsafe void SetName(in VisualNodeHandle handle, string name)
	{
		//The blocks IL_003a are reachable both inside and outside the pinned region starting at IL_0029. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetName_Injected(intPtr, in handle, ref managedSpanWrapper);
					return;
				}
			}
			SetName_Injected(intPtr, in handle, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeThrows]
	internal string GetName(in VisualNodeHandle handle)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetName_Injected(intPtr, in handle, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[NativeThrows]
	internal int GetChildrenCount(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetChildrenCount_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal IntPtr GetChildrenPtr(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetChildrenPtr_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal int IndexOfChild(in VisualNodeHandle handle, in VisualNodeHandle child)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IndexOfChild_Injected(intPtr, in handle, in child);
	}

	[NativeThrows]
	internal bool AddChild(in VisualNodeHandle handle, in VisualNodeHandle child)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddChild_Injected(intPtr, in handle, in child);
	}

	[NativeThrows]
	internal bool RemoveChild(in VisualNodeHandle handle, in VisualNodeHandle child)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveChild_Injected(intPtr, in handle, in child);
	}

	[NativeThrows]
	internal bool InsertChildAtIndex(in VisualNodeHandle handle, int index, in VisualNodeHandle child)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return InsertChildAtIndex_Injected(intPtr, in handle, index, in child);
	}

	[NativeThrows]
	internal bool RemoveChildAtIndex(in VisualNodeHandle handle, int index)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveChildAtIndex_Injected(intPtr, in handle, index);
	}

	[NativeThrows]
	internal bool ClearChildren(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ClearChildren_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal VisualNodeHandle GetParent(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetParent_Injected(intPtr, in handle, out var ret);
		return ret;
	}

	[NativeThrows]
	internal bool RemoveFromParent(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveFromParent_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal unsafe bool AddToClassList(in VisualNodeHandle handle, string className)
	{
		//The blocks IL_003a are reachable both inside and outside the pinned region starting at IL_0029. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(className);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return AddToClassList_Injected(intPtr, in handle, ref managedSpanWrapper);
				}
			}
			return AddToClassList_Injected(intPtr, in handle, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeThrows]
	internal unsafe bool RemoveFromClassList(in VisualNodeHandle handle, string className)
	{
		//The blocks IL_003a are reachable both inside and outside the pinned region starting at IL_0029. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(className);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return RemoveFromClassList_Injected(intPtr, in handle, ref managedSpanWrapper);
				}
			}
			return RemoveFromClassList_Injected(intPtr, in handle, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeThrows]
	internal unsafe bool ClassListContains(in VisualNodeHandle handle, string className)
	{
		//The blocks IL_003a are reachable both inside and outside the pinned region starting at IL_0029. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(className);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return ClassListContains_Injected(intPtr, in handle, ref managedSpanWrapper);
				}
			}
			return ClassListContains_Injected(intPtr, in handle, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeThrows]
	internal bool ClearClassList(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ClearClassList_Injected(intPtr, in handle);
	}

	[NativeThrows]
	private VisualNodeClassNameStore GetClassNameStore()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr classNameStore_Injected = GetClassNameStore_Injected(intPtr);
		return (classNameStore_Injected == (IntPtr)0) ? null : VisualNodeClassNameStore.BindingsMarshaller.ConvertToManaged(classNameStore_Injected);
	}

	[NativeThrows]
	internal bool IsEnabled(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsEnabled_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal void SetEnabled(in VisualNodeHandle handle, bool enabled)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetEnabled_Injected(intPtr, in handle, enabled);
	}

	[NativeThrows]
	internal bool IsEnabledInHierarchy(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsEnabledInHierarchy_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal VisualPanelHandle GetPanel(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPanel_Injected(intPtr, in handle, out var ret);
		return ret;
	}

	[NativeThrows]
	internal void SetPanel(in VisualNodeHandle handle, in VisualPanelHandle panel)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPanel_Injected(intPtr, in handle, in panel);
	}

	[NativeThrows]
	[return: Unmarshalled]
	internal PseudoStates GetPseudoStates(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPseudoStates_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal void SetPseudoStates(in VisualNodeHandle handle, PseudoStates states)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPseudoStates_Injected(intPtr, in handle, states);
	}

	[NativeThrows]
	[return: Unmarshalled]
	internal RenderHints GetRenderHints(in VisualNodeHandle handles)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetRenderHints_Injected(intPtr, in handles);
	}

	[NativeThrows]
	internal void SetRenderHints(in VisualNodeHandle handle, RenderHints hints)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetRenderHints_Injected(intPtr, in handle, hints);
	}

	[NativeThrows]
	[return: Unmarshalled]
	internal LanguageDirection GetLanguageDirection(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetLanguageDirection_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal void SetLanguageDirection(in VisualNodeHandle handle, LanguageDirection direction)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLanguageDirection_Injected(intPtr, in handle, direction);
	}

	[NativeThrows]
	[return: Unmarshalled]
	internal LanguageDirection GetLocalLanguageDirection(in VisualNodeHandle handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetLocalLanguageDirection_Injected(intPtr, in handle);
	}

	[NativeThrows]
	internal void SetLocalLanguageDirection(in VisualNodeHandle handle, LanguageDirection direction)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLocalLanguageDirection_Injected(intPtr, in handle, direction);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_Root_Injected(IntPtr _unity_self, out VisualNodeHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetHierarchyChangedCallback_Injected(IntPtr _unity_self, IntPtr callback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVersionChangedCallback_Injected(IntPtr _unity_self, IntPtr callback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBlurCallback_Injected(IntPtr _unity_self, IntPtr callback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetChildAddedCallback_Injected(IntPtr _unity_self, IntPtr callback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetChildRemovedCallback_Injected(IntPtr _unity_self, IntPtr callback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetPropertyPtr_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PanelCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddPanel_Injected(IntPtr _unity_self, out VisualPanelHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemovePanel_Injected(IntPtr _unity_self, in VisualPanelHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ContainsPanel_Injected(IntPtr _unity_self, in VisualPanelHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearPanels_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* GetPanelDataPtr_Injected(IntPtr _unity_self, in VisualPanelHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRootContainer_Injected(IntPtr _unity_self, in VisualPanelHandle handle, out VisualNodeHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetRootContainer_Injected(IntPtr _unity_self, in VisualPanelHandle handle, in VisualNodeHandle container);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int NodeCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddNode_Injected(IntPtr _unity_self, out VisualNodeHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveNode_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ContainsNode_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearNodes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetName_Injected(IntPtr _unity_self, in VisualNodeHandle handle, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetName_Injected(IntPtr _unity_self, in VisualNodeHandle handle, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetChildrenCount_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetChildrenPtr_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int IndexOfChild_Injected(IntPtr _unity_self, in VisualNodeHandle handle, in VisualNodeHandle child);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool AddChild_Injected(IntPtr _unity_self, in VisualNodeHandle handle, in VisualNodeHandle child);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveChild_Injected(IntPtr _unity_self, in VisualNodeHandle handle, in VisualNodeHandle child);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InsertChildAtIndex_Injected(IntPtr _unity_self, in VisualNodeHandle handle, int index, in VisualNodeHandle child);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveChildAtIndex_Injected(IntPtr _unity_self, in VisualNodeHandle handle, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ClearChildren_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetParent_Injected(IntPtr _unity_self, in VisualNodeHandle handle, out VisualNodeHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveFromParent_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool AddToClassList_Injected(IntPtr _unity_self, in VisualNodeHandle handle, ref ManagedSpanWrapper className);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveFromClassList_Injected(IntPtr _unity_self, in VisualNodeHandle handle, ref ManagedSpanWrapper className);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ClassListContains_Injected(IntPtr _unity_self, in VisualNodeHandle handle, ref ManagedSpanWrapper className);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ClearClassList_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetClassNameStore_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsEnabled_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetEnabled_Injected(IntPtr _unity_self, in VisualNodeHandle handle, bool enabled);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsEnabledInHierarchy_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPanel_Injected(IntPtr _unity_self, in VisualNodeHandle handle, out VisualPanelHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPanel_Injected(IntPtr _unity_self, in VisualNodeHandle handle, in VisualPanelHandle panel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PseudoStates GetPseudoStates_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPseudoStates_Injected(IntPtr _unity_self, in VisualNodeHandle handle, PseudoStates states);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RenderHints GetRenderHints_Injected(IntPtr _unity_self, in VisualNodeHandle handles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRenderHints_Injected(IntPtr _unity_self, in VisualNodeHandle handle, RenderHints hints);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LanguageDirection GetLanguageDirection_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLanguageDirection_Injected(IntPtr _unity_self, in VisualNodeHandle handle, LanguageDirection direction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LanguageDirection GetLocalLanguageDirection_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLocalLanguageDirection_Injected(IntPtr _unity_self, in VisualNodeHandle handle, LanguageDirection direction);
}
