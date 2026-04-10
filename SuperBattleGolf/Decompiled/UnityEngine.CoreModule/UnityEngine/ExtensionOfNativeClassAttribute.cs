using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
[RequiredByNativeCode]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
internal sealed class ExtensionOfNativeClassAttribute : Attribute
{
}
