using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class RivalryVfxTester : MonoBehaviour
{
	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private Transform head;

	private RivalryVfx currentInstance;

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
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.SetActive(value: true);
			currentInstance = gameObject.GetComponent<RivalryVfx>();
			currentInstance.Spawn(head);
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
		await UniTask.WaitForSeconds(1.5f);
		Object.Destroy(currentInstance.gameObject);
		currentInstance = null;
		despawning = false;
	}
}
