using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations;

[NativeHeader("Modules/Animation/MuscleHandle.h")]
[NativeHeader("Modules/Animation/Animator.h")]
[MovedFrom("UnityEngine.Experimental.Animations")]
public struct MuscleHandle
{
	public HumanPartDof humanPartDof { get; private set; }

	public int dof { get; private set; }

	public string name => GetName();

	public static int muscleHandleCount => GetMuscleHandleCount();

	public MuscleHandle(BodyDof bodyDof)
	{
		humanPartDof = HumanPartDof.Body;
		dof = (int)bodyDof;
	}

	public MuscleHandle(HeadDof headDof)
	{
		humanPartDof = HumanPartDof.Head;
		dof = (int)headDof;
	}

	public MuscleHandle(HumanPartDof partDof, LegDof legDof)
	{
		if (partDof != HumanPartDof.LeftLeg && partDof != HumanPartDof.RightLeg)
		{
			throw new InvalidOperationException("Invalid HumanPartDof for a leg, please use either HumanPartDof.LeftLeg or HumanPartDof.RightLeg.");
		}
		humanPartDof = partDof;
		dof = (int)legDof;
	}

	public MuscleHandle(HumanPartDof partDof, ArmDof armDof)
	{
		if (partDof != HumanPartDof.LeftArm && partDof != HumanPartDof.RightArm)
		{
			throw new InvalidOperationException("Invalid HumanPartDof for an arm, please use either HumanPartDof.LeftArm or HumanPartDof.RightArm.");
		}
		humanPartDof = partDof;
		dof = (int)armDof;
	}

	public MuscleHandle(HumanPartDof partDof, FingerDof fingerDof)
	{
		if (partDof < HumanPartDof.LeftThumb || partDof > HumanPartDof.RightLittle)
		{
			throw new InvalidOperationException("Invalid HumanPartDof for a finger.");
		}
		humanPartDof = partDof;
		dof = (int)fingerDof;
	}

	public unsafe static void GetMuscleHandles([Out][NotNull] MuscleHandle[] muscleHandles)
	{
		if (muscleHandles == null)
		{
			ThrowHelper.ThrowArgumentNullException(muscleHandles, "muscleHandles");
		}
		BlittableArrayWrapper muscleHandles2 = default(BlittableArrayWrapper);
		try
		{
			fixed (MuscleHandle[] array = muscleHandles)
			{
				if (array.Length != 0)
				{
					muscleHandles2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				GetMuscleHandles_Injected(out muscleHandles2);
			}
		}
		finally
		{
			muscleHandles2.Unmarshal(ref array);
		}
	}

	private string GetName()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetName_Injected(ref this, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetMuscleHandleCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMuscleHandles_Injected(out BlittableArrayWrapper muscleHandles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetName_Injected(ref MuscleHandle _unity_self, out ManagedSpanWrapper ret);
}
