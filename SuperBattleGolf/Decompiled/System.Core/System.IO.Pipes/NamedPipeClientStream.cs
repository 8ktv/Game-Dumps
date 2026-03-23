using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

public sealed class NamedPipeClientStream : PipeStream
{
	private const int CancellationCheckInterval = 50;

	private readonly string _normalizedPipePath;

	private readonly TokenImpersonationLevel _impersonationLevel;

	private readonly PipeOptions _pipeOptions;

	private readonly HandleInheritability _inheritability;

	private readonly PipeDirection _direction;

	private int _access;

	public int NumberOfServerInstances
	{
		get
		{
			CheckPipePropertyOperations();
			if (!global::Interop.Kernel32.GetNamedPipeHandleState(base.InternalHandle, IntPtr.Zero, out var lpCurInstances, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0))
			{
				throw WinIOError(Marshal.GetLastWin32Error());
			}
			return lpCurInstances;
		}
	}

	private bool TryConnect(int timeout, CancellationToken cancellationToken)
	{
		global::Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs = PipeStream.GetSecAttrs(_inheritability);
		int num = (int)(_pipeOptions & ~PipeOptions.CurrentUserOnly);
		if (_impersonationLevel != TokenImpersonationLevel.None)
		{
			num |= 0x100000;
			num |= (int)(_impersonationLevel - 1) << 16;
		}
		int num2 = _access;
		if ((PipeDirection.In & _direction) != 0)
		{
			num2 |= int.MinValue;
		}
		if ((PipeDirection.Out & _direction) != 0)
		{
			num2 |= 0x40000000;
		}
		SafePipeHandle safePipeHandle = global::Interop.Kernel32.CreateNamedPipeClient(_normalizedPipePath, num2, FileShare.None, ref secAttrs, FileMode.Open, num, IntPtr.Zero);
		if (safePipeHandle.IsInvalid)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 231 && lastWin32Error != 2)
			{
				throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error);
			}
			if (!global::Interop.Kernel32.WaitNamedPipe(_normalizedPipePath, timeout))
			{
				lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 2 || lastWin32Error == 121)
				{
					return false;
				}
				throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error);
			}
			safePipeHandle = global::Interop.Kernel32.CreateNamedPipeClient(_normalizedPipePath, num2, FileShare.None, ref secAttrs, FileMode.Open, num, IntPtr.Zero);
			if (safePipeHandle.IsInvalid)
			{
				lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 231)
				{
					return false;
				}
				throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error);
			}
		}
		InitializeHandle(safePipeHandle, isExposed: false, (_pipeOptions & PipeOptions.Asynchronous) != 0);
		base.State = PipeState.Connected;
		ValidateRemotePipeUser();
		return true;
	}

	private void ValidateRemotePipeUser()
	{
		if (!base.IsCurrentUserOnly)
		{
			return;
		}
		IdentityReference owner = GetAccessControl().GetOwner(typeof(SecurityIdentifier));
		using WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
		SecurityIdentifier owner2 = windowsIdentity.Owner;
		if (owner != owner2)
		{
			base.State = PipeState.Closed;
			throw new UnauthorizedAccessException("Could not connect to the pipe because it was not owned by the current user.");
		}
	}

	public NamedPipeClientStream(string pipeName)
		: this(".", pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName)
		: this(serverName, pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction)
		: this(serverName, pipeName, direction, PipeOptions.None, TokenImpersonationLevel.None, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options)
		: this(serverName, pipeName, direction, options, TokenImpersonationLevel.None, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel)
		: this(serverName, pipeName, direction, options, impersonationLevel, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
		: base(direction, 0)
	{
		if (pipeName == null)
		{
			throw new ArgumentNullException("pipeName");
		}
		if (serverName == null)
		{
			throw new ArgumentNullException("serverName", "serverName cannot be null. Use \\\".\\\" for current machine.");
		}
		if (pipeName.Length == 0)
		{
			throw new ArgumentException("pipeName cannot be an empty string.");
		}
		if (serverName.Length == 0)
		{
			throw new ArgumentException("serverName cannot be an empty string.  Use \\\\\\\".\\\\\\\" for current machine.");
		}
		if ((options & (PipeOptions)536870911) != PipeOptions.None)
		{
			throw new ArgumentOutOfRangeException("options", "options contains an invalid flag.");
		}
		if (impersonationLevel < TokenImpersonationLevel.None || impersonationLevel > TokenImpersonationLevel.Delegation)
		{
			throw new ArgumentOutOfRangeException("impersonationLevel", "TokenImpersonationLevel.None, TokenImpersonationLevel.Anonymous, TokenImpersonationLevel.Identification, TokenImpersonationLevel.Impersonation or TokenImpersonationLevel.Delegation required.");
		}
		if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
		{
			throw new ArgumentOutOfRangeException("inheritability", "HandleInheritability.None or HandleInheritability.Inheritable required.");
		}
		if ((options & PipeOptions.CurrentUserOnly) != PipeOptions.None)
		{
			base.IsCurrentUserOnly = true;
		}
		_normalizedPipePath = PipeStream.GetPipePath(serverName, pipeName);
		_direction = direction;
		_inheritability = inheritability;
		_impersonationLevel = impersonationLevel;
		_pipeOptions = options;
	}

	public NamedPipeClientStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
		: base(direction, 0)
	{
		if (safePipeHandle == null)
		{
			throw new ArgumentNullException("safePipeHandle");
		}
		if (safePipeHandle.IsInvalid)
		{
			throw new ArgumentException("Invalid handle.", "safePipeHandle");
		}
		ValidateHandleIsPipe(safePipeHandle);
		InitializeHandle(safePipeHandle, isExposed: true, isAsync);
		if (isConnected)
		{
			base.State = PipeState.Connected;
		}
	}

	~NamedPipeClientStream()
	{
		Dispose(disposing: false);
	}

	public void Connect()
	{
		Connect(-1);
	}

	public void Connect(int timeout)
	{
		CheckConnectOperationsClient();
		if (timeout < 0 && timeout != -1)
		{
			throw new ArgumentOutOfRangeException("timeout", "Timeout must be non-negative or equal to -1 (Timeout.Infinite)");
		}
		ConnectInternal(timeout, CancellationToken.None, Environment.TickCount);
	}

	private void ConnectInternal(int timeout, CancellationToken cancellationToken, int startTime)
	{
		int num = 0;
		SpinWait spinWait = default(SpinWait);
		do
		{
			cancellationToken.ThrowIfCancellationRequested();
			int num2 = timeout - num;
			if (cancellationToken.CanBeCanceled && num2 > 50)
			{
				num2 = 50;
			}
			if (TryConnect(num2, cancellationToken))
			{
				return;
			}
			spinWait.SpinOnce();
		}
		while (timeout == -1 || (num = Environment.TickCount - startTime) < timeout);
		throw new TimeoutException();
	}

	public Task ConnectAsync()
	{
		return ConnectAsync(-1, CancellationToken.None);
	}

	public Task ConnectAsync(int timeout)
	{
		return ConnectAsync(timeout, CancellationToken.None);
	}

	public Task ConnectAsync(CancellationToken cancellationToken)
	{
		return ConnectAsync(-1, cancellationToken);
	}

	public Task ConnectAsync(int timeout, CancellationToken cancellationToken)
	{
		CheckConnectOperationsClient();
		if (timeout < 0 && timeout != -1)
		{
			throw new ArgumentOutOfRangeException("timeout", "Timeout must be non-negative or equal to -1 (Timeout.Infinite)");
		}
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}
		int startTime = Environment.TickCount;
		return Task.Run(delegate
		{
			ConnectInternal(timeout, cancellationToken, startTime);
		}, cancellationToken);
	}

	protected internal override void CheckPipePropertyOperations()
	{
		base.CheckPipePropertyOperations();
		if (base.State == PipeState.WaitingToConnect)
		{
			throw new InvalidOperationException("Pipe hasn't been connected yet.");
		}
		if (base.State == PipeState.Broken)
		{
			throw new IOException("Pipe is broken.");
		}
	}

	private void CheckConnectOperationsClient()
	{
		if (base.State == PipeState.Connected)
		{
			throw new InvalidOperationException("Already in a connected state.");
		}
		if (base.State == PipeState.Closed)
		{
			throw Error.GetPipeNotOpen();
		}
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeAccessRights desiredAccessRights, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
		: this(serverName, pipeName, (PipeDirection)(desiredAccessRights & (PipeAccessRights.ReadData | PipeAccessRights.WriteData)), options, impersonationLevel, inheritability)
	{
		if ((desiredAccessRights & ~(PipeAccessRights.FullControl | PipeAccessRights.AccessSystemSecurity)) != 0)
		{
			throw new ArgumentOutOfRangeException("desiredAccessRights", "Invalid PipeAccessRights flag.");
		}
		_access = (int)desiredAccessRights;
	}
}
