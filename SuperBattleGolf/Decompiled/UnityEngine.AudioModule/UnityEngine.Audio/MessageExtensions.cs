using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Audio;

public static class MessageExtensions
{
	public static T Get<T>(this in ProcessorInstance.Message message) where T : class
	{
		if (!message.Is<T>())
		{
			throw new InvalidCastException($"Message does not contain data of type {typeof(T)}");
		}
		return (T)GCHandle.FromIntPtr(message.ManagedHandle).Target;
	}

	public static ProcessorInstance.Response SendMessage<T>(this ControlContext context, ProcessorInstance processorInstance, T message) where T : class
	{
		return context.SendManagedMessage(processorInstance, message);
	}
}
