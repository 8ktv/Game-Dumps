using UnityEngine;

public class DoorVfx : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private DoorRotationDirection rotationDirection;

	private static readonly int clockwiseHash = Animator.StringToHash("clockwise");

	private static readonly int counterClockwiseHash = Animator.StringToHash("counter_clockwise");

	public void Open()
	{
		Animator animator = this.animator;
		animator.SetTrigger(rotationDirection switch
		{
			DoorRotationDirection.Clockwise => clockwiseHash, 
			DoorRotationDirection.CounterClockwise => counterClockwiseHash, 
			_ => clockwiseHash, 
		});
	}
}
