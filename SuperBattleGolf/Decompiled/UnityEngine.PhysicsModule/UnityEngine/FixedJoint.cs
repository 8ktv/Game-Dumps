using UnityEngine.Bindings;

namespace UnityEngine;

[NativeClass("Unity::FixedJoint")]
[RequireComponent(typeof(Rigidbody))]
[NativeHeader("Modules/Physics/FixedJoint.h")]
public class FixedJoint : Joint
{
}
