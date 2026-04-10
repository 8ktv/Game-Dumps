using UnityEngine;

public class JumpPadVfx : MonoBehaviour
{
	[SerializeField]
	private Animator mainAnimator;

	[SerializeField]
	private Animator arrowAnimator;

	[SerializeField]
	private MeshRenderer jumpPadBaseRenderer;

	[SerializeField]
	[Range(0f, 1f)]
	private float patternWeight;

	[SerializeField]
	[Range(0f, 1f)]
	private float patternEmission;

	[SerializeField]
	private float overlayBaseScale;

	[SerializeField]
	private float overlayArrowScale;

	private static int patternWeightId = Shader.PropertyToID("_Pattern_Weight");

	private static int patternEmissionId = Shader.PropertyToID("_Pattern_Emission");

	private static int overlayBaseScaleId = Shader.PropertyToID("_Base_Scale");

	private static int overlayArrowScaleId = Shader.PropertyToID("_Arrow_Scale");

	private static int jumpParameterHash = Animator.StringToHash("jump");

	private MaterialPropertyBlock props;

	private float previousPatternWeight = -1f;

	private float previousPatternEmission = -1f;

	private float previousOverlayBaseScale = -1f;

	private float previousOverlayArrowScale = -1f;

	private void Awake()
	{
		if (props == null)
		{
			props = new MaterialPropertyBlock();
		}
	}

	private void Update()
	{
		CheckForDifference(patternWeight, ref previousPatternWeight, patternWeightId);
		CheckForDifference(patternEmission, ref previousPatternEmission, patternEmissionId);
		CheckForDifference(overlayBaseScale, ref previousOverlayBaseScale, overlayBaseScaleId);
		CheckForDifference(overlayArrowScale, ref previousOverlayArrowScale, overlayArrowScaleId);
		jumpPadBaseRenderer.SetPropertyBlock(props, 1);
	}

	private void CheckForDifference(float currentValue, ref float previousValue, int parameterId, bool forced = false)
	{
		if (!previousValue.Approximately(currentValue) || forced)
		{
			previousValue = currentValue;
			props.SetFloat(parameterId, currentValue);
		}
	}

	public void OnActivated(bool playVfx = true)
	{
		mainAnimator.SetTrigger(jumpParameterHash);
		arrowAnimator.SetTrigger(jumpParameterHash);
		if (playVfx)
		{
			VfxManager.PlayPooledVfxLocalOnly(VfxType.JumpPadActivation, base.transform.position, base.transform.rotation);
		}
	}
}
