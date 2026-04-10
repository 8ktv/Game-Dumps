using System.Collections.Generic;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.Bindings;

namespace UnityEngine.AdaptivePerformance.Basic;

[VisibleToOtherModules(new string[] { "UnityEditor.AdaptivePerformanceModule" })]
internal class BasicProviderLoader : AdaptivePerformanceLoaderHelper
{
	private static List<AdaptivePerformanceSubsystemDescriptor> s_BasicSubsystemDescriptors = new List<AdaptivePerformanceSubsystemDescriptor>();

	public override bool Initialized => BasicSubsystem != null;

	public override bool Running => BasicSubsystem != null && BasicSubsystem.running;

	public BasicAdaptivePerformanceSubsystem BasicSubsystem => GetLoadedSubsystem<BasicAdaptivePerformanceSubsystem>();

	public override ISubsystem GetDefaultSubsystem()
	{
		return BasicSubsystem;
	}

	public override IAdaptivePerformanceSettings GetSettings()
	{
		return BasicProviderSettings.GetSettings();
	}

	public override bool Initialize()
	{
		CreateSubsystem<AdaptivePerformanceSubsystemDescriptor, BasicAdaptivePerformanceSubsystem>(s_BasicSubsystemDescriptors, "BasicAdaptivePerformanceSubsystem");
		if (BasicSubsystem == null)
		{
			Debug.LogError("Unable to start the Basic subsystem.");
		}
		return BasicSubsystem != null;
	}

	public override bool Start()
	{
		StartSubsystem<BasicAdaptivePerformanceSubsystem>();
		return true;
	}

	public override bool Stop()
	{
		StopSubsystem<BasicAdaptivePerformanceSubsystem>();
		return true;
	}

	public override bool Deinitialize()
	{
		DestroySubsystem<BasicAdaptivePerformanceSubsystem>();
		return base.Deinitialize();
	}
}
