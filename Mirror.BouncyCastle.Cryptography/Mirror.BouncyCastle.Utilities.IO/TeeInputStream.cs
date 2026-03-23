using System.IO;

namespace Mirror.BouncyCastle.Utilities.IO;

public class TeeInputStream : BaseInputStream
{
	private readonly Stream input;

	private readonly Stream tee;

	public TeeInputStream(Stream input, Stream tee)
	{
		this.input = input;
		this.tee = tee;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			input.Dispose();
			tee.Dispose();
		}
		base.Dispose(disposing);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = input.Read(buffer, offset, count);
		if (num > 0)
		{
			tee.Write(buffer, offset, num);
		}
		return num;
	}

	public override int ReadByte()
	{
		int num = input.ReadByte();
		if (num >= 0)
		{
			tee.WriteByte((byte)num);
		}
		return num;
	}
}
