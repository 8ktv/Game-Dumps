namespace Mirror.BouncyCastle.Crypto;

internal static class Check
{
	internal static void DataLength(bool condition, string message)
	{
		if (condition)
		{
			ThrowDataLengthException(message);
		}
	}

	internal static void DataLength(byte[] buf, int off, int len, string message)
	{
		if (off > buf.Length - len)
		{
			ThrowDataLengthException(message);
		}
	}

	internal static void OutputLength(bool condition, string message)
	{
		if (condition)
		{
			ThrowOutputLengthException(message);
		}
	}

	internal static void OutputLength(byte[] buf, int off, int len, string message)
	{
		if (off > buf.Length - len)
		{
			ThrowOutputLengthException(message);
		}
	}

	internal static void ThrowDataLengthException(string message)
	{
		throw new DataLengthException(message);
	}

	internal static void ThrowOutputLengthException(string message)
	{
		throw new OutputLengthException(message);
	}
}
