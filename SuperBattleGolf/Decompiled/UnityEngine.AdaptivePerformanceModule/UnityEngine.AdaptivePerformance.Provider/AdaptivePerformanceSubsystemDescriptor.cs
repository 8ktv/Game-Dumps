using System;
using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.AdaptivePerformance.Provider;

public sealed class AdaptivePerformanceSubsystemDescriptor : SubsystemDescriptorWithProvider<AdaptivePerformanceSubsystem, AdaptivePerformanceSubsystem.APProvider>
{
	public struct Cinfo
	{
		public string id { get; set; }

		public Type providerType { get; set; }

		public Type subsystemTypeOverride { get; set; }

		[Obsolete("AdaptivePerformanceSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2023.1. Use providerType and, optionally, subsystemTypeOverride instead.", true)]
		public Type subsystemImplementationType { get; set; }
	}

	public AdaptivePerformanceSubsystemDescriptor(Cinfo cinfo)
	{
		base.id = cinfo.id;
		base.providerType = cinfo.providerType;
		base.subsystemTypeOverride = cinfo.subsystemTypeOverride;
	}

	public static AdaptivePerformanceSubsystemDescriptor RegisterDescriptor(Cinfo cinfo)
	{
		List<AdaptivePerformanceSubsystemDescriptor> registeredDescriptors = AdaptivePerformanceSubsystemRegistry.GetRegisteredDescriptors();
		foreach (AdaptivePerformanceSubsystemDescriptor item in registeredDescriptors)
		{
			if (item.id == cinfo.id)
			{
				return item;
			}
		}
		return AdaptivePerformanceSubsystemRegistry.RegisterDescriptor(cinfo);
	}
}
