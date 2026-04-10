using System;
using UnityEngine.Scripting;

namespace UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
[RequiredByNativeCode]
public sealed class SerializeField : Attribute
{
}
