using System;
using UnityEngine;

namespace Edgegap;

public class RelayCredentialsFromArgs : MonoBehaviour
{
	private void Awake()
	{
		string commandLine = Environment.CommandLine;
		string text = EdgegapKcpTransport.ReParse(commandLine, "session_id=(\\d+)", "111111");
		string text2 = EdgegapKcpTransport.ReParse(commandLine, "user_id=(\\d+)", "222222");
		Debug.Log("Parsed sessionId: " + text + " user_id: " + text2);
		EdgegapKcpTransport component = GetComponent<EdgegapKcpTransport>();
		component.sessionId = uint.Parse(text);
		component.userId = uint.Parse(text2);
	}
}
