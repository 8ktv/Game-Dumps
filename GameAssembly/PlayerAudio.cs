using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerAudio : NetworkBehaviour
{
	private PlayerInfo playerInfo;

	private EventInstance swingChargeInstance;

	private EventInstance knockedOutInstance;

	private EventInstance itemUseInstance;

	private EventInstance gunAimInstance;

	private EventInstance electromagnetShieldHumInstance;

	private EventInstance movingInFoliageLoopInstance;

	private EventInstance respawnInstance;

	private EventInstance electromagnetShieldMuffleSnapshotInstance;

	private bool isPlayingFoliageSound;

	private AntiCheatRateChecker serverJumpCommandRateLimiter;

	private AntiCheatRateChecker serverSwingCommandRateLimiter;

	private AntiCheatRateChecker serverOverchargedSwingCommandRateLimiter;

	private AntiCheatRateChecker serverSwingHitCommandRateLimiter;

	private AntiCheatRateChecker serverPistolShotCommandRateLimiter;

	private AntiCheatRateChecker serverElephantGunShotCommandRateLimiter;

	private AntiCheatRateChecker serverRocketLauncherShotCommandRateLimiter;

	private AntiCheatRateChecker serverGunAimCommandRateLimiter;

	private AntiCheatRateChecker serverItemUseCommandRateLimiter;

	private AntiCheatRateChecker serverCancelItemUseCommandRateLimiter;

	private AntiCheatRateChecker serverLandminePlantCommandRateLimiter;

	private AntiCheatRateChecker serverMovingInFoliageCommandRateLimiter;

	private AntiCheatRateChecker serverStopMovingInFoliageCommandRateLimiter;

	private AntiCheatRateChecker serverCancelRespawnCommandRateLimiter;

	private void Awake()
	{
		playerInfo = GetComponent<PlayerInfo>();
	}

	private void OnDestroy()
	{
		if (swingChargeInstance.isValid())
		{
			swingChargeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		if (knockedOutInstance.isValid())
		{
			knockedOutInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		if (itemUseInstance.isValid())
		{
			itemUseInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		if (gunAimInstance.isValid())
		{
			gunAimInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		if (electromagnetShieldHumInstance.isValid())
		{
			electromagnetShieldHumInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		if (movingInFoliageLoopInstance.isValid())
		{
			movingInFoliageLoopInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		if (respawnInstance.isValid())
		{
			respawnInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
		if (electromagnetShieldMuffleSnapshotInstance.isValid())
		{
			electromagnetShieldMuffleSnapshotInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
	}

	public override void OnStartServer()
	{
		serverJumpCommandRateLimiter = new AntiCheatRateChecker("Jump sound", base.connectionToClient.connectionId, 0.05f, 10, 30, 1f, 3);
		serverSwingCommandRateLimiter = new AntiCheatRateChecker("Swing sound", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverOverchargedSwingCommandRateLimiter = new AntiCheatRateChecker("Overcharged swing sound", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverSwingHitCommandRateLimiter = new AntiCheatRateChecker("Swing hit sound", base.connectionToClient.connectionId, 0.05f, 50, 150, 1f, 10);
		serverPistolShotCommandRateLimiter = new AntiCheatRateChecker("Pistol shot sound", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverElephantGunShotCommandRateLimiter = new AntiCheatRateChecker("Elephant gun shot sound", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverRocketLauncherShotCommandRateLimiter = new AntiCheatRateChecker("Rocket launcher shot sound", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverGunAimCommandRateLimiter = new AntiCheatRateChecker("Gun aim sound", base.connectionToClient.connectionId, 0.025f, 20, 40, 0.25f);
		serverItemUseCommandRateLimiter = new AntiCheatRateChecker("Item use sound", base.connectionToClient.connectionId, 0.25f, 5, 10, 2f);
		serverCancelItemUseCommandRateLimiter = new AntiCheatRateChecker("Cancel item use sound", base.connectionToClient.connectionId, 0.25f, 5, 10, 2f);
		serverLandminePlantCommandRateLimiter = new AntiCheatRateChecker("Landmine plant sound", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverMovingInFoliageCommandRateLimiter = new AntiCheatRateChecker("Moving in foliage sound", base.connectionToClient.connectionId, 0.01f, 20, 50, 0.02f);
		serverStopMovingInFoliageCommandRateLimiter = new AntiCheatRateChecker("Stop moving in foliage sound", base.connectionToClient.connectionId, 0.1f, 5, 10, 1f);
		serverCancelRespawnCommandRateLimiter = new AntiCheatRateChecker("Cancel respawn sound", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
	}

	public void PlayFootstepLocalOnly(Foot foot)
	{
		if (playerInfo.Movement.IsGrounded)
		{
			Vector3 position = playerInfo.GetFootBone(foot).position;
			EventInstance eventInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.FootstepEvent);
			eventInstance.set3DAttributes(position.To3DAttributes());
			eventInstance.setParameterByID(AudioSettings.MaterialTypeId, (float)GetMaterial());
			eventInstance.start();
			eventInstance.release();
		}
		FootstepAudioMaterial GetMaterial()
		{
			if (playerInfo.Movement.IsWadingInWater)
			{
				return FootstepAudioMaterial.Water;
			}
			if (playerInfo.Movement.GroundTerrainType == GroundTerrainType.NotTerrain)
			{
				return FootstepAudioMaterial.Default;
			}
			TerrainLayer groundTerrainDominantGlobalLayer = playerInfo.Movement.GroundTerrainDominantGlobalLayer;
			if (!TerrainManager.Settings.LayerSettings.TryGetValue(groundTerrainDominantGlobalLayer, out var value))
			{
				return FootstepAudioMaterial.Default;
			}
			return value.FootstepAudioMaterial;
		}
	}

	public void PlayJumpForAllClients()
	{
		PlayJumpInternal();
		CmdPlayJumpForAllClients();
	}

	[Command]
	private void CmdPlayJumpForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayJumpForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAudio::CmdPlayJumpForAllClients(Mirror.NetworkConnectionToClient)", 1724879877, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayJump(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlayJump(Mirror.NetworkConnectionToClient)", 570359138, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayJumpInternal()
	{
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.JumpEvent, base.transform.position);
	}

	public void PlaySwingChargeLocalOnly()
	{
		swingChargeInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.SwingChargeEvent);
		swingChargeInstance.start();
		swingChargeInstance.release();
	}

	public void StopSwingChargeLocalOnly()
	{
		swingChargeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	public void PlaySwingForAllClients(float normalizedPower)
	{
		PlaySwingInternal(normalizedPower);
		CmdPlaySwingForAllClients(normalizedPower);
	}

	[Command]
	private void CmdPlaySwingForAllClients(float normalizedPower, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlaySwingForAllClients__Single__NetworkConnectionToClient(normalizedPower, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(normalizedPower);
		SendCommandInternal("System.Void PlayerAudio::CmdPlaySwingForAllClients(System.Single,Mirror.NetworkConnectionToClient)", 2116338308, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlaySwing(NetworkConnectionToClient connection, float normalizedPower)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(normalizedPower);
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlaySwing(Mirror.NetworkConnectionToClient,System.Single)", -2081079069, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlaySwingInternal(float normalizedPower)
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.SwingEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, base.gameObject);
		instance.setParameterByID(AudioSettings.StrengthId, GetStrengthParameterValue());
		instance.start();
		instance.release();
		float GetStrengthParameterValue()
		{
			if (normalizedPower < 0.375f)
			{
				return 0f;
			}
			if (normalizedPower < 0.625f)
			{
				return 1f;
			}
			if (normalizedPower < 0.875f)
			{
				return 2f;
			}
			return 3f;
		}
	}

	public void PlayOverchargedSwingForAllClients(bool isLockedOn)
	{
		PlayOverchargedSwingInternal(isLockedOn);
		CmdPlayOverchargedSwingForAllClients(isLockedOn);
	}

	[Command]
	private void CmdPlayOverchargedSwingForAllClients(bool isLockedOn, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayOverchargedSwingForAllClients__Boolean__NetworkConnectionToClient(isLockedOn, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isLockedOn);
		SendCommandInternal("System.Void PlayerAudio::CmdPlayOverchargedSwingForAllClients(System.Boolean,Mirror.NetworkConnectionToClient)", -221196264, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayOverchargedSwing(NetworkConnectionToClient connection, bool isLockedOn)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isLockedOn);
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlayOverchargedSwing(Mirror.NetworkConnectionToClient,System.Boolean)", 597625165, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayOverchargedSwingInternal(bool isLockedOn)
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.OverchargedSwingEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, base.gameObject);
		instance.setParameterByID(AudioSettings.LockedOnId, isLockedOn ? 1f : 0f);
		instance.start();
		instance.release();
	}

	public void PlaySwingHitForAllClients(Hittable hitHittable)
	{
		if (!(hitHittable == null))
		{
			PlaySwingHitInternal(hitHittable);
			CmdPlaySwingHitForAllClients(hitHittable);
		}
	}

	[Command]
	private void CmdPlaySwingHitForAllClients(Hittable hitHittable, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlaySwingHitForAllClients__Hittable__NetworkConnectionToClient(hitHittable, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(hitHittable);
		SendCommandInternal("System.Void PlayerAudio::CmdPlaySwingHitForAllClients(Hittable,Mirror.NetworkConnectionToClient)", -1187357637, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlaySwingHit(NetworkConnectionToClient connection, Hittable hitHittable)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(hitHittable);
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlaySwingHit(Mirror.NetworkConnectionToClient,Hittable)", 1933817884, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlaySwingHitInternal(Hittable hitHittable)
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.SwingHitEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, hitHittable.gameObject);
		instance.setParameterByID(AudioSettings.ObjectId, (float)GetObjectType());
		instance.start();
		instance.release();
		AudioHitObjectType GetObjectType()
		{
			Entity asEntity = hitHittable.AsEntity;
			if (asEntity.IsGolfBall)
			{
				return AudioHitObjectType.GolfBall;
			}
			if (asEntity.IsPlayer)
			{
				return AudioHitObjectType.Player;
			}
			if (asEntity.IsGolfCart)
			{
				return AudioHitObjectType.GolfCart;
			}
			if (asEntity.IsItem)
			{
				return AudioHitObjectType.Item;
			}
			return AudioHitObjectType.Default;
		}
	}

	public void PlayPistolShotForAllClients()
	{
		PlayPistolShotInternal();
		CmdPlayPistolShotForAllClients();
	}

	[Command]
	private void CmdPlayPistolShotForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayPistolShotForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAudio::CmdPlayPistolShotForAllClients(Mirror.NetworkConnectionToClient)", 1988320696, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayPistolShot(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlayPistolShot(Mirror.NetworkConnectionToClient)", -1902257847, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayPistolShotInternal()
	{
		Vector3 duelingPistolBarrelEndPosition = playerInfo.Inventory.GetDuelingPistolBarrelEndPosition();
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.PistolShotEvent, duelingPistolBarrelEndPosition);
	}

	public void PlayElephantGunShotForAllClients()
	{
		PlayElephantGunShotInternal();
		CmdPlayElephantGunShotForAllClients();
	}

	[Command]
	private void CmdPlayElephantGunShotForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayElephantGunShotForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAudio::CmdPlayElephantGunShotForAllClients(Mirror.NetworkConnectionToClient)", 619969184, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayElephantGunShot(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlayElephantGunShot(Mirror.NetworkConnectionToClient)", -1999268193, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayElephantGunShotInternal()
	{
		Vector3 elephantGunBarrelEndPosition = playerInfo.Inventory.GetElephantGunBarrelEndPosition();
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.ElephantGunShotEvent, elephantGunBarrelEndPosition);
	}

	public void PlayRocketLauncherShotForAllClients()
	{
		PlayRocketLauncherShotInternal();
		CmdPlayRocketLauncherShotForAllClients();
	}

	[Command]
	private void CmdPlayRocketLauncherShotForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayRocketLauncherShotForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAudio::CmdPlayRocketLauncherShotForAllClients(Mirror.NetworkConnectionToClient)", -1855065351, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayRocketLauncherShot(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlayRocketLauncherShot(Mirror.NetworkConnectionToClient)", 866914494, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayRocketLauncherShotInternal()
	{
		Vector3 rocketLauncherRocketPosition = playerInfo.Inventory.GetRocketLauncherRocketPosition();
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.RocketLauncherShotEvent, rocketLauncherRocketPosition);
	}

	public void PlayGunAimForAllClients(ItemType gunType)
	{
		PlayGunAimInternal(gunType);
		CmdPlayGunAimForAllClients(gunType);
	}

	[Command]
	private void CmdPlayGunAimForAllClients(ItemType gunType, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayGunAimForAllClients__ItemType__NetworkConnectionToClient(gunType, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemType(writer, gunType);
		SendCommandInternal("System.Void PlayerAudio::CmdPlayGunAimForAllClients(ItemType,Mirror.NetworkConnectionToClient)", -2135649989, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayGunAim(NetworkConnectionToClient connection, ItemType gunType)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemType(writer, gunType);
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlayGunAim(Mirror.NetworkConnectionToClient,ItemType)", 2080912328, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayGunAimInternal(ItemType gunType)
	{
		if (gunAimInstance.isValid())
		{
			gunAimInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		EventReference eventReference = ((gunType == ItemType.RocketLauncher) ? GameManager.AudioSettings.RocketLauncherAimEvent : GameManager.AudioSettings.GunAimEvent);
		GameObject gameObject = ((gunType == ItemType.DuelingPistol) ? playerInfo.LeftHandEquipmentSwitcher.gameObject : playerInfo.RightHandEquipmentSwitcher.gameObject);
		gunAimInstance = RuntimeManager.CreateInstance(eventReference);
		RuntimeManager.AttachInstanceToGameObject(gunAimInstance, gameObject);
		gunAimInstance.start();
		gunAimInstance.release();
	}

	public void PlaySpeedBoostLocalOnly()
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.SpeedBoostEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, playerInfo.HeadBone.gameObject);
		instance.start();
		instance.release();
	}

	public void PlayItemUseForAllClients(ItemType itemType)
	{
		PlayItemUseInternal(itemType);
		CmdPlayItemUseForAllClients(itemType);
	}

	[Command]
	private void CmdPlayItemUseForAllClients(ItemType itemType, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayItemUseForAllClients__ItemType__NetworkConnectionToClient(itemType, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemType(writer, itemType);
		SendCommandInternal("System.Void PlayerAudio::CmdPlayItemUseForAllClients(ItemType,Mirror.NetworkConnectionToClient)", 2023368720, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayItemUse(NetworkConnectionToClient connection, ItemType itemType)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemType(writer, itemType);
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlayItemUse(Mirror.NetworkConnectionToClient,ItemType)", 318686579, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayItemUseInternal(ItemType itemType)
	{
		if (TryGetSoundData(out var eventReference, out var source))
		{
			if (itemUseInstance.isValid())
			{
				itemUseInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			EventInstance instance = RuntimeManager.CreateInstance(eventReference);
			RuntimeManager.AttachInstanceToGameObject(instance, source);
			instance.start();
			instance.release();
		}
		bool TryGetSoundData(out EventReference reference, out GameObject reference2)
		{
			switch (itemType)
			{
			case ItemType.Coffee:
				reference = GameManager.AudioSettings.CoffeeDrinkEvent;
				reference2 = playerInfo.HeadBone.gameObject;
				return true;
			case ItemType.Airhorn:
				reference = GameManager.AudioSettings.AirhornEvent;
				reference2 = playerInfo.RightHandEquipmentSwitcher.gameObject;
				return true;
			default:
				reference = default(EventReference);
				reference2 = null;
				return false;
			}
		}
	}

	public void CancelItemUseForAllClients()
	{
		CancelItemUseInternal();
		CmdCancelItemUseForAllClients();
	}

	[Command]
	private void CmdCancelItemUseForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdCancelItemUseForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAudio::CmdCancelItemUseForAllClients(Mirror.NetworkConnectionToClient)", -1271799979, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcCancelItemUse(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcCancelItemUse(Mirror.NetworkConnectionToClient)", -1142981440, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void CancelItemUseInternal()
	{
		if (itemUseInstance.isValid())
		{
			itemUseInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void PlayLandminePlantForAllClients(bool stomp)
	{
		PlayLandminePlantInternal(stomp);
		CmdPlayLandminePlantForAllClients(stomp);
	}

	[Command]
	private void CmdPlayLandminePlantForAllClients(bool stomp, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayLandminePlantForAllClients__Boolean__NetworkConnectionToClient(stomp, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(stomp);
		SendCommandInternal("System.Void PlayerAudio::CmdPlayLandminePlantForAllClients(System.Boolean,Mirror.NetworkConnectionToClient)", 668748453, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayLandminePlant(NetworkConnectionToClient connection, bool stomp)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(stomp);
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlayLandminePlant(Mirror.NetworkConnectionToClient,System.Boolean)", -1929263600, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayLandminePlantInternal(bool stomp)
	{
		RuntimeManager.PlayOneShot(stomp ? GameManager.AudioSettings.LandminePlantStompEvent : GameManager.AudioSettings.LandminePlantJamEvent, base.transform.position);
	}

	public void PlayGolfCartKnockoutLocalOnly(Vector3 worldPosition)
	{
		EventInstance eventInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.GolfCartCollisionEvent);
		eventInstance.set3DAttributes(worldPosition.To3DAttributes());
		eventInstance.setParameterByID(AudioSettings.ObjectId, 3f);
		eventInstance.start();
		eventInstance.release();
	}

	public void SetElectromagnetShieldActiveLocalOnly(bool isActive)
	{
		if (playerInfo == null)
		{
			return;
		}
		if (electromagnetShieldHumInstance.isValid())
		{
			electromagnetShieldHumInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (base.isLocalPlayer && electromagnetShieldMuffleSnapshotInstance.isValid())
		{
			electromagnetShieldMuffleSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (isActive)
		{
			RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.ElectromagnetShieldActivationEvent, playerInfo.ElectromagnetShieldCollider.gameObject);
			electromagnetShieldHumInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.ElectromagnetShieldHumEvent);
			RuntimeManager.AttachInstanceToGameObject(electromagnetShieldHumInstance, playerInfo.ElectromagnetShieldCollider.gameObject);
			electromagnetShieldHumInstance.start();
			electromagnetShieldHumInstance.release();
			if (base.isLocalPlayer)
			{
				electromagnetShieldMuffleSnapshotInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.ElectromagnetShieldMuffleSnapshot);
				electromagnetShieldMuffleSnapshotInstance.start();
				electromagnetShieldMuffleSnapshotInstance.release();
			}
		}
		else
		{
			RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.ElectromagnetShieldDeactivationEvent, playerInfo.ElectromagnetShieldCollider.gameObject);
		}
	}

	public void PlayElectromagnetShieldHitLocalOnly(Vector3 worldPosition)
	{
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.ElectromagnetShieldHitEvent, worldPosition);
	}

	public void PlaySpringBootsActivationLocalOnly()
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.SpringBootsActivationEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, base.gameObject);
		instance.start();
		instance.release();
	}

	private void PlaySpringBootsFlipLocalOnly()
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.SpringBootFlipEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, base.gameObject);
		instance.start();
		instance.release();
	}

	public void StartKnockoutLoopLocalOnly()
	{
		knockedOutInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.KnockedOutLoopEvent);
		RuntimeManager.AttachInstanceToGameObject(knockedOutInstance, playerInfo.HeadBone.gameObject);
		knockedOutInstance.start();
		knockedOutInstance.release();
	}

	public void EndKnockoutLoopLocalOnly()
	{
		if (knockedOutInstance.isValid())
		{
			knockedOutInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void PlayKnockoutImmunityStartLocalOnly()
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.KnockoutImmunityStartEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, playerInfo.ChestBone.gameObject);
		instance.start();
		instance.release();
	}

	public void PlayKnockoutImmunityEndLocalOnly()
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.KnockoutImmunityEndEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, playerInfo.ChestBone.gameObject);
		instance.start();
		instance.release();
	}

	public void PlayKnockoutImmunityBlockedKnockoutLocalOnly()
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.KnockoutImmunityBlockedKnockoutEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, playerInfo.ChestBone.gameObject);
		instance.start();
		instance.release();
	}

	public void PlayOrUpdateMovingInFoliageForAllClients(float speed)
	{
		PlayOrUpdateMovingInFoliageInternal(speed);
		CmdPlayOrUpdateMovingInFoliageForAllClients(speed);
	}

	[Command]
	private void CmdPlayOrUpdateMovingInFoliageForAllClients(float speed, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayOrUpdateMovingInFoliageForAllClients__Single__NetworkConnectionToClient(speed, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(speed);
		SendCommandInternal("System.Void PlayerAudio::CmdPlayOrUpdateMovingInFoliageForAllClients(System.Single,Mirror.NetworkConnectionToClient)", -519473458, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayOrUpdateMovingInFoliage(NetworkConnectionToClient connection, float speed)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(speed);
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcPlayOrUpdateMovingInFoliage(Mirror.NetworkConnectionToClient,System.Single)", -53051219, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayOrUpdateMovingInFoliageInternal(float speed)
	{
		float value = BMath.InverseLerpClamped(GameManager.AudioSettings.MinMovementInFoliageSpeed, GameManager.AudioSettings.MaxMovementInFoliageSpeed, speed);
		if (isPlayingFoliageSound)
		{
			movingInFoliageLoopInstance.setParameterByID(AudioSettings.VelocityId, value);
			return;
		}
		isPlayingFoliageSound = true;
		movingInFoliageLoopInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.FoliageLoopEvent);
		RuntimeManager.AttachInstanceToGameObject(movingInFoliageLoopInstance, playerInfo.ChestBone.gameObject);
		movingInFoliageLoopInstance.setParameterByID(AudioSettings.VelocityId, value);
		movingInFoliageLoopInstance.start();
		movingInFoliageLoopInstance.release();
	}

	public void StopMovingInFoliageForAllClients()
	{
		StopMovingInFoliageInternal();
		CmdStopMovingInFoliageForAllClients();
	}

	[Command]
	private void CmdStopMovingInFoliageForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdStopMovingInFoliageForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAudio::CmdStopMovingInFoliageForAllClients(Mirror.NetworkConnectionToClient)", 1976960405, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcStopMovingInFoliage(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcStopMovingInFoliage(Mirror.NetworkConnectionToClient)", 606529956, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void StopMovingInFoliageInternal()
	{
		if (isPlayingFoliageSound)
		{
			isPlayingFoliageSound = false;
			if (movingInFoliageLoopInstance.isValid())
			{
				movingInFoliageLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}
	}

	public void PlayRespawnLocalOnly()
	{
		respawnInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.RespawnEvent);
		RuntimeManager.AttachInstanceToGameObject(respawnInstance, playerInfo.ChestBone.gameObject);
		respawnInstance.start();
		respawnInstance.release();
	}

	public void CancelRespawnForAllClients()
	{
		CancelRespawnInternal();
		CmdCancelRespawnForAllClients();
	}

	[Command]
	private void CmdCancelRespawnForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdCancelRespawnForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAudio::CmdCancelRespawnForAllClients(Mirror.NetworkConnectionToClient)", 836981743, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcCancelRespawn(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerAudio::RpcCancelRespawn(Mirror.NetworkConnectionToClient)", 1493825018, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void CancelRespawnInternal()
	{
		if (respawnInstance.isValid())
		{
			respawnInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void PlayWaterSplashLocalOnly(Vector3 worldPosition)
	{
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.PlayerWaterSplashEvent, worldPosition);
	}

	public void PlayWaterWadeLocalOnly(Foot foot)
	{
		Transform transform = ((foot == Foot.Left) ? playerInfo.LeftFootBone : playerInfo.RightFootBone);
		RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.PlayerWaterWadeEvent, transform.gameObject);
	}

	private void PlayClapLocalOnly()
	{
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.PlayerEmoteClapEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, playerInfo.RightHandEquipmentSwitcher.gameObject);
		instance.start();
		instance.release();
	}

	public void PlayGolfCartBriefcaseOpenLocalOnly(bool isStart)
	{
		RuntimeManager.PlayOneShotAttached(isStart ? GameManager.AudioSettings.GolfCartBriefcaseOpenStart : GameManager.AudioSettings.GolfCartBriefcaseOpenEnd, playerInfo.RightHandEquipmentSwitcher.gameObject);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdPlayJumpForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverJumpCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayJumpInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayJump(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayJumpForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayJumpForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlayJumpForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlayJump__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayJumpInternal();
	}

	protected static void InvokeUserCode_RpcPlayJump__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayJump called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlayJump__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdPlaySwingForAllClients__Single__NetworkConnectionToClient(float normalizedPower, NetworkConnectionToClient sender)
	{
		if (!serverSwingCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlaySwingInternal(normalizedPower);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlaySwing(value, normalizedPower);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlaySwingForAllClients__Single__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlaySwingForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlaySwingForAllClients__Single__NetworkConnectionToClient(reader.ReadFloat(), senderConnection);
		}
	}

	protected void UserCode_RpcPlaySwing__NetworkConnectionToClient__Single(NetworkConnectionToClient connection, float normalizedPower)
	{
		PlaySwingInternal(normalizedPower);
	}

	protected static void InvokeUserCode_RpcPlaySwing__NetworkConnectionToClient__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlaySwing called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlaySwing__NetworkConnectionToClient__Single(null, reader.ReadFloat());
		}
	}

	protected void UserCode_CmdPlayOverchargedSwingForAllClients__Boolean__NetworkConnectionToClient(bool isLockedOn, NetworkConnectionToClient sender)
	{
		if (!serverOverchargedSwingCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayOverchargedSwingInternal(isLockedOn);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayOverchargedSwing(value, isLockedOn);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayOverchargedSwingForAllClients__Boolean__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayOverchargedSwingForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlayOverchargedSwingForAllClients__Boolean__NetworkConnectionToClient(reader.ReadBool(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayOverchargedSwing__NetworkConnectionToClient__Boolean(NetworkConnectionToClient connection, bool isLockedOn)
	{
		PlayOverchargedSwingInternal(isLockedOn);
	}

	protected static void InvokeUserCode_RpcPlayOverchargedSwing__NetworkConnectionToClient__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayOverchargedSwing called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlayOverchargedSwing__NetworkConnectionToClient__Boolean(null, reader.ReadBool());
		}
	}

	protected void UserCode_CmdPlaySwingHitForAllClients__Hittable__NetworkConnectionToClient(Hittable hitHittable, NetworkConnectionToClient sender)
	{
		if (!serverSwingHitCommandRateLimiter.RegisterHit() || hitHittable == null)
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlaySwingHitInternal(hitHittable);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlaySwingHit(value, hitHittable);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlaySwingHitForAllClients__Hittable__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlaySwingHitForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlaySwingHitForAllClients__Hittable__NetworkConnectionToClient(reader.ReadNetworkBehaviour<Hittable>(), senderConnection);
		}
	}

	protected void UserCode_RpcPlaySwingHit__NetworkConnectionToClient__Hittable(NetworkConnectionToClient connection, Hittable hitHittable)
	{
		if (!(hitHittable == null))
		{
			PlaySwingHitInternal(hitHittable);
		}
	}

	protected static void InvokeUserCode_RpcPlaySwingHit__NetworkConnectionToClient__Hittable(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlaySwingHit called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlaySwingHit__NetworkConnectionToClient__Hittable(null, reader.ReadNetworkBehaviour<Hittable>());
		}
	}

	protected void UserCode_CmdPlayPistolShotForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverPistolShotCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayPistolShotInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayPistolShot(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayPistolShotForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayPistolShotForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlayPistolShotForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlayPistolShot__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayPistolShotInternal();
	}

	protected static void InvokeUserCode_RpcPlayPistolShot__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayPistolShot called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlayPistolShot__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdPlayElephantGunShotForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverElephantGunShotCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayElephantGunShotInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayElephantGunShot(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayElephantGunShotForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayElephantGunShotForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlayElephantGunShotForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlayElephantGunShot__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayElephantGunShotInternal();
	}

	protected static void InvokeUserCode_RpcPlayElephantGunShot__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayElephantGunShot called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlayElephantGunShot__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdPlayRocketLauncherShotForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverRocketLauncherShotCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayRocketLauncherShotInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayRocketLauncherShot(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayRocketLauncherShotForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayRocketLauncherShotForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlayRocketLauncherShotForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlayRocketLauncherShot__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayRocketLauncherShotInternal();
	}

	protected static void InvokeUserCode_RpcPlayRocketLauncherShot__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayRocketLauncherShot called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlayRocketLauncherShot__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdPlayGunAimForAllClients__ItemType__NetworkConnectionToClient(ItemType gunType, NetworkConnectionToClient sender)
	{
		if (!serverGunAimCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayGunAimInternal(gunType);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayGunAim(value, gunType);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayGunAimForAllClients__ItemType__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayGunAimForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlayGunAimForAllClients__ItemType__NetworkConnectionToClient(GeneratedNetworkCode._Read_ItemType(reader), senderConnection);
		}
	}

	protected void UserCode_RpcPlayGunAim__NetworkConnectionToClient__ItemType(NetworkConnectionToClient connection, ItemType gunType)
	{
		PlayGunAimInternal(gunType);
	}

	protected static void InvokeUserCode_RpcPlayGunAim__NetworkConnectionToClient__ItemType(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayGunAim called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlayGunAim__NetworkConnectionToClient__ItemType(null, GeneratedNetworkCode._Read_ItemType(reader));
		}
	}

	protected void UserCode_CmdPlayItemUseForAllClients__ItemType__NetworkConnectionToClient(ItemType itemType, NetworkConnectionToClient sender)
	{
		if (!serverItemUseCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayItemUseInternal(itemType);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayItemUse(value, itemType);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayItemUseForAllClients__ItemType__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayItemUseForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlayItemUseForAllClients__ItemType__NetworkConnectionToClient(GeneratedNetworkCode._Read_ItemType(reader), senderConnection);
		}
	}

	protected void UserCode_RpcPlayItemUse__NetworkConnectionToClient__ItemType(NetworkConnectionToClient connection, ItemType itemType)
	{
		PlayItemUseInternal(itemType);
	}

	protected static void InvokeUserCode_RpcPlayItemUse__NetworkConnectionToClient__ItemType(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayItemUse called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlayItemUse__NetworkConnectionToClient__ItemType(null, GeneratedNetworkCode._Read_ItemType(reader));
		}
	}

	protected void UserCode_CmdCancelItemUseForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverCancelItemUseCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			CancelItemUseInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcCancelItemUse(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdCancelItemUseForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdCancelItemUseForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdCancelItemUseForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcCancelItemUse__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		CancelItemUseInternal();
	}

	protected static void InvokeUserCode_RpcCancelItemUse__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcCancelItemUse called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcCancelItemUse__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdPlayLandminePlantForAllClients__Boolean__NetworkConnectionToClient(bool stomp, NetworkConnectionToClient sender)
	{
		if (!serverLandminePlantCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayLandminePlantInternal(stomp);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayLandminePlant(value, stomp);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayLandminePlantForAllClients__Boolean__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayLandminePlantForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlayLandminePlantForAllClients__Boolean__NetworkConnectionToClient(reader.ReadBool(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayLandminePlant__NetworkConnectionToClient__Boolean(NetworkConnectionToClient connection, bool stomp)
	{
		PlayLandminePlantInternal(stomp);
	}

	protected static void InvokeUserCode_RpcPlayLandminePlant__NetworkConnectionToClient__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayLandminePlant called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlayLandminePlant__NetworkConnectionToClient__Boolean(null, reader.ReadBool());
		}
	}

	protected void UserCode_CmdPlayOrUpdateMovingInFoliageForAllClients__Single__NetworkConnectionToClient(float speed, NetworkConnectionToClient sender)
	{
		if (!serverMovingInFoliageCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayOrUpdateMovingInFoliageInternal(speed);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayOrUpdateMovingInFoliage(value, speed);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayOrUpdateMovingInFoliageForAllClients__Single__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayOrUpdateMovingInFoliageForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdPlayOrUpdateMovingInFoliageForAllClients__Single__NetworkConnectionToClient(reader.ReadFloat(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayOrUpdateMovingInFoliage__NetworkConnectionToClient__Single(NetworkConnectionToClient connection, float speed)
	{
		PlayOrUpdateMovingInFoliageInternal(speed);
	}

	protected static void InvokeUserCode_RpcPlayOrUpdateMovingInFoliage__NetworkConnectionToClient__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayOrUpdateMovingInFoliage called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcPlayOrUpdateMovingInFoliage__NetworkConnectionToClient__Single(null, reader.ReadFloat());
		}
	}

	protected void UserCode_CmdStopMovingInFoliageForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverStopMovingInFoliageCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			StopMovingInFoliageInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcStopMovingInFoliage(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdStopMovingInFoliageForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdStopMovingInFoliageForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdStopMovingInFoliageForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcStopMovingInFoliage__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		StopMovingInFoliageInternal();
	}

	protected static void InvokeUserCode_RpcStopMovingInFoliage__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcStopMovingInFoliage called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcStopMovingInFoliage__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdCancelRespawnForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverCancelRespawnCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			CancelRespawnInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcCancelRespawn(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdCancelRespawnForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdCancelRespawnForAllClients called on client.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_CmdCancelRespawnForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcCancelRespawn__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		CancelRespawnInternal();
	}

	protected static void InvokeUserCode_RpcCancelRespawn__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcCancelRespawn called on server.");
		}
		else
		{
			((PlayerAudio)obj).UserCode_RpcCancelRespawn__NetworkConnectionToClient(null);
		}
	}

	static PlayerAudio()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlayJumpForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayJumpForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlaySwingForAllClients(System.Single,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlaySwingForAllClients__Single__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlayOverchargedSwingForAllClients(System.Boolean,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayOverchargedSwingForAllClients__Boolean__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlaySwingHitForAllClients(Hittable,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlaySwingHitForAllClients__Hittable__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlayPistolShotForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayPistolShotForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlayElephantGunShotForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayElephantGunShotForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlayRocketLauncherShotForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayRocketLauncherShotForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlayGunAimForAllClients(ItemType,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayGunAimForAllClients__ItemType__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlayItemUseForAllClients(ItemType,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayItemUseForAllClients__ItemType__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdCancelItemUseForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdCancelItemUseForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlayLandminePlantForAllClients(System.Boolean,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayLandminePlantForAllClients__Boolean__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdPlayOrUpdateMovingInFoliageForAllClients(System.Single,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayOrUpdateMovingInFoliageForAllClients__Single__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdStopMovingInFoliageForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdStopMovingInFoliageForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAudio), "System.Void PlayerAudio::CmdCancelRespawnForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdCancelRespawnForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlayJump(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayJump__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlaySwing(Mirror.NetworkConnectionToClient,System.Single)", InvokeUserCode_RpcPlaySwing__NetworkConnectionToClient__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlayOverchargedSwing(Mirror.NetworkConnectionToClient,System.Boolean)", InvokeUserCode_RpcPlayOverchargedSwing__NetworkConnectionToClient__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlaySwingHit(Mirror.NetworkConnectionToClient,Hittable)", InvokeUserCode_RpcPlaySwingHit__NetworkConnectionToClient__Hittable);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlayPistolShot(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayPistolShot__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlayElephantGunShot(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayElephantGunShot__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlayRocketLauncherShot(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayRocketLauncherShot__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlayGunAim(Mirror.NetworkConnectionToClient,ItemType)", InvokeUserCode_RpcPlayGunAim__NetworkConnectionToClient__ItemType);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlayItemUse(Mirror.NetworkConnectionToClient,ItemType)", InvokeUserCode_RpcPlayItemUse__NetworkConnectionToClient__ItemType);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcCancelItemUse(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcCancelItemUse__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlayLandminePlant(Mirror.NetworkConnectionToClient,System.Boolean)", InvokeUserCode_RpcPlayLandminePlant__NetworkConnectionToClient__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcPlayOrUpdateMovingInFoliage(Mirror.NetworkConnectionToClient,System.Single)", InvokeUserCode_RpcPlayOrUpdateMovingInFoliage__NetworkConnectionToClient__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcStopMovingInFoliage(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcStopMovingInFoliage__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAudio), "System.Void PlayerAudio::RpcCancelRespawn(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcCancelRespawn__NetworkConnectionToClient);
	}
}
