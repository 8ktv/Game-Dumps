using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[UsedByNativeCode]
[NativeAsStruct]
public sealed class TreePrototype
{
	[NativeName("prefab")]
	internal GameObject m_Prefab;

	[NativeName("bendFactor")]
	internal float m_BendFactor;

	[NativeName("navMeshLod")]
	internal int m_NavMeshLod;

	public GameObject prefab
	{
		get
		{
			return m_Prefab;
		}
		set
		{
			m_Prefab = value;
		}
	}

	public float bendFactor
	{
		get
		{
			return m_BendFactor;
		}
		set
		{
			m_BendFactor = value;
		}
	}

	public int navMeshLod
	{
		get
		{
			return m_NavMeshLod;
		}
		set
		{
			m_NavMeshLod = value;
		}
	}

	public TreePrototype()
	{
	}

	public TreePrototype(TreePrototype other)
	{
		prefab = other.prefab;
		bendFactor = other.bendFactor;
		navMeshLod = other.navMeshLod;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as TreePrototype);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	private bool Equals(TreePrototype other)
	{
		if (other == null)
		{
			return false;
		}
		if (other == this)
		{
			return true;
		}
		if (GetType() != other.GetType())
		{
			return false;
		}
		return prefab == other.prefab && bendFactor == other.bendFactor && navMeshLod == other.navMeshLod;
	}

	internal bool Validate(out string errorMessage)
	{
		return ValidateTreePrototype(this, out errorMessage);
	}

	[FreeFunction("TerrainDataScriptingInterface::ValidateTreePrototype")]
	internal static bool ValidateTreePrototype([NotNull] TreePrototype prototype, out string errorMessage)
	{
		if (prototype == null)
		{
			ThrowHelper.ThrowArgumentNullException(prototype, "prototype");
		}
		ManagedSpanWrapper errorMessage2 = default(ManagedSpanWrapper);
		try
		{
			return ValidateTreePrototype_Injected(prototype, out errorMessage2);
		}
		finally
		{
			errorMessage = OutStringMarshaller.GetStringAndDispose(errorMessage2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ValidateTreePrototype_Injected(TreePrototype prototype, out ManagedSpanWrapper errorMessage);
}
