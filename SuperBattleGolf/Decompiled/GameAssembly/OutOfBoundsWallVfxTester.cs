using UnityEngine;
using UnityEngine.InputSystem;

public class OutOfBoundsWallVfxTester : MonoBehaviour
{
	[SerializeField]
	private Transform player;

	[SerializeField]
	private MeshRenderer[] walls;

	private MaterialPropertyBlock props;

	private int playerPosProperty;

	private void Start()
	{
		props = new MaterialPropertyBlock();
		playerPosProperty = Shader.PropertyToID("_Player_World_Position");
		SetWallActive(0);
	}

	private void Update()
	{
		if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
		{
			SetWallActive(0);
		}
		if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
		{
			SetWallActive(1);
		}
		if (Keyboard.current[Key.Digit3].wasPressedThisFrame)
		{
			SetWallActive(2);
		}
		props.SetVector(playerPosProperty, player.transform.position);
		for (int i = 0; i < walls.Length; i++)
		{
			walls[i].SetPropertyBlock(props);
		}
	}

	private void SetWallActive(int index)
	{
		for (int i = 0; i < walls.Length; i++)
		{
			walls[i].gameObject.SetActive(i == index);
		}
	}
}
