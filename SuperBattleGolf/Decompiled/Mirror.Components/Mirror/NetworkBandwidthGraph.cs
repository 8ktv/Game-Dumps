using System;
using UnityEngine;

namespace Mirror;

public class NetworkBandwidthGraph : BaseUIGraph
{
	private int dataIn;

	private int dataOut;

	private static readonly string[] Units = new string[3] { "B/s", "KiB/s", "MiB/s" };

	private const float UnitScale = 1024f;

	private void Start()
	{
		NetworkDiagnostics.InMessageEvent += OnReceive;
		NetworkDiagnostics.OutMessageEvent += OnSend;
	}

	private void OnEnable()
	{
		dataIn = 0;
		dataOut = 0;
	}

	private void OnDestroy()
	{
		NetworkDiagnostics.InMessageEvent -= OnReceive;
		NetworkDiagnostics.OutMessageEvent -= OnSend;
	}

	private void OnSend(NetworkDiagnostics.MessageInfo obj)
	{
		dataOut += obj.bytes;
	}

	private void OnReceive(NetworkDiagnostics.MessageInfo obj)
	{
		dataIn += obj.bytes;
	}

	protected override void CollectData(int category, out float value, out GraphAggregationMode mode)
	{
		mode = GraphAggregationMode.PerSecond;
		switch (category)
		{
		case 0:
			value = dataIn;
			dataIn = 0;
			break;
		case 1:
			value = dataOut;
			dataOut = 0;
			break;
		default:
			throw new ArgumentOutOfRangeException($"{category} is not valid.");
		}
	}

	protected override string FormatValue(float value)
	{
		string arg = null;
		for (int i = 0; i < Units.Length; i++)
		{
			arg = Units[i];
			if (i > 0)
			{
				value /= 1024f;
			}
			if (value < 1024f)
			{
				break;
			}
		}
		return $"{value:N0} {arg}";
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (CategoryColors.Length != 2)
		{
			CategoryColors = new Color[2]
			{
				Color.red,
				Color.green
			};
		}
		IsStacked = false;
	}
}
