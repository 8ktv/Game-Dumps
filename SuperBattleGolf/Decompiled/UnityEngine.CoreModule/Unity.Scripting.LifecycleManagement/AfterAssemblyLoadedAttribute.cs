using System;
using UnityEngine.Bindings;

namespace Unity.Scripting.LifecycleManagement;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[VisibleToOtherModules]
internal sealed class AfterAssemblyLoadedAttribute : LifecycleAttributeBase
{
}
