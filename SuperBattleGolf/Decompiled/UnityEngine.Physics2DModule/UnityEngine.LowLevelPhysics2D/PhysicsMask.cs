using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsMask : IEnumerable<int>, IEnumerable
{
	public struct ResetBitIterator : IEnumerable<int>, IEnumerable, IEnumerator<int>, IEnumerator, IDisposable
	{
		private int m_BitIndex;

		private ulong bitMask;

		readonly int IEnumerator<int>.Current => m_BitIndex;

		readonly object IEnumerator.Current => m_BitIndex;

		public ResetBitIterator(PhysicsMask bitMask)
		{
			m_BitIndex = -1;
			this.bitMask = bitMask;
		}

		bool IEnumerator.MoveNext()
		{
			if (m_BitIndex >= 63)
			{
				return false;
			}
			while (++m_BitIndex < 64 && (bitMask & (ulong)(1L << m_BitIndex)) != 0)
			{
			}
			return m_BitIndex < 64;
		}

		void IEnumerator.Reset()
		{
			m_BitIndex = -1;
		}

		public IEnumerator<int> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		readonly void IDisposable.Dispose()
		{
		}
	}

	public struct SetBitIterator : IEnumerable<int>, IEnumerable, IEnumerator<int>, IEnumerator, IDisposable
	{
		private int m_BitIndex;

		private ulong bitMask;

		readonly int IEnumerator<int>.Current => m_BitIndex;

		readonly object IEnumerator.Current => m_BitIndex;

		public SetBitIterator(PhysicsMask bitMask)
		{
			m_BitIndex = -1;
			this.bitMask = bitMask;
		}

		bool IEnumerator.MoveNext()
		{
			if (bitMask == 0L || m_BitIndex >= 63)
			{
				return false;
			}
			while (++m_BitIndex < 64 && (bitMask & (ulong)(1L << m_BitIndex)) == 0)
			{
			}
			return m_BitIndex < 64;
		}

		void IEnumerator.Reset()
		{
			m_BitIndex = -1;
		}

		public IEnumerator<int> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		readonly void IDisposable.Dispose()
		{
		}
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class ShowAsPhysicsMaskAttribute : PropertyAttribute
	{
		public ShowAsPhysicsMaskAttribute()
			: base(applyToCollection: true)
		{
		}
	}

	public ulong bitMask;

	public static readonly PhysicsMask None = default(PhysicsMask);

	public static readonly PhysicsMask One = new PhysicsMask
	{
		bitMask = 1uL
	};

	public static readonly PhysicsMask All = new PhysicsMask
	{
		bitMask = ulong.MaxValue
	};

	public readonly ResetBitIterator resetBits => new ResetBitIterator(this);

	public readonly SetBitIterator setBits => new SetBitIterator(this);

	public PhysicsMask(params int[] bitIndicies)
	{
		ulong num = 0uL;
		foreach (int num2 in bitIndicies)
		{
			if (num2 >= 0 && num2 <= 63)
			{
				num |= (ulong)(1L << num2);
				continue;
			}
			throw new ArgumentOutOfRangeException("bitIndex", $"Bit index is out of range [0, 63]: {num2}.");
		}
		bitMask = num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PhysicsMask(LayerMask layerMask)
	{
		bitMask = (ulong)layerMask.value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly LayerMask ToLayerMask()
	{
		return (int)bitMask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetBit(int bitIndex)
	{
		if (bitIndex >= 0 && bitIndex <= 63)
		{
			bitMask |= (ulong)(1L << bitIndex);
			return;
		}
		throw new ArgumentOutOfRangeException("bitIndex", $"Bit index is out of range (0 to 63): {bitIndex}.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetBit(int bitIndex)
	{
		if (bitIndex >= 0 && bitIndex <= 63)
		{
			bitMask &= (ulong)(~(1L << bitIndex));
			return;
		}
		throw new ArgumentOutOfRangeException("bitIndex", $"Bit index is out of range [0, 63]: {bitIndex}.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool IsBitSet(int bitIndex)
	{
		if (bitIndex >= 0 && bitIndex <= 63)
		{
			return (bitMask & (ulong)(1L << bitIndex)) != 0;
		}
		throw new ArgumentOutOfRangeException("bitIndex", $"Bit index is out of range [0, 63]: {bitIndex}.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool AreBitsSet(PhysicsMask physicsMask)
	{
		return (ulong)(bitMask & physicsMask) == (ulong)physicsMask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator PhysicsMask(ulong value)
	{
		return new PhysicsMask
		{
			bitMask = value
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ulong(PhysicsMask bitMask)
	{
		return bitMask.bitMask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PhysicsMask operator |(PhysicsMask bitMaskA, PhysicsMask bitMaskB)
	{
		return bitMaskA.bitMask | bitMaskB.bitMask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PhysicsMask operator &(PhysicsMask bitMaskA, PhysicsMask bitMaskB)
	{
		return bitMaskA.bitMask & bitMaskB.bitMask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PhysicsMask operator ^(PhysicsMask bitMaskA, PhysicsMask bitMaskB)
	{
		return bitMaskA.bitMask ^ bitMaskB.bitMask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PhysicsMask operator ~(PhysicsMask bitMask)
	{
		return ~bitMask.bitMask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PhysicsMask operator <<(PhysicsMask bitMask, int bitShift)
	{
		return bitMask.bitMask << bitShift;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PhysicsMask operator >>(PhysicsMask bitMask, int bitShift)
	{
		return bitMask.bitMask >> bitShift;
	}

	public readonly IEnumerator<int> GetEnumerator()
	{
		return new SetBitIterator(this);
	}

	readonly IEnumerator IEnumerable.GetEnumerator()
	{
		return new SetBitIterator(this);
	}

	public override readonly string ToString()
	{
		return $"bitMask={bitMask}";
	}
}
