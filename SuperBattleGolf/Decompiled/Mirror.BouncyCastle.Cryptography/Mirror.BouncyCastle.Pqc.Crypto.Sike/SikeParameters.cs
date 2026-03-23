using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

[Obsolete("Will be removed")]
public sealed class SikeParameters
{
	private class SikeP434Engine
	{
		internal static readonly SikeEngine Instance = new SikeEngine(434, isCompressed: false, null);
	}

	private class SikeP503Engine
	{
		internal static readonly SikeEngine Instance = new SikeEngine(503, isCompressed: false, null);
	}

	private class SikeP610Engine
	{
		internal static readonly SikeEngine Instance = new SikeEngine(610, isCompressed: false, null);
	}

	private class SikeP751Engine
	{
		internal static readonly SikeEngine Instance = new SikeEngine(751, isCompressed: false, null);
	}

	private class SikeP434CompressedEngine
	{
		internal static readonly SikeEngine Instance = new SikeEngine(434, isCompressed: true, null);
	}

	private class SikeP503CompressedEngine
	{
		internal static readonly SikeEngine Instance = new SikeEngine(503, isCompressed: true, null);
	}

	private class SikeP610CompressedEngine
	{
		internal static readonly SikeEngine Instance = new SikeEngine(610, isCompressed: true, null);
	}

	private class SikeP751CompressedEngine
	{
		internal static readonly SikeEngine Instance = new SikeEngine(751, isCompressed: true, null);
	}

	public static readonly SikeParameters sikep434 = new SikeParameters(434, isCompressed: false, "sikep434");

	public static readonly SikeParameters sikep503 = new SikeParameters(503, isCompressed: false, "sikep503");

	public static readonly SikeParameters sikep610 = new SikeParameters(610, isCompressed: false, "sikep610");

	public static readonly SikeParameters sikep751 = new SikeParameters(751, isCompressed: false, "sikep751");

	public static readonly SikeParameters sikep434_compressed = new SikeParameters(434, isCompressed: true, "sikep434_compressed");

	public static readonly SikeParameters sikep503_compressed = new SikeParameters(503, isCompressed: true, "sikep503_compressed");

	public static readonly SikeParameters sikep610_compressed = new SikeParameters(610, isCompressed: true, "sikep610_compressed");

	public static readonly SikeParameters sikep751_compressed = new SikeParameters(751, isCompressed: true, "sikep751_compressed");

	private readonly int ver;

	private readonly bool isCompressed;

	private readonly string name;

	public string Name => name;

	public int DefaultKeySize => (int)GetEngine().GetDefaultSessionKeySize();

	private SikeParameters(int ver, bool isCompressed, string name)
	{
		this.ver = ver;
		this.isCompressed = isCompressed;
		this.name = name;
	}

	internal SikeEngine GetEngine()
	{
		if (isCompressed)
		{
			return ver switch
			{
				434 => SikeP434CompressedEngine.Instance, 
				503 => SikeP503CompressedEngine.Instance, 
				610 => SikeP610CompressedEngine.Instance, 
				751 => SikeP751CompressedEngine.Instance, 
				_ => throw new InvalidOperationException(), 
			};
		}
		return ver switch
		{
			434 => SikeP434Engine.Instance, 
			503 => SikeP503Engine.Instance, 
			610 => SikeP610Engine.Instance, 
			751 => SikeP751Engine.Instance, 
			_ => throw new InvalidOperationException(), 
		};
	}
}
