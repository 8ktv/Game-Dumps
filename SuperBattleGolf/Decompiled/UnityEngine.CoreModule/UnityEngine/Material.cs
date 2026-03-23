using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
[NativeHeader("Runtime/Shaders/Material.h")]
public class Material : Object
{
	private static readonly int k_ColorId = Shader.PropertyToID("_Color");

	private static readonly int k_MainTexId = Shader.PropertyToID("_MainTex");

	public Shader shader
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Shader>(get_shader_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shader_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Color color
	{
		get
		{
			int firstPropertyNameIdByAttribute = GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainColor);
			if (firstPropertyNameIdByAttribute >= 0)
			{
				return GetColor(firstPropertyNameIdByAttribute);
			}
			return GetColor(k_ColorId);
		}
		set
		{
			int firstPropertyNameIdByAttribute = GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainColor);
			if (firstPropertyNameIdByAttribute >= 0)
			{
				SetColor(firstPropertyNameIdByAttribute, value);
			}
			else
			{
				SetColor(k_ColorId, value);
			}
		}
	}

	public Texture mainTexture
	{
		get
		{
			int firstPropertyNameIdByAttribute = GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
			if (firstPropertyNameIdByAttribute >= 0)
			{
				return GetTexture(firstPropertyNameIdByAttribute);
			}
			return GetTexture(k_MainTexId);
		}
		set
		{
			int firstPropertyNameIdByAttribute = GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
			if (firstPropertyNameIdByAttribute >= 0)
			{
				SetTexture(firstPropertyNameIdByAttribute, value);
			}
			else
			{
				SetTexture(k_MainTexId, value);
			}
		}
	}

	public Vector2 mainTextureOffset
	{
		get
		{
			int firstPropertyNameIdByAttribute = GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
			if (firstPropertyNameIdByAttribute >= 0)
			{
				return GetTextureOffset(firstPropertyNameIdByAttribute);
			}
			return GetTextureOffset(k_MainTexId);
		}
		set
		{
			int firstPropertyNameIdByAttribute = GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
			if (firstPropertyNameIdByAttribute >= 0)
			{
				SetTextureOffset(firstPropertyNameIdByAttribute, value);
			}
			else
			{
				SetTextureOffset(k_MainTexId, value);
			}
		}
	}

	public Vector2 mainTextureScale
	{
		get
		{
			int firstPropertyNameIdByAttribute = GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
			if (firstPropertyNameIdByAttribute >= 0)
			{
				return GetTextureScale(firstPropertyNameIdByAttribute);
			}
			return GetTextureScale(k_MainTexId);
		}
		set
		{
			int firstPropertyNameIdByAttribute = GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
			if (firstPropertyNameIdByAttribute >= 0)
			{
				SetTextureScale(firstPropertyNameIdByAttribute, value);
			}
			else
			{
				SetTextureScale(k_MainTexId, value);
			}
		}
	}

	public int renderQueue
	{
		[NativeName("GetActualRenderQueue")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_renderQueue_Injected(intPtr);
		}
		[NativeName("SetCustomRenderQueue")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_renderQueue_Injected(intPtr, value);
		}
	}

	public int rawRenderQueue
	{
		[NativeName("GetCustomRenderQueue")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rawRenderQueue_Injected(intPtr);
		}
	}

	public LocalKeyword[] enabledKeywords
	{
		get
		{
			return GetEnabledKeywords();
		}
		set
		{
			SetEnabledKeywords(value);
		}
	}

	public MaterialGlobalIlluminationFlags globalIlluminationFlags
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_globalIlluminationFlags_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_globalIlluminationFlags_Injected(intPtr, value);
		}
	}

	public bool doubleSidedGI
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_doubleSidedGI_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_doubleSidedGI_Injected(intPtr, value);
		}
	}

	[NativeProperty("EnableInstancingVariants")]
	public bool enableInstancing
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enableInstancing_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enableInstancing_Injected(intPtr, value);
		}
	}

	public int passCount
	{
		[NativeName("GetShader()->GetPassCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_passCount_Injected(intPtr);
		}
	}

	public string[] shaderKeywords
	{
		get
		{
			return GetShaderKeywords();
		}
		set
		{
			SetShaderKeywords(value);
		}
	}

	[Obsolete("Creating materials from shader source string will be removed in the future. Use Shader assets instead.", true)]
	public static Material Create(string scriptContents)
	{
		return new Material(scriptContents);
	}

	[FreeFunction("MaterialScripting::CreateWithShader")]
	private static void CreateWithShader([Writable] Material self, [NotNull] Shader shader)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		CreateWithShader_Injected(self, intPtr);
	}

	[FreeFunction("MaterialScripting::CreateWithMaterial")]
	private static void CreateWithMaterial([Writable] Material self, [NotNull] Material source)
	{
		if ((object)source == null)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(source);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(source, "source");
		}
		CreateWithMaterial_Injected(self, intPtr);
	}

	public Material(Shader shader)
	{
		CreateWithShader(this, shader);
	}

	[RequiredByNativeCode]
	public Material(Material source)
	{
		CreateWithMaterial(this, source);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Creating materials from shader source string is no longer supported. Use Shader assets instead.", true)]
	public Material(string contents)
	{
	}

	internal static Material GetDefaultMaterial()
	{
		return Unmarshal.UnmarshalUnityObject<Material>(GetDefaultMaterial_Injected());
	}

	internal static Material GetDefaultParticleMaterial()
	{
		return Unmarshal.UnmarshalUnityObject<Material>(GetDefaultParticleMaterial_Injected());
	}

	internal static Material GetDefaultLineMaterial()
	{
		return Unmarshal.UnmarshalUnityObject<Material>(GetDefaultLineMaterial_Injected());
	}

	[NativeName("GetFirstPropertyNameIdByAttributeFromScript")]
	private int GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags attributeFlag)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetFirstPropertyNameIdByAttribute_Injected(intPtr, attributeFlag);
	}

	[NativeName("HasPropertyFromScript")]
	public bool HasProperty(int nameID)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasProperty_Injected(intPtr, nameID);
	}

	public bool HasProperty(string name)
	{
		return HasProperty(Shader.PropertyToID(name));
	}

	[NativeName("HasFloatFromScript")]
	private bool HasFloatImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasFloatImpl_Injected(intPtr, name);
	}

	public bool HasFloat(string name)
	{
		return HasFloatImpl(Shader.PropertyToID(name));
	}

	public bool HasFloat(int nameID)
	{
		return HasFloatImpl(nameID);
	}

	public bool HasInt(string name)
	{
		return HasFloatImpl(Shader.PropertyToID(name));
	}

	public bool HasInt(int nameID)
	{
		return HasFloatImpl(nameID);
	}

	[NativeName("HasIntegerFromScript")]
	private bool HasIntImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasIntImpl_Injected(intPtr, name);
	}

	public bool HasInteger(string name)
	{
		return HasIntImpl(Shader.PropertyToID(name));
	}

	public bool HasInteger(int nameID)
	{
		return HasIntImpl(nameID);
	}

	[NativeName("HasTextureFromScript")]
	private bool HasTextureImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasTextureImpl_Injected(intPtr, name);
	}

	public bool HasTexture(string name)
	{
		return HasTextureImpl(Shader.PropertyToID(name));
	}

	public bool HasTexture(int nameID)
	{
		return HasTextureImpl(nameID);
	}

	[NativeName("HasMatrixFromScript")]
	private bool HasMatrixImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasMatrixImpl_Injected(intPtr, name);
	}

	public bool HasMatrix(string name)
	{
		return HasMatrixImpl(Shader.PropertyToID(name));
	}

	public bool HasMatrix(int nameID)
	{
		return HasMatrixImpl(nameID);
	}

	[NativeName("HasVectorFromScript")]
	private bool HasVectorImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasVectorImpl_Injected(intPtr, name);
	}

	public bool HasVector(string name)
	{
		return HasVectorImpl(Shader.PropertyToID(name));
	}

	public bool HasVector(int nameID)
	{
		return HasVectorImpl(nameID);
	}

	public bool HasColor(string name)
	{
		return HasVectorImpl(Shader.PropertyToID(name));
	}

	public bool HasColor(int nameID)
	{
		return HasVectorImpl(nameID);
	}

	[NativeName("HasBufferFromScript")]
	private bool HasBufferImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasBufferImpl_Injected(intPtr, name);
	}

	public bool HasBuffer(string name)
	{
		return HasBufferImpl(Shader.PropertyToID(name));
	}

	public bool HasBuffer(int nameID)
	{
		return HasBufferImpl(nameID);
	}

	[NativeName("HasConstantBufferFromScript")]
	private bool HasConstantBufferImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasConstantBufferImpl_Injected(intPtr, name);
	}

	public bool HasConstantBuffer(string name)
	{
		return HasConstantBufferImpl(Shader.PropertyToID(name));
	}

	public bool HasConstantBuffer(int nameID)
	{
		return HasConstantBufferImpl(nameID);
	}

	public unsafe void EnableKeyword(string keyword)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					EnableKeyword_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			EnableKeyword_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public unsafe void DisableKeyword(string keyword)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					DisableKeyword_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			DisableKeyword_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public unsafe bool IsKeywordEnabled(string keyword)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return IsKeywordEnabled_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return IsKeywordEnabled_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("MaterialScripting::EnableKeyword", HasExplicitThis = true)]
	private void EnableLocalKeyword(LocalKeyword keyword)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		EnableLocalKeyword_Injected(intPtr, ref keyword);
	}

	[FreeFunction("MaterialScripting::DisableKeyword", HasExplicitThis = true)]
	private void DisableLocalKeyword(LocalKeyword keyword)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		DisableLocalKeyword_Injected(intPtr, ref keyword);
	}

	[FreeFunction("MaterialScripting::SetKeyword", HasExplicitThis = true)]
	private void SetLocalKeyword(LocalKeyword keyword, bool value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLocalKeyword_Injected(intPtr, ref keyword, value);
	}

	[FreeFunction("MaterialScripting::IsKeywordEnabled", HasExplicitThis = true)]
	private bool IsLocalKeywordEnabled(LocalKeyword keyword)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsLocalKeywordEnabled_Injected(intPtr, ref keyword);
	}

	public void EnableKeyword(in LocalKeyword keyword)
	{
		EnableLocalKeyword(keyword);
	}

	public void DisableKeyword(in LocalKeyword keyword)
	{
		DisableLocalKeyword(keyword);
	}

	public void SetKeyword(in LocalKeyword keyword, bool value)
	{
		SetLocalKeyword(keyword, value);
	}

	public bool IsKeywordEnabled(in LocalKeyword keyword)
	{
		return IsLocalKeywordEnabled(keyword);
	}

	[FreeFunction("MaterialScripting::GetEnabledKeywords", HasExplicitThis = true)]
	private LocalKeyword[] GetEnabledKeywords()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetEnabledKeywords_Injected(intPtr);
	}

	[FreeFunction("MaterialScripting::SetEnabledKeywords", HasExplicitThis = true)]
	private void SetEnabledKeywords(LocalKeyword[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetEnabledKeywords_Injected(intPtr, keywords);
	}

	[FreeFunction("MaterialScripting::SetShaderPassEnabled", HasExplicitThis = true)]
	public unsafe void SetShaderPassEnabled(string passName, bool enabled)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(passName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(passName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetShaderPassEnabled_Injected(intPtr, ref managedSpanWrapper, enabled);
					return;
				}
			}
			SetShaderPassEnabled_Injected(intPtr, ref managedSpanWrapper, enabled);
		}
		finally
		{
		}
	}

	[FreeFunction("MaterialScripting::GetShaderPassEnabled", HasExplicitThis = true)]
	public unsafe bool GetShaderPassEnabled(string passName)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(passName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(passName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetShaderPassEnabled_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return GetShaderPassEnabled_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public string GetPassName(int pass)
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
			GetPassName_Injected(intPtr, pass, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	public unsafe int FindPass(string passName)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(passName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(passName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return FindPass_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return FindPass_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public unsafe void SetOverrideTag(string tag, string val)
	{
		//The blocks IL_0039, IL_0046, IL_0054, IL_0062, IL_0067 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0067 are reachable both inside and outside the pinned region starting at IL_0054. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0067 are reachable both inside and outside the pinned region starting at IL_0054. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper tag2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(tag);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					tag2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(val, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(val);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							SetOverrideTag_Injected(intPtr, ref tag2, ref managedSpanWrapper2);
							return;
						}
					}
					SetOverrideTag_Injected(intPtr, ref tag2, ref managedSpanWrapper2);
					return;
				}
			}
			tag2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(val, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(val);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					SetOverrideTag_Injected(intPtr, ref tag2, ref managedSpanWrapper2);
					return;
				}
			}
			SetOverrideTag_Injected(intPtr, ref tag2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	[NativeName("GetTag")]
	private unsafe string GetTagImpl(string tag, bool currentSubShaderOnly, string defaultValue)
	{
		//The blocks IL_0039, IL_0047, IL_0055, IL_0063, IL_0068 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0068 are reachable both inside and outside the pinned region starting at IL_0055. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0068 are reachable both inside and outside the pinned region starting at IL_0055. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper tag2;
			bool currentSubShaderOnly2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(tag);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					tag2 = ref managedSpanWrapper;
					currentSubShaderOnly2 = currentSubShaderOnly;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(defaultValue, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(defaultValue);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							GetTagImpl_Injected(intPtr, ref tag2, currentSubShaderOnly2, ref managedSpanWrapper2, out ret);
						}
					}
					else
					{
						GetTagImpl_Injected(intPtr, ref tag2, currentSubShaderOnly2, ref managedSpanWrapper2, out ret);
					}
				}
			}
			else
			{
				tag2 = ref managedSpanWrapper;
				currentSubShaderOnly2 = currentSubShaderOnly;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(defaultValue, ref managedSpanWrapper2))
				{
					readOnlySpan2 = MemoryExtensions.AsSpan(defaultValue);
					fixed (char* begin2 = readOnlySpan2)
					{
						managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
						GetTagImpl_Injected(intPtr, ref tag2, currentSubShaderOnly2, ref managedSpanWrapper2, out ret);
					}
				}
				else
				{
					GetTagImpl_Injected(intPtr, ref tag2, currentSubShaderOnly2, ref managedSpanWrapper2, out ret);
				}
			}
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	public string GetTag(string tag, bool searchFallbacks, string defaultValue)
	{
		return GetTagImpl(tag, !searchFallbacks, defaultValue);
	}

	public string GetTag(string tag, bool searchFallbacks)
	{
		return GetTagImpl(tag, !searchFallbacks, "");
	}

	[NativeThrows]
	[FreeFunction("MaterialScripting::Lerp", HasExplicitThis = true)]
	public void Lerp(Material start, Material end, float t)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Lerp_Injected(intPtr, MarshalledUnityObject.Marshal(start), MarshalledUnityObject.Marshal(end), t);
	}

	[FreeFunction("MaterialScripting::SetPass", HasExplicitThis = true)]
	public bool SetPass(int pass)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPass_Injected(intPtr, pass);
	}

	[FreeFunction("MaterialScripting::CopyPropertiesFrom", HasExplicitThis = true)]
	public void CopyPropertiesFromMaterial(Material mat)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPropertiesFromMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(mat));
	}

	[FreeFunction("MaterialScripting::CopyMatchingPropertiesFrom", HasExplicitThis = true)]
	public void CopyMatchingPropertiesFromMaterial(Material mat)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyMatchingPropertiesFromMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(mat));
	}

	[FreeFunction("MaterialScripting::GetShaderKeywords", HasExplicitThis = true)]
	private string[] GetShaderKeywords()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetShaderKeywords_Injected(intPtr);
	}

	[FreeFunction("MaterialScripting::SetShaderKeywords", HasExplicitThis = true)]
	private void SetShaderKeywords(string[] names)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetShaderKeywords_Injected(intPtr, names);
	}

	[FreeFunction("MaterialScripting::GetPropertyNames", HasExplicitThis = true)]
	private string[] GetPropertyNamesImpl(int propertyType)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPropertyNamesImpl_Injected(intPtr, propertyType);
	}

	public int ComputeCRC()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ComputeCRC_Injected(intPtr);
	}

	[FreeFunction("MaterialScripting::GetTexturePropertyNames", HasExplicitThis = true)]
	public string[] GetTexturePropertyNames()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetTexturePropertyNames_Injected(intPtr);
	}

	[FreeFunction("MaterialScripting::GetTexturePropertyNameIDs", HasExplicitThis = true)]
	public int[] GetTexturePropertyNameIDs()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		int[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetTexturePropertyNameIDs_Injected(intPtr, out ret);
		}
		finally
		{
			int[] array = default(int[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("MaterialScripting::GetTexturePropertyNamesInternal", HasExplicitThis = true)]
	private void GetTexturePropertyNamesInternal(object outNames)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetTexturePropertyNamesInternal_Injected(intPtr, outNames);
	}

	[FreeFunction("MaterialScripting::GetTexturePropertyNameIDsInternal", HasExplicitThis = true)]
	private void GetTexturePropertyNameIDsInternal(object outNames)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetTexturePropertyNameIDsInternal_Injected(intPtr, outNames);
	}

	public void GetTexturePropertyNames(List<string> outNames)
	{
		if (outNames == null)
		{
			throw new ArgumentNullException("outNames");
		}
		GetTexturePropertyNamesInternal(outNames);
	}

	public void GetTexturePropertyNameIDs(List<int> outNames)
	{
		if (outNames == null)
		{
			throw new ArgumentNullException("outNames");
		}
		GetTexturePropertyNameIDsInternal(outNames);
	}

	[NativeName("SetIntFromScript")]
	private void SetIntImpl(int name, int value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetIntImpl_Injected(intPtr, name, value);
	}

	[NativeName("SetFloatFromScript")]
	private void SetFloatImpl(int name, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetFloatImpl_Injected(intPtr, name, value);
	}

	[NativeName("SetColorFromScript")]
	private void SetColorImpl(int name, Color value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetColorImpl_Injected(intPtr, name, ref value);
	}

	[NativeName("SetMatrixFromScript")]
	private void SetMatrixImpl(int name, Matrix4x4 value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetMatrixImpl_Injected(intPtr, name, ref value);
	}

	[NativeName("SetTextureFromScript")]
	private void SetTextureImpl(int name, Texture value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTextureImpl_Injected(intPtr, name, MarshalledUnityObject.Marshal(value));
	}

	[NativeName("SetRenderTextureFromScript")]
	private void SetRenderTextureImpl(int name, RenderTexture value, RenderTextureSubElement element)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetRenderTextureImpl_Injected(intPtr, name, MarshalledUnityObject.Marshal(value), element);
	}

	[NativeName("SetBufferFromScript")]
	private void SetBufferImpl(int name, ComputeBuffer value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value));
	}

	[NativeName("SetBufferFromScript")]
	private void SetGraphicsBufferImpl(int name, GraphicsBuffer value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetGraphicsBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value));
	}

	[NativeName("SetConstantBufferFromScript")]
	private void SetConstantBufferImpl(int name, ComputeBuffer value, int offset, int size)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetConstantBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
	}

	[NativeName("SetConstantBufferFromScript")]
	private void SetConstantGraphicsBufferImpl(int name, GraphicsBuffer value, int offset, int size)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetConstantGraphicsBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
	}

	[NativeName("GetIntFromScript")]
	private int GetIntImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetIntImpl_Injected(intPtr, name);
	}

	[NativeName("GetFloatFromScript")]
	private float GetFloatImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetFloatImpl_Injected(intPtr, name);
	}

	[NativeName("GetColorFromScript")]
	private Color GetColorImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetColorImpl_Injected(intPtr, name, out var ret);
		return ret;
	}

	[NativeName("GetMatrixFromScript")]
	private Matrix4x4 GetMatrixImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetMatrixImpl_Injected(intPtr, name, out var ret);
		return ret;
	}

	[NativeName("GetTextureFromScript")]
	private Texture GetTextureImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Texture>(GetTextureImpl_Injected(intPtr, name));
	}

	[NativeName("GetBufferFromScript")]
	private GraphicsBufferHandle GetBufferImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetBufferImpl_Injected(intPtr, name, out var ret);
		return ret;
	}

	[NativeName("GetConstantBufferFromScript")]
	private GraphicsBufferHandle GetConstantBufferImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetConstantBufferImpl_Injected(intPtr, name, out var ret);
		return ret;
	}

	[FreeFunction(Name = "MaterialScripting::SetFloatArray", HasExplicitThis = true)]
	private unsafe void SetFloatArrayImpl(int name, float[] values, int count)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(values);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetFloatArrayImpl_Injected(intPtr, name, ref values2, count);
		}
	}

	[FreeFunction(Name = "MaterialScripting::SetVectorArray", HasExplicitThis = true)]
	private unsafe void SetVectorArrayImpl(int name, Vector4[] values, int count)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector4> span = new Span<Vector4>(values);
		fixed (Vector4* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetVectorArrayImpl_Injected(intPtr, name, ref values2, count);
		}
	}

	[FreeFunction(Name = "MaterialScripting::SetColorArray", HasExplicitThis = true)]
	private unsafe void SetColorArrayImpl(int name, Color[] values, int count)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Color> span = new Span<Color>(values);
		fixed (Color* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetColorArrayImpl_Injected(intPtr, name, ref values2, count);
		}
	}

	[FreeFunction(Name = "MaterialScripting::SetMatrixArray", HasExplicitThis = true)]
	private unsafe void SetMatrixArrayImpl(int name, Matrix4x4[] values, int count)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Matrix4x4> span = new Span<Matrix4x4>(values);
		fixed (Matrix4x4* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetMatrixArrayImpl_Injected(intPtr, name, ref values2, count);
		}
	}

	[FreeFunction(Name = "MaterialScripting::GetFloatArray", HasExplicitThis = true)]
	private float[] GetFloatArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		float[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetFloatArrayImpl_Injected(intPtr, name, out ret);
		}
		finally
		{
			float[] array = default(float[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "MaterialScripting::GetVectorArray", HasExplicitThis = true)]
	private Vector4[] GetVectorArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Vector4[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetVectorArrayImpl_Injected(intPtr, name, out ret);
		}
		finally
		{
			Vector4[] array = default(Vector4[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "MaterialScripting::GetColorArray", HasExplicitThis = true)]
	private Color[] GetColorArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Color[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetColorArrayImpl_Injected(intPtr, name, out ret);
		}
		finally
		{
			Color[] array = default(Color[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "MaterialScripting::GetMatrixArray", HasExplicitThis = true)]
	private Matrix4x4[] GetMatrixArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Matrix4x4[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetMatrixArrayImpl_Injected(intPtr, name, out ret);
		}
		finally
		{
			Matrix4x4[] array = default(Matrix4x4[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "MaterialScripting::GetFloatArrayCount", HasExplicitThis = true)]
	private int GetFloatArrayCountImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetFloatArrayCountImpl_Injected(intPtr, name);
	}

	[FreeFunction(Name = "MaterialScripting::GetVectorArrayCount", HasExplicitThis = true)]
	private int GetVectorArrayCountImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVectorArrayCountImpl_Injected(intPtr, name);
	}

	[FreeFunction(Name = "MaterialScripting::GetColorArrayCount", HasExplicitThis = true)]
	private int GetColorArrayCountImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetColorArrayCountImpl_Injected(intPtr, name);
	}

	[FreeFunction(Name = "MaterialScripting::GetMatrixArrayCount", HasExplicitThis = true)]
	private int GetMatrixArrayCountImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetMatrixArrayCountImpl_Injected(intPtr, name);
	}

	[FreeFunction(Name = "MaterialScripting::ExtractFloatArray", HasExplicitThis = true)]
	private unsafe void ExtractFloatArrayImpl(int name, [Out] float[] val)
	{
		//The blocks IL_002c are reachable both inside and outside the pinned region starting at IL_0015. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (val != null)
			{
				fixed (float[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractFloatArrayImpl_Injected(intPtr, name, out val2);
					return;
				}
			}
			ExtractFloatArrayImpl_Injected(intPtr, name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "MaterialScripting::ExtractVectorArray", HasExplicitThis = true)]
	private unsafe void ExtractVectorArrayImpl(int name, [Out] Vector4[] val)
	{
		//The blocks IL_002c are reachable both inside and outside the pinned region starting at IL_0015. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (val != null)
			{
				fixed (Vector4[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractVectorArrayImpl_Injected(intPtr, name, out val2);
					return;
				}
			}
			ExtractVectorArrayImpl_Injected(intPtr, name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "MaterialScripting::ExtractColorArray", HasExplicitThis = true)]
	private unsafe void ExtractColorArrayImpl(int name, [Out] Color[] val)
	{
		//The blocks IL_002c are reachable both inside and outside the pinned region starting at IL_0015. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (val != null)
			{
				fixed (Color[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractColorArrayImpl_Injected(intPtr, name, out val2);
					return;
				}
			}
			ExtractColorArrayImpl_Injected(intPtr, name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "MaterialScripting::ExtractMatrixArray", HasExplicitThis = true)]
	private unsafe void ExtractMatrixArrayImpl(int name, [Out] Matrix4x4[] val)
	{
		//The blocks IL_002c are reachable both inside and outside the pinned region starting at IL_0015. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (val != null)
			{
				fixed (Matrix4x4[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractMatrixArrayImpl_Injected(intPtr, name, out val2);
					return;
				}
			}
			ExtractMatrixArrayImpl_Injected(intPtr, name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	[NativeName("GetTextureScaleAndOffsetFromScript")]
	private Vector4 GetTextureScaleAndOffsetImpl(int name)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetTextureScaleAndOffsetImpl_Injected(intPtr, name, out var ret);
		return ret;
	}

	[NativeName("SetTextureOffsetFromScript")]
	private void SetTextureOffsetImpl(int name, Vector2 offset)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTextureOffsetImpl_Injected(intPtr, name, ref offset);
	}

	[NativeName("SetTextureScaleFromScript")]
	private void SetTextureScaleImpl(int name, Vector2 scale)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTextureScaleImpl_Injected(intPtr, name, ref scale);
	}

	private void SetFloatArray(int name, float[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetFloatArrayImpl(name, values, count);
	}

	private void SetVectorArray(int name, Vector4[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetVectorArrayImpl(name, values, count);
	}

	private void SetColorArray(int name, Color[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetColorArrayImpl(name, values, count);
	}

	private void SetMatrixArray(int name, Matrix4x4[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetMatrixArrayImpl(name, values, count);
	}

	private void ExtractFloatArray(int name, List<float> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int floatArrayCountImpl = GetFloatArrayCountImpl(name);
		if (floatArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, floatArrayCountImpl);
			ExtractFloatArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private void ExtractVectorArray(int name, List<Vector4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int vectorArrayCountImpl = GetVectorArrayCountImpl(name);
		if (vectorArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, vectorArrayCountImpl);
			ExtractVectorArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private void ExtractColorArray(int name, List<Color> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int colorArrayCountImpl = GetColorArrayCountImpl(name);
		if (colorArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, colorArrayCountImpl);
			ExtractColorArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private void ExtractMatrixArray(int name, List<Matrix4x4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int matrixArrayCountImpl = GetMatrixArrayCountImpl(name);
		if (matrixArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, matrixArrayCountImpl);
			ExtractMatrixArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	public void SetInt(string name, int value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	public void SetInt(int nameID, int value)
	{
		SetFloatImpl(nameID, value);
	}

	public void SetFloat(string name, float value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	public void SetFloat(int nameID, float value)
	{
		SetFloatImpl(nameID, value);
	}

	public void SetInteger(string name, int value)
	{
		SetIntImpl(Shader.PropertyToID(name), value);
	}

	public void SetInteger(int nameID, int value)
	{
		SetIntImpl(nameID, value);
	}

	public void SetColor(string name, Color value)
	{
		SetColorImpl(Shader.PropertyToID(name), value);
	}

	public void SetColor(int nameID, Color value)
	{
		SetColorImpl(nameID, value);
	}

	public void SetVector(string name, Vector4 value)
	{
		SetColorImpl(Shader.PropertyToID(name), value);
	}

	public void SetVector(int nameID, Vector4 value)
	{
		SetColorImpl(nameID, value);
	}

	public void SetMatrix(string name, Matrix4x4 value)
	{
		SetMatrixImpl(Shader.PropertyToID(name), value);
	}

	public void SetMatrix(int nameID, Matrix4x4 value)
	{
		SetMatrixImpl(nameID, value);
	}

	public void SetTexture(string name, Texture value)
	{
		SetTextureImpl(Shader.PropertyToID(name), value);
	}

	public void SetTexture(int nameID, Texture value)
	{
		SetTextureImpl(nameID, value);
	}

	public void SetTexture(string name, RenderTexture value, RenderTextureSubElement element)
	{
		SetRenderTextureImpl(Shader.PropertyToID(name), value, element);
	}

	public void SetTexture(int nameID, RenderTexture value, RenderTextureSubElement element)
	{
		SetRenderTextureImpl(nameID, value, element);
	}

	public void SetBuffer(string name, ComputeBuffer value)
	{
		SetBufferImpl(Shader.PropertyToID(name), value);
	}

	public void SetBuffer(int nameID, ComputeBuffer value)
	{
		SetBufferImpl(nameID, value);
	}

	public void SetBuffer(string name, GraphicsBuffer value)
	{
		SetGraphicsBufferImpl(Shader.PropertyToID(name), value);
	}

	public void SetBuffer(int nameID, GraphicsBuffer value)
	{
		SetGraphicsBufferImpl(nameID, value);
	}

	public void SetConstantBuffer(string name, ComputeBuffer value, int offset, int size)
	{
		SetConstantBufferImpl(Shader.PropertyToID(name), value, offset, size);
	}

	public void SetConstantBuffer(int nameID, ComputeBuffer value, int offset, int size)
	{
		SetConstantBufferImpl(nameID, value, offset, size);
	}

	public void SetConstantBuffer(string name, GraphicsBuffer value, int offset, int size)
	{
		SetConstantGraphicsBufferImpl(Shader.PropertyToID(name), value, offset, size);
	}

	public void SetConstantBuffer(int nameID, GraphicsBuffer value, int offset, int size)
	{
		SetConstantGraphicsBufferImpl(nameID, value, offset, size);
	}

	public void SetFloatArray(string name, List<float> values)
	{
		SetFloatArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetFloatArray(int nameID, List<float> values)
	{
		SetFloatArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetFloatArray(string name, float[] values)
	{
		SetFloatArray(Shader.PropertyToID(name), values, values.Length);
	}

	public void SetFloatArray(int nameID, float[] values)
	{
		SetFloatArray(nameID, values, values.Length);
	}

	public void SetColorArray(string name, List<Color> values)
	{
		SetColorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetColorArray(int nameID, List<Color> values)
	{
		SetColorArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetColorArray(string name, Color[] values)
	{
		SetColorArray(Shader.PropertyToID(name), values, values.Length);
	}

	public void SetColorArray(int nameID, Color[] values)
	{
		SetColorArray(nameID, values, values.Length);
	}

	public void SetVectorArray(string name, List<Vector4> values)
	{
		SetVectorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetVectorArray(int nameID, List<Vector4> values)
	{
		SetVectorArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetVectorArray(string name, Vector4[] values)
	{
		SetVectorArray(Shader.PropertyToID(name), values, values.Length);
	}

	public void SetVectorArray(int nameID, Vector4[] values)
	{
		SetVectorArray(nameID, values, values.Length);
	}

	public void SetMatrixArray(string name, List<Matrix4x4> values)
	{
		SetMatrixArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetMatrixArray(int nameID, List<Matrix4x4> values)
	{
		SetMatrixArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetMatrixArray(string name, Matrix4x4[] values)
	{
		SetMatrixArray(Shader.PropertyToID(name), values, values.Length);
	}

	public void SetMatrixArray(int nameID, Matrix4x4[] values)
	{
		SetMatrixArray(nameID, values, values.Length);
	}

	public int GetInt(string name)
	{
		return (int)GetFloatImpl(Shader.PropertyToID(name));
	}

	public int GetInt(int nameID)
	{
		return (int)GetFloatImpl(nameID);
	}

	public float GetFloat(string name)
	{
		return GetFloatImpl(Shader.PropertyToID(name));
	}

	public float GetFloat(int nameID)
	{
		return GetFloatImpl(nameID);
	}

	public int GetInteger(string name)
	{
		return GetIntImpl(Shader.PropertyToID(name));
	}

	public int GetInteger(int nameID)
	{
		return GetIntImpl(nameID);
	}

	public Color GetColor(string name)
	{
		return GetColorImpl(Shader.PropertyToID(name));
	}

	public Color GetColor(int nameID)
	{
		return GetColorImpl(nameID);
	}

	public Vector4 GetVector(string name)
	{
		return GetColorImpl(Shader.PropertyToID(name));
	}

	public Vector4 GetVector(int nameID)
	{
		return GetColorImpl(nameID);
	}

	public Matrix4x4 GetMatrix(string name)
	{
		return GetMatrixImpl(Shader.PropertyToID(name));
	}

	public Matrix4x4 GetMatrix(int nameID)
	{
		return GetMatrixImpl(nameID);
	}

	public Texture GetTexture(string name)
	{
		return GetTextureImpl(Shader.PropertyToID(name));
	}

	public Texture GetTexture(int nameID)
	{
		return GetTextureImpl(nameID);
	}

	public GraphicsBufferHandle GetBuffer(string name)
	{
		return GetBufferImpl(Shader.PropertyToID(name));
	}

	public GraphicsBufferHandle GetConstantBuffer(string name)
	{
		return GetConstantBufferImpl(Shader.PropertyToID(name));
	}

	public float[] GetFloatArray(string name)
	{
		return GetFloatArray(Shader.PropertyToID(name));
	}

	public float[] GetFloatArray(int nameID)
	{
		return (GetFloatArrayCountImpl(nameID) != 0) ? GetFloatArrayImpl(nameID) : null;
	}

	public Color[] GetColorArray(string name)
	{
		return GetColorArray(Shader.PropertyToID(name));
	}

	public Color[] GetColorArray(int nameID)
	{
		return (GetColorArrayCountImpl(nameID) != 0) ? GetColorArrayImpl(nameID) : null;
	}

	public Vector4[] GetVectorArray(string name)
	{
		return GetVectorArray(Shader.PropertyToID(name));
	}

	public Vector4[] GetVectorArray(int nameID)
	{
		return (GetVectorArrayCountImpl(nameID) != 0) ? GetVectorArrayImpl(nameID) : null;
	}

	public Matrix4x4[] GetMatrixArray(string name)
	{
		return GetMatrixArray(Shader.PropertyToID(name));
	}

	public Matrix4x4[] GetMatrixArray(int nameID)
	{
		return (GetMatrixArrayCountImpl(nameID) != 0) ? GetMatrixArrayImpl(nameID) : null;
	}

	public void GetFloatArray(string name, List<float> values)
	{
		ExtractFloatArray(Shader.PropertyToID(name), values);
	}

	public void GetFloatArray(int nameID, List<float> values)
	{
		ExtractFloatArray(nameID, values);
	}

	public void GetColorArray(string name, List<Color> values)
	{
		ExtractColorArray(Shader.PropertyToID(name), values);
	}

	public void GetColorArray(int nameID, List<Color> values)
	{
		ExtractColorArray(nameID, values);
	}

	public void GetVectorArray(string name, List<Vector4> values)
	{
		ExtractVectorArray(Shader.PropertyToID(name), values);
	}

	public void GetVectorArray(int nameID, List<Vector4> values)
	{
		ExtractVectorArray(nameID, values);
	}

	public void GetMatrixArray(string name, List<Matrix4x4> values)
	{
		ExtractMatrixArray(Shader.PropertyToID(name), values);
	}

	public void GetMatrixArray(int nameID, List<Matrix4x4> values)
	{
		ExtractMatrixArray(nameID, values);
	}

	public void SetTextureOffset(string name, Vector2 value)
	{
		SetTextureOffsetImpl(Shader.PropertyToID(name), value);
	}

	public void SetTextureOffset(int nameID, Vector2 value)
	{
		SetTextureOffsetImpl(nameID, value);
	}

	public void SetTextureScale(string name, Vector2 value)
	{
		SetTextureScaleImpl(Shader.PropertyToID(name), value);
	}

	public void SetTextureScale(int nameID, Vector2 value)
	{
		SetTextureScaleImpl(nameID, value);
	}

	public Vector2 GetTextureOffset(string name)
	{
		return GetTextureOffset(Shader.PropertyToID(name));
	}

	public Vector2 GetTextureOffset(int nameID)
	{
		Vector4 textureScaleAndOffsetImpl = GetTextureScaleAndOffsetImpl(nameID);
		return new Vector2(textureScaleAndOffsetImpl.z, textureScaleAndOffsetImpl.w);
	}

	public Vector2 GetTextureScale(string name)
	{
		return GetTextureScale(Shader.PropertyToID(name));
	}

	public Vector2 GetTextureScale(int nameID)
	{
		Vector4 textureScaleAndOffsetImpl = GetTextureScaleAndOffsetImpl(nameID);
		return new Vector2(textureScaleAndOffsetImpl.x, textureScaleAndOffsetImpl.y);
	}

	public string[] GetPropertyNames(MaterialPropertyType type)
	{
		return GetPropertyNamesImpl((int)type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateWithShader_Injected([Writable] Material self, IntPtr shader);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateWithMaterial_Injected([Writable] Material self, IntPtr source);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetDefaultMaterial_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetDefaultParticleMaterial_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetDefaultLineMaterial_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_shader_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shader_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetFirstPropertyNameIdByAttribute_Injected(IntPtr _unity_self, ShaderPropertyFlags attributeFlag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasProperty_Injected(IntPtr _unity_self, int nameID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasFloatImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasIntImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasTextureImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasMatrixImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasVectorImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasBufferImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasConstantBufferImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_renderQueue_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_renderQueue_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_rawRenderQueue_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisableKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsKeywordEnabled_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisableLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsLocalKeywordEnabled_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LocalKeyword[] GetEnabledKeywords_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetEnabledKeywords_Injected(IntPtr _unity_self, LocalKeyword[] keywords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern MaterialGlobalIlluminationFlags get_globalIlluminationFlags_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_globalIlluminationFlags_Injected(IntPtr _unity_self, MaterialGlobalIlluminationFlags value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_doubleSidedGI_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_doubleSidedGI_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enableInstancing_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enableInstancing_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_passCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetShaderPassEnabled_Injected(IntPtr _unity_self, ref ManagedSpanWrapper passName, bool enabled);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetShaderPassEnabled_Injected(IntPtr _unity_self, ref ManagedSpanWrapper passName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPassName_Injected(IntPtr _unity_self, int pass, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int FindPass_Injected(IntPtr _unity_self, ref ManagedSpanWrapper passName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetOverrideTag_Injected(IntPtr _unity_self, ref ManagedSpanWrapper tag, ref ManagedSpanWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTagImpl_Injected(IntPtr _unity_self, ref ManagedSpanWrapper tag, bool currentSubShaderOnly, ref ManagedSpanWrapper defaultValue, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Lerp_Injected(IntPtr _unity_self, IntPtr start, IntPtr end, float t);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPass_Injected(IntPtr _unity_self, int pass);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPropertiesFromMaterial_Injected(IntPtr _unity_self, IntPtr mat);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyMatchingPropertiesFromMaterial_Injected(IntPtr _unity_self, IntPtr mat);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string[] GetShaderKeywords_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetShaderKeywords_Injected(IntPtr _unity_self, string[] names);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string[] GetPropertyNamesImpl_Injected(IntPtr _unity_self, int propertyType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ComputeCRC_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string[] GetTexturePropertyNames_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTexturePropertyNameIDs_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTexturePropertyNamesInternal_Injected(IntPtr _unity_self, object outNames);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTexturePropertyNameIDsInternal_Injected(IntPtr _unity_self, object outNames);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIntImpl_Injected(IntPtr _unity_self, int name, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloatImpl_Injected(IntPtr _unity_self, int name, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetColorImpl_Injected(IntPtr _unity_self, int name, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMatrixImpl_Injected(IntPtr _unity_self, int name, [In] ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTextureImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRenderTextureImpl_Injected(IntPtr _unity_self, int name, IntPtr value, RenderTextureSubElement element);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGraphicsBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetConstantBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetConstantGraphicsBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetIntImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetFloatImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetColorImpl_Injected(IntPtr _unity_self, int name, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMatrixImpl_Injected(IntPtr _unity_self, int name, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetTextureImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetBufferImpl_Injected(IntPtr _unity_self, int name, out GraphicsBufferHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetConstantBufferImpl_Injected(IntPtr _unity_self, int name, out GraphicsBufferHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloatArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVectorArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetColorArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMatrixArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetFloatArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetVectorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetColorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMatrixArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetFloatArrayCountImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVectorArrayCountImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColorArrayCountImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetMatrixArrayCountImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractFloatArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractVectorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractColorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractMatrixArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTextureScaleAndOffsetImpl_Injected(IntPtr _unity_self, int name, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTextureOffsetImpl_Injected(IntPtr _unity_self, int name, [In] ref Vector2 offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTextureScaleImpl_Injected(IntPtr _unity_self, int name, [In] ref Vector2 scale);
}
