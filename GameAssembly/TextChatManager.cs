using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class TextChatManager : SingletonNetworkBehaviour<TextChatManager>
{
	public int maxMessageLength = 160;

	private readonly AntiCheatPerPlayerRateChecker serverSendMessageCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Send text chat message", 0.1f, 5, 10, 1f);

	public override void OnStartClient()
	{
		SingletonBehaviour<TextChatUi>.Instance.SetMessageLimit(maxMessageLength);
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
		PlayerInfo value;
		if (sender == null)
		{
			value = GameManager.LocalPlayerInfo;
		}
		else
		{
			GameManager.RemotePlayerPerConnectionId.TryGetValue(sender.connectionId, out value);
		}
		if (!(value == null))
		{
			if (message.Length > maxMessageLength)
			{
				message = message.Substring(0, maxMessageLength);
			}
			GameManager.FilterProfanity(message, out message);
			RpcMessage(message, value);
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
		if (!(sender == null) && !GameSettings.All.General.MuteChat && !sender.IsBlockedOnSteam())
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

	static TextChatManager()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(TextChatManager), "System.Void TextChatManager::CmdSendMessageInternal(System.String,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdSendMessageInternal__String__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(TextChatManager), "System.Void TextChatManager::RpcMessage(System.String,PlayerInfo)", InvokeUserCode_RpcMessage__String__PlayerInfo);
	}
}
