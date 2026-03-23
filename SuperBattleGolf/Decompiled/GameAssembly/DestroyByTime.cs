using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
	[SerializeField]
	private float timeToTake = 2f;

	public float TimeToTake
	{
		set
		{
			timeToTake = value;
		}
	}

	private void OnEnable()
	{
		if (timeToTake == 0f)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Invoke("DestroyObject", timeToTake);
		}
	}

	private void DestroyObject()
	{
		Object.Destroy(base.gameObject);
	}
}
