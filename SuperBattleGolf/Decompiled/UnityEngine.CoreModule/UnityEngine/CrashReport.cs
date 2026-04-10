using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Export/CrashReport/CrashReport.bindings.h")]
public sealed class CrashReport
{
	private static List<CrashReport> internalReports;

	private static object reportsLock = new object();

	private readonly string id;

	public readonly DateTime time;

	public readonly string text;

	public static CrashReport[] reports
	{
		get
		{
			PopulateReports();
			lock (reportsLock)
			{
				return internalReports.ToArray();
			}
		}
	}

	public static CrashReport lastReport
	{
		get
		{
			PopulateReports();
			lock (reportsLock)
			{
				if (internalReports.Count > 0)
				{
					return internalReports[internalReports.Count - 1];
				}
			}
			return null;
		}
	}

	private static int Compare(CrashReport c1, CrashReport c2)
	{
		long ticks = c1.time.Ticks;
		long ticks2 = c2.time.Ticks;
		if (ticks > ticks2)
		{
			return 1;
		}
		if (ticks < ticks2)
		{
			return -1;
		}
		return 0;
	}

	private static void PopulateReports()
	{
		lock (reportsLock)
		{
			if (internalReports == null)
			{
				string[] array = GetReports();
				internalReports = new List<CrashReport>(array.Length);
				string[] array2 = array;
				foreach (string text in array2)
				{
					double secondsSinceUnixEpoch;
					string reportData = GetReportData(text, out secondsSinceUnixEpoch);
					DateTime dateTime = new DateTime(1970, 1, 1).AddSeconds(secondsSinceUnixEpoch);
					internalReports.Add(new CrashReport(text, dateTime, reportData));
				}
				internalReports.Sort(Compare);
			}
		}
	}

	public static void RemoveAll()
	{
		CrashReport[] array = reports;
		foreach (CrashReport crashReport in array)
		{
			crashReport.Remove();
		}
	}

	private CrashReport(string id, DateTime time, string text)
	{
		this.id = id;
		this.time = time;
		this.text = text;
	}

	public void Remove()
	{
		if (RemoveReport(id))
		{
			lock (reportsLock)
			{
				internalReports.Remove(this);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "CrashReport_Bindings::GetReports", IsThreadSafe = true)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	private static extern string[] GetReports();

	[FreeFunction(Name = "CrashReport_Bindings::GetReportData", IsThreadSafe = true)]
	private unsafe static string GetReportData(string id, out double secondsSinceUnixEpoch)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(id, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(id);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetReportData_Injected(ref managedSpanWrapper, out secondsSinceUnixEpoch, out ret);
				}
			}
			else
			{
				GetReportData_Injected(ref managedSpanWrapper, out secondsSinceUnixEpoch, out ret);
			}
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction(Name = "CrashReport_Bindings::RemoveReport", IsThreadSafe = true)]
	private unsafe static bool RemoveReport(string id)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(id, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(id);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return RemoveReport_Injected(ref managedSpanWrapper);
				}
			}
			return RemoveReport_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetReportData_Injected(ref ManagedSpanWrapper id, out double secondsSinceUnixEpoch, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveReport_Injected(ref ManagedSpanWrapper id);
}
