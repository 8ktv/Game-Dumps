using System;
using System.Collections.Generic;

namespace UnityEngine.AdaptivePerformance;

public abstract class AdaptivePerformanceLoaderHelper : AdaptivePerformanceLoader
{
	protected Dictionary<Type, ISubsystem> m_SubsystemInstanceMap = new Dictionary<Type, ISubsystem>();

	public override T GetLoadedSubsystem<T>()
	{
		Type typeFromHandle = typeof(T);
		m_SubsystemInstanceMap.TryGetValue(typeFromHandle, out var value);
		return value as T;
	}

	protected void StartSubsystem<T>() where T : class, ISubsystem
	{
		GetLoadedSubsystem<T>()?.Start();
	}

	protected void StopSubsystem<T>() where T : class, ISubsystem
	{
		GetLoadedSubsystem<T>()?.Stop();
	}

	protected void DestroySubsystem<T>() where T : class, ISubsystem
	{
		T loadedSubsystem = GetLoadedSubsystem<T>();
		if (loadedSubsystem != null)
		{
			if (loadedSubsystem.running)
			{
				loadedSubsystem.Stop();
			}
			Type typeFromHandle = typeof(T);
			if (m_SubsystemInstanceMap.ContainsKey(typeFromHandle))
			{
				m_SubsystemInstanceMap.Remove(typeFromHandle);
			}
			loadedSubsystem.Destroy();
		}
	}

	protected void CreateSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id) where TDescriptor : ISubsystemDescriptor where TSubsystem : ISubsystem
	{
		if (descriptors == null)
		{
			throw new ArgumentNullException("descriptors");
		}
		SubsystemManager.GetSubsystemDescriptors(descriptors);
		if (descriptors.Count <= 0)
		{
			return;
		}
		foreach (TDescriptor descriptor in descriptors)
		{
			ISubsystem subsystem = null;
			if (string.Compare(descriptor.id, id, ignoreCase: true) == 0)
			{
				subsystem = descriptor.Create();
			}
			if (subsystem != null)
			{
				m_SubsystemInstanceMap[typeof(TSubsystem)] = subsystem;
				break;
			}
		}
	}

	public override bool Deinitialize()
	{
		m_SubsystemInstanceMap.Clear();
		return base.Deinitialize();
	}
}
