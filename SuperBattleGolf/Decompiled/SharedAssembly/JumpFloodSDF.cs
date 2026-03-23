using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public static class JumpFloodSDF
{
	[BurstCompile]
	private struct InitSeedJob : IJobParallelFor
	{
		public NativeArray<Color> pixels;

		public NativeArray<int2> seedOut;

		public bool inverseAlpha;

		public int2 resolution;

		public void Execute(int index)
		{
			Color color = pixels[index];
			int2 @int = 0;
			@int.x = index % resolution.x;
			@int.y = index / resolution.x;
			if (color.a > 0.999999f)
			{
				seedOut[index] = (inverseAlpha ? @int : ((int2)int.MinValue));
			}
			else
			{
				seedOut[index] = (inverseAlpha ? ((int2)int.MinValue) : @int);
			}
		}
	}

	[BurstCompile]
	private struct JFAStepJob : IJobParallelFor
	{
		[ReadOnly]
		public FixedList128Bytes<int2> offsets;

		[ReadOnly]
		public NativeArray<int2> seedIn;

		public NativeArray<int2> seedOut;

		public int2 resolution;

		public int currentStep;

		public void Execute(int index)
		{
			float num = 1E+10f;
			int2 value = int.MinValue;
			int2 @int = 0;
			@int.x = index % resolution.x;
			@int.y = index / resolution.x;
			for (int i = 0; i < offsets.Length; i++)
			{
				int2 int2 = offsets[i] * currentStep;
				int2 int3 = @int + int2;
				if (math.any(int3 < 0) || math.any(int3 >= resolution))
				{
					continue;
				}
				int2 int4 = seedIn[int3.x + int3.y * resolution.x];
				if (!math.any(int4 < 0))
				{
					float num2 = math.distance(int4, @int);
					if (num2 <= num)
					{
						num = num2;
						value = int4;
					}
				}
			}
			seedOut[index] = value;
		}
	}

	[BurstCompile]
	private struct DiluteJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<Color> pixelsIn;

		[ReadOnly]
		public NativeArray<int2> seedIn;

		public NativeArray<Color> pixelsOut;

		public int2 resolution;

		public bool alphaToSdf;

		public bool srgb;

		public int stepCount;

		public void Execute(int index)
		{
			int2 @int = seedIn[index];
			if (math.any(@int < 0) || math.any(@int > resolution))
			{
				pixelsOut[index] = new Color(0f, 0f, 0f, 0f);
				return;
			}
			int2 int2 = 0;
			int2.x = index % resolution.x;
			int2.y = index / resolution.x;
			Color value = pixelsIn[@int.x + @int.y * resolution.x];
			if (alphaToSdf)
			{
				value.a = math.saturate(1f - math.length((float2)(int2 - @int) / ((float)stepCount * 1.41f)));
			}
			else
			{
				value.a = pixelsIn[index].a;
			}
			pixelsOut[index] = value;
		}
	}

	public static void Dilute(Texture2D texture, int steps, bool alphaToSdf)
	{
		NativeArray<Color> nativeArray = new NativeArray<Color>(texture.GetPixels(), Allocator.TempJob);
		NativeArray<int2> nativeArray2 = new NativeArray<int2>(nativeArray.Length, Allocator.TempJob);
		NativeArray<int2> nativeArray3 = new NativeArray<int2>(nativeArray.Length, Allocator.TempJob);
		NativeArray<Color> pixelsOut = new NativeArray<Color>(nativeArray.Length, Allocator.TempJob);
		int2 resolution = new int2(texture.width, texture.height);
		FixedList128Bytes<int2> offsets = new FixedList128Bytes<int2>
		{
			new int2(-1, -1),
			new int2(0, -1),
			new int2(1, -1),
			new int2(-1, 0),
			new int2(0, 0),
			new int2(1, 0),
			new int2(-1, 1),
			new int2(0, 1),
			new int2(1, 1)
		};
		JobHandle jobHandle = IJobParallelForExtensions.Schedule(new InitSeedJob
		{
			inverseAlpha = true,
			pixels = nativeArray,
			seedOut = nativeArray2,
			resolution = resolution
		}, nativeArray.Length, 1);
		JFAStepJob jobData = new JFAStepJob
		{
			offsets = offsets,
			resolution = resolution
		};
		JobHandle dependsOn = jobHandle;
		for (int i = 0; i < steps; i++)
		{
			jobData.currentStep = i;
			jobData.seedIn = nativeArray2;
			jobData.seedOut = nativeArray3;
			dependsOn = IJobParallelForExtensions.Schedule(jobData, nativeArray.Length, 1, dependsOn);
			NativeArray<int2> nativeArray4 = nativeArray2;
			nativeArray2 = nativeArray3;
			nativeArray3 = nativeArray4;
		}
		IJobParallelForExtensions.Schedule(new DiluteJob
		{
			pixelsIn = nativeArray,
			pixelsOut = pixelsOut,
			alphaToSdf = alphaToSdf,
			resolution = resolution,
			seedIn = nativeArray2,
			srgb = texture.isDataSRGB,
			stepCount = steps
		}, nativeArray.Length, 1, dependsOn).Complete();
		texture.SetPixels(pixelsOut.ToArray());
		texture.Apply();
		nativeArray.Dispose();
		nativeArray2.Dispose();
		nativeArray3.Dispose();
		pixelsOut.Dispose();
	}
}
