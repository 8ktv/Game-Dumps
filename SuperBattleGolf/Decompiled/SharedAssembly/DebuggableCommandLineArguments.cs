using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DebuggableCommandLineArguments
{
	public static string[] Arguments { get; private set; }

	static DebuggableCommandLineArguments()
	{
		Path.Combine(Application.streamingAssetsPath, "Debug command line arguments.txt");
		List<string> list = new List<string>();
		list.AddRange(Environment.GetCommandLineArgs());
		Arguments = list.ToArray();
	}
}
