using System;
using UnityEngine;

namespace Mirror;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ReadOnlyAttribute : PropertyAttribute
{
}
