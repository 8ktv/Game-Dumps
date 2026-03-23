using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPool<T> : IDisposable where T : Component, IPoolable<T>
{
	private AsyncOperationHandle<GameObject> addressableLoadHandle;

	private bool hasAddressableLoadHandle;

	private T prefab;

	private readonly Transform poolParent;

	private readonly int initialPoolSize;

	private readonly int maxPoolSize;

	private readonly Queue<T> pool = new Queue<T>();

	private bool usingInstantiatedPrefab;

	private bool initializationFinalized;

	private bool disposed;

	public bool IsDisposed => disposed;

	public bool PrefabLoaded { get; private set; }

	public T Prefab => prefab;

	public ObjectPool(T prefab, Transform poolParent, int initialPoolSize, int maxPoolSize)
	{
		if (prefab == null)
		{
			throw new ArgumentNullException("prefab");
		}
		if (poolParent == null)
		{
			Debug.LogWarning("An object pool is being instantiated without a parent Transform, and its instances will be top-level objects in the scene");
		}
		if (initialPoolSize <= 0)
		{
			throw new ArgumentOutOfRangeException("initialPoolSize");
		}
		if (maxPoolSize < initialPoolSize)
		{
			throw new ArgumentOutOfRangeException("maxPoolSize");
		}
		SetPrefab(prefab);
		this.poolParent = poolParent;
		this.initialPoolSize = initialPoolSize;
		this.maxPoolSize = maxPoolSize;
		FinalizePrefab(prefab);
	}

	public ObjectPool(string prefabPath, Transform poolParent, int initialPoolSize, int maxPoolSize)
	{
		if (poolParent == null)
		{
			Debug.LogWarning("An object pool is being instantiated without a parent Transform, and its instances will be top-level objects in the scene");
		}
		if (initialPoolSize <= 0)
		{
			throw new ArgumentOutOfRangeException("initialPoolSize");
		}
		if (maxPoolSize < initialPoolSize)
		{
			throw new ArgumentOutOfRangeException("maxPoolSize");
		}
		this.poolParent = poolParent;
		this.initialPoolSize = initialPoolSize;
		this.maxPoolSize = maxPoolSize;
		LoadPrefab(prefabPath);
	}

	private async void LoadPrefab(string path)
	{
		await UniTask.Yield();
		if (path.EndsWith(".prefab"))
		{
			addressableLoadHandle = Addressables.InstantiateAsync(path);
			hasAddressableLoadHandle = true;
			addressableLoadHandle.Task.AsUniTask();
			usingInstantiatedPrefab = true;
			addressableLoadHandle.Completed += OnAddressableLoadCompleted;
			return;
		}
		throw new ArgumentException("Prefab path does not match an addressable or Brimstone prefab! Did you forget to specify the file extension?", "path");
		void OnAddressableLoadCompleted(AsyncOperationHandle<GameObject> loadedGameObjectHandle)
		{
			if (!disposed)
			{
				GameObject result = loadedGameObjectHandle.Result;
				result.SetActive(value: false);
				result.hideFlags = HideFlags.HideAndDontSave;
				if (!result.TryGetComponent<T>(out var component))
				{
					throw new ArgumentException($"Prefab at passed path does not contain a {typeof(T)} component", "prefab");
				}
				SetPrefab(component);
				InitializePool();
			}
		}
	}

	private void FinalizePrefab(T prefab)
	{
		prefab.gameObject.SetActive(value: false);
		SetPrefab(prefab);
		InitializePool();
	}

	private void InitializePool()
	{
		pool.Clear();
		for (int i = 0; i < initialPoolSize; i++)
		{
			ExpandPool();
		}
		initializationFinalized = true;
	}

	private void SetPrefab(T prefab)
	{
		this.prefab = prefab;
		PrefabLoaded = true;
	}

	public T GetInstance()
	{
		if (disposed)
		{
			Debug.LogError("Attempted to get an instance from a disposed ObjectPool. No reference should be kept to an ObjectPool after disposal");
			return null;
		}
		if (!initializationFinalized)
		{
			if (!hasAddressableLoadHandle)
			{
				Debug.LogWarning($"Attempted to get an instance from a pool of type {typeof(T)} before it finished initializing");
				return null;
			}
			addressableLoadHandle.WaitForCompletion();
		}
		T val = null;
		while (val == null)
		{
			if (pool.Count <= 0)
			{
				ExpandPool();
			}
			val = pool.Dequeue();
		}
		val.gameObject.SetActive(value: true);
		return val;
	}

	public void RegisterFreeInstance(T instance)
	{
		if (disposed)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
		if (!(instance == null) && !pool.Contains(instance))
		{
			if (pool.Count >= maxPoolSize)
			{
				UnityEngine.Object.Destroy(instance.gameObject);
				return;
			}
			pool.Enqueue(instance);
			instance.gameObject.SetActive(value: false);
			instance.transform.SetParent(poolParent);
		}
	}

	private void ExpandPool()
	{
		T val = UnityEngine.Object.Instantiate(prefab, poolParent);
		val.gameObject.name = prefab.gameObject.name + " (pooled)";
		val.SetPool(this);
		pool.Enqueue(val);
	}

	private void CullInstanceFromPool()
	{
		T val = pool.Dequeue();
		if (!(val == null))
		{
			UnityEngine.Object.Destroy(val.gameObject);
		}
	}

	private void CullAllInstancesFromPool()
	{
		for (int i = 0; i < pool.Count; i++)
		{
			CullInstanceFromPool();
		}
	}

	public void Dispose()
	{
		CullAllInstancesFromPool();
		if (usingInstantiatedPrefab && prefab != null)
		{
			UnityEngine.Object.DestroyImmediate(prefab.gameObject);
		}
		if (hasAddressableLoadHandle)
		{
			Addressables.Release(addressableLoadHandle);
		}
		disposed = true;
	}
}
