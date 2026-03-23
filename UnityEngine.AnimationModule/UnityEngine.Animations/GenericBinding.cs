using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[NativeType(CodegenOptions.Custom, "UnityEngine::Animation::MonoGenericBinding")]
[UsedByNativeCode]
public readonly struct GenericBinding
{
	private readonly uint m_Path;

	private readonly uint m_PropertyName;

	private readonly EntityId m_ScriptEntityId;

	private readonly int m_TypeID;

	private readonly byte m_CustomType;

	internal readonly Flags m_Flags;

	public bool isObjectReference => (m_Flags & Flags.kPPtr) == Flags.kPPtr;

	public bool isDiscrete => (m_Flags & Flags.kDiscrete) != 0;

	public bool isSerializeReference => (m_Flags & Flags.kSerializeReference) == Flags.kSerializeReference;

	public uint transformPathHash => m_Path;

	public uint propertyNameHash => m_PropertyName;

	public EntityId scriptEntityId => m_ScriptEntityId;

	[Obsolete("scriptInstanceID is deprecated. Use scriptEntityId instead.", false)]
	public int scriptInstanceID => m_ScriptEntityId;

	public int typeID => m_TypeID;

	public byte customTypeID => m_CustomType;
}
