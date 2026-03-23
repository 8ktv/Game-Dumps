using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror;

public abstract class BaseUIGraph : MonoBehaviour
{
	private static readonly int MaxValue = Shader.PropertyToID("_MaxValue");

	private static readonly int GraphData = Shader.PropertyToID("_GraphData");

	private static readonly int CategoryCount = Shader.PropertyToID("_CategoryCount");

	private static readonly int Colors = Shader.PropertyToID("_CategoryColors");

	private static readonly int Width = Shader.PropertyToID("_Width");

	private static readonly int DataStart = Shader.PropertyToID("_DataStart");

	public Material Material;

	public Graphic Renderer;

	[Range(1f, 64f)]
	public int Points = 64;

	public float SecondsPerPoint = 1f;

	public Color[] CategoryColors = new Color[1] { Color.cyan };

	public bool IsStacked;

	public Text[] LegendTexts;

	[Header("Diagnostics")]
	[ReadOnly]
	[SerializeField]
	private Material runtimeMaterial;

	private float[] graphData;

	private int graphDataStartIndex;

	private bool isGraphDataDirty;

	private float[] aggregatingData;

	private GraphAggregationMode[] aggregatingModes;

	private int[] aggregatingDataCounts;

	private float aggregatingTime;

	private int DataLastIndex => (graphDataStartIndex - 1 + Points) % Points;

	private void Awake()
	{
		Renderer.material = (runtimeMaterial = UnityEngine.Object.Instantiate(Material));
		graphData = new float[Points * CategoryColors.Length];
		aggregatingData = new float[CategoryColors.Length];
		aggregatingDataCounts = new int[CategoryColors.Length];
		aggregatingModes = new GraphAggregationMode[CategoryColors.Length];
		isGraphDataDirty = true;
	}

	protected virtual void OnValidate()
	{
		if (Renderer == null)
		{
			Renderer = GetComponent<Graphic>();
		}
	}

	protected virtual void Update()
	{
		for (int i = 0; i < CategoryColors.Length; i++)
		{
			CollectData(i, out var value, out var mode);
			if (value < 0f)
			{
				Debug.LogWarning("Graphing negative values is not supported.");
				value = 0f;
			}
			if (mode != aggregatingModes[i])
			{
				aggregatingModes[i] = mode;
				ResetCurrent(i);
			}
			switch (mode)
			{
			case GraphAggregationMode.Sum:
			case GraphAggregationMode.Average:
			case GraphAggregationMode.PerSecond:
				aggregatingData[i] += value;
				aggregatingDataCounts[i]++;
				break;
			case GraphAggregationMode.Min:
				if (aggregatingData[i] > value)
				{
					aggregatingData[i] = value;
				}
				break;
			case GraphAggregationMode.Max:
				if (value > aggregatingData[i])
				{
					aggregatingData[i] = value;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		aggregatingTime += Time.deltaTime;
		if (!(aggregatingTime > SecondsPerPoint))
		{
			return;
		}
		graphDataStartIndex = (graphDataStartIndex + 1) % Points;
		ClearDataAt(DataLastIndex);
		for (int j = 0; j < CategoryColors.Length; j++)
		{
			float num = aggregatingData[j];
			switch (aggregatingModes[j])
			{
			case GraphAggregationMode.Average:
				num /= (float)aggregatingDataCounts[j];
				break;
			case GraphAggregationMode.PerSecond:
				num /= aggregatingTime;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case GraphAggregationMode.Sum:
			case GraphAggregationMode.Min:
			case GraphAggregationMode.Max:
				break;
			}
			SetCurrentGraphData(j, num);
			ResetCurrent(j);
		}
		aggregatingTime = 0f;
	}

	private void ResetCurrent(int i)
	{
		if (aggregatingModes[i] == GraphAggregationMode.Min)
		{
			aggregatingData[i] = float.MaxValue;
		}
		else
		{
			aggregatingData[i] = 0f;
		}
		aggregatingDataCounts[i] = 0;
	}

	protected virtual string FormatValue(float value)
	{
		return $"{value:N1}";
	}

	protected abstract void CollectData(int category, out float value, out GraphAggregationMode mode);

	private void SetCurrentGraphData(int c, float value)
	{
		graphData[DataLastIndex * CategoryColors.Length + c] = value;
		isGraphDataDirty = true;
	}

	private void ClearDataAt(int i)
	{
		for (int j = 0; j < CategoryColors.Length; j++)
		{
			graphData[i * CategoryColors.Length + j] = 0f;
		}
		isGraphDataDirty = true;
	}

	public void LateUpdate()
	{
		if (!isGraphDataDirty)
		{
			return;
		}
		runtimeMaterial.SetInt(Width, Points);
		runtimeMaterial.SetInt(DataStart, graphDataStartIndex);
		float num = 1f;
		if (IsStacked)
		{
			for (int i = 0; i < Points; i++)
			{
				float num2 = 0f;
				for (int j = 0; j < CategoryColors.Length; j++)
				{
					num2 += graphData[i * CategoryColors.Length + j];
				}
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		else
		{
			for (int k = 0; k < graphData.Length; k++)
			{
				float num3 = graphData[k];
				if (num3 > num)
				{
					num = num3;
				}
			}
		}
		num = AdjustMaxValue(num);
		for (int l = 0; l < LegendTexts.Length; l++)
		{
			Text obj = LegendTexts[l];
			float num4 = (float)l / (float)(LegendTexts.Length - 1);
			obj.text = FormatValue(num * num4);
		}
		runtimeMaterial.SetFloat(MaxValue, num);
		runtimeMaterial.SetFloatArray(GraphData, graphData);
		runtimeMaterial.SetInt(CategoryCount, CategoryColors.Length);
		runtimeMaterial.SetColorArray(Colors, CategoryColors);
		isGraphDataDirty = false;
	}

	protected virtual float AdjustMaxValue(float max)
	{
		return Mathf.Ceil(max);
	}
}
