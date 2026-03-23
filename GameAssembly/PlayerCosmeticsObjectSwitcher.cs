using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerCosmeticsObjectSwitcher : MonoBehaviour
{
	[SerializeField]
	private Renderer renderer;

	private GameObject modelOverride;

	public GameObject ModelOverride => modelOverride;

	public event Action OnModelOverride;

	public async UniTask<bool> OverrideModel(string metadataKey, PlayerCosmeticsSwitcher switcher, CancellationToken cancellationToken)
	{
		if (renderer == null)
		{
			return false;
		}
		GameObject prevModel = modelOverride;
		try
		{
			if (metadataKey == null || metadataKey == string.Empty)
			{
				this.OnModelOverride?.Invoke();
				return false;
			}
			AsyncOperationHandle<PlayerCosmeticsMetadata> metadata = default(AsyncOperationHandle<PlayerCosmeticsMetadata>);
			try
			{
				metadata = Addressables.LoadAssetAsync<PlayerCosmeticsMetadata>(metadataKey);
				await metadata;
				if (this == null || switcher == null)
				{
					metadata.Release();
					return false;
				}
			}
			catch (Exception)
			{
				metadata.Release();
				if (!cancellationToken.IsCancellationRequested)
				{
					Debug.LogWarning("Failed to load cosmetic with key " + metadataKey);
					this.OnModelOverride?.Invoke();
				}
				return false;
			}
			if (metadata.Result == null || !metadata.Result.model.RuntimeKeyIsValid() || !switcher.CheckOwnership(metadata.Result) || cancellationToken.IsCancellationRequested)
			{
				metadata.Release();
				if (!cancellationToken.IsCancellationRequested)
				{
					this.OnModelOverride?.Invoke();
				}
				return false;
			}
			AsyncOperationHandle<GameObject> instantiate = Addressables.InstantiateAsync(metadata.Result.model.RuntimeKey, base.transform.position, base.transform.rotation, base.transform);
			metadata.Release();
			await instantiate;
			if (this == null || switcher == null || instantiate.Result == null || cancellationToken.IsCancellationRequested)
			{
				if (instantiate.Result != null)
				{
					Addressables.ReleaseInstance(instantiate.Result);
				}
				if (this == null)
				{
					return false;
				}
				if (!cancellationToken.IsCancellationRequested)
				{
					this.OnModelOverride?.Invoke();
				}
				return false;
			}
			GameObject gameObject = (modelOverride = instantiate.Result);
			renderer.forceRenderingOff = true;
			gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			gameObject.gameObject.SetLayerRecursively(renderer.gameObject.layer);
			this.OnModelOverride?.Invoke();
			return true;
		}
		finally
		{
			if (prevModel != null)
			{
				if (prevModel == modelOverride)
				{
					modelOverride = null;
				}
				Addressables.ReleaseInstance(prevModel);
				renderer.forceRenderingOff = modelOverride != null;
			}
		}
	}
}
