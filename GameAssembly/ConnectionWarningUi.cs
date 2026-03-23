using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

public class ConnectionWarningUi : MonoBehaviour
{
	public float warningTimeout = 0.5f;

	public float fadeDuration = 0.25f;

	private UiVisibilityController visibilityController;

	private float currentAlpha;

	private bool isShowingWarning;

	private void Awake()
	{
		visibilityController = GetComponent<UiVisibilityController>();
		visibilityController.SetDesiredAlpha(0f);
		isShowingWarning = false;
	}

	private void Update()
	{
		if (NetworkServer.active || !NetworkClient.isConnected)
		{
			return;
		}
		bool flag = BMath.GetTimeSince(NetworkTime.ClientLastPongsTime) > warningTimeout;
		if (flag && !isShowingWarning)
		{
			visibilityController.AnimatedDesiredAlpha(1f, fadeDuration, (float x) => x).Forget();
		}
		else if (!flag && isShowingWarning)
		{
			visibilityController.AnimatedDesiredAlpha(0f, fadeDuration, (float x) => x).Forget();
		}
		isShowingWarning = flag;
	}
}
