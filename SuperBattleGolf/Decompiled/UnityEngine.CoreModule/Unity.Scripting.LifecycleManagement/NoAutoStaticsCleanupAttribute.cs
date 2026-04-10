using System;
using UnityEngine.Bindings;

namespace Unity.Scripting.LifecycleManagement;

[VisibleToOtherModules]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, AllowMultiple = false)]
internal sealed class NoAutoStaticsCleanupAttribute : Attribute
{
}
