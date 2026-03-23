using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Steamworks;
using UnityEngine;
using UnityEngine.Pool;

public class CosmeticsUnlocksManager : SingletonBehaviour<CosmeticsUnlocksManager>
{
	private const string COSMETICS_FILENAME = "Player.dat";

	private const uint GUID_LEN = 32u;

	private const uint PURCHASES_INFO_OFFSET = 32u;

	private const uint PURCHASES_LIST_OFFSET = 64u;

	private int credits;

	private List<string> purchases = new List<string>();

	private int lastLoad;

	[SerializeField]
	[DisableField]
	private string[] defaultUnlockedGuids;

	public PlayerCosmeticsVictoryDances allDances;

	public int startCredits = 1500;

	public int maxCredits = 99999;

	[CVar("timeToGetStylish", "", "", false, true, hidden = true, resetOnSceneChangeOrCheatsDisabled = false)]
	public static bool everythingUnlocked;

	public static PlayerCosmeticsVictoryDances AllDances
	{
		get
		{
			if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<CosmeticsUnlocksManager>.Instance.allDances;
		}
	}

	public static event Action<int, int> OnCreditsUpdate;

	[CCommand("listCosmeticsUnlocks", "", false, false)]
	public static void ListCosmeticsUnlocks()
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			return;
		}
		SingletonBehaviour<CosmeticsUnlocksManager>.Instance.RefreshPlayerData();
		Debug.Log("Credits: " + SingletonBehaviour<CosmeticsUnlocksManager>.Instance.credits);
		foreach (string purchase in SingletonBehaviour<CosmeticsUnlocksManager>.Instance.purchases)
		{
			Debug.Log("Guid: " + purchase);
		}
	}

	[CCommand("resetPlayerUnlocks", "", false, false, description = "Warning! This will wipe all credits and purchased cosmetics")]
	public static void ResetPlayerUnlocks()
	{
		if (File.Exists("Player.dat"))
		{
			File.Delete("Player.dat");
		}
		if (SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			SingletonBehaviour<CosmeticsUnlocksManager>.Instance.RefreshPlayerData();
		}
	}

	public static void RewardCredits(int amount, bool checkCheats = true)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			Debug.LogWarning("You need to be on the driving range for this command to work!");
			return;
		}
		if (MatchSetupRules.IsCheatsEnabled())
		{
			if (checkCheats)
			{
				return;
			}
			Debug.LogWarning("Cheats are enabled, rewarding points anyway");
		}
		Debug.Log("Rewarding " + amount);
		SingletonBehaviour<CosmeticsUnlocksManager>.Instance.RewardCreditsInternal(amount);
	}

	public static bool OwnsDance(PlayerCosmeticsVictoryDanceMetadata dance)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			return false;
		}
		return IsOwnedInternal(dance?.persistentGuid);
	}

	public static bool OwnsDance(VictoryDance dance)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			return false;
		}
		return IsOwnedInternal(SingletonBehaviour<CosmeticsUnlocksManager>.Instance.allDances.GetDance(dance)?.persistentGuid);
	}

	public static bool OwnsCosmetic(PlayerCosmeticsMetadata metadata)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			return false;
		}
		if (metadata.IsHidden())
		{
			return false;
		}
		return IsOwnedInternal(metadata.PersistentGuid);
	}

	public static int GetUnlockOrder(string guid)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			return -1;
		}
		return SingletonBehaviour<CosmeticsUnlocksManager>.Instance.purchases.IndexOf(guid);
	}

	private static bool IsOwnedInternal(string guid)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			return false;
		}
		SingletonBehaviour<CosmeticsUnlocksManager>.Instance.RefreshPlayerData();
		if (!everythingUnlocked)
		{
			return SingletonBehaviour<CosmeticsUnlocksManager>.Instance.purchases.Contains(guid);
		}
		return true;
	}

	public static bool IsDefaultUnlock(string guid)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<CosmeticsUnlocksManager>.Instance.defaultUnlockedGuids.Contains(guid);
	}

	public static int GetDefaultUnlockOrder(string guid)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			return -1;
		}
		return Array.IndexOf(SingletonBehaviour<CosmeticsUnlocksManager>.Instance.defaultUnlockedGuids, guid);
	}

	public static bool PurchaseCosmetic(PlayerCosmeticsMetadata metadata)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance || metadata == null || metadata.IsHidden())
		{
			return false;
		}
		return SingletonBehaviour<CosmeticsUnlocksManager>.Instance.PurchaseCosmeticInternal(metadata.PersistentGuid, metadata.cost);
	}

	public static bool PurchaseDance(PlayerCosmeticsVictoryDanceMetadata metadata)
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance || metadata == null)
		{
			return false;
		}
		return SingletonBehaviour<CosmeticsUnlocksManager>.Instance.PurchaseCosmeticInternal(metadata.persistentGuid, metadata.cost);
	}

	public static int GetCredits()
	{
		if (!SingletonBehaviour<CosmeticsUnlocksManager>.HasInstance)
		{
			return 0;
		}
		SingletonBehaviour<CosmeticsUnlocksManager>.Instance.RefreshPlayerData();
		return SingletonBehaviour<CosmeticsUnlocksManager>.Instance.credits;
	}

	private void RewardCreditsInternal(int amount, bool checkCheats = true)
	{
		if (amount >= 0)
		{
			int num = credits;
			RefreshPlayerData();
			credits = BMath.Min(credits + amount, maxCredits);
			SavePlayerData();
			if (num != credits)
			{
				CosmeticsUnlocksManager.OnCreditsUpdate?.Invoke(num, credits);
			}
		}
	}

	private bool PurchaseCosmeticInternal(string guid, int cost)
	{
		if (string.IsNullOrEmpty(guid) || cost < 0)
		{
			return false;
		}
		if ((long)guid.Length != 32)
		{
			Debug.LogError("Guid length mismatch!!!");
			return false;
		}
		int num = credits;
		RefreshPlayerData();
		if (cost > credits)
		{
			return false;
		}
		if (purchases.Contains(guid))
		{
			return false;
		}
		purchases.Add(guid);
		credits -= cost;
		SavePlayerData();
		if (num != credits)
		{
			CosmeticsUnlocksManager.OnCreditsUpdate?.Invoke(num, credits);
		}
		return true;
	}

	private void SavePlayerData()
	{
		try
		{
			List<byte> value;
			using (CollectionPool<List<byte>, byte>.Get(out value))
			{
				value.Add(103);
				value.Add(111);
				value.Add(108);
				value.Add(102);
				value.AddRange(BitConverter.GetBytes(credits));
				value.AddRange(BitConverter.GetBytes(SteamEnabler.IsSteamEnabled ? SteamClient.SteamId.AccountId : 1337u));
				value.AddRange(Enumerable.Repeat((byte)0, 32 - value.Count));
				value.AddRange(BitConverter.GetBytes(64u));
				value.AddRange(BitConverter.GetBytes((uint)purchases.Count));
				value.AddRange(BitConverter.GetBytes(32u));
				value.AddRange(Enumerable.Repeat((byte)0, 64 - value.Count));
				foreach (string purchase in purchases)
				{
					for (int i = 0; (long)i < 32L; i++)
					{
						value.AddRange(BitConverter.GetBytes(purchase[i]));
					}
				}
				using SHA256 sHA = SHA256.Create();
				byte[] collection = sHA.ComputeHash(value.ToArray());
				value.AddRange(collection);
				using Aes aes = Aes.Create();
				Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(SteamEnabler.IsSteamEnabled ? SteamClient.SteamId.ToString() : "1337", BitConverter.GetBytes((ulong)SteamClient.AppId.Value));
				aes.Key = rfc2898DeriveBytes.GetBytes(32);
				aes.IV = rfc2898DeriveBytes.GetBytes(16);
				byte[] source = aes.CreateEncryptor(aes.Key, aes.IV).TransformFinalBlock(value.ToArray(), 0, value.Count);
				SteamRemoteStorage.FileWrite("Player.dat", source.ToArray());
				aes.Clear();
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Failed to save Player.dat");
			Debug.LogException(exception);
		}
	}

	private void RefreshPlayerData()
	{
		if (lastLoad != Time.frameCount)
		{
			LoadPlayerData();
			lastLoad = Time.frameCount;
		}
	}

	private void LoadPlayerData()
	{
		try
		{
			credits = startCredits;
			purchases.Clear();
			if (!SteamRemoteStorage.FileExists("Player.dat"))
			{
				return;
			}
			byte[] array = SteamRemoteStorage.FileRead("Player.dat");
			using Aes aes = Aes.Create();
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(SteamEnabler.IsSteamEnabled ? SteamClient.SteamId.ToString() : "1337", BitConverter.GetBytes((ulong)SteamClient.AppId.Value));
			aes.Key = rfc2898DeriveBytes.GetBytes(32);
			aes.IV = rfc2898DeriveBytes.GetBytes(16);
			byte[] array2 = aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(array, 0, array.Length);
			aes.Clear();
			if (array2.Length < 96)
			{
				Debug.LogError("Credits file size too small!");
				return;
			}
			if (array2[0] != 103 || array2[1] != 111 || array2[2] != 108 || array2[3] != 102)
			{
				Debug.LogError("Invalid fileformat!");
				return;
			}
			using SHA256 sHA = SHA256.Create();
			byte[] array3 = sHA.ComputeHash(array2, 0, array2.Length - 32);
			for (int i = 0; i < 32; i++)
			{
				if (array3[i] != array2[array2.Length - 32 + i])
				{
					Debug.LogError("Hash mismatch!");
					return;
				}
			}
			int startIndex = 4;
			credits = BitConverter.ToInt32(array2, startIndex);
			startIndex = 32;
			uint num = BitConverter.ToUInt32(array2, startIndex);
			startIndex += 4;
			uint num2 = BitConverter.ToUInt32(array2, startIndex);
			startIndex += 4;
			uint b = BitConverter.ToUInt32(array2, startIndex);
			b = BMath.Min(32u, b);
			startIndex = (int)num;
			for (uint num3 = 0u; num3 < num2; num3++)
			{
				string text = string.Empty;
				for (int j = 0; j < 32; j++)
				{
					text += BitConverter.ToChar(array2, startIndex);
					startIndex += 2;
				}
				if (!purchases.Contains(text))
				{
					purchases.Add(text);
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Failed to load Player.dat");
			Debug.LogException(exception);
			credits = startCredits;
			purchases.Clear();
		}
		finally
		{
			string[] array4 = defaultUnlockedGuids;
			foreach (string item in array4)
			{
				purchases.Add(item);
			}
		}
	}
}
