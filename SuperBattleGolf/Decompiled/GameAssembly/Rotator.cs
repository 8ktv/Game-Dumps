using UnityEngine;

public class Rotator : MonoBehaviour
{
	[SerializeField]
	private Vector3 rotationSpeed;

	private Vector3 currentRotation;

	private void Start()
	{
		currentRotation = base.transform.localRotation.eulerAngles;
	}

	private void Update()
	{
		Rotate();
	}

	private void Rotate()
	{
		float deltaTime = Time.deltaTime;
		currentRotation += rotationSpeed * deltaTime;
		base.transform.localRotation = Quaternion.Euler(currentRotation);
	}
}
