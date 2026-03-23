using System;

namespace FMOD;

public struct DSP_METERING_INFO
{
	public struct LEVEL_ARRAY
	{
		private float ch0;

		private float ch1;

		private float ch2;

		private float ch3;

		private float ch4;

		private float ch5;

		private float ch6;

		private float ch7;

		private float ch8;

		private float ch9;

		private float ch10;

		private float ch11;

		private float ch12;

		private float ch13;

		private float ch14;

		private float ch15;

		private float ch16;

		private float ch17;

		private float ch18;

		private float ch19;

		private float ch20;

		private float ch21;

		private float ch22;

		private float ch23;

		private float ch24;

		private float ch25;

		private float ch26;

		private float ch27;

		private float ch28;

		private float ch29;

		private float ch30;

		private float ch31;

		public float this[int index] => index switch
		{
			0 => ch0, 
			1 => ch1, 
			2 => ch2, 
			3 => ch3, 
			4 => ch4, 
			5 => ch5, 
			6 => ch6, 
			7 => ch7, 
			8 => ch8, 
			9 => ch9, 
			10 => ch10, 
			11 => ch11, 
			12 => ch12, 
			13 => ch13, 
			14 => ch14, 
			15 => ch15, 
			16 => ch16, 
			17 => ch17, 
			18 => ch18, 
			19 => ch19, 
			20 => ch20, 
			21 => ch21, 
			22 => ch22, 
			23 => ch23, 
			24 => ch24, 
			25 => ch25, 
			26 => ch26, 
			27 => ch27, 
			28 => ch28, 
			29 => ch29, 
			30 => ch30, 
			31 => ch31, 
			_ => throw new IndexOutOfRangeException(), 
		};

		public readonly int Length => 32;

		public static implicit operator float[](LEVEL_ARRAY levels)
		{
			float[] array = new float[levels.Length];
			for (int i = 0; i < levels.Length; i++)
			{
				array[i] = levels[i];
			}
			return array;
		}

		public void CopyTo(float[] buffer)
		{
			int num = ((buffer.Length >= Length) ? Length : buffer.Length);
			for (int i = 0; i < num; i++)
			{
				buffer[i] = this[i];
			}
		}
	}

	public int numsamples;

	public LEVEL_ARRAY peaklevel;

	public LEVEL_ARRAY rmslevel;

	public short numchannels;
}
