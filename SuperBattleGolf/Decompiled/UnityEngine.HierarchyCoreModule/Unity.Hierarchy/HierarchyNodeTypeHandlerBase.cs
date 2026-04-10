using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/HierarchyCore/Public/HierarchyNodeTypeHandlerBase.h")]
[RequiredByNativeCode]
[NativeHeader("Modules/HierarchyCore/HierarchyNodeTypeHandlerBaseBindings.h")]
public abstract class HierarchyNodeTypeHandlerBase : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToUnmanaged(HierarchyNodeTypeHandlerBase handler)
		{
			return handler.m_Ptr;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct ConstructorScope : IDisposable
	{
		[NoAutoStaticsCleanup]
		[ThreadStatic]
		private static IntPtr m_Ptr;

		[NoAutoStaticsCleanup]
		[ThreadStatic]
		private static Hierarchy m_Hierarchy;

		[NoAutoStaticsCleanup]
		[ThreadStatic]
		private static HierarchyCommandList m_CommandList;

		public static IntPtr Ptr
		{
			get
			{
				return m_Ptr;
			}
			private set
			{
				m_Ptr = value;
			}
		}

		public static Hierarchy Hierarchy
		{
			get
			{
				return m_Hierarchy;
			}
			private set
			{
				m_Hierarchy = value;
			}
		}

		public static HierarchyCommandList CommandList
		{
			get
			{
				return m_CommandList;
			}
			private set
			{
				m_CommandList = value;
			}
		}

		public ConstructorScope(IntPtr nativePtr, Hierarchy hierarchy, HierarchyCommandList cmdList)
		{
			Ptr = nativePtr;
			Hierarchy = hierarchy;
			CommandList = cmdList;
		}

		public void Dispose()
		{
			Ptr = IntPtr.Zero;
			Hierarchy = null;
			CommandList = null;
		}
	}

	internal readonly IntPtr m_Ptr;

	private readonly Hierarchy m_Hierarchy;

	private readonly HierarchyCommandList m_CommandList;

	[AutoStaticsCleanupOnCodeReload]
	private static readonly Dictionary<Type, int> s_NodeTypes = new Dictionary<Type, int>();

	public Hierarchy Hierarchy => m_Hierarchy;

	protected HierarchyCommandList CommandList => m_CommandList;

	protected HierarchyNodeTypeHandlerBase()
	{
		m_Ptr = ConstructorScope.Ptr;
		m_Hierarchy = ConstructorScope.Hierarchy;
		m_CommandList = ConstructorScope.CommandList;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	internal HierarchyNodeTypeHandlerBase(IntPtr nativePtr, Hierarchy hierarchy, HierarchyCommandList cmdList)
	{
		m_Ptr = nativePtr;
		m_Hierarchy = hierarchy;
		m_CommandList = cmdList;
	}

	~HierarchyNodeTypeHandlerBase()
	{
		Dispose(disposing: false);
	}

	protected virtual void Initialize()
	{
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	public HierarchyNodeType GetNodeType()
	{
		return new HierarchyNodeType(GetNodeTypeFromType(GetType()));
	}

	[NativeMethod(IsThreadSafe = true)]
	public virtual string GetNodeTypeName()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetNodeTypeName_Injected(intPtr, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public virtual int GetNodeHashCode(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNodeHashCode_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public virtual HierarchyNodeFlags GetDefaultNodeFlags(in HierarchyNode node, HierarchyNodeFlags defaultFlags = HierarchyNodeFlags.None)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDefaultNodeFlags_Injected(intPtr, in node, defaultFlags);
	}

	protected virtual void SearchBegin(HierarchySearchQueryDescriptor query)
	{
	}

	protected virtual bool SearchMatch(in HierarchyNode node)
	{
		return false;
	}

	protected virtual void SearchEnd()
	{
	}

	protected virtual void ViewModelPostUpdate(HierarchyViewModel viewModel)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	internal static HierarchyNodeTypeHandlerBase FromIntPtr(IntPtr handlePtr)
	{
		return (handlePtr != IntPtr.Zero) ? ((HierarchyNodeTypeHandlerBase)GCHandle.FromIntPtr(handlePtr).Target) : null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void Internal_SearchBegin(HierarchySearchQueryDescriptor query)
	{
		SearchBegin(query);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool Internal_SearchMatch(in HierarchyNode node)
	{
		return SearchMatch(in node);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("HierarchyNodeTypeHandlerManager::Get().GetNodeType", IsThreadSafe = true, ThrowsException = true)]
	private static extern int GetNodeTypeFromType(Type type);

	[RequiredByNativeCode]
	private static IntPtr CreateNodeTypeHandlerFromType(IntPtr nativePtr, Type handlerType, IntPtr hierarchyPtr, IntPtr cmdListPtr)
	{
		if (nativePtr == IntPtr.Zero)
		{
			throw new ArgumentNullException("nativePtr");
		}
		if (hierarchyPtr == IntPtr.Zero)
		{
			throw new ArgumentNullException("hierarchyPtr");
		}
		if (cmdListPtr == IntPtr.Zero)
		{
			throw new ArgumentNullException("cmdListPtr");
		}
		Hierarchy hierarchy = Hierarchy.FromIntPtr(hierarchyPtr);
		HierarchyCommandList cmdList = HierarchyCommandList.FromIntPtr(cmdListPtr);
		using (new ConstructorScope(nativePtr, hierarchy, cmdList))
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			HierarchyNodeTypeHandlerBase hierarchyNodeTypeHandlerBase = (HierarchyNodeTypeHandlerBase)Activator.CreateInstance(handlerType, bindingAttr, null, null, null);
			if (hierarchyNodeTypeHandlerBase == null)
			{
				return IntPtr.Zero;
			}
			return GCHandle.ToIntPtr(GCHandle.Alloc(hierarchyNodeTypeHandlerBase));
		}
	}

	[RequiredByNativeCode]
	private static bool TryGetStaticNodeType(Type handlerType, out int nodeType)
	{
		if (s_NodeTypes.TryGetValue(handlerType, out nodeType))
		{
			return true;
		}
		MethodInfo method = handlerType.GetMethod("GetStaticNodeType", BindingFlags.Static | BindingFlags.NonPublic);
		if (method != null)
		{
			nodeType = (int)method.Invoke(null, null);
			s_NodeTypes.Add(handlerType, nodeType);
			return true;
		}
		nodeType = 0;
		return false;
	}

	[RequiredByNativeCode]
	private static void InvokeInitialize(IntPtr handlePtr)
	{
		FromIntPtr(handlePtr).Initialize();
	}

	[RequiredByNativeCode]
	private static void InvokeDispose(IntPtr handlePtr)
	{
		HierarchyNodeTypeHandlerBase hierarchyNodeTypeHandlerBase = FromIntPtr(handlePtr);
		hierarchyNodeTypeHandlerBase.Dispose(disposing: true);
		GC.SuppressFinalize(hierarchyNodeTypeHandlerBase);
	}

	[RequiredByNativeCode]
	private static string InvokeGetNodeTypeName(IntPtr handlePtr)
	{
		return FromIntPtr(handlePtr).GetNodeTypeName();
	}

	[RequiredByNativeCode]
	private static int InvokeGetNodeHashCode(IntPtr handlePtr, in HierarchyNode node)
	{
		return FromIntPtr(handlePtr).GetNodeHashCode(in node);
	}

	[RequiredByNativeCode]
	private static HierarchyNodeFlags InvokeGetDefaultNodeFlags(IntPtr handlePtr, in HierarchyNode node, HierarchyNodeFlags defaultFlags)
	{
		return FromIntPtr(handlePtr).GetDefaultNodeFlags(in node, defaultFlags);
	}

	[RequiredByNativeCode]
	private static bool InvokeChangesPending(IntPtr handlePtr)
	{
		return FromIntPtr(handlePtr).ChangesPending();
	}

	[RequiredByNativeCode]
	private static bool InvokeIntegrateChanges(IntPtr handlePtr, IntPtr cmdListPtr)
	{
		return FromIntPtr(handlePtr).IntegrateChanges(HierarchyCommandList.FromIntPtr(cmdListPtr));
	}

	[RequiredByNativeCode]
	private static bool InvokeSearchMatch(IntPtr handlePtr, in HierarchyNode node)
	{
		return FromIntPtr(handlePtr).SearchMatch(in node);
	}

	[RequiredByNativeCode]
	private static void InvokeSearchEnd(IntPtr handlePtr)
	{
		FromIntPtr(handlePtr).SearchEnd();
	}

	[RequiredByNativeCode]
	private static void InvokeViewModelPostUpdate(IntPtr handlePtr, IntPtr viewModelPtr)
	{
		FromIntPtr(handlePtr).ViewModelPostUpdate(HierarchyViewModel.FromIntPtr(viewModelPtr));
	}

	[Obsolete("The constructor with a hierarchy parameter is obsolete and is no longer used. Remove the hierarchy parameter from your constructor.")]
	protected HierarchyNodeTypeHandlerBase(Hierarchy hierarchy)
		: this()
	{
	}

	[Obsolete("The IDisposable interface is obsolete and no longer has any effect. Instances of handlers are owned and disposed by the hierarchy so they do not need to be disposed by user code.")]
	public void Dispose()
	{
	}

	[FreeFunction("HierarchyNodeTypeHandlerBaseBindings::ChangesPending", HasExplicitThis = true, IsThreadSafe = true)]
	[Obsolete("ChangesPending is obsolete, it is replaced by adding commands into the hierarchy node type handler's CommandList.", false)]
	protected virtual bool ChangesPending()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ChangesPending_Injected(intPtr);
	}

	[Obsolete("IntegrateChanges is obsolete, it is replaced by adding commands into the hierarchy node type handler's CommandList.", false)]
	[FreeFunction("HierarchyNodeTypeHandlerBaseBindings::IntegrateChanges", HasExplicitThis = true, IsThreadSafe = true)]
	protected virtual bool IntegrateChanges(HierarchyCommandList cmdList)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IntegrateChanges_Injected(intPtr, (cmdList == null) ? ((IntPtr)0) : HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(cmdList));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetNodeTypeName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetNodeHashCode_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern HierarchyNodeFlags GetDefaultNodeFlags_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags defaultFlags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ChangesPending_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IntegrateChanges_Injected(IntPtr _unity_self, IntPtr cmdList);
}
