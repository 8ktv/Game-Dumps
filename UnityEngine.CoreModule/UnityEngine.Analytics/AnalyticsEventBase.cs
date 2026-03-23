using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics;

[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode(GenerateProxy = true)]
public class AnalyticsEventBase
{
	private string eventName;

	private int eventVersion;

	private string eventPrefix;

	private SendEventOptions sendEventOptions;

	public string EventName()
	{
		return eventName;
	}

	public int EventVersion()
	{
		return eventVersion;
	}

	public string EventPrefix()
	{
		return eventPrefix;
	}

	public AnalyticsEventBase(string eventName, int eventVersion, SendEventOptions sendEventOptions = SendEventOptions.kAppendNone, string eventPrefix = "")
	{
		this.eventName = eventName;
		this.eventVersion = eventVersion;
		this.sendEventOptions = sendEventOptions;
		this.eventPrefix = eventPrefix;
	}

	public AnalyticsEventBase(AnalyticsEventBase e)
		: this(e.eventName, e.eventVersion)
	{
	}

	public AnalyticsEventBase()
	{
	}
}
