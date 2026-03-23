using System;

namespace UnityEngine;

public struct HumanPose
{
	private static int k_NumIkGoals = Enum.GetValues(typeof(AvatarIKGoal)).Length;

	internal static Quaternion[] s_IKGoalOffsets = new Quaternion[4]
	{
		new Quaternion(0.5f, -0.5f, 0.5f, 0.5f),
		new Quaternion(0.5f, -0.5f, 0.5f, 0.5f),
		new Quaternion(0.707107f, 0f, 0.707107f, 0f),
		new Quaternion(0f, 0.707107f, 0f, 0.707107f)
	};

	public Vector3 bodyPosition;

	public Quaternion bodyRotation;

	public float[] muscles;

	internal Vector3[] m_IkGoalPositions;

	internal Quaternion[] m_IkGoalRotations;

	internal Quaternion[] m_OffsetIkGoalRotations;

	public ReadOnlySpan<Vector3> ikGoalPositions => new ReadOnlySpan<Vector3>(m_IkGoalPositions);

	public ReadOnlySpan<Quaternion> internalIkGoalRotations => new ReadOnlySpan<Quaternion>(m_IkGoalRotations);

	public ReadOnlySpan<Quaternion> ikGoalRotations => new ReadOnlySpan<Quaternion>(m_OffsetIkGoalRotations);

	internal void Init()
	{
		if (muscles != null && muscles.Length != HumanTrait.MuscleCount)
		{
			throw new InvalidOperationException("Bad array size for HumanPose.muscles. Size must equal HumanTrait.MuscleCount");
		}
		if (muscles == null)
		{
			muscles = new float[HumanTrait.MuscleCount];
			if (bodyRotation.x == 0f && bodyRotation.y == 0f && bodyRotation.z == 0f && bodyRotation.w == 0f)
			{
				bodyRotation.w = 1f;
			}
		}
		if (m_IkGoalPositions != null && m_IkGoalPositions.Length != k_NumIkGoals)
		{
			throw new InvalidOperationException("Bad array size for HumanPose.ikGoalPositions. Size must equal AvatakIKGoal size");
		}
		if (m_IkGoalPositions == null)
		{
			m_IkGoalPositions = new Vector3[k_NumIkGoals];
		}
		if (m_IkGoalRotations != null && m_IkGoalRotations.Length != k_NumIkGoals)
		{
			throw new InvalidOperationException("Bad array size for HumanPose.ikGoalPositions. Size must equal AvatakIKGoal size");
		}
		if (m_IkGoalRotations == null)
		{
			m_IkGoalRotations = new Quaternion[k_NumIkGoals];
		}
		if (m_OffsetIkGoalRotations != null && m_OffsetIkGoalRotations.Length != k_NumIkGoals)
		{
			throw new InvalidOperationException("Bad array size for HumanPose.ikGoalPositions. Size must equal AvatakIKGoal size");
		}
		if (m_OffsetIkGoalRotations == null)
		{
			m_OffsetIkGoalRotations = new Quaternion[k_NumIkGoals];
		}
	}
}
