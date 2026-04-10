using System.Collections.Generic;
using UnityEngine;

public class FreezeBombManager : SingletonBehaviour<FreezeBombManager>
{
	[SerializeField]
	private FreezeBombIceBlock iceBlockPrefab;

	[SerializeField]
	private int maxIceBlockPoolSize;

	private static Transform iceBlockPoolParent;

	private static readonly Stack<FreezeBombIceBlock> iceBlockPool = new Stack<FreezeBombIceBlock>();

	public static FreezeBombIceBlock GetUnusedIceBlock()
	{
		if (!SingletonBehaviour<FreezeBombManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<FreezeBombManager>.Instance.GetUnusedIceBlockInternal();
	}

	public static void ReturnIceBlock(FreezeBombIceBlock iceBlock)
	{
		if (SingletonBehaviour<FreezeBombManager>.HasInstance)
		{
			SingletonBehaviour<FreezeBombManager>.Instance.ReturnIceBlockInternal(iceBlock);
		}
	}

	private FreezeBombIceBlock GetUnusedIceBlockInternal()
	{
		EnsurePoolParentExists();
		FreezeBombIceBlock result = null;
		while (result == null)
		{
			if (!iceBlockPool.TryPop(out result))
			{
				result = Object.Instantiate(iceBlockPrefab);
			}
		}
		result.gameObject.SetActive(value: true);
		return result;
		static void EnsurePoolParentExists()
		{
			if (!(iceBlockPoolParent != null))
			{
				GameObject obj = new GameObject("Freeze bomb ice block pool");
				Object.DontDestroyOnLoad(obj);
				iceBlockPoolParent = obj.transform;
			}
		}
	}

	private void ReturnIceBlockInternal(FreezeBombIceBlock iceBlock)
	{
		iceBlock.InformReturnedToPool();
		if (iceBlockPool.Count >= maxIceBlockPoolSize)
		{
			Object.Destroy(iceBlock.gameObject);
			return;
		}
		iceBlock.gameObject.SetActive(value: false);
		iceBlock.transform.SetParent(iceBlockPoolParent);
		iceBlockPool.Push(iceBlock);
	}
}
