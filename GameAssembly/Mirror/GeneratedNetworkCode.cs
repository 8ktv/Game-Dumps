using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror.Discovery;
using UnityEngine;

namespace Mirror;

[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
public static class GeneratedNetworkCode
{
	public static TimeSnapshotMessage _Read_Mirror_002ETimeSnapshotMessage(NetworkReader reader)
	{
		return default(TimeSnapshotMessage);
	}

	public static void _Write_Mirror_002ETimeSnapshotMessage(NetworkWriter writer, TimeSnapshotMessage value)
	{
	}

	public static ReadyMessage _Read_Mirror_002EReadyMessage(NetworkReader reader)
	{
		return default(ReadyMessage);
	}

	public static void _Write_Mirror_002EReadyMessage(NetworkWriter writer, ReadyMessage value)
	{
	}

	public static NotReadyMessage _Read_Mirror_002ENotReadyMessage(NetworkReader reader)
	{
		return default(NotReadyMessage);
	}

	public static void _Write_Mirror_002ENotReadyMessage(NetworkWriter writer, NotReadyMessage value)
	{
	}

	public static AddPlayerMessage _Read_Mirror_002EAddPlayerMessage(NetworkReader reader)
	{
		return default(AddPlayerMessage);
	}

	public static void _Write_Mirror_002EAddPlayerMessage(NetworkWriter writer, AddPlayerMessage value)
	{
	}

	public static SceneMessage _Read_Mirror_002ESceneMessage(NetworkReader reader)
	{
		return new SceneMessage
		{
			sceneName = reader.ReadString(),
			sceneOperation = _Read_Mirror_002ESceneOperation(reader),
			customHandling = reader.ReadBool()
		};
	}

	public static SceneOperation _Read_Mirror_002ESceneOperation(NetworkReader reader)
	{
		return (SceneOperation)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_Mirror_002ESceneMessage(NetworkWriter writer, SceneMessage value)
	{
		writer.WriteString(value.sceneName);
		_Write_Mirror_002ESceneOperation(writer, value.sceneOperation);
		writer.WriteBool(value.customHandling);
	}

	public static void _Write_Mirror_002ESceneOperation(NetworkWriter writer, SceneOperation value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static CommandMessage _Read_Mirror_002ECommandMessage(NetworkReader reader)
	{
		return new CommandMessage
		{
			netId = reader.ReadVarUInt(),
			componentIndex = NetworkReaderExtensions.ReadByte(reader),
			functionHash = reader.ReadUShort(),
			payload = reader.ReadArraySegmentAndSize()
		};
	}

	public static void _Write_Mirror_002ECommandMessage(NetworkWriter writer, CommandMessage value)
	{
		writer.WriteVarUInt(value.netId);
		NetworkWriterExtensions.WriteByte(writer, value.componentIndex);
		writer.WriteUShort(value.functionHash);
		writer.WriteArraySegmentAndSize(value.payload);
	}

	public static RpcMessage _Read_Mirror_002ERpcMessage(NetworkReader reader)
	{
		return new RpcMessage
		{
			netId = reader.ReadVarUInt(),
			componentIndex = NetworkReaderExtensions.ReadByte(reader),
			functionHash = reader.ReadUShort(),
			payload = reader.ReadArraySegmentAndSize()
		};
	}

	public static void _Write_Mirror_002ERpcMessage(NetworkWriter writer, RpcMessage value)
	{
		writer.WriteVarUInt(value.netId);
		NetworkWriterExtensions.WriteByte(writer, value.componentIndex);
		writer.WriteUShort(value.functionHash);
		writer.WriteArraySegmentAndSize(value.payload);
	}

	public static SpawnMessage _Read_Mirror_002ESpawnMessage(NetworkReader reader)
	{
		return new SpawnMessage
		{
			netId = reader.ReadVarUInt(),
			spawnFlags = _Read_Mirror_002ESpawnFlags(reader),
			sceneId = reader.ReadVarULong(),
			assetId = reader.ReadVarUInt(),
			position = reader.ReadVector3(),
			rotation = reader.ReadQuaternion(),
			scale = reader.ReadVector3(),
			payload = reader.ReadArraySegmentAndSize()
		};
	}

	public static SpawnFlags _Read_Mirror_002ESpawnFlags(NetworkReader reader)
	{
		return (SpawnFlags)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_Mirror_002ESpawnMessage(NetworkWriter writer, SpawnMessage value)
	{
		writer.WriteVarUInt(value.netId);
		_Write_Mirror_002ESpawnFlags(writer, value.spawnFlags);
		writer.WriteVarULong(value.sceneId);
		writer.WriteVarUInt(value.assetId);
		writer.WriteVector3(value.position);
		writer.WriteQuaternion(value.rotation);
		writer.WriteVector3(value.scale);
		writer.WriteArraySegmentAndSize(value.payload);
	}

	public static void _Write_Mirror_002ESpawnFlags(NetworkWriter writer, SpawnFlags value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static ChangeOwnerMessage _Read_Mirror_002EChangeOwnerMessage(NetworkReader reader)
	{
		return new ChangeOwnerMessage
		{
			netId = reader.ReadVarUInt(),
			spawnFlags = _Read_Mirror_002ESpawnFlags(reader)
		};
	}

	public static void _Write_Mirror_002EChangeOwnerMessage(NetworkWriter writer, ChangeOwnerMessage value)
	{
		writer.WriteVarUInt(value.netId);
		_Write_Mirror_002ESpawnFlags(writer, value.spawnFlags);
	}

	public static ObjectSpawnStartedMessage _Read_Mirror_002EObjectSpawnStartedMessage(NetworkReader reader)
	{
		return default(ObjectSpawnStartedMessage);
	}

	public static void _Write_Mirror_002EObjectSpawnStartedMessage(NetworkWriter writer, ObjectSpawnStartedMessage value)
	{
	}

	public static ObjectSpawnFinishedMessage _Read_Mirror_002EObjectSpawnFinishedMessage(NetworkReader reader)
	{
		return default(ObjectSpawnFinishedMessage);
	}

	public static void _Write_Mirror_002EObjectSpawnFinishedMessage(NetworkWriter writer, ObjectSpawnFinishedMessage value)
	{
	}

	public static ObjectDestroyMessage _Read_Mirror_002EObjectDestroyMessage(NetworkReader reader)
	{
		return new ObjectDestroyMessage
		{
			netId = reader.ReadVarUInt()
		};
	}

	public static void _Write_Mirror_002EObjectDestroyMessage(NetworkWriter writer, ObjectDestroyMessage value)
	{
		writer.WriteVarUInt(value.netId);
	}

	public static ObjectHideMessage _Read_Mirror_002EObjectHideMessage(NetworkReader reader)
	{
		return new ObjectHideMessage
		{
			netId = reader.ReadVarUInt()
		};
	}

	public static void _Write_Mirror_002EObjectHideMessage(NetworkWriter writer, ObjectHideMessage value)
	{
		writer.WriteVarUInt(value.netId);
	}

	public static EntityStateMessage _Read_Mirror_002EEntityStateMessage(NetworkReader reader)
	{
		return new EntityStateMessage
		{
			netId = reader.ReadVarUInt(),
			payload = reader.ReadArraySegmentAndSize()
		};
	}

	public static void _Write_Mirror_002EEntityStateMessage(NetworkWriter writer, EntityStateMessage value)
	{
		writer.WriteVarUInt(value.netId);
		writer.WriteArraySegmentAndSize(value.payload);
	}

	public static EntityStateMessageUnreliableBaseline _Read_Mirror_002EEntityStateMessageUnreliableBaseline(NetworkReader reader)
	{
		return new EntityStateMessageUnreliableBaseline
		{
			baselineTick = NetworkReaderExtensions.ReadByte(reader),
			netId = reader.ReadVarUInt(),
			payload = reader.ReadArraySegmentAndSize()
		};
	}

	public static void _Write_Mirror_002EEntityStateMessageUnreliableBaseline(NetworkWriter writer, EntityStateMessageUnreliableBaseline value)
	{
		NetworkWriterExtensions.WriteByte(writer, value.baselineTick);
		writer.WriteVarUInt(value.netId);
		writer.WriteArraySegmentAndSize(value.payload);
	}

	public static EntityStateMessageUnreliableDelta _Read_Mirror_002EEntityStateMessageUnreliableDelta(NetworkReader reader)
	{
		return new EntityStateMessageUnreliableDelta
		{
			baselineTick = NetworkReaderExtensions.ReadByte(reader),
			netId = reader.ReadVarUInt(),
			payload = reader.ReadArraySegmentAndSize()
		};
	}

	public static void _Write_Mirror_002EEntityStateMessageUnreliableDelta(NetworkWriter writer, EntityStateMessageUnreliableDelta value)
	{
		NetworkWriterExtensions.WriteByte(writer, value.baselineTick);
		writer.WriteVarUInt(value.netId);
		writer.WriteArraySegmentAndSize(value.payload);
	}

	public static NetworkPingMessage _Read_Mirror_002ENetworkPingMessage(NetworkReader reader)
	{
		return new NetworkPingMessage
		{
			localTime = reader.ReadDouble(),
			predictedTimeAdjusted = reader.ReadDouble()
		};
	}

	public static void _Write_Mirror_002ENetworkPingMessage(NetworkWriter writer, NetworkPingMessage value)
	{
		writer.WriteDouble(value.localTime);
		writer.WriteDouble(value.predictedTimeAdjusted);
	}

	public static NetworkPongMessage _Read_Mirror_002ENetworkPongMessage(NetworkReader reader)
	{
		return new NetworkPongMessage
		{
			localTime = reader.ReadDouble(),
			predictionErrorUnadjusted = reader.ReadDouble(),
			predictionErrorAdjusted = reader.ReadDouble()
		};
	}

	public static void _Write_Mirror_002ENetworkPongMessage(NetworkWriter writer, NetworkPongMessage value)
	{
		writer.WriteDouble(value.localTime);
		writer.WriteDouble(value.predictionErrorUnadjusted);
		writer.WriteDouble(value.predictionErrorAdjusted);
	}

	public static ServerRequest _Read_Mirror_002EDiscovery_002EServerRequest(NetworkReader reader)
	{
		return default(ServerRequest);
	}

	public static void _Write_Mirror_002EDiscovery_002EServerRequest(NetworkWriter writer, ServerRequest value)
	{
	}

	public static ServerResponse _Read_Mirror_002EDiscovery_002EServerResponse(NetworkReader reader)
	{
		return new ServerResponse
		{
			uri = reader.ReadUri(),
			serverId = reader.ReadVarLong()
		};
	}

	public static void _Write_Mirror_002EDiscovery_002EServerResponse(NetworkWriter writer, ServerResponse value)
	{
		writer.WriteUri(value.uri);
		writer.WriteVarLong(value.serverId);
	}

	public static BClientAuthenticator.ClientCanceledAuthenticationMessage _Read_BClientAuthenticator_002FClientCanceledAuthenticationMessage(NetworkReader reader)
	{
		return default(BClientAuthenticator.ClientCanceledAuthenticationMessage);
	}

	public static void _Write_BClientAuthenticator_002FClientCanceledAuthenticationMessage(NetworkWriter writer, BClientAuthenticator.ClientCanceledAuthenticationMessage value)
	{
	}

	public static BClientAuthenticator.IsPasswordRequiredRequestMessage _Read_BClientAuthenticator_002FIsPasswordRequiredRequestMessage(NetworkReader reader)
	{
		return default(BClientAuthenticator.IsPasswordRequiredRequestMessage);
	}

	public static void _Write_BClientAuthenticator_002FIsPasswordRequiredRequestMessage(NetworkWriter writer, BClientAuthenticator.IsPasswordRequiredRequestMessage value)
	{
	}

	public static BClientAuthenticator.IsPasswordRequiredResponseMessage _Read_BClientAuthenticator_002FIsPasswordRequiredResponseMessage(NetworkReader reader)
	{
		return new BClientAuthenticator.IsPasswordRequiredResponseMessage
		{
			isPasswordRequired = reader.ReadBool()
		};
	}

	public static void _Write_BClientAuthenticator_002FIsPasswordRequiredResponseMessage(NetworkWriter writer, BClientAuthenticator.IsPasswordRequiredResponseMessage value)
	{
		writer.WriteBool(value.isPasswordRequired);
	}

	public static BClientAuthenticator.AuthenticationRequestMessage _Read_BClientAuthenticator_002FAuthenticationRequestMessage(NetworkReader reader)
	{
		return new BClientAuthenticator.AuthenticationRequestMessage
		{
			versionGuid = reader.ReadString(),
			passwordHash = reader.ReadBytesAndSize()
		};
	}

	public static void _Write_BClientAuthenticator_002FAuthenticationRequestMessage(NetworkWriter writer, BClientAuthenticator.AuthenticationRequestMessage value)
	{
		writer.WriteString(value.versionGuid);
		writer.WriteBytesAndSize(value.passwordHash);
	}

	public static BClientAuthenticator.AuthenticationResponseMessage _Read_BClientAuthenticator_002FAuthenticationResponseMessage(NetworkReader reader)
	{
		return new BClientAuthenticator.AuthenticationResponseMessage
		{
			response = _Read_BClientAuthenticator_002FAuthenticationResponse(reader)
		};
	}

	public static BClientAuthenticator.AuthenticationResponse _Read_BClientAuthenticator_002FAuthenticationResponse(NetworkReader reader)
	{
		return (BClientAuthenticator.AuthenticationResponse)reader.ReadVarInt();
	}

	public static void _Write_BClientAuthenticator_002FAuthenticationResponseMessage(NetworkWriter writer, BClientAuthenticator.AuthenticationResponseMessage value)
	{
		_Write_BClientAuthenticator_002FAuthenticationResponse(writer, value.response);
	}

	public static void _Write_BClientAuthenticator_002FAuthenticationResponse(NetworkWriter writer, BClientAuthenticator.AuthenticationResponse value)
	{
		writer.WriteVarInt((int)value);
	}

	public static MaxPlayersUpdatedMessage _Read_MaxPlayersUpdatedMessage(NetworkReader reader)
	{
		return new MaxPlayersUpdatedMessage
		{
			maxPlayers = reader.ReadVarInt()
		};
	}

	public static void _Write_MaxPlayersUpdatedMessage(NetworkWriter writer, MaxPlayersUpdatedMessage value)
	{
		writer.WriteVarInt(value.maxPlayers);
	}

	public static SetPlayerGuidMessage _Read_SetPlayerGuidMessage(NetworkReader reader)
	{
		return new SetPlayerGuidMessage
		{
			playerGuidOnServer = reader.ReadVarULong()
		};
	}

	public static void _Write_SetPlayerGuidMessage(NetworkWriter writer, SetPlayerGuidMessage value)
	{
		writer.WriteVarULong(value.playerGuidOnServer);
	}

	public static DisconnectReasonMessage _Read_DisconnectReasonMessage(NetworkReader reader)
	{
		return new DisconnectReasonMessage
		{
			reason = _Read_DisconnectReason(reader)
		};
	}

	public static DisconnectReason _Read_DisconnectReason(NetworkReader reader)
	{
		return (DisconnectReason)reader.ReadVarInt();
	}

	public static void _Write_DisconnectReasonMessage(NetworkWriter writer, DisconnectReasonMessage value)
	{
		_Write_DisconnectReason(writer, value.reason);
	}

	public static void _Write_DisconnectReason(NetworkWriter writer, DisconnectReason value)
	{
		writer.WriteVarInt((int)value);
	}

	public static SetStandardCourseMessage _Read_SetStandardCourseMessage(NetworkReader reader)
	{
		return new SetStandardCourseMessage
		{
			courseIndex = reader.ReadVarInt()
		};
	}

	public static void _Write_SetStandardCourseMessage(NetworkWriter writer, SetStandardCourseMessage value)
	{
		writer.WriteVarInt(value.courseIndex);
	}

	public static SetNonStandardCourseMessage _Read_SetNonStandardCourseMessage(NetworkReader reader)
	{
		return new SetNonStandardCourseMessage
		{
			globalHoleIndices = _Read_System_002EInt32_005B_005D(reader),
			isRandom = reader.ReadBool()
		};
	}

	public static int[] _Read_System_002EInt32_005B_005D(NetworkReader reader)
	{
		return reader.ReadArray<int>();
	}

	public static void _Write_SetNonStandardCourseMessage(NetworkWriter writer, SetNonStandardCourseMessage value)
	{
		_Write_System_002EInt32_005B_005D(writer, value.globalHoleIndices);
		writer.WriteBool(value.isRandom);
	}

	public static void _Write_System_002EInt32_005B_005D(NetworkWriter writer, int[] value)
	{
		writer.WriteArray(value);
	}

	public static StartMatchMessage _Read_StartMatchMessage(NetworkReader reader)
	{
		return default(StartMatchMessage);
	}

	public static void _Write_StartMatchMessage(NetworkWriter writer, StartMatchMessage value)
	{
	}

	public static RemotePlayerDisconnectMessage _Read_RemotePlayerDisconnectMessage(NetworkReader reader)
	{
		return new RemotePlayerDisconnectMessage
		{
			playerGuidOnServer = reader.ReadVarULong()
		};
	}

	public static void _Write_RemotePlayerDisconnectMessage(NetworkWriter writer, RemotePlayerDisconnectMessage value)
	{
		writer.WriteVarULong(value.playerGuidOnServer);
	}

	public static TriggerNetworkClientToServerExceptionMessage _Read_TriggerNetworkClientToServerExceptionMessage(NetworkReader reader)
	{
		return default(TriggerNetworkClientToServerExceptionMessage);
	}

	public static void _Write_TriggerNetworkClientToServerExceptionMessage(NetworkWriter writer, TriggerNetworkClientToServerExceptionMessage value)
	{
	}

	public static RequestNetworkServerToClientExceptionMessage _Read_RequestNetworkServerToClientExceptionMessage(NetworkReader reader)
	{
		return default(RequestNetworkServerToClientExceptionMessage);
	}

	public static void _Write_RequestNetworkServerToClientExceptionMessage(NetworkWriter writer, RequestNetworkServerToClientExceptionMessage value)
	{
	}

	public static TriggerNetworkServerToClientExceptionMessage _Read_TriggerNetworkServerToClientExceptionMessage(NetworkReader reader)
	{
		return default(TriggerNetworkServerToClientExceptionMessage);
	}

	public static void _Write_TriggerNetworkServerToClientExceptionMessage(NetworkWriter writer, TriggerNetworkServerToClientExceptionMessage value)
	{
	}

	public static ServerRequestFriendCheckMessage _Read_ServerRequestFriendCheckMessage(NetworkReader reader)
	{
		return new ServerRequestFriendCheckMessage
		{
			playerGuidOnServer = reader.ReadVarULong()
		};
	}

	public static void _Write_ServerRequestFriendCheckMessage(NetworkWriter writer, ServerRequestFriendCheckMessage value)
	{
		writer.WriteVarULong(value.playerGuidOnServer);
	}

	public static ClientFriendCheckConfirmationMessage _Read_ClientFriendCheckConfirmationMessage(NetworkReader reader)
	{
		return new ClientFriendCheckConfirmationMessage
		{
			friendPlayerGuid = reader.ReadVarULong()
		};
	}

	public static void _Write_ClientFriendCheckConfirmationMessage(NetworkWriter writer, ClientFriendCheckConfirmationMessage value)
	{
		writer.WriteVarULong(value.friendPlayerGuid);
	}

	public static void _Write_PlayerCosmeticsSwitcher_002FCosmeticKey(NetworkWriter writer, PlayerCosmeticsSwitcher.CosmeticKey value)
	{
		if (value == null)
		{
			writer.WriteBool(value: false);
			return;
		}
		writer.WriteBool(value: true);
		writer.WriteString(value.metadataKey);
		writer.WriteSByte(value.variationIndex);
	}

	public static void _Write_VictoryDance(NetworkWriter writer, VictoryDance value)
	{
		writer.WriteVarInt((int)value);
	}

	public static PlayerCosmeticsSwitcher.CosmeticKey _Read_PlayerCosmeticsSwitcher_002FCosmeticKey(NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey = new PlayerCosmeticsSwitcher.CosmeticKey();
		cosmeticKey.metadataKey = reader.ReadString();
		cosmeticKey.variationIndex = reader.ReadSByte();
		return cosmeticKey;
	}

	public static VictoryDance _Read_VictoryDance(NetworkReader reader)
	{
		return (VictoryDance)reader.ReadVarInt();
	}

	public static void _Write_EquipmentType(NetworkWriter writer, EquipmentType value)
	{
		writer.WriteVarInt((int)value);
	}

	public static EquipmentType _Read_EquipmentType(NetworkReader reader)
	{
		return (EquipmentType)reader.ReadVarInt();
	}

	public static void _Write_OrbitalLaserState(NetworkWriter writer, OrbitalLaserState value)
	{
		writer.WriteVarInt((int)value);
	}

	public static OrbitalLaserState _Read_OrbitalLaserState(NetworkReader reader)
	{
		return (OrbitalLaserState)reader.ReadVarInt();
	}

	public static void _Write_Mirror_002ESyncDirection(NetworkWriter writer, SyncDirection value)
	{
		writer.WriteVarInt((int)value);
	}

	public static SyncDirection _Read_Mirror_002ESyncDirection(NetworkReader reader)
	{
		return (SyncDirection)reader.ReadVarInt();
	}

	public static CourseManager.PlayerState _Read_CourseManager_002FPlayerState(NetworkReader reader)
	{
		return new CourseManager.PlayerState
		{
			playerGuid = reader.ReadVarULong(),
			joinIndex = reader.ReadVarInt(),
			name = reader.ReadString(),
			isConnected = reader.ReadBool(),
			isHost = reader.ReadBool(),
			isRespawning = reader.ReadBool(),
			isSpectator = reader.ReadBool(),
			matchResolution = _Read_PlayerMatchResolution(reader),
			courseScore = reader.ReadVarInt(),
			matchScore = reader.ReadVarInt(),
			scoreTimestamp = reader.ReadDouble(),
			courseStrokes = reader.ReadVarInt(),
			matchStrokes = reader.ReadVarInt(),
			courseStrokesOnFinishedHoles = reader.ReadVarInt(),
			courseParOnFinishedHoles = reader.ReadVarInt(),
			eliminations = reader.ReadVarInt(),
			matchKnockouts = reader.ReadVarInt(),
			courseKnockouts = reader.ReadVarInt(),
			courseKnockedOut = reader.ReadVarInt(),
			matchKnockedOut = reader.ReadVarInt(),
			wins = reader.ReadVarInt(),
			finishes = reader.ReadVarInt(),
			multiplayerFinishes = reader.ReadVarInt(),
			losses = reader.ReadVarInt(),
			dominatingCount = reader.ReadVarInt(),
			bestHoleScore = _Read_StrokesUnderParType(reader),
			avgFinishTime = reader.ReadFloat(),
			longestChipIn = reader.ReadFloat(),
			itemPickups = reader.ReadVarInt()
		};
	}

	public static PlayerMatchResolution _Read_PlayerMatchResolution(NetworkReader reader)
	{
		return (PlayerMatchResolution)reader.ReadVarInt();
	}

	public static StrokesUnderParType _Read_StrokesUnderParType(NetworkReader reader)
	{
		return (StrokesUnderParType)reader.ReadVarInt();
	}

	public static void _Write_CourseManager_002FPlayerState(NetworkWriter writer, CourseManager.PlayerState value)
	{
		writer.WriteVarULong(value.playerGuid);
		writer.WriteVarInt(value.joinIndex);
		writer.WriteString(value.name);
		writer.WriteBool(value.isConnected);
		writer.WriteBool(value.isHost);
		writer.WriteBool(value.isRespawning);
		writer.WriteBool(value.isSpectator);
		_Write_PlayerMatchResolution(writer, value.matchResolution);
		writer.WriteVarInt(value.courseScore);
		writer.WriteVarInt(value.matchScore);
		writer.WriteDouble(value.scoreTimestamp);
		writer.WriteVarInt(value.courseStrokes);
		writer.WriteVarInt(value.matchStrokes);
		writer.WriteVarInt(value.courseStrokesOnFinishedHoles);
		writer.WriteVarInt(value.courseParOnFinishedHoles);
		writer.WriteVarInt(value.eliminations);
		writer.WriteVarInt(value.matchKnockouts);
		writer.WriteVarInt(value.courseKnockouts);
		writer.WriteVarInt(value.courseKnockedOut);
		writer.WriteVarInt(value.matchKnockedOut);
		writer.WriteVarInt(value.wins);
		writer.WriteVarInt(value.finishes);
		writer.WriteVarInt(value.multiplayerFinishes);
		writer.WriteVarInt(value.losses);
		writer.WriteVarInt(value.dominatingCount);
		_Write_StrokesUnderParType(writer, value.bestHoleScore);
		writer.WriteFloat(value.avgFinishTime);
		writer.WriteFloat(value.longestChipIn);
		writer.WriteVarInt(value.itemPickups);
	}

	public static void _Write_PlayerMatchResolution(NetworkWriter writer, PlayerMatchResolution value)
	{
		writer.WriteVarInt((int)value);
	}

	public static void _Write_StrokesUnderParType(NetworkWriter writer, StrokesUnderParType value)
	{
		writer.WriteVarInt((int)value);
	}

	public static CourseManager.PlayerPair _Read_CourseManager_002FPlayerPair(NetworkReader reader)
	{
		return new CourseManager.PlayerPair
		{
			playerAGuid = reader.ReadVarULong(),
			playerBGuid = reader.ReadVarULong()
		};
	}

	public static void _Write_CourseManager_002FPlayerPair(NetworkWriter writer, CourseManager.PlayerPair value)
	{
		writer.WriteVarULong(value.playerAGuid);
		writer.WriteVarULong(value.playerBGuid);
	}

	public static void _Write_AnnouncerLine(NetworkWriter writer, AnnouncerLine value)
	{
		writer.WriteVarInt((int)value);
	}

	public static AnnouncerLine _Read_AnnouncerLine(NetworkReader reader)
	{
		return (AnnouncerLine)reader.ReadVarInt();
	}

	public static void _Write_System_002ECollections_002EGeneric_002EList_00601_003CAnnouncerLine_003E(NetworkWriter writer, List<AnnouncerLine> value)
	{
		writer.WriteList(value);
	}

	public static List<AnnouncerLine> _Read_System_002ECollections_002EGeneric_002EList_00601_003CAnnouncerLine_003E(NetworkReader reader)
	{
		return reader.ReadList<AnnouncerLine>();
	}

	public static void _Write_MatchState(NetworkWriter writer, MatchState value)
	{
		writer.WriteVarInt((int)value);
	}

	public static MatchState _Read_MatchState(NetworkReader reader)
	{
		return (MatchState)reader.ReadVarInt();
	}

	public static void _Write_BallOutOfBoundsReturnState(NetworkWriter writer, BallOutOfBoundsReturnState value)
	{
		writer.WriteVarInt((int)value);
	}

	public static BallOutOfBoundsReturnState _Read_BallOutOfBoundsReturnState(NetworkReader reader)
	{
		return (BallOutOfBoundsReturnState)reader.ReadVarInt();
	}

	public static void _Write_ItemType(NetworkWriter writer, ItemType value)
	{
		writer.WriteVarInt((int)value);
	}

	public static void _Write_ItemUseId(NetworkWriter writer, ItemUseId value)
	{
		writer.WriteVarULong(value.userGuid);
		writer.WriteVarInt(value.useIndex);
		_Write_ItemType(writer, value.itemType);
	}

	public static ItemType _Read_ItemType(NetworkReader reader)
	{
		return (ItemType)reader.ReadVarInt();
	}

	public static ItemUseId _Read_ItemUseId(NetworkReader reader)
	{
		return new ItemUseId
		{
			userGuid = reader.ReadVarULong(),
			useIndex = reader.ReadVarInt(),
			itemType = _Read_ItemType(reader)
		};
	}

	public static void _Write_SwingProjectileState(NetworkWriter writer, SwingProjectileState value)
	{
		writer.WriteVarInt((int)value);
	}

	public static SwingProjectileState _Read_SwingProjectileState(NetworkReader reader)
	{
		return (SwingProjectileState)reader.ReadVarInt();
	}

	public static void _Write_BoundsState(NetworkWriter writer, BoundsState value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static BoundsState _Read_BoundsState(NetworkReader reader)
	{
		return (BoundsState)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_EliminationReason(NetworkWriter writer, EliminationReason value)
	{
		writer.WriteVarInt((int)value);
	}

	public static EliminationReason _Read_EliminationReason(NetworkReader reader)
	{
		return (EliminationReason)reader.ReadVarInt();
	}

	public static void _Write_PlayerTextPopupType(NetworkWriter writer, PlayerTextPopupType value)
	{
		writer.WriteVarInt((int)value);
	}

	public static PlayerTextPopupType _Read_PlayerTextPopupType(NetworkReader reader)
	{
		return (PlayerTextPopupType)reader.ReadVarInt();
	}

	public static void _Write_KnockoutType(NetworkWriter writer, KnockoutType value)
	{
		writer.WriteVarInt((int)value);
	}

	public static KnockoutType _Read_KnockoutType(NetworkReader reader)
	{
		return (KnockoutType)reader.ReadVarInt();
	}

	public static void _Write_GolfCartSeat(NetworkWriter writer, GolfCartSeat value)
	{
		writer.WriteNetworkBehaviour(value.golfCart);
		writer.WriteVarInt(value.seat);
	}

	public static GolfCartSeat _Read_GolfCartSeat(NetworkReader reader)
	{
		return new GolfCartSeat
		{
			golfCart = reader.ReadNetworkBehaviour<GolfCartInfo>(),
			seat = reader.ReadVarInt()
		};
	}

	public static InventorySlot _Read_InventorySlot(NetworkReader reader)
	{
		return new InventorySlot
		{
			itemType = _Read_ItemType(reader),
			remainingUses = reader.ReadVarInt()
		};
	}

	public static void _Write_InventorySlot(NetworkWriter writer, InventorySlot value)
	{
		_Write_ItemType(writer, value.itemType);
		writer.WriteVarInt(value.remainingUses);
	}

	public static void _Write_LandmineArmType(NetworkWriter writer, LandmineArmType value)
	{
		writer.WriteVarInt((int)value);
	}

	public static LandmineArmType _Read_LandmineArmType(NetworkReader reader)
	{
		return (LandmineArmType)reader.ReadVarInt();
	}

	public static void _Write_ThrownUsedItemType(NetworkWriter writer, ThrownUsedItemType value)
	{
		writer.WriteVarInt((int)value);
	}

	public static ThrownUsedItemType _Read_ThrownUsedItemType(NetworkReader reader)
	{
		return (ThrownUsedItemType)reader.ReadVarInt();
	}

	public static void _Write_GroundTerrainType(NetworkWriter writer, GroundTerrainType value)
	{
		writer.WriteVarInt((int)value);
	}

	public static void _Write_TerrainLayer(NetworkWriter writer, TerrainLayer value)
	{
		writer.WriteVarInt((int)value);
	}

	public static void _Write_KnockoutState(NetworkWriter writer, KnockoutState value)
	{
		writer.WriteVarInt((int)value);
	}

	public static void _Write_PlayerMovement_002FKnockedOutVfxData(NetworkWriter writer, PlayerMovement.KnockedOutVfxData value)
	{
		writer.WriteVarInt(value.totalStarCount);
		writer.WriteVarInt(value.coloredStarCount);
	}

	public static void _Write_StatusEffect(NetworkWriter writer, StatusEffect value)
	{
		writer.WriteVarInt((int)value);
	}

	public static void _Write_DivingState(NetworkWriter writer, DivingState value)
	{
		writer.WriteVarInt((int)value);
	}

	public static void _Write_RespawnState(NetworkWriter writer, RespawnState value)
	{
		writer.WriteVarInt((int)value);
	}

	public static GroundTerrainType _Read_GroundTerrainType(NetworkReader reader)
	{
		return (GroundTerrainType)reader.ReadVarInt();
	}

	public static TerrainLayer _Read_TerrainLayer(NetworkReader reader)
	{
		return (TerrainLayer)reader.ReadVarInt();
	}

	public static KnockoutState _Read_KnockoutState(NetworkReader reader)
	{
		return (KnockoutState)reader.ReadVarInt();
	}

	public static PlayerMovement.KnockedOutVfxData _Read_PlayerMovement_002FKnockedOutVfxData(NetworkReader reader)
	{
		return new PlayerMovement.KnockedOutVfxData
		{
			totalStarCount = reader.ReadVarInt(),
			coloredStarCount = reader.ReadVarInt()
		};
	}

	public static StatusEffect _Read_StatusEffect(NetworkReader reader)
	{
		return (StatusEffect)reader.ReadVarInt();
	}

	public static DivingState _Read_DivingState(NetworkReader reader)
	{
		return (DivingState)reader.ReadVarInt();
	}

	public static RespawnState _Read_RespawnState(NetworkReader reader)
	{
		return (RespawnState)reader.ReadVarInt();
	}

	public static void _Write_HoleOverviewCameraUi_002FState(NetworkWriter writer, HoleOverviewCameraUi.State value)
	{
		writer.WriteVarInt((int)value);
	}

	public static HoleOverviewCameraUi.State _Read_HoleOverviewCameraUi_002FState(NetworkReader reader)
	{
		return (HoleOverviewCameraUi.State)reader.ReadVarInt();
	}

	public static void _Write_InfoFeed_002FGenericMessageData(NetworkWriter writer, InfoFeed.GenericMessageData value)
	{
		writer.WriteString(value.preIconText);
		writer.WriteString(value.postIconText);
		_Write_InfoFeedIconSettings_002FType(writer, value.icon1);
		_Write_InfoFeedIconSettings_002FType(writer, value.icon2);
	}

	public static void _Write_InfoFeedIconSettings_002FType(NetworkWriter writer, InfoFeedIconSettings.Type value)
	{
		writer.WriteVarInt((int)value);
	}

	public static InfoFeed.GenericMessageData _Read_InfoFeed_002FGenericMessageData(NetworkReader reader)
	{
		return new InfoFeed.GenericMessageData
		{
			preIconText = reader.ReadString(),
			postIconText = reader.ReadString(),
			icon1 = _Read_InfoFeedIconSettings_002FType(reader),
			icon2 = _Read_InfoFeedIconSettings_002FType(reader)
		};
	}

	public static InfoFeedIconSettings.Type _Read_InfoFeedIconSettings_002FType(NetworkReader reader)
	{
		return (InfoFeedIconSettings.Type)reader.ReadVarInt();
	}

	public static void _Write_InfoFeed_002FFinishedHoleMessageData(NetworkWriter writer, InfoFeed.FinishedHoleMessageData value)
	{
		writer.WriteString(value.playerName);
		writer.WriteVarInt(value.displayPlacement);
	}

	public static InfoFeed.FinishedHoleMessageData _Read_InfoFeed_002FFinishedHoleMessageData(NetworkReader reader)
	{
		return new InfoFeed.FinishedHoleMessageData
		{
			playerName = reader.ReadString(),
			displayPlacement = reader.ReadVarInt()
		};
	}

	public static void _Write_InfoFeed_002FScoredOnDrivingRangeMessageData(NetworkWriter writer, InfoFeed.ScoredOnDrivingRangeMessageData value)
	{
		writer.WriteString(value.playerName);
	}

	public static InfoFeed.ScoredOnDrivingRangeMessageData _Read_InfoFeed_002FScoredOnDrivingRangeMessageData(NetworkReader reader)
	{
		return new InfoFeed.ScoredOnDrivingRangeMessageData
		{
			playerName = reader.ReadString()
		};
	}

	public static void _Write_InfoFeed_002FStrokesMessageData(NetworkWriter writer, InfoFeed.StrokesMessageData value)
	{
		writer.WriteString(value.playerName);
		_Write_StrokesUnderParType(writer, value.strokesUnderParType);
		writer.WriteVarInt(value.strokesUnderPar);
	}

	public static InfoFeed.StrokesMessageData _Read_InfoFeed_002FStrokesMessageData(NetworkReader reader)
	{
		return new InfoFeed.StrokesMessageData
		{
			playerName = reader.ReadString(),
			strokesUnderParType = _Read_StrokesUnderParType(reader),
			strokesUnderPar = reader.ReadVarInt()
		};
	}

	public static void _Write_InfoFeed_002FChipInMessageData(NetworkWriter writer, InfoFeed.ChipInMessageData value)
	{
		writer.WriteString(value.playerName);
		writer.WriteFloat(value.distance);
	}

	public static InfoFeed.ChipInMessageData _Read_InfoFeed_002FChipInMessageData(NetworkReader reader)
	{
		return new InfoFeed.ChipInMessageData
		{
			playerName = reader.ReadString(),
			distance = reader.ReadFloat()
		};
	}

	public static void _Write_InfoFeed_002FSpeedrunMessageData(NetworkWriter writer, InfoFeed.SpeedrunMessageData value)
	{
		writer.WriteString(value.playerName);
		writer.WriteFloat(value.time);
	}

	public static InfoFeed.SpeedrunMessageData _Read_InfoFeed_002FSpeedrunMessageData(NetworkReader reader)
	{
		return new InfoFeed.SpeedrunMessageData
		{
			playerName = reader.ReadString(),
			time = reader.ReadFloat()
		};
	}

	public static void _Write_InfoFeed_002FDominatingMessageData(NetworkWriter writer, InfoFeed.DominatingMessageData value)
	{
		writer.WriteString(value.dominatingPlayerName);
		writer.WriteString(value.dominatedPlayerName);
	}

	public static InfoFeed.DominatingMessageData _Read_InfoFeed_002FDominatingMessageData(NetworkReader reader)
	{
		return new InfoFeed.DominatingMessageData
		{
			dominatingPlayerName = reader.ReadString(),
			dominatedPlayerName = reader.ReadString()
		};
	}

	public static void _Write_InfoFeed_002FRevengeMessageData(NetworkWriter writer, InfoFeed.RevengeMessageData value)
	{
		writer.WriteString(value.previouslyDominatedPlayerName);
		writer.WriteString(value.previouslyDominatingPlayerName);
	}

	public static InfoFeed.RevengeMessageData _Read_InfoFeed_002FRevengeMessageData(NetworkReader reader)
	{
		return new InfoFeed.RevengeMessageData
		{
			previouslyDominatedPlayerName = reader.ReadString(),
			previouslyDominatingPlayerName = reader.ReadString()
		};
	}

	public static void _Write_LobbyMode(NetworkWriter writer, LobbyMode value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static LobbyMode _Read_LobbyMode(NetworkReader reader)
	{
		return (LobbyMode)NetworkReaderExtensions.ReadByte(reader);
	}

	public static MatchSetupRules.Rule _Read_MatchSetupRules_002FRule(NetworkReader reader)
	{
		return (MatchSetupRules.Rule)reader.ReadVarInt();
	}

	public static void _Write_MatchSetupRules_002FRule(NetworkWriter writer, MatchSetupRules.Rule value)
	{
		writer.WriteVarInt((int)value);
	}

	public static MatchSetupRules.ItemPoolId _Read_MatchSetupRules_002FItemPoolId(NetworkReader reader)
	{
		return new MatchSetupRules.ItemPoolId
		{
			itemType = _Read_ItemType(reader),
			itemPoolIndex = reader.ReadVarInt()
		};
	}

	public static void _Write_MatchSetupRules_002FItemPoolId(NetworkWriter writer, MatchSetupRules.ItemPoolId value)
	{
		_Write_ItemType(writer, value.itemType);
		writer.WriteVarInt(value.itemPoolIndex);
	}

	public static void _Write_MatchSetupRules_002FPreset(NetworkWriter writer, MatchSetupRules.Preset value)
	{
		writer.WriteVarInt((int)value);
	}

	public static MatchSetupRules.Preset _Read_MatchSetupRules_002FPreset(NetworkReader reader)
	{
		return (MatchSetupRules.Preset)reader.ReadVarInt();
	}

	public static void _Write_VfxType(NetworkWriter writer, VfxType value)
	{
		writer.WriteVarInt((int)value);
	}

	public static VfxType _Read_VfxType(NetworkReader reader)
	{
		return (VfxType)reader.ReadVarInt();
	}

	public static void _Write_VfxManager_002FGunShotHitVfxData(NetworkWriter writer, VfxManager.GunShotHitVfxData value)
	{
		writer.WriteNetworkBehaviour(value.hitHittable);
		writer.WriteBool(value.hitElectromagnetShield);
		writer.WriteVector3(value.localHitPoint);
		writer.WriteVector3(value.fallbackWorldPoint);
	}

	public static VfxManager.GunShotHitVfxData _Read_VfxManager_002FGunShotHitVfxData(NetworkReader reader)
	{
		return new VfxManager.GunShotHitVfxData
		{
			hitHittable = reader.ReadNetworkBehaviour<Hittable>(),
			hitElectromagnetShield = reader.ReadBool(),
			localHitPoint = reader.ReadVector3(),
			fallbackWorldPoint = reader.ReadVector3()
		};
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void InitReadWriters()
	{
		Writer<byte>.write = NetworkWriterExtensions.WriteByte;
		Writer<byte?>.write = NetworkWriterExtensions.WriteByteNullable;
		Writer<sbyte>.write = NetworkWriterExtensions.WriteSByte;
		Writer<sbyte?>.write = NetworkWriterExtensions.WriteSByteNullable;
		Writer<char>.write = NetworkWriterExtensions.WriteChar;
		Writer<char?>.write = NetworkWriterExtensions.WriteCharNullable;
		Writer<bool>.write = NetworkWriterExtensions.WriteBool;
		Writer<bool?>.write = NetworkWriterExtensions.WriteBoolNullable;
		Writer<short>.write = NetworkWriterExtensions.WriteShort;
		Writer<short?>.write = NetworkWriterExtensions.WriteShortNullable;
		Writer<ushort>.write = NetworkWriterExtensions.WriteUShort;
		Writer<ushort?>.write = NetworkWriterExtensions.WriteUShortNullable;
		Writer<int>.write = NetworkWriterExtensions.WriteVarInt;
		Writer<int?>.write = NetworkWriterExtensions.WriteIntNullable;
		Writer<uint>.write = NetworkWriterExtensions.WriteVarUInt;
		Writer<uint?>.write = NetworkWriterExtensions.WriteUIntNullable;
		Writer<long>.write = NetworkWriterExtensions.WriteVarLong;
		Writer<long?>.write = NetworkWriterExtensions.WriteLongNullable;
		Writer<ulong>.write = NetworkWriterExtensions.WriteVarULong;
		Writer<ulong?>.write = NetworkWriterExtensions.WriteULongNullable;
		Writer<float>.write = NetworkWriterExtensions.WriteFloat;
		Writer<float?>.write = NetworkWriterExtensions.WriteFloatNullable;
		Writer<double>.write = NetworkWriterExtensions.WriteDouble;
		Writer<double?>.write = NetworkWriterExtensions.WriteDoubleNullable;
		Writer<decimal>.write = NetworkWriterExtensions.WriteDecimal;
		Writer<decimal?>.write = NetworkWriterExtensions.WriteDecimalNullable;
		Writer<Half>.write = NetworkWriterExtensions.WriteHalf;
		Writer<string>.write = NetworkWriterExtensions.WriteString;
		Writer<byte[]>.write = NetworkWriterExtensions.WriteBytesAndSize;
		Writer<ArraySegment<byte>>.write = NetworkWriterExtensions.WriteArraySegmentAndSize;
		Writer<Vector2>.write = NetworkWriterExtensions.WriteVector2;
		Writer<Vector2?>.write = NetworkWriterExtensions.WriteVector2Nullable;
		Writer<Vector3>.write = NetworkWriterExtensions.WriteVector3;
		Writer<Vector3?>.write = NetworkWriterExtensions.WriteVector3Nullable;
		Writer<Vector4>.write = NetworkWriterExtensions.WriteVector4;
		Writer<Vector4?>.write = NetworkWriterExtensions.WriteVector4Nullable;
		Writer<Vector2Int>.write = NetworkWriterExtensions.WriteVector2Int;
		Writer<Vector2Int?>.write = NetworkWriterExtensions.WriteVector2IntNullable;
		Writer<Vector3Int>.write = NetworkWriterExtensions.WriteVector3Int;
		Writer<Vector3Int?>.write = NetworkWriterExtensions.WriteVector3IntNullable;
		Writer<Color>.write = NetworkWriterExtensions.WriteColor;
		Writer<Color?>.write = NetworkWriterExtensions.WriteColorNullable;
		Writer<Color32>.write = NetworkWriterExtensions.WriteColor32;
		Writer<Color32?>.write = NetworkWriterExtensions.WriteColor32Nullable;
		Writer<Quaternion>.write = NetworkWriterExtensions.WriteQuaternion;
		Writer<Quaternion?>.write = NetworkWriterExtensions.WriteQuaternionNullable;
		Writer<Rect>.write = NetworkWriterExtensions.WriteRect;
		Writer<Rect?>.write = NetworkWriterExtensions.WriteRectNullable;
		Writer<Plane>.write = NetworkWriterExtensions.WritePlane;
		Writer<Plane?>.write = NetworkWriterExtensions.WritePlaneNullable;
		Writer<Ray>.write = NetworkWriterExtensions.WriteRay;
		Writer<Ray?>.write = NetworkWriterExtensions.WriteRayNullable;
		Writer<LayerMask>.write = NetworkWriterExtensions.WriteLayerMask;
		Writer<LayerMask?>.write = NetworkWriterExtensions.WriteLayerMaskNullable;
		Writer<Matrix4x4>.write = NetworkWriterExtensions.WriteMatrix4x4;
		Writer<Matrix4x4?>.write = NetworkWriterExtensions.WriteMatrix4x4Nullable;
		Writer<Guid>.write = NetworkWriterExtensions.WriteGuid;
		Writer<Guid?>.write = NetworkWriterExtensions.WriteGuidNullable;
		Writer<NetworkIdentity>.write = NetworkWriterExtensions.WriteNetworkIdentity;
		Writer<NetworkBehaviour>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<Transform>.write = NetworkWriterExtensions.WriteTransform;
		Writer<GameObject>.write = NetworkWriterExtensions.WriteGameObject;
		Writer<Uri>.write = NetworkWriterExtensions.WriteUri;
		Writer<Texture2D>.write = NetworkWriterExtensions.WriteTexture2D;
		Writer<Sprite>.write = NetworkWriterExtensions.WriteSprite;
		Writer<DateTime>.write = NetworkWriterExtensions.WriteDateTime;
		Writer<DateTime?>.write = NetworkWriterExtensions.WriteDateTimeNullable;
		Writer<TimeSnapshotMessage>.write = _Write_Mirror_002ETimeSnapshotMessage;
		Writer<ReadyMessage>.write = _Write_Mirror_002EReadyMessage;
		Writer<NotReadyMessage>.write = _Write_Mirror_002ENotReadyMessage;
		Writer<AddPlayerMessage>.write = _Write_Mirror_002EAddPlayerMessage;
		Writer<SceneMessage>.write = _Write_Mirror_002ESceneMessage;
		Writer<SceneOperation>.write = _Write_Mirror_002ESceneOperation;
		Writer<CommandMessage>.write = _Write_Mirror_002ECommandMessage;
		Writer<RpcMessage>.write = _Write_Mirror_002ERpcMessage;
		Writer<SpawnMessage>.write = _Write_Mirror_002ESpawnMessage;
		Writer<SpawnFlags>.write = _Write_Mirror_002ESpawnFlags;
		Writer<ChangeOwnerMessage>.write = _Write_Mirror_002EChangeOwnerMessage;
		Writer<ObjectSpawnStartedMessage>.write = _Write_Mirror_002EObjectSpawnStartedMessage;
		Writer<ObjectSpawnFinishedMessage>.write = _Write_Mirror_002EObjectSpawnFinishedMessage;
		Writer<ObjectDestroyMessage>.write = _Write_Mirror_002EObjectDestroyMessage;
		Writer<ObjectHideMessage>.write = _Write_Mirror_002EObjectHideMessage;
		Writer<EntityStateMessage>.write = _Write_Mirror_002EEntityStateMessage;
		Writer<EntityStateMessageUnreliableBaseline>.write = _Write_Mirror_002EEntityStateMessageUnreliableBaseline;
		Writer<EntityStateMessageUnreliableDelta>.write = _Write_Mirror_002EEntityStateMessageUnreliableDelta;
		Writer<NetworkPingMessage>.write = _Write_Mirror_002ENetworkPingMessage;
		Writer<NetworkPongMessage>.write = _Write_Mirror_002ENetworkPongMessage;
		Writer<SyncData>.write = SyncDataReaderWriter.WriteSyncData;
		Writer<PredictedGolfCartSyncData>.write = PredictedSyncDataReadWrite.WritePredictedGolfCartSyncData;
		Writer<PredictedSyncData>.write = PredictedGolfCartSyncDataReadWrite.WritePredictedSyncData;
		Writer<ServerRequest>.write = _Write_Mirror_002EDiscovery_002EServerRequest;
		Writer<ServerResponse>.write = _Write_Mirror_002EDiscovery_002EServerResponse;
		Writer<BClientAuthenticator.ClientCanceledAuthenticationMessage>.write = _Write_BClientAuthenticator_002FClientCanceledAuthenticationMessage;
		Writer<BClientAuthenticator.IsPasswordRequiredRequestMessage>.write = _Write_BClientAuthenticator_002FIsPasswordRequiredRequestMessage;
		Writer<BClientAuthenticator.IsPasswordRequiredResponseMessage>.write = _Write_BClientAuthenticator_002FIsPasswordRequiredResponseMessage;
		Writer<BClientAuthenticator.AuthenticationRequestMessage>.write = _Write_BClientAuthenticator_002FAuthenticationRequestMessage;
		Writer<BClientAuthenticator.AuthenticationResponseMessage>.write = _Write_BClientAuthenticator_002FAuthenticationResponseMessage;
		Writer<BClientAuthenticator.AuthenticationResponse>.write = _Write_BClientAuthenticator_002FAuthenticationResponse;
		Writer<MaxPlayersUpdatedMessage>.write = _Write_MaxPlayersUpdatedMessage;
		Writer<SetPlayerGuidMessage>.write = _Write_SetPlayerGuidMessage;
		Writer<DisconnectReasonMessage>.write = _Write_DisconnectReasonMessage;
		Writer<DisconnectReason>.write = _Write_DisconnectReason;
		Writer<SetStandardCourseMessage>.write = _Write_SetStandardCourseMessage;
		Writer<SetNonStandardCourseMessage>.write = _Write_SetNonStandardCourseMessage;
		Writer<int[]>.write = _Write_System_002EInt32_005B_005D;
		Writer<StartMatchMessage>.write = _Write_StartMatchMessage;
		Writer<RemotePlayerDisconnectMessage>.write = _Write_RemotePlayerDisconnectMessage;
		Writer<TriggerNetworkClientToServerExceptionMessage>.write = _Write_TriggerNetworkClientToServerExceptionMessage;
		Writer<RequestNetworkServerToClientExceptionMessage>.write = _Write_RequestNetworkServerToClientExceptionMessage;
		Writer<TriggerNetworkServerToClientExceptionMessage>.write = _Write_TriggerNetworkServerToClientExceptionMessage;
		Writer<ServerRequestFriendCheckMessage>.write = _Write_ServerRequestFriendCheckMessage;
		Writer<ClientFriendCheckConfirmationMessage>.write = _Write_ClientFriendCheckConfirmationMessage;
		Writer<PlayerCosmeticsSwitcher.CosmeticKey>.write = _Write_PlayerCosmeticsSwitcher_002FCosmeticKey;
		Writer<VictoryDance>.write = _Write_VictoryDance;
		Writer<PlayerGolfer>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<GolfBall>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<EquipmentType>.write = _Write_EquipmentType;
		Writer<OrbitalLaserState>.write = _Write_OrbitalLaserState;
		Writer<PlayerInfo>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<SyncDirection>.write = _Write_Mirror_002ESyncDirection;
		Writer<Checkpoint>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<CourseManager.PlayerState>.write = _Write_CourseManager_002FPlayerState;
		Writer<PlayerMatchResolution>.write = _Write_PlayerMatchResolution;
		Writer<StrokesUnderParType>.write = _Write_StrokesUnderParType;
		Writer<CourseManager.PlayerPair>.write = _Write_CourseManager_002FPlayerPair;
		Writer<AnnouncerLine>.write = _Write_AnnouncerLine;
		Writer<List<AnnouncerLine>>.write = _Write_System_002ECollections_002EGeneric_002EList_00601_003CAnnouncerLine_003E;
		Writer<MatchState>.write = _Write_MatchState;
		Writer<GolfHole>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<BallOutOfBoundsReturnState>.write = _Write_BallOutOfBoundsReturnState;
		Writer<Hittable>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<PlayerMovement>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<ItemType>.write = _Write_ItemType;
		Writer<ItemUseId>.write = _Write_ItemUseId;
		Writer<PlayerInventory>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<SwingProjectileState>.write = _Write_SwingProjectileState;
		Writer<BoundsState>.write = _Write_BoundsState;
		Writer<EliminationReason>.write = _Write_EliminationReason;
		Writer<GolfCartInfo>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<PlayerTextPopupType>.write = _Write_PlayerTextPopupType;
		Writer<KnockoutType>.write = _Write_KnockoutType;
		Writer<GolfCartSeat>.write = _Write_GolfCartSeat;
		Writer<InventorySlot>.write = _Write_InventorySlot;
		Writer<LandmineArmType>.write = _Write_LandmineArmType;
		Writer<ThrownUsedItemType>.write = _Write_ThrownUsedItemType;
		Writer<GroundTerrainType>.write = _Write_GroundTerrainType;
		Writer<TerrainLayer>.write = _Write_TerrainLayer;
		Writer<KnockoutState>.write = _Write_KnockoutState;
		Writer<PlayerMovement.KnockedOutVfxData>.write = _Write_PlayerMovement_002FKnockedOutVfxData;
		Writer<StatusEffect>.write = _Write_StatusEffect;
		Writer<DivingState>.write = _Write_DivingState;
		Writer<RespawnState>.write = _Write_RespawnState;
		Writer<HoleOverviewCameraUi.State>.write = _Write_HoleOverviewCameraUi_002FState;
		Writer<InfoFeed.GenericMessageData>.write = _Write_InfoFeed_002FGenericMessageData;
		Writer<InfoFeedIconSettings.Type>.write = _Write_InfoFeedIconSettings_002FType;
		Writer<InfoFeed.FinishedHoleMessageData>.write = _Write_InfoFeed_002FFinishedHoleMessageData;
		Writer<InfoFeed.ScoredOnDrivingRangeMessageData>.write = _Write_InfoFeed_002FScoredOnDrivingRangeMessageData;
		Writer<InfoFeed.StrokesMessageData>.write = _Write_InfoFeed_002FStrokesMessageData;
		Writer<InfoFeed.ChipInMessageData>.write = _Write_InfoFeed_002FChipInMessageData;
		Writer<InfoFeed.SpeedrunMessageData>.write = _Write_InfoFeed_002FSpeedrunMessageData;
		Writer<InfoFeed.DominatingMessageData>.write = _Write_InfoFeed_002FDominatingMessageData;
		Writer<InfoFeed.RevengeMessageData>.write = _Write_InfoFeed_002FRevengeMessageData;
		Writer<LobbyMode>.write = _Write_LobbyMode;
		Writer<MatchSetupRules.Rule>.write = _Write_MatchSetupRules_002FRule;
		Writer<MatchSetupRules.ItemPoolId>.write = _Write_MatchSetupRules_002FItemPoolId;
		Writer<MatchSetupRules.Preset>.write = _Write_MatchSetupRules_002FPreset;
		Writer<VfxType>.write = _Write_VfxType;
		Writer<VfxManager.GunShotHitVfxData>.write = _Write_VfxManager_002FGunShotHitVfxData;
		Reader<byte>.read = NetworkReaderExtensions.ReadByte;
		Reader<byte?>.read = NetworkReaderExtensions.ReadByteNullable;
		Reader<sbyte>.read = NetworkReaderExtensions.ReadSByte;
		Reader<sbyte?>.read = NetworkReaderExtensions.ReadSByteNullable;
		Reader<char>.read = NetworkReaderExtensions.ReadChar;
		Reader<char?>.read = NetworkReaderExtensions.ReadCharNullable;
		Reader<bool>.read = NetworkReaderExtensions.ReadBool;
		Reader<bool?>.read = NetworkReaderExtensions.ReadBoolNullable;
		Reader<short>.read = NetworkReaderExtensions.ReadShort;
		Reader<short?>.read = NetworkReaderExtensions.ReadShortNullable;
		Reader<ushort>.read = NetworkReaderExtensions.ReadUShort;
		Reader<ushort?>.read = NetworkReaderExtensions.ReadUShortNullable;
		Reader<int>.read = NetworkReaderExtensions.ReadVarInt;
		Reader<int?>.read = NetworkReaderExtensions.ReadIntNullable;
		Reader<uint>.read = NetworkReaderExtensions.ReadVarUInt;
		Reader<uint?>.read = NetworkReaderExtensions.ReadUIntNullable;
		Reader<long>.read = NetworkReaderExtensions.ReadVarLong;
		Reader<long?>.read = NetworkReaderExtensions.ReadLongNullable;
		Reader<ulong>.read = NetworkReaderExtensions.ReadVarULong;
		Reader<ulong?>.read = NetworkReaderExtensions.ReadULongNullable;
		Reader<float>.read = NetworkReaderExtensions.ReadFloat;
		Reader<float?>.read = NetworkReaderExtensions.ReadFloatNullable;
		Reader<double>.read = NetworkReaderExtensions.ReadDouble;
		Reader<double?>.read = NetworkReaderExtensions.ReadDoubleNullable;
		Reader<decimal>.read = NetworkReaderExtensions.ReadDecimal;
		Reader<decimal?>.read = NetworkReaderExtensions.ReadDecimalNullable;
		Reader<Half>.read = NetworkReaderExtensions.ReadHalf;
		Reader<string>.read = NetworkReaderExtensions.ReadString;
		Reader<byte[]>.read = NetworkReaderExtensions.ReadBytesAndSize;
		Reader<ArraySegment<byte>>.read = NetworkReaderExtensions.ReadArraySegmentAndSize;
		Reader<Vector2>.read = NetworkReaderExtensions.ReadVector2;
		Reader<Vector2?>.read = NetworkReaderExtensions.ReadVector2Nullable;
		Reader<Vector3>.read = NetworkReaderExtensions.ReadVector3;
		Reader<Vector3?>.read = NetworkReaderExtensions.ReadVector3Nullable;
		Reader<Vector4>.read = NetworkReaderExtensions.ReadVector4;
		Reader<Vector4?>.read = NetworkReaderExtensions.ReadVector4Nullable;
		Reader<Vector2Int>.read = NetworkReaderExtensions.ReadVector2Int;
		Reader<Vector2Int?>.read = NetworkReaderExtensions.ReadVector2IntNullable;
		Reader<Vector3Int>.read = NetworkReaderExtensions.ReadVector3Int;
		Reader<Vector3Int?>.read = NetworkReaderExtensions.ReadVector3IntNullable;
		Reader<Color>.read = NetworkReaderExtensions.ReadColor;
		Reader<Color?>.read = NetworkReaderExtensions.ReadColorNullable;
		Reader<Color32>.read = NetworkReaderExtensions.ReadColor32;
		Reader<Color32?>.read = NetworkReaderExtensions.ReadColor32Nullable;
		Reader<Quaternion>.read = NetworkReaderExtensions.ReadQuaternion;
		Reader<Quaternion?>.read = NetworkReaderExtensions.ReadQuaternionNullable;
		Reader<Rect>.read = NetworkReaderExtensions.ReadRect;
		Reader<Rect?>.read = NetworkReaderExtensions.ReadRectNullable;
		Reader<Plane>.read = NetworkReaderExtensions.ReadPlane;
		Reader<Plane?>.read = NetworkReaderExtensions.ReadPlaneNullable;
		Reader<Ray>.read = NetworkReaderExtensions.ReadRay;
		Reader<Ray?>.read = NetworkReaderExtensions.ReadRayNullable;
		Reader<LayerMask>.read = NetworkReaderExtensions.ReadLayerMask;
		Reader<LayerMask?>.read = NetworkReaderExtensions.ReadLayerMaskNullable;
		Reader<Matrix4x4>.read = NetworkReaderExtensions.ReadMatrix4x4;
		Reader<Matrix4x4?>.read = NetworkReaderExtensions.ReadMatrix4x4Nullable;
		Reader<Guid>.read = NetworkReaderExtensions.ReadGuid;
		Reader<Guid?>.read = NetworkReaderExtensions.ReadGuidNullable;
		Reader<NetworkIdentity>.read = NetworkReaderExtensions.ReadNetworkIdentity;
		Reader<NetworkBehaviour>.read = NetworkReaderExtensions.ReadNetworkBehaviour;
		Reader<NetworkBehaviourSyncVar>.read = NetworkReaderExtensions.ReadNetworkBehaviourSyncVar;
		Reader<Transform>.read = NetworkReaderExtensions.ReadTransform;
		Reader<GameObject>.read = NetworkReaderExtensions.ReadGameObject;
		Reader<Uri>.read = NetworkReaderExtensions.ReadUri;
		Reader<Texture2D>.read = NetworkReaderExtensions.ReadTexture2D;
		Reader<Sprite>.read = NetworkReaderExtensions.ReadSprite;
		Reader<DateTime>.read = NetworkReaderExtensions.ReadDateTime;
		Reader<DateTime?>.read = NetworkReaderExtensions.ReadDateTimeNullable;
		Reader<TimeSnapshotMessage>.read = _Read_Mirror_002ETimeSnapshotMessage;
		Reader<ReadyMessage>.read = _Read_Mirror_002EReadyMessage;
		Reader<NotReadyMessage>.read = _Read_Mirror_002ENotReadyMessage;
		Reader<AddPlayerMessage>.read = _Read_Mirror_002EAddPlayerMessage;
		Reader<SceneMessage>.read = _Read_Mirror_002ESceneMessage;
		Reader<SceneOperation>.read = _Read_Mirror_002ESceneOperation;
		Reader<CommandMessage>.read = _Read_Mirror_002ECommandMessage;
		Reader<RpcMessage>.read = _Read_Mirror_002ERpcMessage;
		Reader<SpawnMessage>.read = _Read_Mirror_002ESpawnMessage;
		Reader<SpawnFlags>.read = _Read_Mirror_002ESpawnFlags;
		Reader<ChangeOwnerMessage>.read = _Read_Mirror_002EChangeOwnerMessage;
		Reader<ObjectSpawnStartedMessage>.read = _Read_Mirror_002EObjectSpawnStartedMessage;
		Reader<ObjectSpawnFinishedMessage>.read = _Read_Mirror_002EObjectSpawnFinishedMessage;
		Reader<ObjectDestroyMessage>.read = _Read_Mirror_002EObjectDestroyMessage;
		Reader<ObjectHideMessage>.read = _Read_Mirror_002EObjectHideMessage;
		Reader<EntityStateMessage>.read = _Read_Mirror_002EEntityStateMessage;
		Reader<EntityStateMessageUnreliableBaseline>.read = _Read_Mirror_002EEntityStateMessageUnreliableBaseline;
		Reader<EntityStateMessageUnreliableDelta>.read = _Read_Mirror_002EEntityStateMessageUnreliableDelta;
		Reader<NetworkPingMessage>.read = _Read_Mirror_002ENetworkPingMessage;
		Reader<NetworkPongMessage>.read = _Read_Mirror_002ENetworkPongMessage;
		Reader<SyncData>.read = SyncDataReaderWriter.ReadSyncData;
		Reader<PredictedGolfCartSyncData>.read = PredictedSyncDataReadWrite.ReadPredictedGolfCartSyncData;
		Reader<PredictedSyncData>.read = PredictedGolfCartSyncDataReadWrite.ReadPredictedSyncData;
		Reader<ServerRequest>.read = _Read_Mirror_002EDiscovery_002EServerRequest;
		Reader<ServerResponse>.read = _Read_Mirror_002EDiscovery_002EServerResponse;
		Reader<BClientAuthenticator.ClientCanceledAuthenticationMessage>.read = _Read_BClientAuthenticator_002FClientCanceledAuthenticationMessage;
		Reader<BClientAuthenticator.IsPasswordRequiredRequestMessage>.read = _Read_BClientAuthenticator_002FIsPasswordRequiredRequestMessage;
		Reader<BClientAuthenticator.IsPasswordRequiredResponseMessage>.read = _Read_BClientAuthenticator_002FIsPasswordRequiredResponseMessage;
		Reader<BClientAuthenticator.AuthenticationRequestMessage>.read = _Read_BClientAuthenticator_002FAuthenticationRequestMessage;
		Reader<BClientAuthenticator.AuthenticationResponseMessage>.read = _Read_BClientAuthenticator_002FAuthenticationResponseMessage;
		Reader<BClientAuthenticator.AuthenticationResponse>.read = _Read_BClientAuthenticator_002FAuthenticationResponse;
		Reader<MaxPlayersUpdatedMessage>.read = _Read_MaxPlayersUpdatedMessage;
		Reader<SetPlayerGuidMessage>.read = _Read_SetPlayerGuidMessage;
		Reader<DisconnectReasonMessage>.read = _Read_DisconnectReasonMessage;
		Reader<DisconnectReason>.read = _Read_DisconnectReason;
		Reader<SetStandardCourseMessage>.read = _Read_SetStandardCourseMessage;
		Reader<SetNonStandardCourseMessage>.read = _Read_SetNonStandardCourseMessage;
		Reader<int[]>.read = _Read_System_002EInt32_005B_005D;
		Reader<StartMatchMessage>.read = _Read_StartMatchMessage;
		Reader<RemotePlayerDisconnectMessage>.read = _Read_RemotePlayerDisconnectMessage;
		Reader<TriggerNetworkClientToServerExceptionMessage>.read = _Read_TriggerNetworkClientToServerExceptionMessage;
		Reader<RequestNetworkServerToClientExceptionMessage>.read = _Read_RequestNetworkServerToClientExceptionMessage;
		Reader<TriggerNetworkServerToClientExceptionMessage>.read = _Read_TriggerNetworkServerToClientExceptionMessage;
		Reader<ServerRequestFriendCheckMessage>.read = _Read_ServerRequestFriendCheckMessage;
		Reader<ClientFriendCheckConfirmationMessage>.read = _Read_ClientFriendCheckConfirmationMessage;
		Reader<PlayerCosmeticsSwitcher.CosmeticKey>.read = _Read_PlayerCosmeticsSwitcher_002FCosmeticKey;
		Reader<VictoryDance>.read = _Read_VictoryDance;
		Reader<PlayerGolfer>.read = NetworkReaderExtensions.ReadNetworkBehaviour<PlayerGolfer>;
		Reader<GolfBall>.read = NetworkReaderExtensions.ReadNetworkBehaviour<GolfBall>;
		Reader<EquipmentType>.read = _Read_EquipmentType;
		Reader<OrbitalLaserState>.read = _Read_OrbitalLaserState;
		Reader<PlayerInfo>.read = NetworkReaderExtensions.ReadNetworkBehaviour<PlayerInfo>;
		Reader<SyncDirection>.read = _Read_Mirror_002ESyncDirection;
		Reader<Checkpoint>.read = NetworkReaderExtensions.ReadNetworkBehaviour<Checkpoint>;
		Reader<CourseManager.PlayerState>.read = _Read_CourseManager_002FPlayerState;
		Reader<PlayerMatchResolution>.read = _Read_PlayerMatchResolution;
		Reader<StrokesUnderParType>.read = _Read_StrokesUnderParType;
		Reader<CourseManager.PlayerPair>.read = _Read_CourseManager_002FPlayerPair;
		Reader<AnnouncerLine>.read = _Read_AnnouncerLine;
		Reader<List<AnnouncerLine>>.read = _Read_System_002ECollections_002EGeneric_002EList_00601_003CAnnouncerLine_003E;
		Reader<MatchState>.read = _Read_MatchState;
		Reader<GolfHole>.read = NetworkReaderExtensions.ReadNetworkBehaviour<GolfHole>;
		Reader<BallOutOfBoundsReturnState>.read = _Read_BallOutOfBoundsReturnState;
		Reader<Hittable>.read = NetworkReaderExtensions.ReadNetworkBehaviour<Hittable>;
		Reader<PlayerMovement>.read = NetworkReaderExtensions.ReadNetworkBehaviour<PlayerMovement>;
		Reader<ItemType>.read = _Read_ItemType;
		Reader<ItemUseId>.read = _Read_ItemUseId;
		Reader<PlayerInventory>.read = NetworkReaderExtensions.ReadNetworkBehaviour<PlayerInventory>;
		Reader<SwingProjectileState>.read = _Read_SwingProjectileState;
		Reader<BoundsState>.read = _Read_BoundsState;
		Reader<EliminationReason>.read = _Read_EliminationReason;
		Reader<GolfCartInfo>.read = NetworkReaderExtensions.ReadNetworkBehaviour<GolfCartInfo>;
		Reader<PlayerTextPopupType>.read = _Read_PlayerTextPopupType;
		Reader<KnockoutType>.read = _Read_KnockoutType;
		Reader<GolfCartSeat>.read = _Read_GolfCartSeat;
		Reader<InventorySlot>.read = _Read_InventorySlot;
		Reader<LandmineArmType>.read = _Read_LandmineArmType;
		Reader<ThrownUsedItemType>.read = _Read_ThrownUsedItemType;
		Reader<GroundTerrainType>.read = _Read_GroundTerrainType;
		Reader<TerrainLayer>.read = _Read_TerrainLayer;
		Reader<KnockoutState>.read = _Read_KnockoutState;
		Reader<PlayerMovement.KnockedOutVfxData>.read = _Read_PlayerMovement_002FKnockedOutVfxData;
		Reader<StatusEffect>.read = _Read_StatusEffect;
		Reader<DivingState>.read = _Read_DivingState;
		Reader<RespawnState>.read = _Read_RespawnState;
		Reader<HoleOverviewCameraUi.State>.read = _Read_HoleOverviewCameraUi_002FState;
		Reader<InfoFeed.GenericMessageData>.read = _Read_InfoFeed_002FGenericMessageData;
		Reader<InfoFeedIconSettings.Type>.read = _Read_InfoFeedIconSettings_002FType;
		Reader<InfoFeed.FinishedHoleMessageData>.read = _Read_InfoFeed_002FFinishedHoleMessageData;
		Reader<InfoFeed.ScoredOnDrivingRangeMessageData>.read = _Read_InfoFeed_002FScoredOnDrivingRangeMessageData;
		Reader<InfoFeed.StrokesMessageData>.read = _Read_InfoFeed_002FStrokesMessageData;
		Reader<InfoFeed.ChipInMessageData>.read = _Read_InfoFeed_002FChipInMessageData;
		Reader<InfoFeed.SpeedrunMessageData>.read = _Read_InfoFeed_002FSpeedrunMessageData;
		Reader<InfoFeed.DominatingMessageData>.read = _Read_InfoFeed_002FDominatingMessageData;
		Reader<InfoFeed.RevengeMessageData>.read = _Read_InfoFeed_002FRevengeMessageData;
		Reader<LobbyMode>.read = _Read_LobbyMode;
		Reader<MatchSetupRules.Rule>.read = _Read_MatchSetupRules_002FRule;
		Reader<MatchSetupRules.ItemPoolId>.read = _Read_MatchSetupRules_002FItemPoolId;
		Reader<MatchSetupRules.Preset>.read = _Read_MatchSetupRules_002FPreset;
		Reader<VfxType>.read = _Read_VfxType;
		Reader<VfxManager.GunShotHitVfxData>.read = _Read_VfxManager_002FGunShotHitVfxData;
	}
}
