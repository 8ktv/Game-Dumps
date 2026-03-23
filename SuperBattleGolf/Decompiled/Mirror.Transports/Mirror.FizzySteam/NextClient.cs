using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace Mirror.FizzySteam;

public class NextClient : NextCommon, IClient
{
	private TimeSpan ConnectionTimeout;

	private CancellationTokenSource cancelToken;

	private TaskCompletionSource<Task> connectedComplete;

	private SteamId hostSteamID = 0uL;

	private FizzyConnectionManager HostConnectionManager;

	private List<Action> BufferedData;

	public bool Connected { get; private set; }

	public bool Error { get; private set; }

	private Connection HostConnection => HostConnectionManager.Connection;

	private event Action<byte[], int> OnReceivedData;

	private event Action OnConnected;

	private event Action OnDisconnected;

	private NextClient(FizzyFacepunch transport)
	{
		ConnectionTimeout = TimeSpan.FromSeconds(Math.Max(1, transport.Timeout));
		BufferedData = new List<Action>();
	}

	public static NextClient CreateClient(FizzyFacepunch transport, string host)
	{
		NextClient nextClient = new NextClient(transport);
		nextClient.OnConnected += delegate
		{
			transport.OnClientConnected();
		};
		nextClient.OnDisconnected += delegate
		{
			transport.OnClientDisconnected();
		};
		nextClient.OnReceivedData += delegate(byte[] data, int ch)
		{
			transport.OnClientDataReceived(new ArraySegment<byte>(data), ch);
		};
		if (SteamClient.IsValid)
		{
			nextClient.Connect(host);
		}
		else
		{
			Debug.LogError("SteamWorks not initialized");
			nextClient.OnConnectionFailed();
		}
		return nextClient;
	}

	private async void Connect(string host)
	{
		cancelToken = new CancellationTokenSource();
		SteamNetworkingSockets.OnConnectionStatusChanged += OnConnectionStatusChanged;
		try
		{
			hostSteamID = ulong.Parse(host);
			connectedComplete = new TaskCompletionSource<Task>();
			OnConnected += SetConnectedComplete;
			HostConnectionManager = SteamNetworkingSockets.ConnectRelay<FizzyConnectionManager>(hostSteamID);
			HostConnectionManager.ForwardMessage = OnMessageReceived;
			Task connectedCompleteTask = connectedComplete.Task;
			Task timeOutTask = Task.Delay(ConnectionTimeout, cancelToken.Token);
			if (await Task.WhenAny(connectedCompleteTask, timeOutTask) != connectedCompleteTask)
			{
				if (cancelToken.IsCancellationRequested)
				{
					Debug.LogError("The connection attempt was cancelled.");
				}
				else if (timeOutTask.IsCompleted)
				{
					Debug.LogError("Connection to " + host + " timed out.");
				}
				OnConnected -= SetConnectedComplete;
				OnConnectionFailed();
			}
			OnConnected -= SetConnectedComplete;
		}
		catch (FormatException)
		{
			Debug.LogError("Connection string was not in the right format. Did you enter a SteamId?");
			Error = true;
			OnConnectionFailed();
		}
		catch (Exception ex2)
		{
			Debug.LogError(ex2.Message);
			Error = true;
			OnConnectionFailed();
		}
		finally
		{
			if (Error)
			{
				Debug.LogError("Connection failed.");
				OnConnectionFailed();
			}
		}
	}

	private void OnMessageReceived(IntPtr dataPtr, int size)
	{
		var (data, ch) = ProcessMessage(dataPtr, size);
		if (Connected)
		{
			this.OnReceivedData(data, ch);
			return;
		}
		BufferedData.Add(delegate
		{
			this.OnReceivedData(data, ch);
		});
	}

	private void OnConnectionStatusChanged(Connection conn, ConnectionInfo info)
	{
		_ = (ulong)info.Identity.SteamId;
		if (info.State == ConnectionState.Connected)
		{
			Connected = true;
			this.OnConnected();
			Debug.Log("Connection established.");
			if (BufferedData.Count <= 0)
			{
				return;
			}
			Debug.Log($"{BufferedData.Count} received before connection was established. Processing now.");
			{
				foreach (Action bufferedDatum in BufferedData)
				{
					bufferedDatum();
				}
				return;
			}
		}
		if (info.State == ConnectionState.ClosedByPeer)
		{
			Connected = false;
			this.OnDisconnected();
			Debug.Log("Disconnected.");
			conn.Close(linger: false, 0, "Disconnected");
		}
		else
		{
			Debug.Log("Connection state changed: " + info.State);
		}
	}

	public void Disconnect()
	{
		cancelToken?.Cancel();
		SteamNetworkingSockets.OnConnectionStatusChanged -= OnConnectionStatusChanged;
		if (HostConnectionManager != null)
		{
			Debug.Log("Sending Disconnect message");
			HostConnection.Close(linger: false, 0, "Graceful disconnect");
			HostConnectionManager = null;
		}
	}

	public void ReceiveData()
	{
		HostConnectionManager.Receive(256);
	}

	public void Send(byte[] data, int channelId)
	{
		Result result = SendSocket(HostConnection, data, channelId);
		if (result != Result.OK)
		{
			Debug.LogError("Could not send: " + result);
		}
	}

	private void SetConnectedComplete()
	{
		connectedComplete.SetResult(connectedComplete.Task);
	}

	private void OnConnectionFailed()
	{
		this.OnDisconnected();
	}

	public void FlushData()
	{
		HostConnection.Flush();
	}
}
