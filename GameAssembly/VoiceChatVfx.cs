using UnityEngine;

public class VoiceChatVfx : MonoBehaviour
{
	[SerializeField]
	private PoolableParticleSystem asPoolable;

	[SerializeField]
	private VoiceChatVfxSettings settings;

	[SerializeField]
	private ParticleSystem[] particles;

	[SerializeField]
	private ParticleSystemRenderer mouthParticlesRenderer;

	[SerializeField]
	private ParticleSystemRenderer headParticlesRenderer;

	private float particlesDuration;

	private float playTimer;

	private float queuedInterpolation = -1f;

	private bool isPlaying;

	private Camera mainCamera;

	public PoolableParticleSystem AsPoolable => asPoolable;

	private void OnEnable()
	{
		TryInitializeCamera();
	}

	private void TryInitializeCamera()
	{
		if (!(mainCamera != null))
		{
			if (GameManager.Camera != null)
			{
				mainCamera = GameManager.Camera;
			}
			else
			{
				mainCamera = Camera.main;
			}
		}
	}

	private void Start()
	{
		TryInitializeCamera();
		for (int i = 0; i < particles.Length; i++)
		{
			ParticleSystem.MainModule main = particles[i].main;
			main.loop = false;
		}
		SetIntensityInternal(0f);
	}

	public void SetPlaying(bool playing)
	{
		if (isPlaying == playing)
		{
			return;
		}
		isPlaying = playing;
		if (isPlaying)
		{
			for (int i = 0; i < particles.Length; i++)
			{
				particles[i].Play(withChildren: true);
			}
			return;
		}
		playTimer = 0f;
		for (int j = 0; j < particles.Length; j++)
		{
			particles[j].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
	}

	private void Update()
	{
		if (!isPlaying)
		{
			return;
		}
		CheckCameraAlignment();
		if (playTimer <= 0f)
		{
			if (queuedInterpolation != -1f)
			{
				SetIntensityInternal(queuedInterpolation);
				queuedInterpolation = -1f;
			}
			for (int i = 0; i < particles.Length; i++)
			{
				particles[i].Play(withChildren: true);
			}
			playTimer = particlesDuration;
		}
		else
		{
			playTimer -= Time.deltaTime;
		}
	}

	public void SetIntensity(float interpolation)
	{
		if (isPlaying)
		{
			queuedInterpolation = interpolation;
		}
		else
		{
			SetIntensityInternal(interpolation);
		}
	}

	private void SetIntensityInternal(float intensity)
	{
		for (int i = 0; i < particles.Length; i++)
		{
			SetIntensityInternal(particles[i], intensity);
		}
	}

	private void SetIntensityInternal(ParticleSystem particles, float intensity)
	{
		particles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		float num = (particlesDuration = Mathf.Lerp(settings.LifetimeRange.x, settings.LifetimeRange.y, intensity));
		ParticleSystem.MainModule main = particles.main;
		main.duration = num;
		main.startLifetime = new ParticleSystem.MinMaxCurve(num);
		main.startSizeX = new ParticleSystem.MinMaxCurve(settings.DefaultXSize.x, settings.DefaultXSize.y);
		main.startSizeY = new ParticleSystem.MinMaxCurve(BMath.LerpClamped(settings.YSizeRangeMin.x, settings.YSizeRangeMax.x, intensity), BMath.LerpClamped(settings.YSizeRangeMin.y, settings.YSizeRangeMax.y, intensity));
		int num2 = Mathf.RoundToInt(Mathf.Lerp(settings.BurstCountRange.x, settings.BurstCountRange.y, intensity));
		particles.emission.SetBurst(0, new ParticleSystem.Burst(0f, new ParticleSystem.MinMaxCurve(num2), 1, 0.01f));
		ParticleSystem.ShapeModule shape = particles.shape;
		shape.length = Mathf.Lerp(settings.ShapeLengthRange.x, settings.ShapeLengthRange.y, intensity);
	}

	private void CheckCameraAlignment()
	{
		if (!(mainCamera == null))
		{
			bool flag = ShouldShowAtMouth();
			mouthParticlesRenderer.enabled = flag;
			headParticlesRenderer.enabled = !flag;
		}
		bool ShouldShowAtMouth()
		{
			Vector3 forward = mainCamera.transform.forward;
			float num = Vector3.Dot(forward, base.transform.up);
			float num2 = Vector3.Dot(forward, mouthParticlesRenderer.transform.forward);
			for (int i = 0; i < settings.MouthCameraThresholds.Length - 1; i++)
			{
				VoiceChatVfxSettings.MouthCameraThreshold mouthCameraThreshold = settings.MouthCameraThresholds[i];
				if (!(num > mouthCameraThreshold.maxUpwardness))
				{
					return num2 < mouthCameraThreshold.forwardednessThreshold;
				}
			}
			return num2 < settings.MouthCameraThresholds[^1].forwardednessThreshold;
		}
	}
}
