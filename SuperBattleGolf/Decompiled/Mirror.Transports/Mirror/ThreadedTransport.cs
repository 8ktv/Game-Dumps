using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Mirror;

public abstract class ThreadedTransport : Transport
{
	private WorkerThread thread;

	private readonly ConcurrentQueue<ClientMainEvent> clientMainQueue = new ConcurrentQueue<ClientMainEvent>();

	private readonly ConcurrentQueue<ServerMainEvent> serverMainQueue = new ConcurrentQueue<ServerMainEvent>();

	private readonly ConcurrentQueue<ThreadEvent> threadQueue = new ConcurrentQueue<ThreadEvent>();

	private volatile bool serverActive;

	private volatile bool clientConnected;

	private const int MaxProcessingPerTick = 10000000;

	[Tooltip("Detect device sleep mode and automatically disconnect + hibernate the thread after 'sleepTimeout' seconds.\nFor example: on mobile / VR, we don't want to drain the battery after putting down the device.")]
	public bool sleepDetection = true;

	public float sleepTimeoutInSeconds = 30f;

	private Stopwatch sleepTimer;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EnqueueClientMain(ClientMainEventType type, object param, int? channelId, TransportError? error)
	{
		clientMainQueue.Enqueue(new ClientMainEvent(type, param, channelId, error));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EnqueueServerMain(ServerMainEventType type, object param, int? connectionId, int? channelId, TransportError? error)
	{
		serverMainQueue.Enqueue(new ServerMainEvent(type, param, connectionId, channelId, error));
	}

	private void EnqueueThread(ThreadEventType type, object param, int? channelId, int? connectionId)
	{
		threadQueue.Enqueue(new ThreadEvent(type, param, connectionId, channelId));
	}

	protected virtual void Awake()
	{
		EnsureThread();
	}

	private void EnsureThread()
	{
		if (thread == null || !thread.IsAlive)
		{
			thread = new WorkerThread(ToString());
			thread.Tick = ThreadTick;
			thread.Cleanup = ThreadedShutdown;
			thread.Start();
			UnityEngine.Debug.Log("ThreadedTransport: started worker thread!");
		}
	}

	protected virtual void OnDestroy()
	{
		Shutdown();
	}

	private void ProcessThreadQueue()
	{
		ThreadEvent result;
		while (threadQueue.TryDequeue(out result))
		{
			switch (result.type)
			{
			case ThreadEventType.DoServerStart:
				ThreadedServerStart();
				break;
			case ThreadEventType.DoServerSend:
			{
				ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled2 = (ConcurrentNetworkWriterPooled)result.param;
				ThreadedServerSend(result.connectionId.Value, concurrentNetworkWriterPooled2, result.channelId.Value);
				ConcurrentNetworkWriterPool.Return(concurrentNetworkWriterPooled2);
				break;
			}
			case ThreadEventType.DoServerDisconnect:
				ThreadedServerDisconnect(result.connectionId.Value);
				break;
			case ThreadEventType.DoServerStop:
				ThreadedServerStop();
				break;
			case ThreadEventType.DoClientConnect:
				if (result.param is string address)
				{
					ThreadedClientConnect(address);
				}
				else if (result.param is Uri address2)
				{
					ThreadedClientConnect(address2);
				}
				break;
			case ThreadEventType.DoClientSend:
			{
				ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = (ConcurrentNetworkWriterPooled)result.param;
				ThreadedClientSend(concurrentNetworkWriterPooled, result.channelId.Value);
				ConcurrentNetworkWriterPool.Return(concurrentNetworkWriterPooled);
				break;
			}
			case ThreadEventType.DoClientDisconnect:
				ThreadedClientDisconnect();
				break;
			case ThreadEventType.Sleep:
				if (sleepTimer == null)
				{
					UnityEngine.Debug.Log($"ThreadedTransport: sleep detected, sleeping in {sleepTimeoutInSeconds:F0}s!");
					sleepTimer = Stopwatch.StartNew();
				}
				break;
			case ThreadEventType.Wake:
				if (sleepTimer != null)
				{
					UnityEngine.Debug.Log("ThreadedTransport: Woke up, interrupting sleep timer!");
					sleepTimer = null;
				}
				break;
			case ThreadEventType.DoShutdown:
				ThreadedShutdown();
				break;
			}
		}
	}

	private bool ThreadTick()
	{
		if (sleepTimer != null && sleepTimer.Elapsed.TotalSeconds >= (double)sleepTimeoutInSeconds)
		{
			UnityEngine.Debug.Log("ThreadedTransport: entering sleep mode and stopping/disconnecting.");
			ThreadedServerStop();
			ThreadedClientDisconnect();
			sleepTimer = null;
			return false;
		}
		ThreadedClientEarlyUpdate();
		ThreadedServerEarlyUpdate();
		ProcessThreadQueue();
		ThreadedClientLateUpdate();
		ThreadedServerLateUpdate();
		Thread.Sleep(1);
		return true;
	}

	protected void OnThreadedClientConnected()
	{
		EnqueueClientMain(ClientMainEventType.OnClientConnected, null, null, null);
	}

	protected void OnThreadedClientSend(ArraySegment<byte> message, int channelId)
	{
		ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = ConcurrentNetworkWriterPool.Get();
		concurrentNetworkWriterPooled.WriteBytes(message.Array, message.Offset, message.Count);
		EnqueueClientMain(ClientMainEventType.OnClientSent, concurrentNetworkWriterPooled, channelId, null);
	}

	protected void OnThreadedClientReceive(ArraySegment<byte> message, int channelId)
	{
		ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = ConcurrentNetworkWriterPool.Get();
		concurrentNetworkWriterPooled.WriteBytes(message.Array, message.Offset, message.Count);
		EnqueueClientMain(ClientMainEventType.OnClientReceived, concurrentNetworkWriterPooled, channelId, null);
	}

	protected void OnThreadedClientError(TransportError error, string reason)
	{
		EnqueueClientMain(ClientMainEventType.OnClientError, reason, null, error);
	}

	protected void OnThreadedClientDisconnected()
	{
		EnqueueClientMain(ClientMainEventType.OnClientDisconnected, null, null, null);
	}

	protected void OnThreadedServerConnected(int connectionId, IPEndPoint endPoint)
	{
		string param = endPoint.PrettyAddress();
		EnqueueServerMain(ServerMainEventType.OnServerConnected, param, connectionId, null, null);
	}

	protected void OnThreadedServerSend(int connectionId, ArraySegment<byte> message, int channelId)
	{
		ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = ConcurrentNetworkWriterPool.Get();
		concurrentNetworkWriterPooled.WriteBytes(message.Array, message.Offset, message.Count);
		EnqueueServerMain(ServerMainEventType.OnServerSent, concurrentNetworkWriterPooled, connectionId, channelId, null);
	}

	protected void OnThreadedServerReceive(int connectionId, ArraySegment<byte> message, int channelId)
	{
		ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = ConcurrentNetworkWriterPool.Get();
		concurrentNetworkWriterPooled.WriteBytes(message.Array, message.Offset, message.Count);
		EnqueueServerMain(ServerMainEventType.OnServerReceived, concurrentNetworkWriterPooled, connectionId, channelId, null);
	}

	protected void OnThreadedServerError(int connectionId, TransportError error, string reason)
	{
		EnqueueServerMain(ServerMainEventType.OnServerError, reason, connectionId, null, error);
	}

	protected void OnThreadedServerDisconnected(int connectionId)
	{
		EnqueueServerMain(ServerMainEventType.OnServerDisconnected, null, connectionId, null, null);
	}

	protected abstract void ThreadedClientConnect(string address);

	protected abstract void ThreadedClientConnect(Uri address);

	protected abstract void ThreadedClientSend(ArraySegment<byte> message, int channelId);

	protected abstract void ThreadedClientDisconnect();

	protected abstract void ThreadedServerStart();

	protected abstract void ThreadedServerStop();

	protected abstract void ThreadedServerSend(int connectionId, ArraySegment<byte> message, int channelId);

	protected abstract void ThreadedServerDisconnect(int connectionId);

	protected abstract void ThreadedClientEarlyUpdate();

	protected abstract void ThreadedClientLateUpdate();

	protected abstract void ThreadedServerEarlyUpdate();

	protected abstract void ThreadedServerLateUpdate();

	protected abstract void ThreadedShutdown();

	public override void ClientEarlyUpdate()
	{
		int num = 0;
		ClientMainEvent result;
		while (clientMainQueue.TryDequeue(out result))
		{
			switch (result.type)
			{
			case ClientMainEventType.OnClientConnected:
				OnClientConnected?.Invoke();
				break;
			case ClientMainEventType.OnClientSent:
			{
				ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = (ConcurrentNetworkWriterPooled)result.param;
				OnClientDataSent?.Invoke(concurrentNetworkWriterPooled, result.channelId.Value);
				ConcurrentNetworkWriterPool.Return(concurrentNetworkWriterPooled);
				break;
			}
			case ClientMainEventType.OnClientReceived:
				if (clientConnected)
				{
					ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled2 = (ConcurrentNetworkWriterPooled)result.param;
					OnClientDataReceived?.Invoke(concurrentNetworkWriterPooled2, result.channelId.Value);
					ConcurrentNetworkWriterPool.Return(concurrentNetworkWriterPooled2);
				}
				break;
			case ClientMainEventType.OnClientError:
				OnClientError?.Invoke(result.error.Value, (string)result.param);
				break;
			case ClientMainEventType.OnClientDisconnected:
				OnClientDisconnected?.Invoke();
				break;
			}
			if (++num >= 10000000)
			{
				UnityEngine.Debug.LogWarning($"ThreadedTransport processed the limit of {10000000} messages this tick. Continuing next tick to prevent deadlock.");
				break;
			}
		}
	}

	public override bool ClientConnected()
	{
		return clientConnected;
	}

	public override void ClientConnect(string address)
	{
		if (ClientConnected())
		{
			UnityEngine.Debug.LogWarning("Threaded transport: client already connected!");
			return;
		}
		EnsureThread();
		EnqueueThread(ThreadEventType.DoClientConnect, address, null, null);
		clientConnected = true;
	}

	public override void ClientConnect(Uri uri)
	{
		if (ClientConnected())
		{
			UnityEngine.Debug.LogWarning("Threaded transport: client already connected!");
			return;
		}
		EnsureThread();
		EnqueueThread(ThreadEventType.DoClientConnect, uri, null, null);
		clientConnected = true;
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId = 0)
	{
		if (ClientConnected())
		{
			ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = ConcurrentNetworkWriterPool.Get();
			concurrentNetworkWriterPooled.WriteBytes(segment.Array, segment.Offset, segment.Count);
			EnqueueThread(ThreadEventType.DoClientSend, concurrentNetworkWriterPooled, channelId, null);
		}
	}

	public override void ClientDisconnect()
	{
		EnqueueThread(ThreadEventType.DoClientDisconnect, null, null, null);
		clientConnected = false;
	}

	public override void ServerEarlyUpdate()
	{
		int num = 0;
		ServerMainEvent result;
		while (serverMainQueue.TryDequeue(out result))
		{
			switch (result.type)
			{
			case ServerMainEventType.OnServerConnected:
			{
				string arg = (string)result.param;
				OnServerConnectedWithAddress?.Invoke(result.connectionId.Value, arg);
				break;
			}
			case ServerMainEventType.OnServerSent:
			{
				ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = (ConcurrentNetworkWriterPooled)result.param;
				OnServerDataSent?.Invoke(result.connectionId.Value, concurrentNetworkWriterPooled, result.channelId.Value);
				ConcurrentNetworkWriterPool.Return(concurrentNetworkWriterPooled);
				break;
			}
			case ServerMainEventType.OnServerReceived:
			{
				ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled2 = (ConcurrentNetworkWriterPooled)result.param;
				OnServerDataReceived?.Invoke(result.connectionId.Value, concurrentNetworkWriterPooled2, result.channelId.Value);
				ConcurrentNetworkWriterPool.Return(concurrentNetworkWriterPooled2);
				break;
			}
			case ServerMainEventType.OnServerError:
				OnServerError?.Invoke(result.connectionId.Value, result.error.Value, (string)result.param);
				break;
			case ServerMainEventType.OnServerDisconnected:
				OnServerDisconnected?.Invoke(result.connectionId.Value);
				break;
			}
			if (++num >= 10000000)
			{
				UnityEngine.Debug.LogWarning($"ThreadedTransport processed the limit of {10000000} messages this tick. Continuing next tick to prevent deadlock.");
				break;
			}
		}
	}

	public override void ServerLateUpdate()
	{
	}

	public override bool ServerActive()
	{
		return serverActive;
	}

	public override void ServerStart()
	{
		if (ServerActive())
		{
			UnityEngine.Debug.LogWarning("Threaded transport: server already started!");
			return;
		}
		EnsureThread();
		EnqueueThread(ThreadEventType.DoServerStart, null, null, null);
		serverActive = true;
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId = 0)
	{
		if (ServerActive())
		{
			ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = ConcurrentNetworkWriterPool.Get();
			concurrentNetworkWriterPooled.WriteBytes(segment.Array, segment.Offset, segment.Count);
			EnqueueThread(ThreadEventType.DoServerSend, concurrentNetworkWriterPooled, channelId, connectionId);
		}
	}

	public override void ServerDisconnect(int connectionId)
	{
		EnqueueThread(ThreadEventType.DoServerDisconnect, null, null, connectionId);
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		throw new NotImplementedException("ThreadedTransport passes each connection's address in OnServerConnectedThreaded. Don't use ServerGetClientAddress.");
	}

	public override void ServerStop()
	{
		EnqueueThread(ThreadEventType.DoServerStop, null, null, null);
		serverActive = false;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		UnityEngine.Debug.Log($"{GetType()}: OnApplicationPause={pauseStatus}");
		if (sleepDetection)
		{
			if (pauseStatus)
			{
				EnqueueThread(ThreadEventType.Sleep, null, null, null);
			}
			else
			{
				EnqueueThread(ThreadEventType.Wake, null, null, null);
			}
		}
	}

	public override void Shutdown()
	{
		EnqueueThread(ThreadEventType.DoShutdown, null, null, null);
		Thread.Sleep(100);
		thread?.StopBlocking(1f);
		clientMainQueue.Clear();
		serverMainQueue.Clear();
		threadQueue.Clear();
	}
}
