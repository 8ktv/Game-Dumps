using UnityEngine;

public class SecondaryOutOfBoundsHazard : MonoBehaviour
{
	private const float unscaledSize = 10f;

	[SerializeField]
	private OutOfBoundsHazard type;

	public OutOfBoundsHazard Type => type;

	private void Start()
	{
		BoundsManager.RegisterSecondaryOutOfBoundsHazard(this);
	}

	private void OnDestroy()
	{
		BoundsManager.DeregisterSecondaryOutOfBoundsHazard(this);
	}

	public EliminationReason GetEliminationReason()
	{
		return type.GetEliminationReason();
	}

	public Vector2 GetSize()
	{
		return base.transform.localScale.AsHorizontal2() * 10f;
	}
}
