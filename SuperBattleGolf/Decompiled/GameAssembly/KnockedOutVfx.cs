using System;
using System.Collections.Generic;
using UnityEngine;

public class KnockedOutVfx : MonoBehaviour
{
	[SerializeField]
	private PoolableParticleSystem asPoolable;

	[SerializeField]
	private KnockedOutStar starTemplate;

	[SerializeField]
	private Transform starContainer;

	[SerializeField]
	private float starOffset;

	private readonly List<KnockedOutStar> stars = new List<KnockedOutStar>();

	public int maxVisibleStarCount;

	public PoolableParticleSystem AsPoolable => asPoolable;

	public void Initialize(int starCount, bool highEnergy)
	{
		starTemplate.gameObject.SetActive(value: false);
		float num = 360f / (float)starCount;
		maxVisibleStarCount = starCount;
		for (int i = 0; i < maxVisibleStarCount; i++)
		{
			float angle = (float)i * num * (MathF.PI / 180f);
			Vector3 vector = new Vector3(BMath.Cos(angle), 0f, BMath.Sin(angle));
			KnockedOutStar knockedOutStar = GetOrCreateKnockedOutStar(i);
			knockedOutStar.gameObject.SetActive(value: false);
			knockedOutStar.transform.localPosition = vector * starOffset;
			knockedOutStar.Initialize(highEnergy);
			knockedOutStar.gameObject.SetActive(value: true);
			stars[i] = knockedOutStar;
		}
		for (int j = maxVisibleStarCount; j < stars.Count; j++)
		{
			stars[j].gameObject.SetActive(value: false);
		}
		KnockedOutStar GetOrCreateKnockedOutStar(int index)
		{
			if (index < stars.Count)
			{
				return stars[index];
			}
			KnockedOutStar knockedOutStar2 = UnityEngine.Object.Instantiate(starTemplate, starContainer);
			stars.Add(knockedOutStar2);
			return knockedOutStar2;
		}
	}

	public void SetColoredStarCount(int coloredStarCount)
	{
		for (int i = 0; i < maxVisibleStarCount; i++)
		{
			stars[i].SetColored(i < coloredStarCount);
		}
	}
}
