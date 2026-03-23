using System.Collections.Generic;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class InfoFeed : SingletonNetworkBehaviour<InfoFeed>
{
	public interface IMessageData
	{
	}

	public struct GenericMessageData : IMessageData
	{
		public string preIconText;

		public string postIconText;

		public InfoFeedIconSettings.Type icon1;

		public InfoFeedIconSettings.Type icon2;

		public GenericMessageData(string preIconText, string postIconText, InfoFeedIconSettings.Type icon1, InfoFeedIconSettings.Type icon2)
		{
			this.preIconText = preIconText;
			this.postIconText = postIconText;
			this.icon1 = icon1;
			this.icon2 = icon2;
		}

		public GenericMessageData(string text, InfoFeedIconSettings.Type icon1, InfoFeedIconSettings.Type icon2)
		{
			preIconText = text;
			postIconText = null;
			this.icon1 = icon1;
			this.icon2 = icon2;
		}

		public GenericMessageData(string preIconText, string postIconText, InfoFeedIconSettings.Type icon)
		{
			this.preIconText = preIconText;
			this.postIconText = postIconText;
			icon1 = icon;
			icon2 = InfoFeedIconSettings.Type.None;
		}

		public GenericMessageData(string text, InfoFeedIconSettings.Type icon)
		{
			preIconText = text;
			postIconText = null;
			icon1 = icon;
			icon2 = InfoFeedIconSettings.Type.None;
		}

		public GenericMessageData(string text)
		{
			preIconText = text;
			postIconText = null;
			icon1 = InfoFeedIconSettings.Type.None;
			icon2 = InfoFeedIconSettings.Type.None;
		}
	}

	public struct FinishedHoleMessageData : IMessageData
	{
		public string playerName;

		public int displayPlacement;

		public FinishedHoleMessageData(string playerName, int displayPlacement)
		{
			this.playerName = playerName;
			this.displayPlacement = displayPlacement;
		}
	}

	public struct ScoredOnDrivingRangeMessageData : IMessageData
	{
		public string playerName;

		public ScoredOnDrivingRangeMessageData(string playerName)
		{
			this.playerName = playerName;
		}
	}

	public struct StrokesMessageData : IMessageData
	{
		public string playerName;

		public StrokesUnderParType strokesUnderParType;

		public int strokesUnderPar;

		public StrokesMessageData(string playerName, StrokesUnderParType strokesUnderParType, int strokesUnderPar)
		{
			this.playerName = playerName;
			this.strokesUnderParType = strokesUnderParType;
			this.strokesUnderPar = strokesUnderPar;
		}
	}

	public struct ChipInMessageData : IMessageData
	{
		public string playerName;

		public float distance;

		public ChipInMessageData(string playerName, float distance)
		{
			this.playerName = playerName;
			this.distance = distance;
		}
	}

	public struct SpeedrunMessageData : IMessageData
	{
		public string playerName;

		public float time;

		public SpeedrunMessageData(string playerName, float time)
		{
			this.playerName = playerName;
			this.time = time;
		}
	}

	public struct DominatingMessageData : IMessageData
	{
		public string dominatingPlayerName;

		public string dominatedPlayerName;

		public DominatingMessageData(string dominatingPlayerName, string dominatedPlayerName)
		{
			this.dominatingPlayerName = dominatingPlayerName;
			this.dominatedPlayerName = dominatedPlayerName;
		}
	}

	public struct RevengeMessageData : IMessageData
	{
		public string previouslyDominatedPlayerName;

		public string previouslyDominatingPlayerName;

		public RevengeMessageData(string previouslyDominatedPlayerName, string previouslyDominatingPlayerName)
		{
			this.previouslyDominatedPlayerName = previouslyDominatedPlayerName;
			this.previouslyDominatingPlayerName = previouslyDominatingPlayerName;
		}
	}

	[SerializeField]
	private InfoFeedMessage messagePrefab;

	[SerializeField]
	private Transform messageContainer;

	[SerializeField]
	private InfoFeedIconSettings iconSettings;

	[SerializeField]
	[Min(1f)]
	private int maxConcurrentMessageCount = 5;

	[SerializeField]
	[Min(0f)]
	private float messageDuration = 3f;

	[SerializeField]
	[Min(0f)]
	private float messageSlideInDuration = 0.5f;

	[SerializeField]
	[Min(0f)]
	private float messageFadeOutDuration = 1.5f;

	[SerializeField]
	private Color player0Color;

	[SerializeField]
	private Color player1Color;

	private readonly Queue<IMessageData> queuedMessages = new Queue<IMessageData>();

	private readonly HashSet<InfoFeedMessage> shownMessages = new HashSet<InfoFeedMessage>();

	private static Transform messagePoolParent;

	private static readonly Stack<InfoFeedMessage> messagePool;

	public static Color Player0Color
	{
		get
		{
			if (!SingletonNetworkBehaviour<InfoFeed>.HasInstance)
			{
				return Color.white;
			}
			return SingletonNetworkBehaviour<InfoFeed>.Instance.player0Color;
		}
	}

	public static Color Player1Color
	{
		get
		{
			if (!SingletonNetworkBehaviour<InfoFeed>.HasInstance)
			{
				return Color.white;
			}
			return SingletonNetworkBehaviour<InfoFeed>.Instance.player1Color;
		}
	}

	public static InfoFeedIconSettings IconSettings
	{
		get
		{
			if (!SingletonNetworkBehaviour<InfoFeed>.HasInstance)
			{
				return null;
			}
			return SingletonNetworkBehaviour<InfoFeed>.Instance.iconSettings;
		}
	}

	public static float MessageDuration
	{
		get
		{
			if (!SingletonNetworkBehaviour<InfoFeed>.HasInstance)
			{
				return 0f;
			}
			return SingletonNetworkBehaviour<InfoFeed>.Instance.messageDuration;
		}
	}

	public static float MessageSlideInDuration
	{
		get
		{
			if (!SingletonNetworkBehaviour<InfoFeed>.HasInstance)
			{
				return 0f;
			}
			return SingletonNetworkBehaviour<InfoFeed>.Instance.messageSlideInDuration;
		}
	}

	public static float MessageFadeOutDuration
	{
		get
		{
			if (!SingletonNetworkBehaviour<InfoFeed>.HasInstance)
			{
				return 0f;
			}
			return SingletonNetworkBehaviour<InfoFeed>.Instance.messageFadeOutDuration;
		}
	}

	[CCommand("showTestInfoFeedMessage", "", false, false)]
	private static void ShowTestMessage(int testMessageIndex = -1, int testParam0 = -1)
	{
		if (GameManager.LocalPlayerAsGolfer == null)
		{
			return;
		}
		if (testMessageIndex < 0)
		{
			testMessageIndex = Random.Range(0, 10);
		}
		switch (testMessageIndex)
		{
		case 0:
			ShowFinishedHoleMessage(GameManager.LocalPlayerInfo, 0);
			break;
		case 1:
			ShowStrokesMessage(GameManager.LocalPlayerInfo, StrokesUnderParType.HoleInOne, Random.Range(0, 4));
			break;
		case 2:
			ShowChipInMessage(GameManager.LocalPlayerInfo, Random.Range(1, 10));
			break;
		case 3:
			ShowSpeedrunMessage(GameManager.LocalPlayerInfo, Random.Range(10, 60));
			break;
		case 4:
		{
			KnockoutType knockoutType2 = (KnockoutType)Random.Range(0, 18);
			if (testParam0 >= 0)
			{
				knockoutType2 = (KnockoutType)testParam0;
			}
			ShowKnockoutMessage(GameManager.LocalPlayerInfo, GameManager.LocalPlayerInfo, knockoutType2);
			break;
		}
		case 5:
		{
			KnockoutType knockoutType = (KnockoutType)Random.Range(0, 18);
			if (testParam0 >= 0)
			{
				knockoutType = (KnockoutType)testParam0;
			}
			ShowSelfKnockoutMessage(GameManager.LocalPlayerInfo, knockoutType);
			break;
		}
		case 6:
			ShowEliminationMessage(GameManager.LocalPlayerInfo, GameManager.LocalPlayerInfo, (EliminationReason)Random.Range(1, 26));
			break;
		case 7:
			ShowSelfEliminationMessage(GameManager.LocalPlayerInfo, (EliminationReason)Random.Range(1, 26));
			break;
		case 8:
			ShowDominationMessage(GameManager.LocalPlayerInfo, GameManager.LocalPlayerInfo);
			break;
		case 9:
			ShowRevengeMessage(GameManager.LocalPlayerInfo, GameManager.LocalPlayerInfo);
			break;
		default:
			Debug.LogWarning("No test message at this index!!!");
			break;
		}
	}

	public static void ShowFinishedHoleMessage(PlayerInfo player, int placement)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowFinishedHoleMessageInternal(player, placement);
		}
	}

	public static void ShowScoredOnDrivingRange(PlayerInfo player)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowScoredOnDrivingRangeInternal(player);
		}
	}

	public static void ShowStrokesMessage(PlayerInfo player, StrokesUnderParType strokesUnderParType, int strokesUnderPar)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowStrokesMessageInternal(player, strokesUnderParType, strokesUnderPar);
		}
	}

	public static void ShowChipInMessage(PlayerInfo player, float chipInDistance)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowChipInMessageInternal(player, chipInDistance);
		}
	}

	public static void ShowSpeedrunMessage(PlayerInfo player, float time)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowSpeedrunMessageInternal(player, time);
		}
	}

	public static void ShowKnockoutMessage(PlayerInfo responsiblePlayer, PlayerInfo knockedOutPlayer, KnockoutType knockoutType)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowKnockoutMessageInternal(responsiblePlayer, knockedOutPlayer, knockoutType);
		}
	}

	public static void ShowSelfKnockoutMessage(PlayerInfo knockedOutPlayer, KnockoutType knockoutType)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowSelfKnockoutMessageInternal(knockedOutPlayer, knockoutType);
		}
	}

	public static void ShowEliminationMessage(PlayerInfo responsiblePlayer, PlayerInfo eliminatedPlayer, EliminationReason eliminationReason)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowEliminationMessageInternal(responsiblePlayer, eliminatedPlayer, eliminationReason);
		}
	}

	public static void ShowSelfEliminationMessage(PlayerInfo knockedOutPlayer, EliminationReason eliminationReason)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowSelfEliminationMessageInternal(knockedOutPlayer, eliminationReason);
		}
	}

	public static void ShowDominationMessage(PlayerInfo dominatingPlayer, PlayerInfo dominatedPlayer)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowDominationMessageInternal(dominatingPlayer, dominatedPlayer);
		}
	}

	public static void ShowRevengeMessage(PlayerInfo previouslyDominatedPlayer, PlayerInfo previouslyDominatingPlayer)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.ShowRevengeMessageInternal(previouslyDominatedPlayer, previouslyDominatingPlayer);
		}
	}

	public static void InformMessageDisappeared(InfoFeedMessage message)
	{
		if (SingletonNetworkBehaviour<InfoFeed>.HasInstance)
		{
			SingletonNetworkBehaviour<InfoFeed>.Instance.InformMessageDisappearedInternal(message);
		}
	}

	public static string ColorizePlayerName(string playerName, Color playerColor)
	{
		return "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">" + playerName + "</color>";
	}

	private void ShowFinishedHoleMessageInternal(PlayerInfo player, int placement)
	{
		if (!(player == null) && placement >= 0)
		{
			FinishedHoleMessageData messageData = new FinishedHoleMessageData(player.PlayerId.PlayerNameNoRichText, placement + 1);
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowScoredOnDrivingRangeInternal(PlayerInfo player)
	{
		if (!(player == null))
		{
			ScoredOnDrivingRangeMessageData messageData = new ScoredOnDrivingRangeMessageData(player.PlayerId.PlayerNameNoRichText);
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowStrokesMessageInternal(PlayerInfo player, StrokesUnderParType strokesUnderParType, int strokesUnderPar)
	{
		if (!(player == null))
		{
			StrokesMessageData messageData = new StrokesMessageData(player.PlayerId.PlayerNameNoRichText, strokesUnderParType, strokesUnderPar);
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowChipInMessageInternal(PlayerInfo player, float chipInDistance)
	{
		if (!(player == null))
		{
			ChipInMessageData messageData = new ChipInMessageData(player.PlayerId.PlayerNameNoRichText, chipInDistance);
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowSpeedrunMessageInternal(PlayerInfo player, float time)
	{
		if (!(player == null))
		{
			SpeedrunMessageData messageData = new SpeedrunMessageData(player.PlayerId.PlayerNameNoRichText, time);
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowKnockoutMessageInternal(PlayerInfo responsiblePlayer, PlayerInfo knockedOutPlayer, KnockoutType knockoutType)
	{
		if (!(responsiblePlayer == null) && !(knockedOutPlayer == null))
		{
			if (!iconSettings.TryGetIconType(knockoutType, out var iconType))
			{
				Debug.LogError($"Attempted to display a knockout message for type {knockoutType}, but it has no icon type", base.gameObject);
				return;
			}
			GenericMessageData messageData = new GenericMessageData(ColorizePlayerName(responsiblePlayer.PlayerId.PlayerNameNoRichText, player0Color), ColorizePlayerName(knockedOutPlayer.PlayerId.PlayerNameNoRichText, player1Color), iconType);
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowSelfKnockoutMessageInternal(PlayerInfo knockedOutPlayer, KnockoutType knockoutType)
	{
		if (!(knockedOutPlayer == null))
		{
			if (!iconSettings.TryGetIconType(knockoutType, out var iconType))
			{
				Debug.LogError($"Attempted to display a knockout message for type {knockoutType}, but it has no icon type", base.gameObject);
				return;
			}
			GenericMessageData messageData = new GenericMessageData(ColorizePlayerName(knockedOutPlayer.PlayerId.PlayerNameNoRichText, player1Color), iconType);
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowEliminationMessageInternal(PlayerInfo responsiblePlayer, PlayerInfo eliminatedPlayer, EliminationReason eliminationReason)
	{
		if (!(responsiblePlayer == null) && !(eliminatedPlayer == null))
		{
			if (!iconSettings.TryGetIconType(eliminationReason, out var iconType))
			{
				Debug.LogError($"Attempted to display an elimination message for reason {eliminationReason}, but it has no icon type", base.gameObject);
				return;
			}
			GenericMessageData messageData = new GenericMessageData(ColorizePlayerName(responsiblePlayer.PlayerId.PlayerNameNoRichText, player0Color), ColorizePlayerName(eliminatedPlayer.PlayerId.PlayerNameNoRichText, player1Color), iconType, InfoFeedIconSettings.Type.Elimination);
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowSelfEliminationMessageInternal(PlayerInfo eliminatedPlayer, EliminationReason eliminationReason)
	{
		if (!(eliminatedPlayer == null))
		{
			if (!iconSettings.TryGetIconType(eliminationReason, out var iconType))
			{
				Debug.LogError($"Attempted to display an elimination message for reason {eliminationReason}, but it has no icon type", base.gameObject);
				return;
			}
			GenericMessageData messageData = new GenericMessageData(ColorizePlayerName(eliminatedPlayer.PlayerId.PlayerNameNoRichText, player1Color), iconType, InfoFeedIconSettings.Type.Elimination);
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowDominationMessageInternal(PlayerInfo dominatingPlayer, PlayerInfo dominatedPlayer)
	{
		if (!(dominatingPlayer == null) && !(dominatedPlayer == null))
		{
			DominatingMessageData messageData = new DominatingMessageData(ColorizePlayerName(dominatingPlayer.PlayerId.PlayerNameNoRichText, player0Color), ColorizePlayerName(dominatedPlayer.PlayerId.PlayerNameNoRichText, player1Color));
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void ShowRevengeMessageInternal(PlayerInfo previouslyDominatedPlayer, PlayerInfo previouslyDominatingPlayer)
	{
		if (!(previouslyDominatedPlayer == null) && !(previouslyDominatingPlayer == null))
		{
			RevengeMessageData messageData = new RevengeMessageData(ColorizePlayerName(previouslyDominatedPlayer.PlayerId.PlayerNameNoRichText, player0Color), ColorizePlayerName(previouslyDominatingPlayer.PlayerId.PlayerNameNoRichText, player1Color));
			ServerShowMessageForAllClients(messageData);
		}
	}

	private void InformMessageDisappearedInternal(InfoFeedMessage message)
	{
		if (shownMessages.Contains(message))
		{
			ReturnMessage(message);
		}
	}

	[Server]
	private void ServerShowMessageForAllClients(GenericMessageData messageData)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void InfoFeed::ServerShowMessageForAllClients(InfoFeed/GenericMessageData)' called when server was not active");
		}
		else
		{
			RpcShowMessage(messageData);
		}
	}

	[Server]
	private void ServerShowMessageForAllClients(FinishedHoleMessageData messageData)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void InfoFeed::ServerShowMessageForAllClients(InfoFeed/FinishedHoleMessageData)' called when server was not active");
		}
		else
		{
			RpcShowMessage(messageData);
		}
	}

	[Server]
	private void ServerShowMessageForAllClients(ScoredOnDrivingRangeMessageData messageData)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void InfoFeed::ServerShowMessageForAllClients(InfoFeed/ScoredOnDrivingRangeMessageData)' called when server was not active");
		}
		else
		{
			RpcShowMessage(messageData);
		}
	}

	[Server]
	private void ServerShowMessageForAllClients(StrokesMessageData messageData)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void InfoFeed::ServerShowMessageForAllClients(InfoFeed/StrokesMessageData)' called when server was not active");
		}
		else
		{
			RpcShowMessage(messageData);
		}
	}

	[Server]
	private void ServerShowMessageForAllClients(ChipInMessageData messageData)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void InfoFeed::ServerShowMessageForAllClients(InfoFeed/ChipInMessageData)' called when server was not active");
		}
		else
		{
			RpcShowMessage(messageData);
		}
	}

	[Server]
	private void ServerShowMessageForAllClients(SpeedrunMessageData messageData)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void InfoFeed::ServerShowMessageForAllClients(InfoFeed/SpeedrunMessageData)' called when server was not active");
		}
		else
		{
			RpcShowMessage(messageData);
		}
	}

	[Server]
	private void ServerShowMessageForAllClients(DominatingMessageData messageData)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void InfoFeed::ServerShowMessageForAllClients(InfoFeed/DominatingMessageData)' called when server was not active");
		}
		else
		{
			RpcShowMessage(messageData);
		}
	}

	[Server]
	private void ServerShowMessageForAllClients(RevengeMessageData messageData)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void InfoFeed::ServerShowMessageForAllClients(InfoFeed/RevengeMessageData)' called when server was not active");
		}
		else
		{
			RpcShowMessage(messageData);
		}
	}

	[ClientRpc]
	private void RpcShowMessage(GenericMessageData messageData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_InfoFeed_002FGenericMessageData(writer, messageData);
		SendRPCInternal("System.Void InfoFeed::RpcShowMessage(InfoFeed/GenericMessageData)", 1444391163, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcShowMessage(FinishedHoleMessageData messageData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_InfoFeed_002FFinishedHoleMessageData(writer, messageData);
		SendRPCInternal("System.Void InfoFeed::RpcShowMessage(InfoFeed/FinishedHoleMessageData)", 1632849890, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcShowMessage(ScoredOnDrivingRangeMessageData messageData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_InfoFeed_002FScoredOnDrivingRangeMessageData(writer, messageData);
		SendRPCInternal("System.Void InfoFeed::RpcShowMessage(InfoFeed/ScoredOnDrivingRangeMessageData)", 302347825, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcShowMessage(StrokesMessageData messageData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_InfoFeed_002FStrokesMessageData(writer, messageData);
		SendRPCInternal("System.Void InfoFeed::RpcShowMessage(InfoFeed/StrokesMessageData)", -182423837, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcShowMessage(ChipInMessageData messageData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_InfoFeed_002FChipInMessageData(writer, messageData);
		SendRPCInternal("System.Void InfoFeed::RpcShowMessage(InfoFeed/ChipInMessageData)", -994662077, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcShowMessage(SpeedrunMessageData messageData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_InfoFeed_002FSpeedrunMessageData(writer, messageData);
		SendRPCInternal("System.Void InfoFeed::RpcShowMessage(InfoFeed/SpeedrunMessageData)", 1573541330, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcShowMessage(DominatingMessageData messageData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_InfoFeed_002FDominatingMessageData(writer, messageData);
		SendRPCInternal("System.Void InfoFeed::RpcShowMessage(InfoFeed/DominatingMessageData)", 1347034844, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcShowMessage(RevengeMessageData messageData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_InfoFeed_002FRevengeMessageData(writer, messageData);
		SendRPCInternal("System.Void InfoFeed::RpcShowMessage(InfoFeed/RevengeMessageData)", 1646614592, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void ShowMessage(IMessageData messageData)
	{
		if (CanShowMessageImmediately())
		{
			ShowMessageImmediately(messageData);
		}
		else
		{
			queuedMessages.Enqueue(messageData);
		}
	}

	private void ShowMessageImmediately(IMessageData messageData)
	{
		InfoFeedMessage unusedMessage = GetUnusedMessage();
		unusedMessage.Initialize(messageData);
		shownMessages.Add(unusedMessage);
	}

	protected override void Awake()
	{
		base.Awake();
		LocalizationManager.LanguageChanged += OnLanguageChanged;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		LocalizationManager.LanguageChanged -= OnLanguageChanged;
	}

	private void Update()
	{
		foreach (InfoFeedMessage shownMessage in shownMessages)
		{
			shownMessage.OnUpdate();
		}
		if (queuedMessages.Count > 0 && CanShowMessageImmediately())
		{
			ShowMessageImmediately(queuedMessages.Dequeue());
		}
	}

	private bool CanShowMessageImmediately()
	{
		if (shownMessages.Count >= maxConcurrentMessageCount)
		{
			return false;
		}
		if (HoleInfoUi.IsVisible)
		{
			return false;
		}
		return true;
	}

	private InfoFeedMessage GetUnusedMessage()
	{
		EnsurePoolParentExists();
		InfoFeedMessage result = null;
		while (result == null)
		{
			if (!messagePool.TryPop(out result))
			{
				result = Object.Instantiate(messagePrefab);
			}
		}
		result.gameObject.SetActive(value: true);
		result.transform.SetParent(messageContainer);
		result.transform.localScale = Vector3.one;
		return result;
	}

	private void ReturnMessage(InfoFeedMessage message)
	{
		shownMessages.Remove(message);
		if (messagePool.Count >= maxConcurrentMessageCount)
		{
			Object.Destroy(message.gameObject);
			return;
		}
		message.gameObject.SetActive(value: false);
		message.transform.SetParent(messagePoolParent);
		messagePool.Push(message);
	}

	private void EnsurePoolParentExists()
	{
		if (!(messagePoolParent != null))
		{
			GameObject obj = new GameObject("Info feed message pool");
			Object.DontDestroyOnLoad(obj);
			messagePoolParent = obj.transform;
		}
	}

	private void OnLanguageChanged()
	{
		foreach (InfoFeedMessage shownMessage in shownMessages)
		{
			shownMessage.RefreshMessage();
		}
	}

	static InfoFeed()
	{
		messagePool = new Stack<InfoFeedMessage>();
		RemoteProcedureCalls.RegisterRpc(typeof(InfoFeed), "System.Void InfoFeed::RpcShowMessage(InfoFeed/GenericMessageData)", InvokeUserCode_RpcShowMessage__GenericMessageData);
		RemoteProcedureCalls.RegisterRpc(typeof(InfoFeed), "System.Void InfoFeed::RpcShowMessage(InfoFeed/FinishedHoleMessageData)", InvokeUserCode_RpcShowMessage__FinishedHoleMessageData);
		RemoteProcedureCalls.RegisterRpc(typeof(InfoFeed), "System.Void InfoFeed::RpcShowMessage(InfoFeed/ScoredOnDrivingRangeMessageData)", InvokeUserCode_RpcShowMessage__ScoredOnDrivingRangeMessageData);
		RemoteProcedureCalls.RegisterRpc(typeof(InfoFeed), "System.Void InfoFeed::RpcShowMessage(InfoFeed/StrokesMessageData)", InvokeUserCode_RpcShowMessage__StrokesMessageData);
		RemoteProcedureCalls.RegisterRpc(typeof(InfoFeed), "System.Void InfoFeed::RpcShowMessage(InfoFeed/ChipInMessageData)", InvokeUserCode_RpcShowMessage__ChipInMessageData);
		RemoteProcedureCalls.RegisterRpc(typeof(InfoFeed), "System.Void InfoFeed::RpcShowMessage(InfoFeed/SpeedrunMessageData)", InvokeUserCode_RpcShowMessage__SpeedrunMessageData);
		RemoteProcedureCalls.RegisterRpc(typeof(InfoFeed), "System.Void InfoFeed::RpcShowMessage(InfoFeed/DominatingMessageData)", InvokeUserCode_RpcShowMessage__DominatingMessageData);
		RemoteProcedureCalls.RegisterRpc(typeof(InfoFeed), "System.Void InfoFeed::RpcShowMessage(InfoFeed/RevengeMessageData)", InvokeUserCode_RpcShowMessage__RevengeMessageData);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcShowMessage__GenericMessageData(GenericMessageData messageData)
	{
		ShowMessage(messageData);
	}

	protected static void InvokeUserCode_RpcShowMessage__GenericMessageData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShowMessage called on server.");
		}
		else
		{
			((InfoFeed)obj).UserCode_RpcShowMessage__GenericMessageData(GeneratedNetworkCode._Read_InfoFeed_002FGenericMessageData(reader));
		}
	}

	protected void UserCode_RpcShowMessage__FinishedHoleMessageData(FinishedHoleMessageData messageData)
	{
		ShowMessage(messageData);
	}

	protected static void InvokeUserCode_RpcShowMessage__FinishedHoleMessageData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShowMessage called on server.");
		}
		else
		{
			((InfoFeed)obj).UserCode_RpcShowMessage__FinishedHoleMessageData(GeneratedNetworkCode._Read_InfoFeed_002FFinishedHoleMessageData(reader));
		}
	}

	protected void UserCode_RpcShowMessage__ScoredOnDrivingRangeMessageData(ScoredOnDrivingRangeMessageData messageData)
	{
		ShowMessage(messageData);
	}

	protected static void InvokeUserCode_RpcShowMessage__ScoredOnDrivingRangeMessageData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShowMessage called on server.");
		}
		else
		{
			((InfoFeed)obj).UserCode_RpcShowMessage__ScoredOnDrivingRangeMessageData(GeneratedNetworkCode._Read_InfoFeed_002FScoredOnDrivingRangeMessageData(reader));
		}
	}

	protected void UserCode_RpcShowMessage__StrokesMessageData(StrokesMessageData messageData)
	{
		ShowMessage(messageData);
	}

	protected static void InvokeUserCode_RpcShowMessage__StrokesMessageData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShowMessage called on server.");
		}
		else
		{
			((InfoFeed)obj).UserCode_RpcShowMessage__StrokesMessageData(GeneratedNetworkCode._Read_InfoFeed_002FStrokesMessageData(reader));
		}
	}

	protected void UserCode_RpcShowMessage__ChipInMessageData(ChipInMessageData messageData)
	{
		ShowMessage(messageData);
	}

	protected static void InvokeUserCode_RpcShowMessage__ChipInMessageData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShowMessage called on server.");
		}
		else
		{
			((InfoFeed)obj).UserCode_RpcShowMessage__ChipInMessageData(GeneratedNetworkCode._Read_InfoFeed_002FChipInMessageData(reader));
		}
	}

	protected void UserCode_RpcShowMessage__SpeedrunMessageData(SpeedrunMessageData messageData)
	{
		ShowMessage(messageData);
	}

	protected static void InvokeUserCode_RpcShowMessage__SpeedrunMessageData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShowMessage called on server.");
		}
		else
		{
			((InfoFeed)obj).UserCode_RpcShowMessage__SpeedrunMessageData(GeneratedNetworkCode._Read_InfoFeed_002FSpeedrunMessageData(reader));
		}
	}

	protected void UserCode_RpcShowMessage__DominatingMessageData(DominatingMessageData messageData)
	{
		ShowMessage(messageData);
	}

	protected static void InvokeUserCode_RpcShowMessage__DominatingMessageData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShowMessage called on server.");
		}
		else
		{
			((InfoFeed)obj).UserCode_RpcShowMessage__DominatingMessageData(GeneratedNetworkCode._Read_InfoFeed_002FDominatingMessageData(reader));
		}
	}

	protected void UserCode_RpcShowMessage__RevengeMessageData(RevengeMessageData messageData)
	{
		ShowMessage(messageData);
	}

	protected static void InvokeUserCode_RpcShowMessage__RevengeMessageData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShowMessage called on server.");
		}
		else
		{
			((InfoFeed)obj).UserCode_RpcShowMessage__RevengeMessageData(GeneratedNetworkCode._Read_InfoFeed_002FRevengeMessageData(reader));
		}
	}
}
