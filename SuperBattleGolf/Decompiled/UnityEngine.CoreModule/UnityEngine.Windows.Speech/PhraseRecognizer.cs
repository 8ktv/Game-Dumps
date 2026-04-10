using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech;

public abstract class PhraseRecognizer : IDisposable
{
	public delegate void PhraseRecognizedDelegate(PhraseRecognizedEventArgs args);

	protected IntPtr m_Recognizer;

	public bool IsRunning => m_Recognizer != IntPtr.Zero && IsRunning_Internal(m_Recognizer);

	public event PhraseRecognizedDelegate OnPhraseRecognized;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
	protected static extern IntPtr CreateFromKeywords(object self, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] string[] keywords, ConfidenceLevel minimumConfidence);

	[NativeThrows]
	[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
	protected unsafe static IntPtr CreateFromGrammarFile(object self, string grammarFilePath, ConfidenceLevel minimumConfidence)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(grammarFilePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(grammarFilePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return CreateFromGrammarFile_Injected(self, ref managedSpanWrapper, minimumConfidence);
				}
			}
			return CreateFromGrammarFile_Injected(self, ref managedSpanWrapper, minimumConfidence);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
	private static extern void Start_Internal(IntPtr recognizer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
	private static extern void Stop_Internal(IntPtr recognizer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
	private static extern bool IsRunning_Internal(IntPtr recognizer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
	private static extern void Destroy(IntPtr recognizer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
	private static extern void DestroyThreaded(IntPtr recognizer);

	internal PhraseRecognizer()
	{
	}

	~PhraseRecognizer()
	{
		if (m_Recognizer != IntPtr.Zero)
		{
			DestroyThreaded(m_Recognizer);
			m_Recognizer = IntPtr.Zero;
			GC.SuppressFinalize(this);
		}
	}

	public void Start()
	{
		if (!(m_Recognizer == IntPtr.Zero))
		{
			Start_Internal(m_Recognizer);
		}
	}

	public void Stop()
	{
		if (!(m_Recognizer == IntPtr.Zero))
		{
			Stop_Internal(m_Recognizer);
		}
	}

	public void Dispose()
	{
		if (m_Recognizer != IntPtr.Zero)
		{
			Destroy(m_Recognizer);
			m_Recognizer = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	[RequiredByNativeCode]
	private unsafe void InvokePhraseRecognizedEvent(IntPtr rawText, int rawTextLength, ConfidenceLevel confidence, SemanticMeaning[] semanticMeanings, long phraseStartFileTime, long phraseDurationTicks)
	{
		this.OnPhraseRecognized?.Invoke(new PhraseRecognizedEventArgs(new string((char*)(void*)rawText, 0, rawTextLength), confidence, semanticMeanings, DateTime.FromFileTime(phraseStartFileTime), TimeSpan.FromTicks(phraseDurationTicks)));
	}

	[RequiredByNativeCode]
	private unsafe static SemanticMeaning[] MarshalSemanticMeaning(IntPtr keys, IntPtr values, IntPtr valueSizes, int valueCount)
	{
		SemanticMeaning[] array = new SemanticMeaning[valueCount];
		int num = 0;
		for (int i = 0; i < valueCount; i++)
		{
			uint num2 = ((uint*)(void*)valueSizes)[i];
			SemanticMeaning semanticMeaning = new SemanticMeaning
			{
				key = new string(((char**)(void*)keys)[i]),
				values = new string[num2]
			};
			for (int j = 0; j < num2; j++)
			{
				semanticMeaning.values[j] = new string(((char**)(void*)values)[num + j]);
			}
			array[i] = semanticMeaning;
			num += (int)num2;
		}
		return array;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateFromGrammarFile_Injected(object self, ref ManagedSpanWrapper grammarFilePath, ConfidenceLevel minimumConfidence);
}
