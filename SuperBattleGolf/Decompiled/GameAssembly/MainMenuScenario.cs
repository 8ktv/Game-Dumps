using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainMenuScenario : MonoBehaviour
{
	[SerializeField]
	private Transform cameraTransform;

	[SerializeField]
	private Transform cameraPointA;

	[SerializeField]
	private Transform cameraPointB;

	public async UniTask Playing(float duration)
	{
		if (!cameraTransform || !cameraPointA || !cameraPointB)
		{
			await UniTask.WaitForSeconds(duration);
			return;
		}
		float timer = 0f;
		while (timer < duration)
		{
			float t = timer / duration;
			cameraTransform.position = Vector3.Lerp(cameraPointA.position, cameraPointB.position, t);
			cameraTransform.rotation = Quaternion.Slerp(cameraPointA.rotation, cameraPointB.rotation, t);
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame();
			if (this == null)
			{
				break;
			}
		}
	}

	public void SetActive(bool isActive)
	{
		cameraTransform.gameObject.SetActive(isActive);
	}
}
