using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.Networking.PlayerConnection;

[Serializable]
internal class PlayerEditorConnectionEvents
{
	[Serializable]
	public class MessageEvent : UnityEvent<MessageEventArgs>
	{
	}

	[Serializable]
	public class ConnectionChangeEvent : UnityEvent<int>
	{
	}

	[Serializable]
	public class MessageTypeSubscribers
	{
		[SerializeField]
		private string m_messageTypeId;

		public int subscriberCount = 0;

		public MessageEvent messageCallback = new MessageEvent();

		public Guid MessageTypeId
		{
			get
			{
				return new Guid(m_messageTypeId);
			}
			set
			{
				m_messageTypeId = value.ToString();
			}
		}
	}

	[SerializeField]
	private List<MessageTypeSubscribers> m_MessageTypeSubscribers = new List<MessageTypeSubscribers>();

	private Dictionary<Guid, MessageTypeSubscribers> m_SubscriberLookup;

	[SerializeField]
	public ConnectionChangeEvent connectionEvent = new ConnectionChangeEvent();

	[SerializeField]
	public ConnectionChangeEvent disconnectionEvent = new ConnectionChangeEvent();

	public IReadOnlyList<MessageTypeSubscribers> messageTypeSubscribers => m_MessageTypeSubscribers;

	private void BuildLookup()
	{
		if (m_SubscriberLookup != null)
		{
			return;
		}
		m_SubscriberLookup = new Dictionary<Guid, MessageTypeSubscribers>();
		foreach (MessageTypeSubscribers messageTypeSubscriber in messageTypeSubscribers)
		{
			m_SubscriberLookup.Add(messageTypeSubscriber.MessageTypeId, messageTypeSubscriber);
		}
	}

	public void InvokeMessageIdSubscribers(Guid messageId, byte[] data, int playerId)
	{
		BuildLookup();
		if (!m_SubscriberLookup.TryGetValue(messageId, out var value))
		{
			Guid guid = messageId;
			Debug.LogError("No actions found for messageId: " + guid.ToString());
			return;
		}
		MessageEventArgs arg = new MessageEventArgs
		{
			playerId = playerId,
			data = data
		};
		value.messageCallback.Invoke(arg);
	}

	public UnityEvent<MessageEventArgs> AddAndCreate(Guid messageId)
	{
		BuildLookup();
		if (!m_SubscriberLookup.TryGetValue(messageId, out var value))
		{
			value = new MessageTypeSubscribers
			{
				MessageTypeId = messageId,
				messageCallback = new MessageEvent()
			};
			m_MessageTypeSubscribers.Add(value);
			m_SubscriberLookup.Add(messageId, value);
		}
		value.subscriberCount++;
		return value.messageCallback;
	}

	public void UnregisterManagedCallback(Guid messageId, UnityAction<MessageEventArgs> callback)
	{
		BuildLookup();
		if (m_SubscriberLookup.TryGetValue(messageId, out var value))
		{
			value.subscriberCount--;
			value.messageCallback.RemoveListener(callback);
			if (value.subscriberCount <= 0)
			{
				m_MessageTypeSubscribers.Remove(value);
				m_SubscriberLookup.Remove(messageId);
			}
		}
	}

	public void Clear()
	{
		if (m_SubscriberLookup != null)
		{
			m_SubscriberLookup.Clear();
		}
		m_MessageTypeSubscribers.Clear();
	}
}
