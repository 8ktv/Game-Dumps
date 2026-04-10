using UnityEngine;

public class KnockOutVfxVisuals : MonoBehaviour
{
	[SerializeField]
	private KnockOutVfxColorSettings settings;

	[SerializeField]
	private bool useBlockedColor;

	[SerializeField]
	private ParticleSystem[] specks;

	[SerializeField]
	private ParticleSystem[] rings;

	[SerializeField]
	private ParticleSystem[] shields;

	private KnockOutVfxColor currentColor;

	public KnockOutVfxColor CurrentColor => currentColor;

	public void SetColor(KnockOutVfxColor color)
	{
		currentColor = color;
		KnockOutVfxColorData data = settings.GetData(color);
		if (data != null)
		{
			for (int i = 0; i < specks.Length; i++)
			{
				ParticleSystem.ColorOverLifetimeModule colorOverLifetime = specks[i].colorOverLifetime;
				colorOverLifetime.color = new ParticleSystem.MinMaxGradient(data.specksColorOverLifetime);
			}
			for (int j = 0; j < rings.Length; j++)
			{
				ParticleSystem.MainModule main = rings[j].main;
				main.startColor = data.ringStartColor;
			}
			for (int k = 0; k < shields.Length; k++)
			{
				ParticleSystem.MainModule main2 = shields[k].main;
				main2.startColor = (useBlockedColor ? data.shieldBlockedOutlineColor : data.shieldOutlineColor);
				shields[k].customData.SetVector(ParticleSystemCustomData.Custom1, 2, (float)color + 0.1f);
			}
		}
	}
}
