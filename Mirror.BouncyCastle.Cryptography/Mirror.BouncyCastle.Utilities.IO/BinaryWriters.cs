using System;
using System.IO;

namespace Mirror.BouncyCastle.Utilities.IO;

public static class BinaryWriters
{
	public static void WriteInt16BigEndian(BinaryWriter binaryWriter, short n)
	{
		short value = (BitConverter.IsLittleEndian ? Shorts.ReverseBytes(n) : n);
		binaryWriter.Write(value);
	}

	public static void WriteInt16LittleEndian(BinaryWriter binaryWriter, short n)
	{
		short value = (BitConverter.IsLittleEndian ? n : Shorts.ReverseBytes(n));
		binaryWriter.Write(value);
	}

	public static void WriteInt32BigEndian(BinaryWriter binaryWriter, int n)
	{
		int value = (BitConverter.IsLittleEndian ? Integers.ReverseBytes(n) : n);
		binaryWriter.Write(value);
	}

	public static void WriteInt32LittleEndian(BinaryWriter binaryWriter, int n)
	{
		int value = (BitConverter.IsLittleEndian ? n : Integers.ReverseBytes(n));
		binaryWriter.Write(value);
	}

	public static void WriteInt64BigEndian(BinaryWriter binaryWriter, long n)
	{
		long value = (BitConverter.IsLittleEndian ? Longs.ReverseBytes(n) : n);
		binaryWriter.Write(value);
	}

	public static void WriteInt64LittleEndian(BinaryWriter binaryWriter, long n)
	{
		long value = (BitConverter.IsLittleEndian ? n : Longs.ReverseBytes(n));
		binaryWriter.Write(value);
	}

	[CLSCompliant(false)]
	public static void WriteUInt16BigEndian(BinaryWriter binaryWriter, ushort n)
	{
		ushort value = (BitConverter.IsLittleEndian ? Shorts.ReverseBytes(n) : n);
		binaryWriter.Write(value);
	}

	[CLSCompliant(false)]
	public static void WriteUInt16LittleEndian(BinaryWriter binaryWriter, ushort n)
	{
		ushort value = (BitConverter.IsLittleEndian ? n : Shorts.ReverseBytes(n));
		binaryWriter.Write(value);
	}

	[CLSCompliant(false)]
	public static void WriteUInt32BigEndian(BinaryWriter binaryWriter, uint n)
	{
		uint value = (BitConverter.IsLittleEndian ? Integers.ReverseBytes(n) : n);
		binaryWriter.Write(value);
	}

	[CLSCompliant(false)]
	public static void WriteUInt32LittleEndian(BinaryWriter binaryWriter, uint n)
	{
		uint value = (BitConverter.IsLittleEndian ? n : Integers.ReverseBytes(n));
		binaryWriter.Write(value);
	}

	[CLSCompliant(false)]
	public static void WriteUInt64BigEndian(BinaryWriter binaryWriter, ulong n)
	{
		ulong value = (BitConverter.IsLittleEndian ? Longs.ReverseBytes(n) : n);
		binaryWriter.Write(value);
	}

	[CLSCompliant(false)]
	public static void WriteUInt64LittleEndian(BinaryWriter binaryWriter, ulong n)
	{
		ulong value = (BitConverter.IsLittleEndian ? n : Longs.ReverseBytes(n));
		binaryWriter.Write(value);
	}
}
