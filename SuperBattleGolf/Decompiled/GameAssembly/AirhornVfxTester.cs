using UnityEngine;
using UnityEngine.InputSystem;

public class AirhornVfxTester : MonoBehaviour
{
	[SerializeField]
	private AirhornTargetIndicator indicatorA;

	[SerializeField]
	private AirhornTargetIndicator indicatorB;

	[SerializeField]
	private AirhornTargetIndicator indicatorC;

	[SerializeField]
	private AirhornTargetIndicator indicatorD;

	[SerializeField]
	private ParticleSystem rangeIndicatorVfx;

	[SerializeField]
	private ParticleSystem activationVfx;

	[SerializeField]
	private ParticleSystem[] airhornTriggeredVfx;

	[SerializeField]
	private Animator airhornAnimator;

	private void Start()
	{
		indicatorA.gameObject.SetActive(value: false);
		indicatorB.gameObject.SetActive(value: false);
		indicatorC.gameObject.SetActive(value: false);
		indicatorD.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			OnArmAirhorn();
		}
		if (Keyboard.current[Key.W].wasPressedThisFrame)
		{
			OnActivateAirhorn();
		}
		if (Keyboard.current[Key.E].wasPressedThisFrame)
		{
			HideIndicators();
		}
	}

	private void OnArmAirhorn()
	{
		rangeIndicatorVfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		rangeIndicatorVfx.Play(withChildren: true);
		indicatorA.gameObject.SetActive(value: true);
		indicatorB.gameObject.SetActive(value: true);
		indicatorC.gameObject.SetActive(value: true);
		indicatorD.gameObject.SetActive(value: true);
		indicatorA.SetState(AirhornTargetState.Idle);
		indicatorB.SetState(AirhornTargetState.Idle);
		indicatorC.SetState(AirhornTargetState.Idle);
		indicatorD.SetState(AirhornTargetState.Idle);
	}

	private void OnActivateAirhorn()
	{
		airhornAnimator.SetTrigger("activate");
		rangeIndicatorVfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		activationVfx.Play(withChildren: true);
		indicatorA.SetState(AirhornTargetState.Triggered);
		indicatorB.SetState(AirhornTargetState.Neutral);
		indicatorC.SetState(AirhornTargetState.Triggered);
		indicatorD.SetState(AirhornTargetState.Neutral);
		for (int i = 0; i < airhornTriggeredVfx.Length; i++)
		{
			airhornTriggeredVfx[i].Play(withChildren: true);
		}
	}

	private void HideIndicators()
	{
		indicatorA.Hide();
		indicatorB.Hide();
		indicatorC.Hide();
		indicatorD.Hide();
	}
}
