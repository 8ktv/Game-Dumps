using UnityEngine;

public class DisableByTime : MonoBehaviour
{
	[SerializeField]
	private float timeToTake;

	private void OnEnable()
	{
		Invoke("DisableObject", timeToTake);
	}

	private void DisableObject()
	{
		base.gameObject.SetActive(value: false);
	}
}
