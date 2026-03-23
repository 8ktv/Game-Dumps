using UnityEngine.Bindings;

namespace UnityEngine;

[NativeClass("Unity::FixedJoint")]
[NativeHeader("Modules/Physics/FixedJoint.h")]
[RequireComponent(typeof(Rigidbody))]
public class FixedJoint : Joint
{
}
