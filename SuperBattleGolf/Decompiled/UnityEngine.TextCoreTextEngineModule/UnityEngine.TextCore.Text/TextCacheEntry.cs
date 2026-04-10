#define UNITY_ASSERTIONS
namespace UnityEngine.TextCore.Text;

internal struct TextCacheEntry
{
	public TextHandle textHandle;

	public TextInfo textInfo;

	public float lastTimeInCache;

	public TextCacheEntry(TextHandle handle, TextInfo info, float time = 0f)
	{
		Debug.Assert(handle != null, "Internal Text Error : Creation of a not assigned to no handle");
		textHandle = handle;
		textInfo = info;
		lastTimeInCache = time;
	}
}
