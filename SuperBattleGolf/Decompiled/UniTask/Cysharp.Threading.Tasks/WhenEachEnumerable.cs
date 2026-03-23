using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks.Internal;

namespace Cysharp.Threading.Tasks;

internal sealed class WhenEachEnumerable<T> : IUniTaskAsyncEnumerable<WhenEachResult<T>>
{
	private sealed class Enumerator : IUniTaskAsyncEnumerator<WhenEachResult<T>>, IUniTaskAsyncDisposable
	{
		private readonly IEnumerable<UniTask<T>> source;

		private CancellationToken cancellationToken;

		private Channel<WhenEachResult<T>> channel;

		private IUniTaskAsyncEnumerator<WhenEachResult<T>> channelEnumerator;

		private int completeCount;

		private WhenEachState state;

		public WhenEachResult<T> Current => channelEnumerator.Current;

		public Enumerator(IEnumerable<UniTask<T>> source, CancellationToken cancellationToken)
		{
			this.source = source;
			this.cancellationToken = cancellationToken;
		}

		public UniTask<bool> MoveNextAsync()
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (state == WhenEachState.NotRunning)
			{
				state = WhenEachState.Running;
				channel = Channel.CreateSingleConsumerUnbounded<WhenEachResult<T>>();
				channelEnumerator = channel.Reader.ReadAllAsync().GetAsyncEnumerator(cancellationToken);
				if (source is UniTask<T>[] array)
				{
					ConsumeAll(this, array, array.Length);
				}
				else
				{
					ArrayPoolUtil.RentArray<UniTask<T>> rentArray = ArrayPoolUtil.Materialize(source);
					try
					{
						ConsumeAll(this, rentArray.Array, rentArray.Length);
					}
					finally
					{
						((IDisposable)rentArray/*cast due to .constrained prefix*/).Dispose();
					}
				}
			}
			return channelEnumerator.MoveNextAsync();
		}

		private static void ConsumeAll(Enumerator self, UniTask<T>[] array, int length)
		{
			for (int i = 0; i < length; i++)
			{
				RunWhenEachTask(self, array[i], length).Forget();
			}
		}

		private static async UniTaskVoid RunWhenEachTask(Enumerator self, UniTask<T> task, int length)
		{
			try
			{
				T result = await task;
				self.channel.Writer.TryWrite(new WhenEachResult<T>(result));
			}
			catch (Exception exception)
			{
				self.channel.Writer.TryWrite(new WhenEachResult<T>(exception));
			}
			if (Interlocked.Increment(ref self.completeCount) == length)
			{
				self.state = WhenEachState.Completed;
				self.channel.Writer.TryComplete();
			}
		}

		public async UniTask DisposeAsync()
		{
			if (channelEnumerator != null)
			{
				await channelEnumerator.DisposeAsync();
			}
			if (state != WhenEachState.Completed)
			{
				state = WhenEachState.Completed;
				channel.Writer.TryComplete(new OperationCanceledException());
			}
		}
	}

	private IEnumerable<UniTask<T>> source;

	public WhenEachEnumerable(IEnumerable<UniTask<T>> source)
	{
		this.source = source;
	}

	public IUniTaskAsyncEnumerator<WhenEachResult<T>> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
	{
		return new Enumerator(source, cancellationToken);
	}
}
