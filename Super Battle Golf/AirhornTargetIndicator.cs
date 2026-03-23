using UnityEngine;
using UnityEngine.UI;

public class AirhornTargetIndicator : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private Image[] images;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Color neutralColor;

	[SerializeField]
	private Color triggeredColor;

	[SerializeField]
	private Vector3 targetWorldOffset = new Vector3(0f, 0.75f, 0f);

	private Transform target;

	private Camera camera;

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	private void OnDestroy()
	{
		target = null;
	}

	private void Start()
	{
		if (SingletonBehaviour<GameManager>.HasInstance)
		{
			camera = GameManager.Camera;
		}
		else
		{
			camera = Camera.main;
		}
	}

	public void SetTarget(Transform newTarget)
	{
		target = newTarget;
	}

	public void SetState(AirhornTargetState state)
	{
		Animator animator = this.animator;
		animator.SetTrigger(state switch
		{
			AirhornTargetState.Idle => "idle", 
			AirhornTargetState.Neutral => "neutral", 
			AirhornTargetState.Triggered => "triggered", 
			_ => "idle", 
		});
		Color color = ((state == AirhornTargetState.Triggered) ? triggeredColor : neutralColor);
		for (int i = 0; i < images.Length; i++)
		{
			images[i].color = color;
		}
	}

	public void Show()
	{
		animator.SetTrigger("idle");
	}

	public void Hide()
	{
		animator.SetTrigger("hide");
	}

	public void OnBUpdate()
	{
		if (!(target == null) && !(camera == null))
		{
			Vector2 vector = camera.WorldToScreenPoint(target.position + targetWorldOffset);
			rectTransform.position = vector;
		}
	}
}
