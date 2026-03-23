using UnityEngine;

public class VelocityScaler : MonoBehaviour
{
	[SerializeField]
	private float minSpeed;

	[SerializeField]
	private float maxSpeed;

	[SerializeField]
	private Vector3 minScale;

	[SerializeField]
	private Vector3 maxScale;

	[SerializeField]
	private AnimationCurve scaleCurve;

	private Vector3 previousPos;

	private Vector3 currentPos;

	private float speed;

	private Camera mainCamera;

	private void Start()
	{
		previousPos = (currentPos = base.transform.position);
		mainCamera = Camera.main;
	}

	private void Update()
	{
		currentPos = base.transform.position;
		if (!(currentPos == previousPos))
		{
			speed = Vector3.Magnitude(currentPos - previousPos) / Time.deltaTime;
			float time = (speed - minSpeed) / (maxSpeed - minSpeed);
			float t = scaleCurve.Evaluate(time);
			base.transform.localScale = Vector3.Lerp(minScale, maxScale, t);
			base.transform.rotation = Quaternion.LookRotation(currentPos - previousPos);
			previousPos = currentPos;
		}
	}
}
