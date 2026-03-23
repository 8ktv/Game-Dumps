using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerCosmeticsSwitcher : MonoBehaviour
{
	[Serializable]
	public class CosmeticKey : IEquatable<CosmeticKey>
	{
		public string metadataKey = string.Empty;

		public sbyte variationIndex;

		public bool Equals(CosmeticKey other)
		{
			if (other != null && metadataKey == other.metadataKey)
			{
				return variationIndex == other.variationIndex;
			}
			return false;
		}

		public bool HasValidKey()
		{
			return !string.IsNullOrEmpty(metadataKey);
		}
	}

	private class CosmeticCancellationTokenSource
	{
		private bool isCanceled;

		private CancellationTokenSource globalTokenSource;

		public void Cancel()
		{
			if (!isCanceled)
			{
				isCanceled = true;
				globalTokenSource?.Cancel();
			}
		}

		public void GetLocalCancellationToken(out CancellationTokenSource localSource)
		{
			if (globalTokenSource != null)
			{
				Cancel();
			}
			globalTokenSource = (localSource = new CancellationTokenSource());
			isCanceled = false;
		}

		public void DisposeCancellationToken(CancellationTokenSource localSource)
		{
			if (globalTokenSource == localSource)
			{
				globalTokenSource = null;
			}
			localSource.Dispose();
		}
	}

	private class LoadedTextureOverride
	{
		public Texture2D loadedTexture;

		public void UnloadTexture()
		{
			if (loadedTexture != null)
			{
				Addressables.Release(loadedTexture);
				loadedTexture = null;
			}
		}

		public static implicit operator LoadedTextureOverride(Texture2D tex)
		{
			return new LoadedTextureOverride
			{
				loadedTexture = tex
			};
		}
	}

	private class LoadedCosmetic
	{
		public PlayerCosmeticObject cosmetic;

		public LoadedTextureOverride textureOverride;

		public void Unload()
		{
			if (cosmetic != null)
			{
				Addressables.ReleaseInstance(cosmetic.gameObject);
				cosmetic = null;
			}
			textureOverride?.UnloadTexture();
		}
	}

	private readonly CosmeticCancellationTokenSource cheeksCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource browsCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource mouthCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource eyesCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource headModelCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource hatModelCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource faceModelCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource lowerFaceModelCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource golfClubCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource golfBallCancellationToken = new CosmeticCancellationTokenSource();

	private readonly CosmeticCancellationTokenSource bodyModelCancellationToken = new CosmeticCancellationTokenSource();

	public PlayerCosmeticsSettings cosmeticsSettings;

	public Mesh headMesh;

	public Mesh headNoEarsMesh;

	public Texture2D[] defaultMouths;

	public Texture2D[] defaultMouthMasks;

	public Texture2D defaultEyes;

	public Texture2D knockedOutEyes;

	public Transform headRoot;

	public SkinnedMeshRenderer bodyRenderer;

	public SkinnedMeshRenderer headRenderer;

	public Renderer mouthRenderer;

	public EquipmentSwitcher rightHand;

	public EquipmentSwitcher leftHand;

	public PlayerCosmeticsObjectSwitcher golfBall;

	public bool isCharacterPreview;

	private LoadedCosmetic currentHeadModel;

	private LoadedCosmetic currentHatModel;

	private LoadedCosmetic currentFaceModel;

	private LoadedCosmetic currentLowerFaceModel;

	private PlayerCosmeticFaceFeature currentEyes;

	private PlayerCosmeticFaceFeature currentMouth;

	private PlayerCosmeticFaceFeature currentCheeks;

	private PlayerCosmeticFaceFeature currentBrows;

	private PlayerMovement playerMovement;

	private GameObject currentBodyAsset;

	private Material bodyDefaultMaterial;

	private Mesh bodyDefaultMesh;

	private LoadedTextureOverride bodyTextureOverride;

	private MaterialPropertyBlock matProps;

	private MaterialPropertyBlock skinColorProps;

	private int mouthIndex;

	private bool isKnockedOut;

	public CosmeticKey CurrentHeadRuntimeKey { get; private set; }

	public CosmeticKey CurrentHatRuntimeKey { get; private set; }

	public CosmeticKey CurrentFaceRuntimeKey { get; private set; }

	public CosmeticKey CurrentLowerFaceRuntimeKey { get; private set; }

	public CosmeticKey CurrentClubRuntimeKey { get; private set; }

	public CosmeticKey CurrentBodyRuntimeKey { get; private set; }

	public int CurrentSkinColorIndex { get; private set; }

	public CosmeticKey CurrentEyesRuntimeKey { get; private set; }

	public CosmeticKey CurrentMouthRuntimeKey { get; private set; }

	public CosmeticKey CurrentCheeksRuntimeKey { get; private set; }

	public CosmeticKey CurrentBrowsRuntimeKey { get; private set; }

	public CosmeticKey CurrentGolfBallRuntimeKey { get; private set; }

	public event Action HeadChanged;

	public event Action HatChanged;

	public event Action FaceChanged;

	public event Action LowerFaceChanged;

	public event Action ClubChanged;

	public event Action BodyChanged;

	public event Action EyesChanged;

	public event Action MouthChanged;

	public event Action CheeksChanged;

	public event Action BrowsChanged;

	public event Action GolfBallChanged;

	public event Action SkinColorChanged;

	private void Awake()
	{
		playerMovement = GetComponent<PlayerMovement>();
		bodyDefaultMaterial = bodyRenderer.sharedMaterial;
		bodyDefaultMesh = bodyRenderer.sharedMesh;
		bodyRenderer.gameObject.layer = GameManager.LayerSettings.PlayerLayer;
		headRenderer.gameObject.layer = GameManager.LayerSettings.PlayerLayer;
		mouthRenderer.gameObject.layer = GameManager.LayerSettings.PlayerLayer;
		matProps = new MaterialPropertyBlock();
		SetSkinColor(0);
		if (playerMovement != null)
		{
			playerMovement.IsVisibleChanged += PlayerVisibilityChanged;
		}
	}

	private void OnDestroy()
	{
		if (playerMovement != null)
		{
			playerMovement.IsVisibleChanged -= PlayerVisibilityChanged;
		}
		CancelLoading();
	}

	private void CancelLoading()
	{
		cheeksCancellationToken.Cancel();
		browsCancellationToken.Cancel();
		mouthCancellationToken.Cancel();
		eyesCancellationToken.Cancel();
		headModelCancellationToken.Cancel();
		hatModelCancellationToken.Cancel();
		faceModelCancellationToken.Cancel();
		lowerFaceModelCancellationToken.Cancel();
		golfClubCancellationToken.Cancel();
		golfBallCancellationToken.Cancel();
		bodyModelCancellationToken.Cancel();
	}

	private void PlayerVisibilityChanged()
	{
		UpdateModelVisibility(currentHeadModel);
		UpdateModelVisibility(currentHatModel);
		UpdateModelVisibility(currentFaceModel);
		UpdateModelVisibility(currentLowerFaceModel);
	}

	private void UpdateModelVisibility(LoadedCosmetic loaded)
	{
		if (playerMovement == null || loaded == null || loaded.cosmetic == null)
		{
			return;
		}
		List<Renderer> value;
		using (CollectionPool<List<Renderer>, Renderer>.Get(out value))
		{
			value.Clear();
			loaded.cosmetic.GetComponentsInChildren(includeInactive: true, value);
			foreach (Renderer item in value)
			{
				item.enabled = playerMovement.IsVisible;
			}
		}
	}

	private PlayerCosmeticsSettings.SkinColor GetSkinColor(int index)
	{
		return cosmeticsSettings.skinColors[index];
	}

	public void SetSkinColor(int index)
	{
		if (skinColorProps == null)
		{
			skinColorProps = new MaterialPropertyBlock();
		}
		PlayerCosmeticsSettings.SkinColor skinColor = GetSkinColor(index);
		Color baseColor = skinColor.baseColor;
		Color mouthColor = skinColor.mouthColor;
		mouthColor.a = (baseColor.a = 1f);
		skinColorProps.Clear();
		if (playerMovement != null)
		{
			skinColorProps.SetPlayerIndex(playerMovement.PlayerInfo);
		}
		skinColorProps.SetColor("_Color", baseColor);
		headRenderer.SetPropertyBlock(skinColorProps);
		if (bodyTextureOverride != null && bodyTextureOverride.loadedTexture != null)
		{
			skinColorProps.SetTexture("_MainTex", bodyTextureOverride.loadedTexture);
		}
		bodyRenderer.SetPropertyBlock(skinColorProps);
		TintCosmeticIfNeeded(currentHeadModel);
		CurrentSkinColorIndex = index;
		UpdateBaseTextureOverrides(currentHeadModel);
		UpdateFaceTextures();
		this.SkinColorChanged?.Invoke();
		void TintCosmeticIfNeeded(LoadedCosmetic cosmetic)
		{
			if (cosmetic != null && !(cosmetic.cosmetic == null) && cosmetic.cosmetic.requireSkinColorTint)
			{
				Renderer[] componentsInChildren = cosmetic.cosmetic.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.GetPropertyBlock(skinColorProps);
					skinColorProps.SetColor("_Color", baseColor);
					if (cosmetic.cosmetic.skinColorTintMaterialIndex < 0)
					{
						renderer.SetPropertyBlock(skinColorProps);
					}
					else
					{
						renderer.SetPropertyBlock(skinColorProps, cosmetic.cosmetic.skinColorTintMaterialIndex);
					}
				}
			}
		}
	}

	public void SetKnockedOut(bool knockedOut)
	{
		isKnockedOut = knockedOut;
		UpdateFaceTextures();
	}

	public void SetTalkingMagnitude(float normalizedTalkingMagnitude)
	{
		mouthIndex = BMath.RoundToInt(BMath.LerpClamped(1f, defaultMouths.Length - 1, normalizedTalkingMagnitude));
		UpdateFaceTextures();
	}

	public void StopTalking()
	{
		mouthIndex = 0;
		UpdateFaceTextures();
	}

	private void UpdateFaceTextures()
	{
		Color mouthColor = GetSkinColor(CurrentSkinColorIndex).mouthColor;
		mouthColor.a = 1f;
		skinColorProps.Clear();
		skinColorProps.SetColor("_Color", mouthColor);
		if (playerMovement != null)
		{
			skinColorProps.SetPlayerIndex(playerMovement.PlayerInfo);
		}
		if (mouthIndex > 0)
		{
			skinColorProps.SetTexture("_FaceTex", defaultMouths[mouthIndex]);
			skinColorProps.SetTexture("_FaceMask", defaultMouthMasks[mouthIndex]);
		}
		else if (currentMouth != null)
		{
			skinColorProps.SetTexture("_FaceTex", currentMouth.texture);
			skinColorProps.SetTexture("_FaceMask", (currentMouth.mask != null) ? currentMouth.mask : Texture2D.blackTexture);
		}
		if (currentCheeks != null)
		{
			skinColorProps.SetTexture("_CheekTex", currentCheeks.texture);
		}
		if (currentBrows != null)
		{
			skinColorProps.SetTexture("_BrowsTex", currentBrows.texture);
		}
		Texture2D texture2D = ((currentEyes != null) ? currentEyes.texture : defaultEyes);
		Texture2D value = ((!isKnockedOut && currentEyes != null && currentEyes.mask != null) ? currentEyes.mask : Texture2D.blackTexture);
		skinColorProps.SetTexture("_EyesTex", isKnockedOut ? knockedOutEyes : texture2D);
		skinColorProps.SetTexture("_EyesMask", value);
		mouthRenderer.SetPropertyBlock(skinColorProps);
	}

	public async UniTask SetCheeksTexture(CosmeticKey cosmeticKey)
	{
		cheeksCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			PlayerCosmeticFaceFeature prev = currentCheeks;
			PlayerCosmeticFaceFeature playerCosmeticFaceFeature = await SetFaceTexture(cosmeticKey, CurrentCheeksRuntimeKey, currentCheeks, cancellationTokenSource.Token);
			if (this == null || cancellationTokenSource.IsCancellationRequested)
			{
				return;
			}
			currentCheeks = playerCosmeticFaceFeature;
			if (prev != currentCheeks)
			{
				UpdateFaceTextures();
				CurrentCheeksRuntimeKey = ((currentCheeks != null) ? cosmeticKey : null);
			}
		}
		finally
		{
			cheeksCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		this.CheeksChanged?.Invoke();
	}

	public async UniTask SetBrowTexture(CosmeticKey cosmeticKey)
	{
		browsCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			PlayerCosmeticFaceFeature prev = currentBrows;
			PlayerCosmeticFaceFeature playerCosmeticFaceFeature = await SetFaceTexture(cosmeticKey, CurrentBrowsRuntimeKey, currentBrows, cancellationTokenSource.Token);
			if (this == null || cancellationTokenSource.IsCancellationRequested)
			{
				return;
			}
			currentBrows = playerCosmeticFaceFeature;
			if (prev != currentBrows)
			{
				UpdateFaceTextures();
				CurrentBrowsRuntimeKey = ((currentBrows != null) ? cosmeticKey : null);
			}
		}
		finally
		{
			browsCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		this.BrowsChanged?.Invoke();
	}

	public async UniTask SetMouthTexture(CosmeticKey cosmeticKey)
	{
		mouthCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			PlayerCosmeticFaceFeature prev = currentMouth;
			PlayerCosmeticFaceFeature playerCosmeticFaceFeature = await SetFaceTexture(cosmeticKey, CurrentMouthRuntimeKey, currentMouth, cancellationTokenSource.Token);
			if (this == null || cancellationTokenSource.IsCancellationRequested)
			{
				return;
			}
			currentMouth = playerCosmeticFaceFeature;
			if (prev != currentMouth)
			{
				UpdateFaceTextures();
				CurrentMouthRuntimeKey = ((currentMouth != null) ? cosmeticKey : null);
			}
		}
		finally
		{
			mouthCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		this.MouthChanged?.Invoke();
	}

	public async UniTask SetEyesTexture(CosmeticKey cosmeticKey)
	{
		eyesCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			PlayerCosmeticFaceFeature prev = currentEyes;
			PlayerCosmeticFaceFeature playerCosmeticFaceFeature = await SetFaceTexture(cosmeticKey, CurrentEyesRuntimeKey, currentEyes, cancellationTokenSource.Token);
			if (this == null || cancellationTokenSource.IsCancellationRequested)
			{
				return;
			}
			currentEyes = playerCosmeticFaceFeature;
			if (prev != currentEyes)
			{
				UpdateFaceTextures();
				CurrentEyesRuntimeKey = ((currentEyes != null) ? cosmeticKey : null);
			}
		}
		finally
		{
			eyesCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		this.EyesChanged?.Invoke();
	}

	private async UniTask<PlayerCosmeticFaceFeature> SetFaceTexture(CosmeticKey assetKey, CosmeticKey prevAssetKey, PlayerCosmeticFaceFeature current, CancellationToken cancellationToken)
	{
		if (assetKey != null && assetKey == prevAssetKey)
		{
			return current;
		}
		if (assetKey == null || assetKey.metadataKey == string.Empty)
		{
			ReleasePrev();
			return null;
		}
		AsyncOperationHandle<PlayerCosmeticsMetadata> metadata = Addressables.LoadAssetAsync<PlayerCosmeticsMetadata>(assetKey.metadataKey);
		await metadata;
		if (this == null || metadata.Result == null || !metadata.Result.model.RuntimeKeyIsValid() || !CheckOwnership(metadata.Result) || cancellationToken.IsCancellationRequested)
		{
			ReleasePrev();
			metadata.Release();
			return null;
		}
		AsyncOperationHandle<PlayerCosmeticFaceFeature> load = Addressables.LoadAssetAsync<PlayerCosmeticFaceFeature>(metadata.Result.model.RuntimeKey);
		metadata.Release();
		await load;
		if (this == null || cancellationToken.IsCancellationRequested)
		{
			Addressables.Release(load.Result);
			return null;
		}
		current = load.Result;
		return current;
		void ReleasePrev()
		{
			if (current != null)
			{
				Addressables.Release(current);
			}
		}
	}

	private void UpdateBaseTextureOverrides(LoadedCosmetic currentModel)
	{
		if (currentModel == null || currentModel.cosmetic == null)
		{
			return;
		}
		PlayerCosmeticObject.TextureOverride[] baseTextureOverrides = currentModel.cosmetic.baseTextureOverrides;
		foreach (PlayerCosmeticObject.TextureOverride textureOverride in baseTextureOverrides)
		{
			if (textureOverride.slot == PlayerCosmeticObject.TextureOverrideSlot.HeadTexture)
			{
				skinColorProps.SetTexture("_MainTex", textureOverride.texture);
				headRenderer.SetPropertyBlock(skinColorProps);
			}
		}
	}

	public async UniTask SetHeadModel(CosmeticKey assetKey)
	{
		headModelCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			LoadedCosmetic loadedCosmetic = await SetModelInternal(assetKey, CurrentHeadRuntimeKey, currentHeadModel, headRoot, cancellationTokenSource.Token);
			if (this == null || cancellationTokenSource.IsCancellationRequested)
			{
				loadedCosmetic?.Unload();
				UpdateModels();
				return;
			}
			currentHeadModel = loadedCosmetic;
			UnequipIncompatible(currentHeadModel, PlayerCosmeticObject.ModelSlot.Head);
			UpdateModels();
			CurrentHeadRuntimeKey = ((currentHeadModel != null) ? assetKey : null);
		}
		finally
		{
			headModelCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		this.HeadChanged?.Invoke();
	}

	public async UniTask SetHatModel(CosmeticKey assetKey)
	{
		hatModelCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			LoadedCosmetic loadedCosmetic = await SetModelInternal(assetKey, CurrentHatRuntimeKey, currentHatModel, headRoot, cancellationTokenSource.Token);
			if (this == null || cancellationTokenSource.IsCancellationRequested)
			{
				loadedCosmetic?.Unload();
				UpdateModels();
				return;
			}
			currentHatModel = loadedCosmetic;
			UnequipIncompatible(currentHatModel, PlayerCosmeticObject.ModelSlot.Hat);
			UpdateModels();
			CurrentHatRuntimeKey = ((currentHatModel != null) ? assetKey : null);
		}
		finally
		{
			hatModelCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		this.HatChanged?.Invoke();
	}

	public async UniTask SetFaceModel(CosmeticKey assetKey)
	{
		faceModelCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			LoadedCosmetic loadedCosmetic = await SetModelInternal(assetKey, CurrentFaceRuntimeKey, currentFaceModel, headRoot, cancellationTokenSource.Token);
			if (this == null || cancellationTokenSource.IsCancellationRequested)
			{
				loadedCosmetic?.Unload();
				UpdateModels();
				return;
			}
			currentFaceModel = loadedCosmetic;
			UnequipIncompatible(currentFaceModel, PlayerCosmeticObject.ModelSlot.Face);
			UpdateModels();
			CurrentFaceRuntimeKey = ((currentFaceModel != null) ? assetKey : null);
		}
		finally
		{
			faceModelCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		this.FaceChanged?.Invoke();
	}

	public async UniTask SetLowerFaceModel(CosmeticKey assetKey)
	{
		lowerFaceModelCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			LoadedCosmetic loadedCosmetic = await SetModelInternal(assetKey, CurrentLowerFaceRuntimeKey, currentLowerFaceModel, headRoot, cancellationTokenSource.Token);
			if (this == null || cancellationTokenSource.IsCancellationRequested)
			{
				loadedCosmetic?.Unload();
				UpdateModels();
				return;
			}
			currentLowerFaceModel = loadedCosmetic;
			UnequipIncompatible(currentLowerFaceModel, PlayerCosmeticObject.ModelSlot.FaceLower);
			UpdateModels();
			CurrentLowerFaceRuntimeKey = ((currentLowerFaceModel != null) ? assetKey : null);
		}
		finally
		{
			lowerFaceModelCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		this.LowerFaceChanged?.Invoke();
	}

	public async UniTask SetClubModel(CosmeticKey assetKey)
	{
		golfClubCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			Equipment golfClub = rightHand.GetEquipment(EquipmentType.GolfClub);
			bool flag = await golfClub.GetComponent<PlayerCosmeticsObjectSwitcher>().OverrideModel((assetKey != null) ? assetKey.metadataKey : string.Empty, this, cancellationTokenSource.Token);
			if (!(this == null) && !cancellationTokenSource.IsCancellationRequested)
			{
				CurrentClubRuntimeKey = (flag ? assetKey : null);
				this.ClubChanged?.Invoke();
				if (playerMovement != null)
				{
					golfClub.gameObject.SetPlayerShaderIndexOnRenderers(playerMovement.PlayerInfo);
				}
			}
		}
		finally
		{
			golfClubCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
	}

	public async UniTask SetGolfBallModel(CosmeticKey assetKey)
	{
		golfBallCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			bool flag = false;
			if (golfBall == null && !isCharacterPreview && TryGetComponent<PlayerGolfer>(out var component) && component.OwnBall != null)
			{
				golfBall = component.OwnBall.GetComponent<PlayerCosmeticsObjectSwitcher>();
			}
			string text = ((assetKey != null) ? assetKey.metadataKey : string.Empty);
			if (golfBall != null)
			{
				flag = await golfBall.OverrideModel(text, this, cancellationTokenSource.Token);
				if (this == null || cancellationTokenSource.IsCancellationRequested)
				{
					return;
				}
			}
			else if (text != string.Empty)
			{
				AsyncOperationHandle<PlayerCosmeticsMetadata> metadata = Addressables.LoadAssetAsync<PlayerCosmeticsMetadata>(text);
				await metadata;
				if (this == null || cancellationTokenSource.IsCancellationRequested)
				{
					metadata.Release();
					return;
				}
				flag = metadata.Result != null;
				if (!isCharacterPreview && !CheckOwnership(metadata.Result))
				{
					flag = false;
				}
				metadata.Release();
			}
			UpdateRuntimeKey(flag ? assetKey : null);
		}
		finally
		{
			golfBallCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		void UpdateRuntimeKey(CosmeticKey currentGolfBallRuntimeKey)
		{
			CurrentGolfBallRuntimeKey = currentGolfBallRuntimeKey;
			this.GolfBallChanged?.Invoke();
		}
	}

	public async UniTask SetBodyModel(CosmeticKey assetKey)
	{
		if (assetKey == null || assetKey.metadataKey == string.Empty)
		{
			await ReleasePrev(null, resetAssetKey: true, callback: true);
			return;
		}
		bodyModelCancellationToken.GetLocalCancellationToken(out var cancellationTokenSource);
		try
		{
			AsyncOperationHandle<PlayerCosmeticsMetadata> metadata = Addressables.LoadAssetAsync<PlayerCosmeticsMetadata>(assetKey.metadataKey);
			await metadata;
			if (this == null || metadata.Result == null || cancellationTokenSource.IsCancellationRequested)
			{
				await ReleasePrev(metadata.Result, resetAssetKey: true, !cancellationTokenSource.IsCancellationRequested);
				metadata.Release();
				return;
			}
			if (!metadata.Result.model.RuntimeKeyIsValid())
			{
				await ReleasePrev(metadata.Result, resetAssetKey: false, callback: true);
				metadata.Release();
				return;
			}
			if (!CheckOwnership(metadata.Result))
			{
				await ReleasePrev(null, resetAssetKey: true, callback: true);
				metadata.Release();
				return;
			}
			AsyncOperationHandle<GameObject> load = Addressables.LoadAssetAsync<GameObject>(metadata.Result.model.RuntimeKey);
			await load;
			if (this == null || load.Result == null)
			{
				await ReleasePrev(null, resetAssetKey: true, callback: false);
				metadata.Release();
				return;
			}
			if (!load.Result.TryGetComponent<SkinnedMeshRenderer>(out var modelRenderer) || cancellationTokenSource.IsCancellationRequested)
			{
				Addressables.Release(load.Result);
				Addressables.Release(metadata.Result);
				return;
			}
			currentBodyAsset = load.Result;
			await UpdateTextureAndColor(metadata.Result, cancellationTokenSource.Token);
			Addressables.Release(metadata.Result);
			if (this == null || cancellationTokenSource.IsCancellationRequested)
			{
				Addressables.Release(currentBodyAsset);
				return;
			}
			bodyRenderer.sharedMesh = modelRenderer.sharedMesh;
			bodyRenderer.sharedMaterial = modelRenderer.sharedMaterial;
			CurrentBodyRuntimeKey = assetKey;
		}
		finally
		{
			bodyModelCancellationToken.DisposeCancellationToken(cancellationTokenSource);
		}
		this.BodyChanged?.Invoke();
		async UniTask ReleasePrev(PlayerCosmeticsMetadata metadata2, bool resetAssetKey, bool callback)
		{
			if (currentBodyAsset != null)
			{
				Addressables.Release(currentBodyAsset);
				currentBodyAsset = null;
			}
			bodyRenderer.sharedMesh = bodyDefaultMesh;
			bodyRenderer.sharedMaterial = bodyDefaultMaterial;
			CosmeticKey prevAssetKey = CurrentBodyRuntimeKey;
			if (resetAssetKey)
			{
				assetKey = null;
			}
			await UpdateTextureAndColor(metadata2, default(CancellationToken));
			CurrentBodyRuntimeKey = assetKey;
			if (callback && CurrentBodyRuntimeKey != prevAssetKey)
			{
				this.BodyChanged?.Invoke();
			}
		}
		async UniTask UpdateTextureAndColor(PlayerCosmeticsMetadata metadata2, CancellationToken cancellationToken)
		{
			LoadedTextureOverride loadedTextureOverride = await SetTextureOverride(bodyRenderer, bodyTextureOverride, assetKey, CurrentBodyRuntimeKey, metadata2, skinColorProps, useTint: false, cancellationToken);
			bodyTextureOverride = loadedTextureOverride;
			if (!(this == null) && !cancellationToken.IsCancellationRequested)
			{
				SetSkinColor(CurrentSkinColorIndex);
			}
		}
	}

	private async UniTask<LoadedCosmetic> SetModelInternal(CosmeticKey assetKey, CosmeticKey prevAssetKey, LoadedCosmetic currentModel, Transform root, CancellationToken cancellationToken)
	{
		if (assetKey == null || assetKey.metadataKey == null || assetKey.metadataKey == string.Empty)
		{
			currentModel?.Unload();
			return null;
		}
		AsyncOperationHandle<PlayerCosmeticsMetadata> metadataLoad = Addressables.LoadAssetAsync<PlayerCosmeticsMetadata>(assetKey.metadataKey);
		await metadataLoad;
		if (this == null || metadataLoad.Result == null || !metadataLoad.Result.model.RuntimeKeyIsValid() || !CheckOwnership(metadataLoad.Result) || cancellationToken.IsCancellationRequested)
		{
			Debug.Log("Cancel before load");
			currentModel?.Unload();
			metadataLoad.Release();
			return null;
		}
		PlayerCosmeticsMetadata metadata = metadataLoad.Result;
		LoadedCosmetic prevModel = currentModel;
		if (assetKey.metadataKey != prevAssetKey?.metadataKey)
		{
			AsyncOperationHandle<GameObject> instantiate = Addressables.InstantiateAsync(metadata.model.RuntimeKey, Vector3.one * -1000f, Quaternion.identity);
			await instantiate;
			if (this == null || cancellationToken.IsCancellationRequested || instantiate.Result == null)
			{
				if (instantiate.Result == null)
				{
					Debug.LogError("Failed to instantiate model!!!");
				}
				metadataLoad.Release();
				instantiate.Release();
				return null;
			}
			instantiate.Result.SetActive(value: false);
			currentModel = new LoadedCosmetic();
			if (!instantiate.Result.TryGetComponent<PlayerCosmeticObject>(out currentModel.cosmetic) || cancellationToken.IsCancellationRequested)
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					Debug.LogError("Instantiated non cosmetic object, this is illegal!!!");
				}
				Addressables.ReleaseInstance(instantiate.Result);
				metadataLoad.Release();
				currentModel.Unload();
				return null;
			}
			currentModel.cosmetic.transform.localPosition = Vector3.zero;
			currentModel.cosmetic.transform.localRotation = Quaternion.identity;
		}
		if (currentModel.cosmetic != null)
		{
			List<Material> value;
			using (CollectionPool<List<Material>, Material>.Get(out value))
			{
				Renderer componentInChildren = currentModel.cosmetic.GetComponentInChildren<Renderer>();
				UpdateModelVisibility(currentModel);
				matProps.Clear();
				bool useTint = currentModel.cosmetic == null || !currentModel.cosmetic.requireSkinColorTint;
				LoadedCosmetic loadedCosmetic = currentModel;
				loadedCosmetic.textureOverride = await SetTextureOverride(componentInChildren, currentModel.textureOverride, assetKey, prevAssetKey, metadata, matProps, useTint, cancellationToken);
			}
		}
		metadataLoad.Release();
		if (prevModel != currentModel)
		{
			prevModel?.Unload();
		}
		if (this == null || cancellationToken.IsCancellationRequested)
		{
			currentModel.Unload();
			return null;
		}
		if (currentModel.cosmetic != null)
		{
			currentModel.cosmetic.transform.SetParent(root, worldPositionStays: false);
			currentModel.cosmetic.gameObject.SetActive(value: true);
		}
		currentModel.cosmetic.gameObject.SetLayerRecursively(GameManager.LayerSettings.PlayerLayer);
		return currentModel;
	}

	private async UniTask<LoadedTextureOverride> SetTextureOverride(Renderer renderer, LoadedTextureOverride texture, CosmeticKey assetKey, CosmeticKey prevAssetKey, PlayerCosmeticsMetadata metadata, MaterialPropertyBlock matProps, bool useTint = true, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (metadata == null || metadata.variations == null)
		{
			texture?.UnloadTexture();
			texture = null;
			matProps.Clear();
			if (playerMovement != null)
			{
				matProps.SetPlayerIndex(playerMovement.PlayerInfo);
			}
			renderer.SetPropertyBlock(matProps);
			return null;
		}
		PlayerCosmeticsMetadata.Variation variation;
		if (assetKey != null && (assetKey.metadataKey != (prevAssetKey?.metadataKey ?? string.Empty) || assetKey.variationIndex != (((int?)prevAssetKey?.variationIndex) ?? int.MinValue)))
		{
			texture?.UnloadTexture();
			variation = ((metadata.variations.Length != 0) ? metadata.variations[assetKey.variationIndex] : PlayerCosmeticsMetadata.NoVariation);
			if (variation.textureOverride != null && variation.textureOverride.RuntimeKeyIsValid())
			{
				AsyncOperationHandle<Texture2D> textureLoad = Addressables.LoadAssetAsync<Texture2D>(variation.textureOverride.RuntimeKey);
				await textureLoad;
				if (this == null || cancellationToken.IsCancellationRequested)
				{
					Addressables.Release(textureLoad);
					return null;
				}
				texture = textureLoad.Result;
			}
		}
		else
		{
			variation = PlayerCosmeticsMetadata.NoVariation;
		}
		if (useTint && variation != PlayerCosmeticsMetadata.NoVariation)
		{
			Color tintColor = variation.tintColor;
			Color emissiveColor = variation.emissiveColor;
			tintColor.a = (emissiveColor.a = 1f);
			matProps.SetColor("_Color", tintColor);
			matProps.SetColor("_EmissionColor", emissiveColor);
		}
		if (texture != null && texture.loadedTexture != null)
		{
			matProps.SetTexture("_MainTex", texture.loadedTexture);
		}
		if (assetKey != null && variation.materialIndex >= 0)
		{
			renderer.SetPropertyBlock(matProps, variation.materialIndex);
		}
		else
		{
			renderer.SetPropertyBlock(matProps);
		}
		if (playerMovement != null)
		{
			renderer.gameObject.SetPlayerShaderIndexOnRenderers(playerMovement.PlayerInfo);
		}
		return texture;
	}

	private void GetMasks(out PlayerCosmeticObject.ModelSlot modelDisable, out PlayerCosmeticObject.ModelSlot allowedCosmetics)
	{
		PlayerCosmeticObject.ModelSlot modelDisableLocal = PlayerCosmeticObject.ModelSlot.None;
		PlayerCosmeticObject.ModelSlot allowedCosmeticsLocal = (PlayerCosmeticObject.ModelSlot)(-1);
		UpdateMasks(currentHeadModel);
		UpdateMasks(currentHatModel);
		UpdateMasks(currentFaceModel);
		UpdateMasks(currentLowerFaceModel);
		modelDisable = modelDisableLocal;
		allowedCosmetics = allowedCosmeticsLocal;
		void UpdateMasks(LoadedCosmetic model)
		{
			if (model != null && !(model.cosmetic == null))
			{
				modelDisableLocal |= model.cosmetic.modelDisable;
				allowedCosmeticsLocal &= model.cosmetic.allowedCosmetics;
			}
		}
	}

	public PlayerCosmeticObject.ModelSlot GetSlotsWillBeUnequipped(PlayerCosmeticObject.ModelSlot allowedCosmetics, PlayerCosmeticObject.ModelSlot slot)
	{
		PlayerCosmeticObject.ModelSlot modelSlot = PlayerCosmeticObject.ModelSlot.None;
		if (slot != PlayerCosmeticObject.ModelSlot.Head && currentHeadModel != null && currentHeadModel.cosmetic != null && (!allowedCosmetics.HasFlag(PlayerCosmeticObject.ModelSlot.Head) || !currentHeadModel.cosmetic.allowedCosmetics.HasFlag(slot)))
		{
			modelSlot |= PlayerCosmeticObject.ModelSlot.Head;
		}
		if (slot != PlayerCosmeticObject.ModelSlot.Hat && currentHatModel != null && currentHatModel.cosmetic != null && (!allowedCosmetics.HasFlag(PlayerCosmeticObject.ModelSlot.Hat) || !currentHatModel.cosmetic.allowedCosmetics.HasFlag(slot)))
		{
			modelSlot |= PlayerCosmeticObject.ModelSlot.Hat;
		}
		if (slot != PlayerCosmeticObject.ModelSlot.Face && currentFaceModel != null && currentFaceModel.cosmetic != null && (!allowedCosmetics.HasFlag(PlayerCosmeticObject.ModelSlot.Face) || !currentFaceModel.cosmetic.allowedCosmetics.HasFlag(slot)))
		{
			modelSlot |= PlayerCosmeticObject.ModelSlot.Face;
		}
		if (slot != PlayerCosmeticObject.ModelSlot.FaceLower && currentLowerFaceModel != null && currentLowerFaceModel.cosmetic != null && (!allowedCosmetics.HasFlag(PlayerCosmeticObject.ModelSlot.FaceLower) || !currentLowerFaceModel.cosmetic.allowedCosmetics.HasFlag(slot)))
		{
			modelSlot |= PlayerCosmeticObject.ModelSlot.FaceLower;
		}
		return modelSlot;
	}

	private void UnequipIncompatible(LoadedCosmetic equipped, PlayerCosmeticObject.ModelSlot slot)
	{
		if (equipped != null && !(equipped.cosmetic == null))
		{
			PlayerCosmeticObject.ModelSlot slotsWillBeUnequipped = GetSlotsWillBeUnequipped(equipped.cosmetic.allowedCosmetics, slot);
			if (currentHeadModel != null && slotsWillBeUnequipped.HasFlag(PlayerCosmeticObject.ModelSlot.Head))
			{
				currentHeadModel.Unload();
				CurrentHeadRuntimeKey = null;
				this.HeadChanged?.Invoke();
			}
			if (currentHatModel != null && slotsWillBeUnequipped.HasFlag(PlayerCosmeticObject.ModelSlot.Hat))
			{
				currentHatModel.Unload();
				CurrentHatRuntimeKey = null;
				this.HatChanged?.Invoke();
			}
			if (currentFaceModel != null && slotsWillBeUnequipped.HasFlag(PlayerCosmeticObject.ModelSlot.Face))
			{
				currentFaceModel.Unload();
				CurrentFaceRuntimeKey = null;
				this.FaceChanged?.Invoke();
			}
			if (currentLowerFaceModel != null && slotsWillBeUnequipped.HasFlag(PlayerCosmeticObject.ModelSlot.FaceLower))
			{
				currentLowerFaceModel.Unload();
				CurrentLowerFaceRuntimeKey = null;
				this.LowerFaceChanged?.Invoke();
			}
		}
	}

	private void UpdateModels()
	{
		GetMasks(out var modelDisable, out var _);
		bool flag = !modelDisable.HasFlag(PlayerCosmeticObject.ModelSlot.Head);
		bool flag2 = !modelDisable.HasFlag(PlayerCosmeticObject.ModelSlot.Ears);
		bool flag3 = !modelDisable.HasFlag(PlayerCosmeticObject.ModelSlot.Face);
		headRenderer.sharedMesh = (flag2 ? headMesh : headNoEarsMesh);
		headRenderer.forceRenderingOff = !flag;
		mouthRenderer.forceRenderingOff = !flag3;
		SetSkinColor(CurrentSkinColorIndex);
	}

	public bool CheckOwnership(PlayerCosmeticsMetadata metadata)
	{
		if (playerMovement == null || !playerMovement.isLocalPlayer)
		{
			return true;
		}
		return CosmeticsUnlocksManager.OwnsCosmetic(metadata);
	}
}
