using Cysharp.Threading.Tasks;
using UnityEngine;

public class BallDispenserVfx : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private Vector3 bloatPointA;

	[SerializeField]
	private Vector3 bloatPointB;

	[SerializeField]
	private Vector3 bloatPointC;

	[SerializeField]
	private float bloatPartADuration = 0.5f;

	[SerializeField]
	private AnimationCurve bloatPartACurve;

	[SerializeField]
	private float bloatPartBDuration = 0.25f;

	[SerializeField]
	private AnimationCurve bloatPartBCurve;

	private MaterialPropertyBlock props;

	private bool animating;

	public float BloatPartADuration => bloatPartADuration;

	private void Start()
	{
		props = new MaterialPropertyBlock();
		animating = false;
		UpdateBloatMultiplier(0f);
		UpdateBloatSourcePos(Vector3.zero);
	}

	private void UpdateBloatMultiplier(float bloatMultiplier)
	{
		props.SetFloat("_BloatMultiplier", bloatMultiplier);
		meshRenderer.SetPropertyBlock(props);
	}

	private void UpdateBloatSourcePos(Vector3 localPos)
	{
		props.SetVector("_BloatSourcePos", localPos);
		meshRenderer.SetPropertyBlock(props);
	}

	public async void Dispensing()
	{
		if (animating)
		{
			return;
		}
		animating = true;
		UpdateBloatMultiplier(1f);
		UpdateBloatSourcePos(bloatPointA);
		float timer = 0f;
		while (timer < bloatPartADuration)
		{
			float time = timer / bloatPartADuration;
			float t = bloatPartACurve.Evaluate(time);
			UpdateBloatSourcePos(Vector3.Lerp(bloatPointA, bloatPointB, t));
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
			if (this == null)
			{
				return;
			}
		}
		UpdateBloatSourcePos(bloatPointB);
		timer = 0f;
		while (timer < bloatPartBDuration)
		{
			float time2 = timer / bloatPartBDuration;
			float t2 = bloatPartBCurve.Evaluate(time2);
			UpdateBloatSourcePos(Vector3.Lerp(bloatPointB, bloatPointC, t2));
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
			if (this == null)
			{
				return;
			}
		}
		UpdateBloatSourcePos(bloatPointC);
		UpdateBloatMultiplier(0f);
		animating = false;
	}
}
