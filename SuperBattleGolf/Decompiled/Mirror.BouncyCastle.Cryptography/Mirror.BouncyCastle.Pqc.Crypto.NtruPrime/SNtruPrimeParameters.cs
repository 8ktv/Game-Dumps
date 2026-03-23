using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.NtruPrime;

public sealed class SNtruPrimeParameters : ICipherParameters
{
	private string name;

	private int p;

	private int q;

	private int _roundedBytes;

	private bool m_LPR;

	private int _w;

	private int _rqBytes;

	private int _tau0;

	private int _tau1;

	private int _tau2;

	private int _tau3;

	private int _skBytes;

	private int _pkBytes;

	private int _ctBytes;

	private int _defaultKeySize;

	private NtruPrimeEngine _primeEngine;

	public static SNtruPrimeParameters sntrup653 = new SNtruPrimeParameters("SNTRU_Prime_653", 653, 4621, LPR: false, 288, -1, -1, -1, -1, 1518, 994, 897, 865, 994, 256);

	public static SNtruPrimeParameters sntrup761 = new SNtruPrimeParameters("SNTRU_Prime_761", 761, 4591, LPR: false, 286, -1, -1, -1, -1, 1763, 1158, 1039, 1007, 1158, 256);

	public static SNtruPrimeParameters sntrup857 = new SNtruPrimeParameters("SNTRU_Prime_857", 857, 5167, LPR: false, 322, -1, -1, -1, -1, 1999, 1322, 1184, 1152, 1322, 256);

	public static SNtruPrimeParameters sntrup953 = new SNtruPrimeParameters("SNTRU_Prime_953", 953, 6343, LPR: false, 396, -1, -1, -1, -1, 2254, 1505, 1349, 1317, 1505, 256);

	public static SNtruPrimeParameters sntrup1013 = new SNtruPrimeParameters("SNTRU_Prime_1013", 1013, 7177, LPR: false, 448, -1, -1, -1, -1, 2417, 1623, 1455, 1423, 1623, 256);

	public static SNtruPrimeParameters sntrup1277 = new SNtruPrimeParameters("SNTRU_Prime_1277", 1277, 7879, LPR: false, 492, -1, -1, -1, -1, 3059, 2067, 1847, 1815, 2067, 256);

	public int P => p;

	public bool LPR => m_LPR;

	public int Q => q;

	public int DefaultKeySize => _defaultKeySize;

	internal NtruPrimeEngine PrimeEngine => _primeEngine;

	private SNtruPrimeParameters(string name, int p, int q, bool LPR, int w, int tau0, int tau1, int tau2, int tau3, int skBytes, int pkBytes, int ctBytes, int roundedBytes, int rqBytes, int defaultKeySize)
	{
		this.name = name;
		this.p = p;
		this.q = q;
		m_LPR = LPR;
		_w = w;
		_tau0 = tau0;
		_tau1 = tau1;
		_tau2 = tau2;
		_tau3 = tau3;
		_roundedBytes = roundedBytes;
		_rqBytes = rqBytes;
		_skBytes = skBytes;
		_pkBytes = pkBytes;
		_ctBytes = ctBytes;
		_primeEngine = new NtruPrimeEngine(p, q, LPR, w, tau0, tau1, tau2, tau3, skBytes, pkBytes, ctBytes, roundedBytes, rqBytes, defaultKeySize / 8);
		_defaultKeySize = defaultKeySize;
	}
}
