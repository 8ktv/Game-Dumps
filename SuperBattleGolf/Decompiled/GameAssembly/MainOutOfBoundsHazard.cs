using UnityEngine;

public class MainOutOfBoundsHazard : SingletonBehaviour<MainOutOfBoundsHazard>
{
	[SerializeField]
	private OutOfBoundsHazard type;

	public static float Height
	{
		get
		{
			if (!SingletonBehaviour<MainOutOfBoundsHazard>.HasInstance)
			{
				return 0f;
			}
			return SingletonBehaviour<MainOutOfBoundsHazard>.Instance.transform.position.y;
		}
	}

	public static OutOfBoundsHazard Type
	{
		get
		{
			if (!SingletonBehaviour<MainOutOfBoundsHazard>.HasInstance)
			{
				return OutOfBoundsHazard.Water;
			}
			return SingletonBehaviour<MainOutOfBoundsHazard>.Instance.type;
		}
	}

	public static EliminationReason GetEliminationReason()
	{
		if (!SingletonBehaviour<MainOutOfBoundsHazard>.HasInstance)
		{
			return EliminationReason.None;
		}
		return SingletonBehaviour<MainOutOfBoundsHazard>.Instance.GetEliminationReasonInternal();
	}

	private EliminationReason GetEliminationReasonInternal()
	{
		return type.GetEliminationReason();
	}
}
