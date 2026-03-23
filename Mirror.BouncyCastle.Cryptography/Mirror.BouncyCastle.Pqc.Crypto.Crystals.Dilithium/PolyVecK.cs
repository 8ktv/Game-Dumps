namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

internal class PolyVecK
{
	public readonly Poly[] Vec;

	private readonly DilithiumEngine Engine;

	private readonly int K;

	public PolyVecK(DilithiumEngine Engine)
	{
		this.Engine = Engine;
		K = Engine.K;
		Vec = new Poly[K];
		for (int i = 0; i < K; i++)
		{
			Vec[i] = new Poly(Engine);
		}
	}

	public void UniformEta(byte[] seed, ushort nonce)
	{
		ushort num = nonce;
		for (int i = 0; i < K; i++)
		{
			Vec[i].UniformEta(seed, num++);
		}
	}

	public void Reduce()
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].ReducePoly();
		}
	}

	public void Ntt()
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].PolyNtt();
		}
	}

	public void InverseNttToMont()
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].InverseNttToMont();
		}
	}

	public void AddPolyVecK(PolyVecK b)
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].AddPoly(b.Vec[i]);
		}
	}

	public void Subtract(PolyVecK v)
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].Subtract(v.Vec[i]);
		}
	}

	public void ConditionalAddQ()
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].ConditionalAddQ();
		}
	}

	public void Power2Round(PolyVecK v)
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].Power2Round(v.Vec[i]);
		}
	}

	public void Decompose(PolyVecK v)
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].Decompose(v.Vec[i]);
		}
	}

	public void PackW1(byte[] r)
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].PackW1(r, i * Engine.PolyW1PackedBytes);
		}
	}

	public void PointwisePolyMontgomery(Poly a, PolyVecK v)
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].PointwiseMontgomery(a, v.Vec[i]);
		}
	}

	public bool CheckNorm(int bound)
	{
		for (int i = 0; i < K; i++)
		{
			if (Vec[i].CheckNorm(bound))
			{
				return true;
			}
		}
		return false;
	}

	public int MakeHint(PolyVecK v0, PolyVecK v1)
	{
		int num = 0;
		for (int i = 0; i < K; i++)
		{
			num += Vec[i].PolyMakeHint(v0.Vec[i], v1.Vec[i]);
		}
		return num;
	}

	public void UseHint(PolyVecK a, PolyVecK h)
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].PolyUseHint(a.Vec[i], h.Vec[i]);
		}
	}

	public void ShiftLeft()
	{
		for (int i = 0; i < K; i++)
		{
			Vec[i].ShiftLeft();
		}
	}
}
