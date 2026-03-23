using System;
using System.Collections.Generic;
using Unity.Jobs;

namespace UnityEngine.UIElements.UIR;

internal class MeshGenerationDeferrer : IDisposable
{
	private struct CallbackInfo
	{
		public MeshGenerationCallback callback;

		public object userData;
	}

	private Queue<CallbackInfo> m_Fork = new Queue<CallbackInfo>(32);

	private Queue<CallbackInfo> m_WorkThenFork = new Queue<CallbackInfo>(32);

	private Queue<CallbackInfo> m_Work = new Queue<CallbackInfo>(32);

	private Queue<CallbackInfo> m_JobDependentFork = new Queue<CallbackInfo>(32);

	private Queue<CallbackInfo> m_JobDependentWorkThenFork = new Queue<CallbackInfo>(32);

	private Queue<CallbackInfo> m_JobDependentWork = new Queue<CallbackInfo>(32);

	private Queue<JobHandle> m_Dependencies = new Queue<JobHandle>(32);

	private JobMerger m_DependencyMerger = new JobMerger(64);

	protected bool disposed { get; private set; }

	public void AddMeshGenerationJob(JobHandle jobHandle)
	{
		m_Dependencies.Enqueue(jobHandle);
	}

	public void AddMeshGenerationCallback(MeshGenerationCallback callback, object userData, MeshGenerationCallbackType callbackType, bool isJobDependent)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		CallbackInfo item = new CallbackInfo
		{
			callback = callback,
			userData = userData
		};
		if (!isJobDependent)
		{
			switch (callbackType)
			{
			case MeshGenerationCallbackType.Fork:
				m_Fork.Enqueue(item);
				break;
			case MeshGenerationCallbackType.WorkThenFork:
				m_WorkThenFork.Enqueue(item);
				break;
			case MeshGenerationCallbackType.Work:
				m_Work.Enqueue(item);
				break;
			default:
				throw new NotImplementedException();
			}
		}
		else
		{
			switch (callbackType)
			{
			case MeshGenerationCallbackType.Fork:
				m_JobDependentFork.Enqueue(item);
				break;
			case MeshGenerationCallbackType.WorkThenFork:
				m_JobDependentWorkThenFork.Enqueue(item);
				break;
			case MeshGenerationCallbackType.Work:
				m_JobDependentWork.Enqueue(item);
				break;
			default:
				throw new NotImplementedException();
			}
		}
	}

	public void ProcessDeferredWork(MeshGenerationContext meshGenerationContext)
	{
		while (true)
		{
			int count = m_Fork.Count;
			int count2 = m_WorkThenFork.Count;
			int count3 = m_Work.Count;
			int count4 = m_JobDependentFork.Count;
			int count5 = m_JobDependentWorkThenFork.Count;
			int count6 = m_JobDependentWork.Count;
			int count7 = m_Dependencies.Count;
			if (count + count2 + count3 + count7 == 0)
			{
				break;
			}
			for (int i = 0; i < count; i++)
			{
				CallbackInfo ci = m_Fork.Dequeue();
				Invoke(ci, meshGenerationContext);
			}
			for (int j = 0; j < count2; j++)
			{
				CallbackInfo ci2 = m_WorkThenFork.Dequeue();
				Invoke(ci2, meshGenerationContext);
			}
			for (int k = 0; k < count3; k++)
			{
				CallbackInfo ci3 = m_Work.Dequeue();
				Invoke(ci3, meshGenerationContext);
			}
			for (int l = 0; l < count7; l++)
			{
				m_DependencyMerger.Add(m_Dependencies.Dequeue());
			}
			m_DependencyMerger.MergeAndReset().Complete();
			for (int m = 0; m < count4; m++)
			{
				CallbackInfo ci4 = m_JobDependentFork.Dequeue();
				Invoke(ci4, meshGenerationContext);
			}
			for (int n = 0; n < count5; n++)
			{
				CallbackInfo ci5 = m_JobDependentWorkThenFork.Dequeue();
				Invoke(ci5, meshGenerationContext);
			}
			for (int num = 0; num < count6; num++)
			{
				CallbackInfo ci6 = m_JobDependentWork.Dequeue();
				Invoke(ci6, meshGenerationContext);
			}
		}
	}

	private static void Invoke(CallbackInfo ci, MeshGenerationContext mgc)
	{
		try
		{
			ci.callback(mgc, ci.userData);
			if (mgc.visualElement != null)
			{
				Debug.LogWarning(string.Format("MeshGenerationContext is assigned to a VisualElement after calling '{0}'. Did you forget to call '{1}'?", ci.callback, "End"));
				mgc.End();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				m_DependencyMerger.Dispose();
				m_DependencyMerger = null;
			}
			disposed = true;
		}
	}
}
