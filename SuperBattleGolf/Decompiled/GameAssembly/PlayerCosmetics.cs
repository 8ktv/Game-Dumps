using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Threading.Tasks;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

public class PlayerCosmetics : NetworkBehaviour
{
	[Serializable]
	public struct SaveData
	{
		public int equippedLoadout;

		public Loadout[] loadout;
	}

	[Serializable]
	public struct Loadout
	{
		public PlayerCosmeticsSwitcher.CosmeticKey head;

		public PlayerCosmeticsSwitcher.CosmeticKey hat;

		public PlayerCosmeticsSwitcher.CosmeticKey upperFace;

		public PlayerCosmeticsSwitcher.CosmeticKey lowerFace;

		public PlayerCosmeticsSwitcher.CosmeticKey body;

		public PlayerCosmeticsSwitcher.CosmeticKey club;

		public PlayerCosmeticsSwitcher.CosmeticKey mouth;

		public PlayerCosmeticsSwitcher.CosmeticKey eyes;

		public PlayerCosmeticsSwitcher.CosmeticKey cheeks;

		public PlayerCosmeticsSwitcher.CosmeticKey brows;

		public PlayerCosmeticsSwitcher.CosmeticKey golfBall;

		public int skinColorIndex;

		public VictoryDance victoryDance;

		public Loadout(PlayerCosmeticsSwitcher cosmetics, VictoryDance victoryDance)
		{
			head = cosmetics.CurrentHeadRuntimeKey;
			hat = cosmetics.CurrentHatRuntimeKey;
			upperFace = cosmetics.CurrentFaceRuntimeKey;
			lowerFace = cosmetics.CurrentLowerFaceRuntimeKey;
			body = cosmetics.CurrentBodyRuntimeKey;
			club = cosmetics.CurrentClubRuntimeKey;
			mouth = cosmetics.CurrentMouthRuntimeKey;
			eyes = cosmetics.CurrentEyesRuntimeKey;
			cheeks = cosmetics.CurrentCheeksRuntimeKey;
			brows = cosmetics.CurrentBrowsRuntimeKey;
			golfBall = cosmetics.CurrentGolfBallRuntimeKey;
			this.victoryDance = victoryDance;
			skinColorIndex = cosmetics.CurrentSkinColorIndex;
		}

		public async UniTask Apply(PlayerCosmeticsSwitcher switcher)
		{
			List<UniTask> value;
			using (CollectionPool<List<UniTask>, UniTask>.Get(out value))
			{
				value.Add(switcher.SetHeadModel(head));
				value.Add(switcher.SetHatModel(hat));
				value.Add(switcher.SetFaceModel(upperFace));
				value.Add(switcher.SetLowerFaceModel(lowerFace));
				value.Add(switcher.SetBodyModel(body));
				value.Add(switcher.SetClubModel(club));
				value.Add(switcher.SetEyesTexture(eyes));
				value.Add(switcher.SetMouthTexture(mouth));
				value.Add(switcher.SetCheeksTexture(cheeks));
				value.Add(switcher.SetBrowTexture(brows));
				value.Add(switcher.SetGolfBallModel(golfBall));
				switcher.SetSkinColor(skinColorIndex);
				await UniTask.WhenAll(value);
			}
		}

		public VictoryDance GetVictoryDance()
		{
			if (!Enum.IsDefined(typeof(VictoryDance), victoryDance) || victoryDance == VictoryDance.None)
			{
				return VictoryDance.L;
			}
			return victoryDance;
		}
	}

	private const string COSMETICS_FILENAME = "Cosmetics.json";

	private const int LOADOUT_SLOTS = 4;

	public AssetReferenceT<PlayerCosmeticsMetadata> defaultHair;

	public AssetReferenceT<PlayerCosmeticsMetadata> defaultHat;

	private PlayerInfo playerInfo;

	private SaveData saveData;

	[SyncVar(hook = "OnHeadCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey headCosmeticKey;

	[SyncVar(hook = "OnHatCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey hatCosmeticKey;

	[SyncVar(hook = "OnFaceCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey faceCosmeticKey;

	[SyncVar(hook = "OnLowerFaceCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey lowerFaceCosmeticKey;

	[SyncVar(hook = "OnClubCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey clubCosmeticKey;

	[SyncVar(hook = "OnBodyCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey bodyCosmeticKey;

	[SyncVar(hook = "OnMouthCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey mouthCosmeticKey;

	[SyncVar(hook = "OnEyesCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey eyesCosmeticKey;

	[SyncVar(hook = "OnCheeksCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey cheeksCosmeticKey;

	[SyncVar(hook = "OnBrowsCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey browsCosmeticKey;

	[SyncVar(hook = "OnGolfBallCosmeticChanged")]
	private PlayerCosmeticsSwitcher.CosmeticKey golfBallCosmeticKey;

	[SyncVar(hook = "OnAreSpringBootsEnabledChanged")]
	private bool areSpringBootsEnabled;

	[SyncVar(hook = "OnSkinColorIndexChanged")]
	private int skinColorIndex;

	[SyncVar(hook = "OnVictoryDanceChanged")]
	public VictoryDance victoryDance;

	private PlayerCosmeticsSwitcher switcher;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_headCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_hatCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_faceCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_lowerFaceCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_clubCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_bodyCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_mouthCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_eyesCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_cheeksCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_browsCosmeticKey;

	public Action<PlayerCosmeticsSwitcher.CosmeticKey, PlayerCosmeticsSwitcher.CosmeticKey> _Mirror_SyncVarHookDelegate_golfBallCosmeticKey;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_areSpringBootsEnabled;

	public Action<int, int> _Mirror_SyncVarHookDelegate_skinColorIndex;

	public Action<VictoryDance, VictoryDance> _Mirror_SyncVarHookDelegate_victoryDance;

	public PlayerCosmeticsSwitcher Switcher => switcher;

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkheadCosmeticKey
	{
		get
		{
			return headCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref headCosmeticKey, 1uL, _Mirror_SyncVarHookDelegate_headCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkhatCosmeticKey
	{
		get
		{
			return hatCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref hatCosmeticKey, 2uL, _Mirror_SyncVarHookDelegate_hatCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkfaceCosmeticKey
	{
		get
		{
			return faceCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref faceCosmeticKey, 4uL, _Mirror_SyncVarHookDelegate_faceCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworklowerFaceCosmeticKey
	{
		get
		{
			return lowerFaceCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref lowerFaceCosmeticKey, 8uL, _Mirror_SyncVarHookDelegate_lowerFaceCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkclubCosmeticKey
	{
		get
		{
			return clubCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref clubCosmeticKey, 16uL, _Mirror_SyncVarHookDelegate_clubCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkbodyCosmeticKey
	{
		get
		{
			return bodyCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref bodyCosmeticKey, 32uL, _Mirror_SyncVarHookDelegate_bodyCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkmouthCosmeticKey
	{
		get
		{
			return mouthCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref mouthCosmeticKey, 64uL, _Mirror_SyncVarHookDelegate_mouthCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkeyesCosmeticKey
	{
		get
		{
			return eyesCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref eyesCosmeticKey, 128uL, _Mirror_SyncVarHookDelegate_eyesCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkcheeksCosmeticKey
	{
		get
		{
			return cheeksCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref cheeksCosmeticKey, 256uL, _Mirror_SyncVarHookDelegate_cheeksCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkbrowsCosmeticKey
	{
		get
		{
			return browsCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref browsCosmeticKey, 512uL, _Mirror_SyncVarHookDelegate_browsCosmeticKey);
		}
	}

	public PlayerCosmeticsSwitcher.CosmeticKey NetworkgolfBallCosmeticKey
	{
		get
		{
			return golfBallCosmeticKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref golfBallCosmeticKey, 1024uL, _Mirror_SyncVarHookDelegate_golfBallCosmeticKey);
		}
	}

	public bool NetworkareSpringBootsEnabled
	{
		get
		{
			return areSpringBootsEnabled;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref areSpringBootsEnabled, 2048uL, _Mirror_SyncVarHookDelegate_areSpringBootsEnabled);
		}
	}

	public int NetworkskinColorIndex
	{
		get
		{
			return skinColorIndex;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref skinColorIndex, 4096uL, _Mirror_SyncVarHookDelegate_skinColorIndex);
		}
	}

	public VictoryDance NetworkvictoryDance
	{
		get
		{
			return victoryDance;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref victoryDance, 8192uL, _Mirror_SyncVarHookDelegate_victoryDance);
		}
	}

	public static event Action<PlayerCosmetics> ClubCosmeticUpdated;

	private void Awake()
	{
		playerInfo = GetComponent<PlayerInfo>();
		switcher = GetComponent<PlayerCosmeticsSwitcher>();
		switcher.ClubChanged += OnClubChanged;
		syncDirection = SyncDirection.ClientToServer;
	}

	private void OnClubChanged()
	{
		PlayerCosmetics.ClubCosmeticUpdated?.Invoke(this);
	}

	public override void OnStartLocalPlayer()
	{
		switcher.HeadChanged += delegate
		{
			NetworkheadCosmeticKey = switcher.CurrentHeadRuntimeKey;
		};
		switcher.HatChanged += delegate
		{
			NetworkhatCosmeticKey = switcher.CurrentHatRuntimeKey;
		};
		switcher.FaceChanged += delegate
		{
			NetworkfaceCosmeticKey = switcher.CurrentFaceRuntimeKey;
		};
		switcher.LowerFaceChanged += delegate
		{
			NetworklowerFaceCosmeticKey = switcher.CurrentLowerFaceRuntimeKey;
		};
		switcher.ClubChanged += delegate
		{
			NetworkclubCosmeticKey = switcher.CurrentClubRuntimeKey;
		};
		switcher.BodyChanged += delegate
		{
			NetworkbodyCosmeticKey = switcher.CurrentBodyRuntimeKey;
		};
		switcher.MouthChanged += delegate
		{
			NetworkmouthCosmeticKey = switcher.CurrentMouthRuntimeKey;
		};
		switcher.EyesChanged += delegate
		{
			NetworkeyesCosmeticKey = switcher.CurrentEyesRuntimeKey;
		};
		switcher.BrowsChanged += delegate
		{
			NetworkbrowsCosmeticKey = switcher.CurrentBrowsRuntimeKey;
		};
		switcher.CheeksChanged += delegate
		{
			NetworkcheeksCosmeticKey = switcher.CurrentCheeksRuntimeKey;
		};
		switcher.SkinColorChanged += delegate
		{
			NetworkskinColorIndex = switcher.CurrentSkinColorIndex;
		};
		switcher.GolfBallChanged += delegate
		{
			NetworkgolfBallCosmeticKey = switcher.CurrentGolfBallRuntimeKey;
		};
		if (!LoadCosmetics())
		{
			saveData = new SaveData
			{
				equippedLoadout = 0,
				loadout = new Loadout[4]
			};
			Loadout loadout = new Loadout
			{
				head = new PlayerCosmeticsSwitcher.CosmeticKey
				{
					metadataKey = (string)defaultHair.RuntimeKey
				},
				hat = new PlayerCosmeticsSwitcher.CosmeticKey
				{
					metadataKey = (string)defaultHat.RuntimeKey
				}
			};
			for (int num = 0; num < 4; num++)
			{
				saveData.loadout[num] = loadout;
			}
			EquipLoadout(0, equipDefaultsOnFail: true).Forget();
		}
	}

	private void SetDefaultHead(int loadoutIndex)
	{
		Loadout loadout = saveData.loadout[loadoutIndex];
		loadout.head = new PlayerCosmeticsSwitcher.CosmeticKey
		{
			metadataKey = (string)defaultHair.RuntimeKey
		};
		saveData.loadout[loadoutIndex] = loadout;
		switcher.SetHeadModel(new PlayerCosmeticsSwitcher.CosmeticKey
		{
			metadataKey = (string)defaultHair.RuntimeKey
		}).Forget();
	}

	private void SetDefaultHat(int loadoutIndex)
	{
		Loadout loadout = saveData.loadout[loadoutIndex];
		loadout.hat = new PlayerCosmeticsSwitcher.CosmeticKey
		{
			metadataKey = (string)defaultHat.RuntimeKey
		};
		saveData.loadout[loadoutIndex] = loadout;
		switcher.SetHatModel(new PlayerCosmeticsSwitcher.CosmeticKey
		{
			metadataKey = (string)defaultHat.RuntimeKey
		}).Forget();
	}

	public bool LoadCosmetics()
	{
		try
		{
			string text = null;
			if (!SteamRemoteStorage.FileExists("Cosmetics.json"))
			{
				return false;
			}
			byte[] bytes = SteamRemoteStorage.FileRead("Cosmetics.json");
			text = Encoding.UTF8.GetString(bytes);
			saveData = JsonUtility.FromJson<SaveData>(text);
			if (saveData.loadout == null)
			{
				return false;
			}
			if (saveData.loadout.Length != 4)
			{
				Array.Resize(ref saveData.loadout, 4);
			}
			saveData.equippedLoadout = BMath.Clamp(saveData.equippedLoadout, 0, 3);
			EquipLoadout(saveData.equippedLoadout, equipDefaultsOnFail: true).Forget();
			return true;
		}
		catch (Exception exception)
		{
			Debug.LogError("Caught exception while loading player cosmetics!");
			Debug.LogException(exception);
			return false;
		}
	}

	public async UniTask EquipLoadout(int index, bool equipDefaultsOnFail)
	{
		Loadout equipped = saveData.loadout[index];
		UniTask task = equipped.Apply(switcher);
		if (playerInfo.AsGolfer.OwnBall == null)
		{
			NetworkgolfBallCosmeticKey = equipped.golfBall;
		}
		if (equipDefaultsOnFail)
		{
			task = task.ContinueWith(delegate
			{
				if (!EquipSuccess(equipped.hat, switcher.CurrentHatRuntimeKey))
				{
					SetDefaultHat(index);
				}
				if (!EquipSuccess(equipped.head, switcher.CurrentHeadRuntimeKey))
				{
					SetDefaultHead(index);
				}
			});
		}
		NetworkvictoryDance = equipped.GetVictoryDance();
		saveData.equippedLoadout = index;
		await task;
		saveData.loadout[index] = new Loadout(switcher, victoryDance);
		static bool EquipSuccess(PlayerCosmeticsSwitcher.CosmeticKey equip, PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey)
		{
			if (equip == null || string.IsNullOrEmpty(equip.metadataKey))
			{
				return true;
			}
			if (cosmeticKey != null && !string.IsNullOrEmpty(cosmeticKey.metadataKey))
			{
				return equip.metadataKey == cosmeticKey.metadataKey;
			}
			return false;
		}
	}

	public async UniTask SetLoadout(PlayerCosmeticsSwitcher switcher, VictoryDance victoryDance, int loadoutIndex, bool equip, bool save)
	{
		saveData.loadout[loadoutIndex] = new Loadout(switcher, victoryDance);
		if (equip)
		{
			await EquipLoadout(loadoutIndex, equipDefaultsOnFail: false);
		}
		if (save)
		{
			SaveCosmetics();
		}
	}

	public Loadout GetLoadout(int index = -1)
	{
		if (index < 0)
		{
			index = saveData.equippedLoadout;
		}
		return saveData.loadout[index];
	}

	public int GetEquippedLoadoutIndex()
	{
		return saveData.equippedLoadout;
	}

	public void SaveCosmetics()
	{
		try
		{
			string s = JsonUtility.ToJson(saveData, prettyPrint: true);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			SteamRemoteStorage.FileWrite("Cosmetics.json", bytes);
		}
		catch (Exception exception)
		{
			Debug.LogError("Caught exception while saving player cosmetics!");
			Debug.LogException(exception);
		}
	}

	public void UpdateSpringBootsEnabled()
	{
		NetworkareSpringBootsEnabled = playerInfo.Inventory.IsUsingSpringBoots || playerInfo.Inventory.GetEffectivelyEquippedItem() == ItemType.SpringBoots;
	}

	private void OnSkinColorIndexChanged(int prev, int curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetSkinColor(curr);
		}
	}

	private void OnHeadCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetHeadModel(curr).Forget();
		}
	}

	private void OnClubCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetClubModel(curr).Forget();
		}
	}

	private void OnFaceCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetFaceModel(curr).Forget();
		}
	}

	private void OnLowerFaceCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetLowerFaceModel(curr).Forget();
		}
	}

	private void OnHatCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetHatModel(curr).Forget();
		}
	}

	private void OnBodyCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetBodyModel(curr).Forget();
		}
	}

	private void OnEyesCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetEyesTexture(curr).Forget();
		}
	}

	private void OnMouthCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetMouthTexture(curr).Forget();
		}
	}

	private void OnCheeksCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetCheeksTexture(curr).Forget();
		}
	}

	private void OnBrowsCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			switcher.SetBrowTexture(curr).Forget();
		}
	}

	private void OnGolfBallCosmeticChanged(PlayerCosmeticsSwitcher.CosmeticKey prev, PlayerCosmeticsSwitcher.CosmeticKey curr)
	{
		if (!base.isLocalPlayer)
		{
			UpdateGolfBallCosmetic(allowUnequip: true);
		}
	}

	public void UpdateGolfBallCosmetic(bool allowUnequip)
	{
		if (allowUnequip || (golfBallCosmeticKey != null && !string.IsNullOrEmpty(golfBallCosmeticKey.metadataKey)))
		{
			switcher.SetGolfBallModel(golfBallCosmeticKey).Forget();
		}
	}

	private void OnAreSpringBootsEnabledChanged(bool wereEnabled, bool areEnabled)
	{
		playerInfo.Shoes.SetActive(!areSpringBootsEnabled);
		playerInfo.SpringBoots.SetActive(areSpringBootsEnabled);
	}

	private void OnVictoryDanceChanged(VictoryDance prev, VictoryDance curr)
	{
		if (base.isLocalPlayer && curr != VictoryDance.L && !CosmeticsUnlocksManager.OwnsDance(curr))
		{
			NetworkvictoryDance = VictoryDance.L;
		}
	}

	public PlayerCosmetics()
	{
		_Mirror_SyncVarHookDelegate_headCosmeticKey = OnHeadCosmeticChanged;
		_Mirror_SyncVarHookDelegate_hatCosmeticKey = OnHatCosmeticChanged;
		_Mirror_SyncVarHookDelegate_faceCosmeticKey = OnFaceCosmeticChanged;
		_Mirror_SyncVarHookDelegate_lowerFaceCosmeticKey = OnLowerFaceCosmeticChanged;
		_Mirror_SyncVarHookDelegate_clubCosmeticKey = OnClubCosmeticChanged;
		_Mirror_SyncVarHookDelegate_bodyCosmeticKey = OnBodyCosmeticChanged;
		_Mirror_SyncVarHookDelegate_mouthCosmeticKey = OnMouthCosmeticChanged;
		_Mirror_SyncVarHookDelegate_eyesCosmeticKey = OnEyesCosmeticChanged;
		_Mirror_SyncVarHookDelegate_cheeksCosmeticKey = OnCheeksCosmeticChanged;
		_Mirror_SyncVarHookDelegate_browsCosmeticKey = OnBrowsCosmeticChanged;
		_Mirror_SyncVarHookDelegate_golfBallCosmeticKey = OnGolfBallCosmeticChanged;
		_Mirror_SyncVarHookDelegate_areSpringBootsEnabled = OnAreSpringBootsEnabledChanged;
		_Mirror_SyncVarHookDelegate_skinColorIndex = OnSkinColorIndexChanged;
		_Mirror_SyncVarHookDelegate_victoryDance = OnVictoryDanceChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, headCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, hatCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, faceCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, lowerFaceCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, clubCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, bodyCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, mouthCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, eyesCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, cheeksCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, browsCosmeticKey);
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, golfBallCosmeticKey);
			writer.WriteBool(areSpringBootsEnabled);
			writer.WriteVarInt(skinColorIndex);
			GeneratedNetworkCode._Write_VictoryDance(writer, victoryDance);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, headCosmeticKey);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, hatCosmeticKey);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, faceCosmeticKey);
		}
		if ((syncVarDirtyBits & 8L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, lowerFaceCosmeticKey);
		}
		if ((syncVarDirtyBits & 0x10L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, clubCosmeticKey);
		}
		if ((syncVarDirtyBits & 0x20L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, bodyCosmeticKey);
		}
		if ((syncVarDirtyBits & 0x40L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, mouthCosmeticKey);
		}
		if ((syncVarDirtyBits & 0x80L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, eyesCosmeticKey);
		}
		if ((syncVarDirtyBits & 0x100L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, cheeksCosmeticKey);
		}
		if ((syncVarDirtyBits & 0x200L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, browsCosmeticKey);
		}
		if ((syncVarDirtyBits & 0x400L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCosmeticsSwitcher_002FCosmeticKey(writer, golfBallCosmeticKey);
		}
		if ((syncVarDirtyBits & 0x800L) != 0L)
		{
			writer.WriteBool(areSpringBootsEnabled);
		}
		if ((syncVarDirtyBits & 0x1000L) != 0L)
		{
			writer.WriteVarInt(skinColorIndex);
		}
		if ((syncVarDirtyBits & 0x2000L) != 0L)
		{
			GeneratedNetworkCode._Write_VictoryDance(writer, victoryDance);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref headCosmeticKey, _Mirror_SyncVarHookDelegate_headCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref hatCosmeticKey, _Mirror_SyncVarHookDelegate_hatCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref faceCosmeticKey, _Mirror_SyncVarHookDelegate_faceCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref lowerFaceCosmeticKey, _Mirror_SyncVarHookDelegate_lowerFaceCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref clubCosmeticKey, _Mirror_SyncVarHookDelegate_clubCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref bodyCosmeticKey, _Mirror_SyncVarHookDelegate_bodyCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref mouthCosmeticKey, _Mirror_SyncVarHookDelegate_mouthCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref eyesCosmeticKey, _Mirror_SyncVarHookDelegate_eyesCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref cheeksCosmeticKey, _Mirror_SyncVarHookDelegate_cheeksCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref browsCosmeticKey, _Mirror_SyncVarHookDelegate_browsCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref golfBallCosmeticKey, _Mirror_SyncVarHookDelegate_golfBallCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
			GeneratedSyncVarDeserialize(ref areSpringBootsEnabled, _Mirror_SyncVarHookDelegate_areSpringBootsEnabled, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref skinColorIndex, _Mirror_SyncVarHookDelegate_skinColorIndex, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref victoryDance, _Mirror_SyncVarHookDelegate_victoryDance, GeneratedNetworkCode._Read_VictoryDance(reader));
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref headCosmeticKey, _Mirror_SyncVarHookDelegate_headCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref hatCosmeticKey, _Mirror_SyncVarHookDelegate_hatCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref faceCosmeticKey, _Mirror_SyncVarHookDelegate_faceCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref lowerFaceCosmeticKey, _Mirror_SyncVarHookDelegate_lowerFaceCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref clubCosmeticKey, _Mirror_SyncVarHookDelegate_clubCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref bodyCosmeticKey, _Mirror_SyncVarHookDelegate_bodyCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref mouthCosmeticKey, _Mirror_SyncVarHookDelegate_mouthCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 0x80L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref eyesCosmeticKey, _Mirror_SyncVarHookDelegate_eyesCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 0x100L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref cheeksCosmeticKey, _Mirror_SyncVarHookDelegate_cheeksCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 0x200L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref browsCosmeticKey, _Mirror_SyncVarHookDelegate_browsCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 0x400L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref golfBallCosmeticKey, _Mirror_SyncVarHookDelegate_golfBallCosmeticKey, GeneratedNetworkCode._Read_PlayerCosmeticsSwitcher_002FCosmeticKey(reader));
		}
		if ((num & 0x800L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref areSpringBootsEnabled, _Mirror_SyncVarHookDelegate_areSpringBootsEnabled, reader.ReadBool());
		}
		if ((num & 0x1000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref skinColorIndex, _Mirror_SyncVarHookDelegate_skinColorIndex, reader.ReadVarInt());
		}
		if ((num & 0x2000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref victoryDance, _Mirror_SyncVarHookDelegate_victoryDance, GeneratedNetworkCode._Read_VictoryDance(reader));
		}
	}
}
