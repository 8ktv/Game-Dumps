using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class TextChatManager : SingletonNetworkBehaviour<TextChatManager>
{
	public int maxMessageLength = 160;

	private readonly AntiCheatPerPlayerRateChecker serverSendMessageCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Send text chat message", 0.1f, 5, 10, 1f);

	private readonly AntiCheatPerPlayerRateChecker serverAchievementCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Achievement unlocked inform", 0.1f, 5, 10, 1f);

	public override void OnStartClient()
	{
		SingletonBehaviour<TextChatUi>.Instance.SetMessageLimit(maxMessageLength);
		GameManager.AchievementsManager.AchievementUnlocked += AchievementUnlocked;
	}

	public override void OnStopClient()
	{
		GameManager.AchievementsManager.AchievementUnlocked -= AchievementUnlocked;
	}

	public static void SendChatMessage(string message)
	{
		if (!SingletonNetworkBehaviour<TextChatManager>.HasInstance)
		{
			return;
		}
		if (GameSettings.All.General.MuteChat)
		{
			TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_ChatMuted, GameManager.UiSettings.TextRedHighlightStartTag + "<b>", "</b>" + GameManager.UiSettings.TextColorEndTag));
			return;
		}
		if (message.Length > SingletonNetworkBehaviour<TextChatManager>.Instance.maxMessageLength)
		{
			message = message.Substring(0, SingletonNetworkBehaviour<TextChatManager>.Instance.maxMessageLength);
		}
		GameManager.FilterProfanity(message, out message);
		SingletonNetworkBehaviour<TextChatManager>.Instance.CmdSendMessageInternal(message);
	}

	private PlayerInfo GetPlayerFromConnection(NetworkConnectionToClient sender)
	{
		if (sender == null)
		{
			return GameManager.LocalPlayerInfo;
		}
		GameManager.RemotePlayerPerConnectionId.TryGetValue(sender.connectionId, out var value);
		return value;
	}

	[Command(requiresAuthority = false)]
	private void CmdSendMessageInternal(string message, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdSendMessageInternal__String__NetworkConnectionToClient(message, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(message);
		SendCommandInternal("System.Void TextChatManager::CmdSendMessageInternal(System.String,Mirror.NetworkConnectionToClient)", 2136655064, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcMessage(string message, PlayerInfo sender)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(message);
		writer.WriteNetworkBehaviour(sender);
		SendRPCInternal("System.Void TextChatManager::RpcMessage(System.String,PlayerInfo)", 210738538, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command(requiresAuthority = false)]
	private void CmdInformAchievementUnlocked(AchievementId id, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformAchievementUnlocked__AchievementId__NetworkConnectionToClient(id, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_AchievementId(writer, id);
		SendCommandInternal("System.Void TextChatManager::CmdInformAchievementUnlocked(AchievementId,Mirror.NetworkConnectionToClient)", 81091047, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcAchievementUnlockedMessage(AchievementId id, PlayerInfo player)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_AchievementId(writer, id);
		writer.WriteNetworkBehaviour(player);
		SendRPCInternal("System.Void TextChatManager::RpcAchievementUnlockedMessage(AchievementId,PlayerInfo)", -1508446620, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void AchievementUnlocked(AchievementId id)
	{
		CmdInformAchievementUnlocked(id);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdSendMessageInternal__String__NetworkConnectionToClient(string message, NetworkConnectionToClient sender)
	{
		if (!serverSendMessageCommandRateLimiter.RegisterHit(sender) || message.Length == 0)
		{
			return;
		}
		PlayerInfo playerFromConnection = GetPlayerFromConnection(sender);
		if (!(playerFromConnection == null))
		{
			if (message.Length > maxMessageLength)
			{
				message = message.Substring(0, maxMessageLength);
			}
			GameManager.FilterProfanity(message, out message);
			RpcMessage(message, playerFromConnection);
		}
	}

	protected static void InvokeUserCode_CmdSendMessageInternal__String__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendMessageInternal called on client.");
		}
		else
		{
			((TextChatManager)obj).UserCode_CmdSendMessageInternal__String__NetworkConnectionToClient(reader.ReadString(), senderConnection);
		}
	}

	protected void UserCode_RpcMessage__String__PlayerInfo(string message, PlayerInfo sender)
	{
		if (!(sender == null) && !GameSettings.All.General.MuteChat && !sender.IsBlockedOnSteam() && (sender.isLocalPlayer || !PlayerVoiceChat.GetPlayerStatus(sender.PlayerId.Guid).isMuted))
		{
			string text = GameManager.UiSettings.ApplyColorTag(sender.PlayerId.PlayerNameNoRichText, TextHighlight.Regular);
			GameManager.FilterProfanity(message, out message);
			message = GameManager.RichTextNoParse(message);
			TextChatUi.ShowMessage(text + ": " + message);
		}
	}

	protected static void InvokeUserCode_RpcMessage__String__PlayerInfo(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcMessage called on server.");
		}
		else
		{
			((TextChatManager)obj).UserCode_RpcMessage__String__PlayerInfo(reader.ReadString(), reader.ReadNetworkBehaviour<PlayerInfo>());
		}
	}

	protected void UserCode_CmdInformAchievementUnlocked__AchievementId__NetworkConnectionToClient(AchievementId id, NetworkConnectionToClient sender)
	{
		if (serverAchievementCommandRateLimiter.RegisterHit(sender))
		{
			PlayerInfo playerFromConnection = GetPlayerFromConnection(sender);
			if (!(playerFromConnection == null))
			{
				RpcAchievementUnlockedMessage(id, playerFromConnection);
			}
		}
	}

	protected static void InvokeUserCode_CmdInformAchievementUnlocked__AchievementId__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformAchievementUnlocked called on client.");
		}
		else
		{
			((TextChatManager)obj).UserCode_CmdInformAchievementUnlocked__AchievementId__NetworkConnectionToClient(GeneratedNetworkCode._Read_AchievementId(reader), senderConnection);
		}
	}

	protected void UserCode_RpcAchievementUnlockedMessage__AchievementId__PlayerInfo(AchievementId id, PlayerInfo player)
	{
		if (!(player == null))
		{
			string arg = GameManager.UiSettings.ApplyColorTag(player.PlayerId.PlayerNameNoRichText, TextHighlight.Regular);
			string text = LocalizationManager.GetString(StringTable.Achievements, $"ACHIEVEMENT_Title_{id}");
			text = GameManager.UiSettings.ApplyColorTag(text, TextHighlight.Red);
			TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_AchievementUnlocked, arg, text));
		}
	}

	protected static void InvokeUserCode_RpcAchievementUnlockedMessage__AchievementId__PlayerInfo(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcAchievementUnlockedMessage called on server.");
		}
		else
		{
			((TextChatManager)obj).UserCode_RpcAchievementUnlockedMessage__AchievementId__PlayerInfo(GeneratedNetworkCode._Read_AchievementId(reader), reader.ReadNetworkBehaviour<PlayerInfo>());
		}
	}

	static TextChatManager()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(TextChatManager), "System.Void TextChatManager::CmdSendMessageInternal(System.String,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdSendMessageInternal__String__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(TextChatManager), "System.Void TextChatManager::CmdInformAchievementUnlocked(AchievementId,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdInformAchievementUnlocked__AchievementId__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(TextChatManager), "System.Void TextChatManager::RpcMessage(System.String,PlayerInfo)", InvokeUserCode_RpcMessage__String__PlayerInfo);
		RemoteProcedureCalls.RegisterRpc(typeof(TextChatManager), "System.Void TextChatManager::RpcAchievementUnlockedMessage(AchievementId,PlayerInfo)", InvokeUserCode_RpcAchievementUnlockedMessage__AchievementId__PlayerInfo);
	}
}
