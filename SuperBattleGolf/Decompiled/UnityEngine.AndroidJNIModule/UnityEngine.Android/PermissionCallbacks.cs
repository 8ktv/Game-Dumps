using System;

namespace UnityEngine.Android;

public class PermissionCallbacks : AndroidJavaProxy
{
	private enum Result
	{
		Dismissed,
		Granted,
		Denied,
		DeniedDontAskAgain
	}

	public event Action<string> PermissionGranted;

	public event Action<string> PermissionDenied;

	[Obsolete("Unreliable. Query ShouldShowRequestPermissionRationale and use PermissionDenied event.", false)]
	public event Action<string> PermissionDeniedAndDontAskAgain;

	public event Action<string> PermissionRequestDismissed;

	public PermissionCallbacks()
		: base("com.unity3d.player.IPermissionRequestCallbacks")
	{
	}

	public override IntPtr Invoke(string methodName, IntPtr javaArgs)
	{
		if (methodName == "onPermissionResult")
		{
			onPermissionResult(javaArgs);
			return IntPtr.Zero;
		}
		return base.Invoke(methodName, javaArgs);
	}

	private void onPermissionResult(IntPtr javaArgs)
	{
		IntPtr objectArrayElement = AndroidJNISafe.GetObjectArrayElement(javaArgs, 0);
		int[] array = AndroidJNISafe.FromIntArray(AndroidJNISafe.GetObjectArrayElement(javaArgs, 1));
		for (int i = 0; i < array.Length; i++)
		{
			string stringChars = AndroidJNISafe.GetStringChars(AndroidJNISafe.GetObjectArrayElement(objectArrayElement, i));
			switch ((Result)array[i])
			{
			case Result.Dismissed:
				if (this.PermissionRequestDismissed == null)
				{
					break;
				}
				this.PermissionRequestDismissed(stringChars);
				continue;
			case Result.Granted:
				this.PermissionGranted?.Invoke(stringChars);
				continue;
			case Result.DeniedDontAskAgain:
				if (this.PermissionDeniedAndDontAskAgain == null)
				{
					break;
				}
				this.PermissionDeniedAndDontAskAgain(stringChars);
				continue;
			case Result.Denied:
				break;
			default:
				continue;
			}
			this.PermissionDenied?.Invoke(stringChars);
		}
	}
}
