using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

public class HistoryBounds
{
	private readonly int boundsPerBucket;

	private readonly Queue<MinMaxBounds> fullBuckets;

	private readonly int bucketLimit;

	private MinMaxBounds? currentBucket;

	private int currentBucketSize;

	private MinMaxBounds totalMinMax;

	public int boundsCount { get; private set; }

	public Bounds total
	{
		get
		{
			Bounds result = default(Bounds);
			result.SetMinMax(totalMinMax.min, totalMinMax.max);
			return result;
		}
	}

	public HistoryBounds(int boundsLimit, int boundsPerBucket)
	{
		this.boundsPerBucket = boundsPerBucket;
		bucketLimit = boundsLimit / boundsPerBucket;
		fullBuckets = new Queue<MinMaxBounds>(bucketLimit + 1);
	}

	public void Insert(Bounds bounds)
	{
		MinMaxBounds minMaxBounds = new MinMaxBounds
		{
			min = bounds.min,
			max = bounds.max
		};
		if (boundsCount == 0)
		{
			totalMinMax = minMaxBounds;
		}
		if (!currentBucket.HasValue)
		{
			currentBucket = minMaxBounds;
		}
		else
		{
			currentBucket.Value.Encapsulate(minMaxBounds);
		}
		currentBucketSize++;
		boundsCount++;
		totalMinMax.Encapsulate(minMaxBounds);
		if (currentBucketSize != boundsPerBucket)
		{
			return;
		}
		fullBuckets.Enqueue(currentBucket.Value);
		currentBucket = null;
		currentBucketSize = 0;
		if (fullBuckets.Count <= bucketLimit)
		{
			return;
		}
		fullBuckets.Dequeue();
		boundsCount -= boundsPerBucket;
		totalMinMax = minMaxBounds;
		foreach (MinMaxBounds fullBucket in fullBuckets)
		{
			totalMinMax.Encapsulate(fullBucket);
		}
	}

	public void Reset()
	{
		fullBuckets.Clear();
		currentBucket = null;
		currentBucketSize = 0;
		boundsCount = 0;
		totalMinMax = default(MinMaxBounds);
	}
}
