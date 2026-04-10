using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class DominatedVfxTester : MonoBehaviour
{
	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private Transform headCenter;

	private DominatedVfx currentInstance;

	private bool despawning;

	private void Start()
	{
		prefab.SetActive(value: false);
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			Spawn();
		}
		if (Keyboard.current[Key.W].wasPressedThisFrame)
		{
			Despawn();
		}
	}

	private void Spawn()
	{
		if (!currentInstance)
		{
			GameObject gameObject = Object.Instantiate(prefab, headCenter);
			gameObject.SetActive(value: true);
			currentInstance = gameObject.GetComponent<DominatedVfx>();
			currentInstance.Spawn(headCenter);
			currentInstance.enabled = false;
			currentInstance.transform.localPosition = Vector3.zero;
		}
	}

	private void Despawn()
	{
		if ((bool)currentInstance && !despawning)
		{
			Despawning().Forget();
		}
	}

	public async UniTaskVoid Despawning()
	{
		despawning = true;
		currentInstance.Despawn();
		await UniTask.WaitForSeconds(2.333f);
		Object.Destroy(currentInstance.gameObject);
		currentInstance = null;
		despawning = false;
	}
}
