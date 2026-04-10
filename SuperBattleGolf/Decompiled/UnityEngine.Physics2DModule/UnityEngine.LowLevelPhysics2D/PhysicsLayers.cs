using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics2D;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct PhysicsLayers
{
	[Serializable]
	public class LayerNames : ISerializationCallbackReceiver
	{
		[SerializeField]
		internal string[] m_Names;

		private Dictionary<string, int> m_NameMap;

		private string[] Names
		{
			get
			{
				if (m_Names == null || m_Names.Length != 64)
				{
					m_Names = new string[64];
				}
				return m_Names;
			}
		}

		private Dictionary<string, int> NameMap
		{
			get
			{
				if (m_NameMap == null)
				{
					m_NameMap = new Dictionary<string, int>(64);
				}
				return m_NameMap;
			}
		}

		internal static LayerNames DefaultLayerNames
		{
			get
			{
				LayerNames layerNames = new LayerNames();
				string[] names = layerNames.Names;
				Dictionary<string, int> nameMap = layerNames.NameMap;
				names[0] = "Default";
				nameMap.Add(names[0], 0);
				return layerNames;
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			string[] names = Names;
			Dictionary<string, int> nameMap = NameMap;
			nameMap.Clear();
			for (int i = 0; i < 64; i++)
			{
				string text = names[i];
				if (!string.IsNullOrEmpty(text))
				{
					nameMap.TryAdd(text, i);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int GetLayerOrdinal(string layerName)
		{
			if (NameMap.TryGetValue(layerName, out var value))
			{
				return value;
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal PhysicsMask GetLayerMask(string layerName)
		{
			int layerOrdinal = GetLayerOrdinal(layerName);
			if (layerOrdinal != -1)
			{
				return new PhysicsMask(layerOrdinal);
			}
			return default(PhysicsMask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal string GetLayerName(int layerOrdinal)
		{
			if (layerOrdinal < 0 || layerOrdinal > 63)
			{
				throw new ArgumentOutOfRangeException($"The layer ordinal `{layerOrdinal}' is out of the valid range [0, 63].");
			}
			return Names[layerOrdinal];
		}
	}

	public const int InvalidLayerOrdinal = -1;

	public static PhysicsMask GetLayerMask(params string[] layerNames)
	{
		if (layerNames.Length == 0)
		{
			throw new ArgumentException("No Layer Names provided.", "layerNames");
		}
		if (PhysicsWorld.useFullLayers && PhysicsLowLevelScripting2D.PhysicsGlobal_GetPhysicsLayers() is LayerNames layerNames2)
		{
			PhysicsMask none = PhysicsMask.None;
			foreach (string text in layerNames)
			{
				PhysicsMask layerMask = layerNames2.GetLayerMask(text);
				if ((ulong)layerMask != (ulong)PhysicsMask.None)
				{
					none |= layerMask;
				}
				else
				{
					Debug.LogWarning("The layer name '" + text + "' could not be found in the full 64-bit layers. Note that the name(s) provided are case-sensitive.");
				}
			}
			return none;
		}
		PhysicsMask none2 = PhysicsMask.None;
		foreach (string text2 in layerNames)
		{
			int num = LayerMask.NameToLayer(text2);
			if (num != -1)
			{
				none2 |= new PhysicsMask(num);
			}
			else
			{
				Debug.LogWarning("The layer name '" + text2 + "' could not be found in the standard 32-bit layers. Note that the name(s) provided are case-sensitive.");
			}
		}
		return none2;
	}

	public static int GetLayerOrdinal(string layerName)
	{
		if (PhysicsWorld.useFullLayers && PhysicsLowLevelScripting2D.PhysicsGlobal_GetPhysicsLayers() is LayerNames layerNames)
		{
			int layerOrdinal = layerNames.GetLayerOrdinal(layerName);
			if (layerOrdinal != -1)
			{
				return layerOrdinal;
			}
			Debug.LogWarning("The layer name '" + layerName + "' could not be found in the full 64-bit layers. Note that the name provided is case-sensitive.");
			return -1;
		}
		int num = LayerMask.NameToLayer(layerName);
		if (num != -1)
		{
			return num;
		}
		Debug.LogWarning("The layer name '" + layerName + "' could not be found in the standard 32-bit layers. Note that the name provided is case-sensitive.");
		return -1;
	}

	public static string GetLayerName(int layerOrdinal)
	{
		if (PhysicsWorld.useFullLayers && PhysicsLowLevelScripting2D.PhysicsGlobal_GetPhysicsLayers() is LayerNames layerNames)
		{
			if (layerOrdinal < 0 || layerOrdinal > 63)
			{
				throw new ArgumentOutOfRangeException($"The layer ordinal `{layerOrdinal}' is out of the valid range [0, 63].");
			}
			return layerNames.GetLayerName(layerOrdinal);
		}
		if (layerOrdinal < 0 || layerOrdinal > 31)
		{
			throw new ArgumentOutOfRangeException($"The layer ordinal `{layerOrdinal}' is out of the valid range [0, 31].");
		}
		return LayerMask.LayerToName(layerOrdinal);
	}

	internal static void GetLayerNamesAndMasks(List<string> layerNames, List<ulong> layerMasks)
	{
		layerNames.Clear();
		layerMasks.Clear();
		if (!(PhysicsLowLevelScripting2D.PhysicsGlobal_GetPhysicsLayers() is LayerNames { m_Names: var names }) || names.Length != 64)
		{
			return;
		}
		for (int i = 0; i < 64; i++)
		{
			string text = names[i];
			if (!string.IsNullOrEmpty(text))
			{
				layerNames.Add($"{text} [{i}]");
				layerMasks.Add((ulong)(1L << i));
			}
		}
	}

	internal static void GetBitNamesAndMasks(List<string> layerNames, List<ulong> layerMasks)
	{
		layerNames.Clear();
		layerMasks.Clear();
		for (int i = 0; i < 64; i++)
		{
			layerNames.Add($"{i}");
			layerMasks.Add((ulong)(1L << i));
		}
	}
}
