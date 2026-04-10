using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.AdaptivePerformance.Provider;

public class AdaptivePerformanceSubsystem : AdaptivePerformanceSubsystemBase<AdaptivePerformanceSubsystem, AdaptivePerformanceSubsystemDescriptor, AdaptivePerformanceSubsystem.APProvider>
{
	public abstract class APProvider : SubsystemProvider<AdaptivePerformanceSubsystem>
	{
		protected new bool m_Running;

		public abstract Feature Capabilities { get; set; }

		public abstract IApplicationLifecycle ApplicationLifecycle { get; }

		public abstract IDevicePerformanceLevelControl PerformanceLevelControl { get; }

		public abstract Version Version { get; }

		public virtual string Stats => "";

		public abstract bool Initialized { get; set; }

		public new bool running => m_Running;

		public abstract PerformanceDataRecord Update();
	}

	public override IApplicationLifecycle ApplicationLifecycle => base.provider.ApplicationLifecycle;

	public override IDevicePerformanceLevelControl PerformanceLevelControl => base.provider.PerformanceLevelControl;

	public override Version Version => base.provider.Version;

	public override Feature Capabilities
	{
		get
		{
			return base.provider.Capabilities;
		}
		protected set
		{
			base.provider.Capabilities = value;
		}
	}

	public override string Stats => base.provider.Stats;

	public override bool Initialized
	{
		get
		{
			return base.provider.Initialized;
		}
		protected set
		{
			base.provider.Initialized = value;
		}
	}

	public override PerformanceDataRecord Update()
	{
		return base.provider.Update();
	}
}
