using UnityEngine;

public class ForceRotation : MonoBehaviour
{
	[SerializeField]
	private Vector3 rotation;

	[SerializeField]
	private bool useLocalRotation;

	private void Update()
	{
		if (useLocalRotation)
		{
			base.transform.localRotation = Quaternion.Euler(rotation);
		}
		else
		{
			base.transform.rotation = Quaternion.Euler(rotation);
		}
	}
}
