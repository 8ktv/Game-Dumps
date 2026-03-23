using Cysharp.Threading.Tasks;
using UnityEngine;

public class CoffeeDispenserVfx : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	public const float bloatDuration = 0.5f;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Transform bloatSource;

	[SerializeField]
	private MeshRenderer meshRenderer;

	private MaterialPropertyBlock props;

	private Vector3 previousBloatLocalPos = Vector3.positiveInfinity;

	private float previousBloatScale = -1f;

	private static readonly int dispenseHash = Animator.StringToHash("dispense");

	private static readonly int hatchHash = Animator.StringToHash("hatch");

	private static readonly int bloatSourcePosId = Shader.PropertyToID("_BloatSourcePos");

	private static readonly int bloatMultiplierId = Shader.PropertyToID("_BloatMultiplier");

	private void Awake()
	{
		props = new MaterialPropertyBlock();
		meshRenderer.SetPropertyBlock(props, 1);
	}

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnBUpdate()
	{
		Vector3 localPosition = bloatSource.localPosition;
		float x = bloatSource.localScale.x;
		bool num = localPosition != previousBloatLocalPos;
		bool flag = x != previousBloatScale;
		if (num || flag)
		{
			props.SetVector(bloatSourcePosId, localPosition);
			props.SetFloat(bloatMultiplierId, x);
			meshRenderer.SetPropertyBlock(props, 1);
			previousBloatLocalPos = localPosition;
			previousBloatScale = x;
		}
	}

	public async void Dispensing()
	{
		animator.SetTrigger(dispenseHash);
		await UniTask.WaitForSeconds(0.5f);
		animator.SetTrigger(hatchHash);
	}
}
