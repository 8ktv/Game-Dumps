using System;
using UnityEngine.Bindings;

namespace Unity.Scripting.LifecycleManagement;

[VisibleToOtherModules]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, AllowMultiple = true)]
internal sealed class AutoStaticsCleanupAttribute : Attribute
{
	public Type ScopeType { get; set; }

	public ScopeTransitionType TransitionType { get; set; }

	public CleanupStrategy CleanupStrategy { get; set; }
}
