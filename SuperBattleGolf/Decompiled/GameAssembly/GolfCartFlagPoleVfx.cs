using UnityEngine;

public class GolfCartFlagPoleVfx : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private Transform rotationTarget;

	[SerializeField]
	private Transform pivot;

	[SerializeField]
	private float rotationTargetLocalBounds;

	[SerializeField]
	private Vector2 springFactors = new Vector2(0.75f, 2f);

	private int forwardLean;

	private Vector3 followVelocity;

	private bool isFrozen;

	private Vector3 rotationTargetOriginalLocalPosition;

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	private void Start()
	{
		rotationTargetOriginalLocalPosition = rotationTarget.localPosition;
	}

	public void SetForwardLean(int forwardLean)
	{
		this.forwardLean = forwardLean;
	}

	public void SetIsFrozen(bool isFrozen)
	{
		this.isFrozen = isFrozen;
	}

	public void OnBUpdate()
	{
		if (!isFrozen)
		{
			(rotationTarget.localPosition, followVelocity) = BMath.UpdateSpring(rotationTarget.localPosition, followVelocity, GetRotationTargetTargetLocalPosition(), springFactors.x, springFactors.y, Time.deltaTime);
			pivot.rotation = Quaternion.LookRotation(rotationTarget.position - pivot.position, -base.transform.forward);
		}
	}

	private Vector3 GetRotationTargetTargetLocalPosition()
	{
		return rotationTargetOriginalLocalPosition + (float)forwardLean * rotationTargetLocalBounds * Vector3.forward;
	}
}
