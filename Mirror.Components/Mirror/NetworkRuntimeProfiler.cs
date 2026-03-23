using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror;

public class NetworkRuntimeProfiler : MonoBehaviour
{
	[Serializable]
	public class Sorter : IComparer<Stat>
	{
		public SortBy Order;

		public int Compare(Stat a, Stat b)
		{
			if (a == null)
			{
				return 1;
			}
			if (b == null)
			{
				return -1;
			}
			return Order switch
			{
				SortBy.RecentBytes => b.RecentBytes.CompareTo(a.RecentBytes), 
				SortBy.RecentCount => b.RecentCount.CompareTo(a.RecentCount), 
				SortBy.TotalBytes => b.TotalBytes.CompareTo(a.TotalBytes), 
				SortBy.TotalCount => b.TotalCount.CompareTo(a.TotalCount), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}

	public enum SortBy
	{
		RecentBytes,
		RecentCount,
		TotalBytes,
		TotalCount
	}

	public class Stat
	{
		public string Name;

		public long TotalCount;

		public long TotalBytes;

		public long RecentCount;

		public long RecentBytes;

		public void ResetRecent()
		{
			RecentCount = 0L;
			RecentBytes = 0L;
		}

		public void Add(int count, int bytes)
		{
			TotalBytes += bytes;
			TotalCount += count;
			RecentBytes += bytes;
			RecentCount += count;
		}
	}

	private class MessageStats
	{
		public readonly Dictionary<Type, Stat> MessageByType = new Dictionary<Type, Stat>();

		public readonly Dictionary<ushort, Stat> RpcByHash = new Dictionary<ushort, Stat>();

		public void Record(NetworkDiagnostics.MessageInfo info)
		{
			Type type = info.message.GetType();
			if (!MessageByType.TryGetValue(type, out var value))
			{
				value = new Stat
				{
					Name = type.ToString(),
					TotalCount = 0L,
					TotalBytes = 0L,
					RecentCount = 0L,
					RecentBytes = 0L
				};
				MessageByType[type] = value;
			}
			value.Add(info.count, info.bytes * info.count);
			if (info.message is CommandMessage commandMessage)
			{
				RecordRpc(commandMessage.functionHash, info);
			}
			else if (info.message is RpcMessage rpcMessage)
			{
				RecordRpc(rpcMessage.functionHash, info);
			}
		}

		private void RecordRpc(ushort hash, NetworkDiagnostics.MessageInfo info)
		{
			if (!RpcByHash.TryGetValue(hash, out var value))
			{
				string name = "n/a";
				RemoteCallDelegate remoteCallDelegate = RemoteProcedureCalls.GetDelegate(hash);
				if (remoteCallDelegate != null)
				{
					name = string.Format("{0}.{1}", remoteCallDelegate.Method.DeclaringType, remoteCallDelegate.GetMethodName().Replace("InvokeUserCode_", ""));
				}
				value = new Stat
				{
					Name = name,
					TotalCount = 0L,
					TotalBytes = 0L,
					RecentCount = 0L,
					RecentBytes = 0L
				};
				RpcByHash[hash] = value;
			}
			value.Add(info.count, info.bytes * info.count);
		}

		public void ResetRecent()
		{
			foreach (Stat value in MessageByType.Values)
			{
				value.ResetRecent();
			}
			foreach (Stat value2 in RpcByHash.Values)
			{
				value2.ResetRecent();
			}
		}
	}

	public enum OutputType
	{
		UnityLog,
		StdOut,
		File
	}

	[Tooltip("How many seconds to accumulate 'recent' stats for, this is also the output interval")]
	public float RecentDuration = 5f;

	public Sorter Sort = new Sorter();

	public OutputType Output;

	[Tooltip("If Output is set to 'File', where to the path of that file")]
	public string OutputFilePath = "network-stats.log";

	private readonly MessageStats inStats = new MessageStats();

	private readonly MessageStats outStats = new MessageStats();

	private readonly StringBuilder printBuilder = new StringBuilder();

	private float elapsedSinceReset;

	private void Start()
	{
		NetworkDiagnostics.InMessageEvent += HandleMessageIn;
		NetworkDiagnostics.OutMessageEvent += HandleMessageOut;
	}

	private void OnDestroy()
	{
		NetworkDiagnostics.InMessageEvent -= HandleMessageIn;
		NetworkDiagnostics.OutMessageEvent -= HandleMessageOut;
	}

	private void HandleMessageOut(NetworkDiagnostics.MessageInfo info)
	{
		outStats.Record(info);
	}

	private void HandleMessageIn(NetworkDiagnostics.MessageInfo info)
	{
		inStats.Record(info);
	}

	private void LateUpdate()
	{
		elapsedSinceReset += Time.deltaTime;
		if (elapsedSinceReset > RecentDuration)
		{
			elapsedSinceReset = 0f;
			Print();
			inStats.ResetRecent();
			outStats.ResetRecent();
		}
	}

	private void Print()
	{
		printBuilder.Clear();
		printBuilder.AppendLine($"Stats for {DateTime.Now} ({RecentDuration:N1}s interval)");
		int length = "OUT Message".Length;
		foreach (Stat value2 in inStats.MessageByType.Values)
		{
			if (value2.Name.Length > length)
			{
				length = value2.Name.Length;
			}
		}
		foreach (Stat value3 in outStats.MessageByType.Values)
		{
			if (value3.Name.Length > length)
			{
				length = value3.Name.Length;
			}
		}
		foreach (Stat value4 in inStats.RpcByHash.Values)
		{
			if (value4.Name.Length > length)
			{
				length = value4.Name.Length;
			}
		}
		foreach (Stat value5 in outStats.RpcByHash.Values)
		{
			if (value5.Name.Length > length)
			{
				length = value5.Name.Length;
			}
		}
		string text = "Recent Bytes";
		string text2 = "Recent Count";
		string text3 = "Total Bytes";
		string text4 = "Total Count";
		int length2 = FormatBytes(999999L).Length;
		int length3 = FormatCount(999999L).Length;
		int totalWidth = Mathf.Max(text.Length, length2);
		int totalWidth2 = Mathf.Max(text2.Length, length3);
		int totalWidth3 = Mathf.Max(text3.Length, length2);
		int totalWidth4 = Mathf.Max(text4.Length, length3);
		string text5 = "| " + "IN Message".PadLeft(length) + " | " + text.PadLeft(totalWidth) + " | " + text2.PadLeft(totalWidth2) + " | " + text3.PadLeft(totalWidth3) + " | " + text4.PadLeft(totalWidth4) + " |";
		string value = "".PadLeft(text5.Length, '-');
		printBuilder.AppendLine(value);
		printBuilder.AppendLine(text5);
		printBuilder.AppendLine(value);
		foreach (Stat item in inStats.MessageByType.Values.OrderBy((Stat stat) => stat, Sort))
		{
			printBuilder.AppendLine("| " + item.Name.PadLeft(length) + " | " + FormatBytes(item.RecentBytes).PadLeft(totalWidth) + " | " + FormatCount(item.RecentCount).PadLeft(totalWidth2) + " | " + FormatBytes(item.TotalBytes).PadLeft(totalWidth3) + " | " + FormatCount(item.TotalCount).PadLeft(totalWidth4) + " |");
		}
		text5 = "| " + "IN RPCs".PadLeft(length) + " | " + text.PadLeft(totalWidth) + " | " + text2.PadLeft(totalWidth2) + " | " + text3.PadLeft(totalWidth3) + " | " + text4.PadLeft(totalWidth4) + " |";
		printBuilder.AppendLine(value);
		printBuilder.AppendLine(text5);
		printBuilder.AppendLine(value);
		foreach (Stat item2 in inStats.RpcByHash.Values.OrderBy((Stat stat) => stat, Sort))
		{
			printBuilder.AppendLine("| " + item2.Name.PadLeft(length) + " | " + FormatBytes(item2.RecentBytes).PadLeft(totalWidth) + " | " + FormatCount(item2.RecentCount).PadLeft(totalWidth2) + " | " + FormatBytes(item2.TotalBytes).PadLeft(totalWidth3) + " | " + FormatCount(item2.TotalCount).PadLeft(totalWidth4) + " |");
		}
		text5 = "| " + "OUT Message".PadLeft(length) + " | " + text.PadLeft(totalWidth) + " | " + text2.PadLeft(totalWidth2) + " | " + text3.PadLeft(totalWidth3) + " | " + text4.PadLeft(totalWidth4) + " |";
		printBuilder.AppendLine(value);
		printBuilder.AppendLine(text5);
		printBuilder.AppendLine(value);
		foreach (Stat item3 in outStats.MessageByType.Values.OrderBy((Stat stat) => stat, Sort))
		{
			printBuilder.AppendLine("| " + item3.Name.PadLeft(length) + " | " + FormatBytes(item3.RecentBytes).PadLeft(totalWidth) + " | " + FormatCount(item3.RecentCount).PadLeft(totalWidth2) + " | " + FormatBytes(item3.TotalBytes).PadLeft(totalWidth3) + " | " + FormatCount(item3.TotalCount).PadLeft(totalWidth4) + " |");
		}
		text5 = "| " + "OUT RPCs".PadLeft(length) + " | " + text.PadLeft(totalWidth) + " | " + text2.PadLeft(totalWidth2) + " | " + text3.PadLeft(totalWidth3) + " | " + text4.PadLeft(totalWidth4) + " |";
		printBuilder.AppendLine(value);
		printBuilder.AppendLine(text5);
		printBuilder.AppendLine(value);
		foreach (Stat item4 in outStats.RpcByHash.Values.OrderBy((Stat stat) => stat, Sort))
		{
			printBuilder.AppendLine("| " + item4.Name.PadLeft(length) + " | " + FormatBytes(item4.RecentBytes).PadLeft(totalWidth) + " | " + FormatCount(item4.RecentCount).PadLeft(totalWidth2) + " | " + FormatBytes(item4.TotalBytes).PadLeft(totalWidth3) + " | " + FormatCount(item4.TotalCount).PadLeft(totalWidth4) + " |");
		}
		printBuilder.AppendLine(value);
		switch (Output)
		{
		case OutputType.UnityLog:
			Debug.Log(printBuilder.ToString());
			break;
		case OutputType.StdOut:
			Console.Write(printBuilder);
			break;
		case OutputType.File:
			File.AppendAllText(OutputFilePath, printBuilder.ToString());
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private static string FormatBytes(long bytes)
	{
		if ((double)bytes < 1024.0)
		{
			return $"{bytes:N0} B";
		}
		if ((double)bytes < 1048576.0)
		{
			return $"{(double)bytes / 1024.0:N2} KiB";
		}
		if ((double)bytes < 1073741824.0)
		{
			return $"{(double)bytes / 1048576.0:N2} MiB";
		}
		if ((double)bytes < 1099511627776.0)
		{
			return $"{(double)bytes / 1073741824.0:N2} GiB";
		}
		return $"{(double)bytes / 1099511627776.0:N2} TiB";
	}

	private string FormatCount(long count)
	{
		if ((double)count < 1000.0)
		{
			return $"{count:N0}";
		}
		if ((double)count < 1000000.0)
		{
			return $"{(double)count / 1000.0:N2} K";
		}
		if ((double)count < 1000000000.0)
		{
			return $"{(double)count / 1000000.0:N2} M";
		}
		if ((double)count < 1000000000000.0)
		{
			return $"{(double)count / 1000000000.0:N2} G";
		}
		return $"{(double)count / 1000000000000.0:N2} T";
	}
}
