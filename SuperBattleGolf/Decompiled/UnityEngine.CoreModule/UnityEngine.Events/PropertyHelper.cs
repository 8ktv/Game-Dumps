using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.Events;

internal class PropertyHelper<TBase>
{
	private interface IEventHolder
	{
		void Invoke(TBase setting, string property);

		bool IsEmpty();
	}

	private class EventHolder<T> : IEventHolder where T : class, TBase
	{
		public event Action<T, string> propertyEvent;

		void IEventHolder.Invoke(TBase setting, string property)
		{
			this.propertyEvent?.Invoke(setting as T, property);
		}

		bool IEventHolder.IsEmpty()
		{
			return this.propertyEvent == null;
		}
	}

	public struct PropertyChangedEvent
	{
		private Dictionary<Type, IEventHolder> m_Subscriptions;

		public PropertyChangedEvent()
		{
			m_Subscriptions = new Dictionary<Type, IEventHolder>();
		}

		public void Subscribe<TChild>(Action<TChild, string> callback) where TChild : class, TBase
		{
			EventHolder<TChild> eventHolder = null;
			if (m_Subscriptions.TryGetValue(typeof(TChild), out var value))
			{
				eventHolder = value as EventHolder<TChild>;
			}
			else
			{
				eventHolder = new EventHolder<TChild>();
				m_Subscriptions.Add(typeof(TChild), eventHolder);
			}
			eventHolder.propertyEvent += callback;
		}

		public void Unsubscribe<TChild>(Action<TChild, string> callback) where TChild : class, TBase
		{
			if (m_Subscriptions.TryGetValue(typeof(TChild), out var value))
			{
				EventHolder<TChild> eventHolder = value as EventHolder<TChild>;
				eventHolder.propertyEvent -= callback;
				if (value.IsEmpty())
				{
					m_Subscriptions.Remove(typeof(TChild));
				}
			}
		}

		public void Notify(TBase instance, [CallerMemberName] string propertyName = "")
		{
			if (m_Subscriptions.TryGetValue(instance.GetType(), out var value))
			{
				value.Invoke(instance, propertyName);
			}
		}
	}

	public PropertyChangedEvent propertyChangedEvent = new PropertyChangedEvent();

	public bool SetProperty<TData>(TBase instance, ref TData currentPropertyValue, TData newValue, [CallerMemberName] string propertyName = "")
	{
		if (object.Equals(currentPropertyValue, newValue))
		{
			return false;
		}
		currentPropertyValue = newValue;
		NotifyValueChange(instance, propertyName);
		return true;
	}

	public void NotifyValueChange(TBase instance, [CallerMemberName] string propertyName = "")
	{
		propertyChangedEvent.Notify(instance, propertyName);
	}
}
