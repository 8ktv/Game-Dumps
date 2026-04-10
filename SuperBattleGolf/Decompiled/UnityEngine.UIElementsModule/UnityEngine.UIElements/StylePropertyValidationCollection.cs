using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal readonly struct StylePropertyValidationCollection : IEnumerable<StylePropertyValidation>, IEnumerable
{
	internal struct Enumerator : IEnumerator<StylePropertyValidation>, IEnumerator, IDisposable
	{
		private List<StylePropertyValidation>.Enumerator m_PersistentValidation;

		private List<StylePropertyValidation>.Enumerator m_Validation;

		private bool persistent;

		public StylePropertyValidation Current { get; private set; }

		object IEnumerator.Current => Current;

		internal Enumerator(List<StylePropertyValidation>.Enumerator persistentValidation, List<StylePropertyValidation>.Enumerator validation)
		{
			m_PersistentValidation = persistentValidation;
			m_Validation = validation;
			Current = null;
			persistent = true;
		}

		public bool MoveNext()
		{
			bool flag;
			if (persistent)
			{
				flag = m_PersistentValidation.MoveNext();
				Current = m_PersistentValidation.Current;
				if (!flag)
				{
					persistent = false;
					flag = m_Validation.MoveNext();
					Current = m_Validation.Current;
				}
			}
			else
			{
				flag = m_Validation.MoveNext();
				Current = m_Validation.Current;
			}
			return flag;
		}

		public void Reset()
		{
			persistent = true;
			Current = null;
			((IEnumerator)m_PersistentValidation).Reset();
			((IEnumerator)m_Validation).Reset();
		}

		public void Dispose()
		{
		}
	}

	private static readonly List<StylePropertyValidation> s_Empty = new List<StylePropertyValidation>();

	private readonly List<StylePropertyValidation> m_PersistentValidation;

	private readonly List<StylePropertyValidation> m_Validation;

	public static StylePropertyValidationCollection Empty { get; } = default(StylePropertyValidationCollection);

	public static implicit operator StylePropertyValidationCollection(List<StylePropertyValidation> validation)
	{
		return new StylePropertyValidationCollection(validation);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StylePropertyValidationCollection(List<StylePropertyValidation> persistentValidation, List<StylePropertyValidation> validation)
	{
		m_PersistentValidation = persistentValidation;
		m_Validation = validation;
	}

	internal StylePropertyValidationCollection(List<StylePropertyValidation> validation)
	{
		m_PersistentValidation = null;
		m_Validation = validation;
	}

	public Enumerator GetEnumerator()
	{
		return (m_PersistentValidation != null) ? new Enumerator(m_PersistentValidation.GetEnumerator(), m_Validation.GetEnumerator()) : new Enumerator(s_Empty.GetEnumerator(), m_Validation.GetEnumerator());
	}

	IEnumerator<StylePropertyValidation> IEnumerable<StylePropertyValidation>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
