using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1;

public class Asn1EncodableVector : IEnumerable<Asn1Encodable>, IEnumerable
{
	internal static readonly Asn1Encodable[] EmptyElements = new Asn1Encodable[0];

	private const int DefaultCapacity = 10;

	private Asn1Encodable[] elements;

	private int elementCount;

	private bool copyOnWrite;

	public Asn1Encodable this[int index]
	{
		get
		{
			if (index >= elementCount)
			{
				throw new IndexOutOfRangeException(index + " >= " + elementCount);
			}
			return elements[index];
		}
	}

	public int Count => elementCount;

	public static Asn1EncodableVector FromEnumerable(IEnumerable<Asn1Encodable> e)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		foreach (Asn1Encodable item in e)
		{
			asn1EncodableVector.Add(item);
		}
		return asn1EncodableVector;
	}

	public Asn1EncodableVector()
		: this(10)
	{
	}

	public Asn1EncodableVector(int initialCapacity)
	{
		if (initialCapacity < 0)
		{
			throw new ArgumentException("must not be negative", "initialCapacity");
		}
		elements = ((initialCapacity == 0) ? EmptyElements : new Asn1Encodable[initialCapacity]);
		elementCount = 0;
		copyOnWrite = false;
	}

	public Asn1EncodableVector(Asn1Encodable element)
		: this()
	{
		Add(element);
	}

	public Asn1EncodableVector(Asn1Encodable element1, Asn1Encodable element2)
		: this()
	{
		Add(element1);
		Add(element2);
	}

	public Asn1EncodableVector(params Asn1Encodable[] v)
		: this()
	{
		Add(v);
	}

	public void Add(Asn1Encodable element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		int num = elements.Length;
		int num2 = elementCount + 1;
		if ((num2 > num) | copyOnWrite)
		{
			Reallocate(num2);
		}
		elements[elementCount] = element;
		elementCount = num2;
	}

	public void Add(Asn1Encodable element1, Asn1Encodable element2)
	{
		Add(element1);
		Add(element2);
	}

	public void Add(params Asn1Encodable[] objs)
	{
		foreach (Asn1Encodable element in objs)
		{
			Add(element);
		}
	}

	public void AddOptional(Asn1Encodable element)
	{
		if (element != null)
		{
			Add(element);
		}
	}

	public void AddOptional(Asn1Encodable element1, Asn1Encodable element2)
	{
		if (element1 != null)
		{
			Add(element1);
		}
		if (element2 != null)
		{
			Add(element2);
		}
	}

	public void AddOptional(params Asn1Encodable[] elements)
	{
		if (elements == null)
		{
			return;
		}
		foreach (Asn1Encodable asn1Encodable in elements)
		{
			if (asn1Encodable != null)
			{
				Add(asn1Encodable);
			}
		}
	}

	public void AddOptionalTagged(bool isExplicit, int tagNo, Asn1Encodable obj)
	{
		if (obj != null)
		{
			Add(new DerTaggedObject(isExplicit, tagNo, obj));
		}
	}

	public void AddOptionalTagged(bool isExplicit, int tagClass, int tagNo, Asn1Encodable obj)
	{
		if (obj != null)
		{
			Add(new DerTaggedObject(isExplicit, tagClass, tagNo, obj));
		}
	}

	public void AddAll(Asn1EncodableVector other)
	{
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		int count = other.Count;
		if (count < 1)
		{
			return;
		}
		int num = elements.Length;
		int num2 = elementCount + count;
		if ((num2 > num) | copyOnWrite)
		{
			Reallocate(num2);
		}
		int num3 = 0;
		do
		{
			Asn1Encodable asn1Encodable = other[num3];
			if (asn1Encodable == null)
			{
				throw new NullReferenceException("'other' elements cannot be null");
			}
			elements[elementCount + num3] = asn1Encodable;
		}
		while (++num3 < count);
		elementCount = num2;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<Asn1Encodable> GetEnumerator()
	{
		return ((IEnumerable<Asn1Encodable>)CopyElements()).GetEnumerator();
	}

	internal Asn1Encodable[] CopyElements()
	{
		if (elementCount == 0)
		{
			return EmptyElements;
		}
		Asn1Encodable[] array = new Asn1Encodable[elementCount];
		Array.Copy(elements, 0, array, 0, elementCount);
		return array;
	}

	internal Asn1Encodable[] TakeElements()
	{
		if (elementCount == 0)
		{
			return EmptyElements;
		}
		if (elements.Length == elementCount)
		{
			copyOnWrite = true;
			return elements;
		}
		Asn1Encodable[] array = new Asn1Encodable[elementCount];
		Array.Copy(elements, 0, array, 0, elementCount);
		return array;
	}

	private void Reallocate(int minCapacity)
	{
		Asn1Encodable[] destinationArray = new Asn1Encodable[System.Math.Max(elements.Length, minCapacity + (minCapacity >> 1))];
		Array.Copy(elements, 0, destinationArray, 0, elementCount);
		elements = destinationArray;
		copyOnWrite = false;
	}

	internal static Asn1Encodable[] CloneElements(Asn1Encodable[] elements)
	{
		if (elements.Length >= 1)
		{
			return (Asn1Encodable[])elements.Clone();
		}
		return EmptyElements;
	}
}
