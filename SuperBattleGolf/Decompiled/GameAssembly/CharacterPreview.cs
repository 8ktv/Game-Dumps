using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPreview : MonoBehaviour
{
	[SerializeField]
	private Camera previewCamera;

	private Animator animator;

	private GameObject playerModel;

	[SerializeField]
	private GameObject playerModelPrefab;

	[SerializeField]
	private Transform playerModelRoot;

	[SerializeField]
	private RawImage previewImage;

	[SerializeField]
	private Vector3 startAngle;

	public PlayerCosmeticsSwitcher cosmeticsSwitcher;

	private RenderTexture previewTexture;

	private readonly int isGroundedHash = Animator.StringToHash("Is grounded");

	public void Initialize()
	{
		previewTexture = RenderTexture.GetTemporary(1024, 1024, 0);
		previewCamera.targetTexture = previewTexture;
		previewImage.texture = previewTexture;
	}

	public void Refresh()
	{
		if (playerModel == null)
		{
			playerModel = Object.Instantiate(playerModelPrefab, playerModelRoot);
			animator = playerModel.GetComponentInChildren<Animator>();
			cosmeticsSwitcher = playerModel.GetComponent<PlayerCosmeticsSwitcher>();
			ParticleSystem[] componentsInChildren = playerModel.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i].gameObject);
			}
		}
		playerModel.transform.localPosition = Vector3.zero;
		playerModel.transform.localEulerAngles = startAngle;
		animator.SetBool(isGroundedHash, value: true);
	}

	public void SetPreviewEnabled(bool enabled)
	{
		previewImage.enabled = enabled;
	}

	private void Update()
	{
		if (PlayerInput.UsingGamepad && InputManager.CurrentGamepad != null && !InputManager.CurrentModeMask.HasMode(InputMode.ForceDisabled))
		{
			Vector2 value = InputManager.CurrentGamepad.rightStick.value;
			if (!value.Approximately(Vector2.zero, 0.2f))
			{
				playerModel.transform.Rotate(Vector3.up * (0f - value.x) * 1.5f);
			}
		}
	}

	private void OnDestroy()
	{
		if (previewTexture != null)
		{
			previewTexture.Release();
		}
		previewTexture = null;
	}

	public void OnPreviewDrag(BaseEventData baseEventData)
	{
		if (baseEventData is PointerEventData pointerEventData)
		{
			playerModel.transform.Rotate(Vector3.up * (0f - pointerEventData.delta.x));
		}
	}
}
