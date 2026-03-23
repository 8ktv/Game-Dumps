using System.Globalization;

namespace UnityEngine.UIElements.UIR;

internal struct BMPAlloc
{
	public static readonly BMPAlloc Invalid = new BMPAlloc
	{
		page = -1
	};

	public int page;

	public ushort pageLine;

	public byte bitIndex;

	public OwnedState ownedState;

	public bool Equals(BMPAlloc other)
	{
		return page == other.page && pageLine == other.pageLine && bitIndex == other.bitIndex;
	}

	public bool IsValid()
	{
		return page >= 0;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", page, pageLine, bitIndex);
	}
}
