using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

public sealed class ShaderVariantCollection : Object
{
	public struct ShaderVariant
	{
		public Shader shader;

		public PassType passType;

		public string[] keywords;

		[FreeFunction]
		[NativeConditional("UNITY_EDITOR")]
		private static string CheckShaderVariant(Shader shader, PassType passType, string[] keywords)
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				CheckShaderVariant_Injected(MarshalledUnityObject.Marshal(shader), passType, keywords, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}

		public ShaderVariant(Shader shader, PassType passType, params string[] keywords)
		{
			this.shader = shader;
			this.passType = passType;
			this.keywords = keywords;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CheckShaderVariant_Injected(IntPtr shader, PassType passType, string[] keywords, out ManagedSpanWrapper ret);
	}

	public int shaderCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shaderCount_Injected(intPtr);
		}
	}

	public int variantCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_variantCount_Injected(intPtr);
		}
	}

	public int warmedUpVariantCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_warmedUpVariantCount_Injected(intPtr);
		}
	}

	public bool isWarmedUp
	{
		[NativeName("IsWarmedUp")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isWarmedUp_Injected(intPtr);
		}
	}

	private bool AddVariant(Shader shader, PassType passType, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] string[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddVariant_Injected(intPtr, MarshalledUnityObject.Marshal(shader), passType, keywords);
	}

	private bool RemoveVariant(Shader shader, PassType passType, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] string[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveVariant_Injected(intPtr, MarshalledUnityObject.Marshal(shader), passType, keywords);
	}

	private bool ContainsVariant(Shader shader, PassType passType, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] string[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ContainsVariant_Injected(intPtr, MarshalledUnityObject.Marshal(shader), passType, keywords);
	}

	[NativeName("ClearVariants")]
	public void Clear()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Clear_Injected(intPtr);
	}

	[NativeName("WarmupShaders")]
	public void WarmUp()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WarmUp_Injected(intPtr);
	}

	[NativeName("WarmupShadersProgressively")]
	public bool WarmUpProgressively(int variantCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return WarmUpProgressively_Injected(intPtr, variantCount);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("CreateFromScript")]
	private static extern void Internal_Create([Writable] ShaderVariantCollection svc);

	public ShaderVariantCollection()
	{
		Internal_Create(this);
	}

	public bool Add(ShaderVariant variant)
	{
		return AddVariant(variant.shader, variant.passType, variant.keywords);
	}

	public bool Remove(ShaderVariant variant)
	{
		return RemoveVariant(variant.shader, variant.passType, variant.keywords);
	}

	public bool Contains(ShaderVariant variant)
	{
		return ContainsVariant(variant.shader, variant.passType, variant.keywords);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_shaderCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_variantCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_warmedUpVariantCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isWarmedUp_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool AddVariant_Injected(IntPtr _unity_self, IntPtr shader, PassType passType, string[] keywords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveVariant_Injected(IntPtr _unity_self, IntPtr shader, PassType passType, string[] keywords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ContainsVariant_Injected(IntPtr _unity_self, IntPtr shader, PassType passType, string[] keywords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Clear_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WarmUp_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool WarmUpProgressively_Injected(IntPtr _unity_self, int variantCount);
}
