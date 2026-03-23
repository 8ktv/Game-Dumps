using System.IO;

namespace Mirror.BouncyCastle.Utilities.IO;

public class TeeOutputStream : BaseOutputStream
{
	private readonly Stream output;

	private readonly Stream tee;

	public TeeOutputStream(Stream output, Stream tee)
	{
		this.output = output;
		this.tee = tee;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			output.Dispose();
			tee.Dispose();
		}
		base.Dispose(disposing);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		output.Write(buffer, offset, count);
		tee.Write(buffer, offset, count);
	}

	public override void WriteByte(byte value)
	{
		output.WriteByte(value);
		tee.WriteByte(value);
	}
}
