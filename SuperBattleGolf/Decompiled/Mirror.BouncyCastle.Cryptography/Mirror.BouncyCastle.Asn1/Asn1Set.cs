using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Asn1;

public abstract class Asn1Set : Asn1Object, IEnumerable<Asn1Encodable>, IEnumerable
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(Asn1Set), 17)
		{
		}

		internal override Asn1Object FromImplicitConstructed(Asn1Sequence sequence)
		{
			return sequence.ToAsn1Set();
		}
	}

	private class Asn1SetParserImpl : Asn1SetParser, IAsn1Convertible
	{
		private readonly Asn1Set m_outer;

		private int m_index;

		public Asn1SetParserImpl(Asn1Set outer)
		{
			m_outer = outer;
			m_index = 0;
		}

		public IAsn1Convertible ReadObject()
		{
			Asn1Encodable[] elements = m_outer.m_elements;
			if (m_index >= elements.Length)
			{
				return null;
			}
			Asn1Encodable asn1Encodable = elements[m_index++];
			if (asn1Encodable is Asn1Sequence asn1Sequence)
			{
				return asn1Sequence.Parser;
			}
			if (asn1Encodable is Asn1Set asn1Set)
			{
				return asn1Set.Parser;
			}
			return asn1Encodable;
		}

		public virtual Asn1Object ToAsn1Object()
		{
			return m_outer;
		}
	}

	internal readonly Asn1Encodable[] m_elements;

	internal DerEncoding[] m_sortedDerEncodings;

	public virtual Asn1Encodable this[int index] => m_elements[index];

	public virtual int Count => m_elements.Length;

	public Asn1SetParser Parser => new Asn1SetParserImpl(this);

	public static Asn1Set GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Asn1Set result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is Asn1Set result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (Asn1Set)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct set from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static Asn1Set GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (Asn1Set)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	protected internal Asn1Set()
	{
		m_elements = Asn1EncodableVector.EmptyElements;
		m_sortedDerEncodings = null;
	}

	protected internal Asn1Set(Asn1Encodable element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		m_elements = new Asn1Encodable[1] { element };
		m_sortedDerEncodings = null;
	}

	protected internal Asn1Set(Asn1Encodable[] elements, bool doSort)
	{
		object[] array = elements;
		if (Arrays.IsNullOrContainsNull(array))
		{
			throw new NullReferenceException("'elements' cannot be null, or contain null");
		}
		elements = Asn1EncodableVector.CloneElements(elements);
		DerEncoding[] sortedDerEncodings = null;
		if (doSort && elements.Length > 1)
		{
			sortedDerEncodings = SortElements(elements);
		}
		m_elements = elements;
		m_sortedDerEncodings = sortedDerEncodings;
	}

	protected internal Asn1Set(Asn1EncodableVector elementVector, bool doSort)
	{
		if (elementVector == null)
		{
			throw new ArgumentNullException("elementVector");
		}
		Asn1Encodable[] elements;
		DerEncoding[] sortedDerEncodings;
		if (doSort && elementVector.Count > 1)
		{
			elements = elementVector.CopyElements();
			sortedDerEncodings = SortElements(elements);
		}
		else
		{
			elements = elementVector.TakeElements();
			sortedDerEncodings = null;
		}
		m_elements = elements;
		m_sortedDerEncodings = sortedDerEncodings;
	}

	protected internal Asn1Set(bool isSorted, Asn1Encodable[] elements)
	{
		m_elements = elements;
		m_sortedDerEncodings = null;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public virtual IEnumerator<Asn1Encodable> GetEnumerator()
	{
		return ((IEnumerable<Asn1Encodable>)m_elements).GetEnumerator();
	}

	public virtual T[] MapElements<T>(Func<Asn1Encodable, T> func)
	{
		int count = Count;
		T[] array = new T[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = func(m_elements[i]);
		}
		return array;
	}

	public virtual Asn1Encodable[] ToArray()
	{
		return Asn1EncodableVector.CloneElements(m_elements);
	}

	protected override int Asn1GetHashCode()
	{
		int num = Count;
		int num2 = num + 1;
		while (--num >= 0)
		{
			num2 *= 257;
			num2 ^= m_elements[num].ToAsn1Object().CallAsn1GetHashCode();
		}
		return num2;
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (!(asn1Object is Asn1Set asn1Set))
		{
			return false;
		}
		int count = Count;
		if (asn1Set.Count != count)
		{
			return false;
		}
		for (int i = 0; i < count; i++)
		{
			Asn1Object asn1Object2 = m_elements[i].ToAsn1Object();
			Asn1Object other = asn1Set.m_elements[i].ToAsn1Object();
			if (!asn1Object2.Equals(other))
			{
				return false;
			}
		}
		return true;
	}

	public override string ToString()
	{
		return CollectionUtilities.ToString(m_elements);
	}

	private static DerEncoding[] SortElements(Asn1Encodable[] elements)
	{
		DerEncoding[] contentsEncodingsDer = Asn1OutputStream.GetContentsEncodingsDer(elements);
		Array.Sort(contentsEncodingsDer, elements);
		return contentsEncodingsDer;
	}
}
