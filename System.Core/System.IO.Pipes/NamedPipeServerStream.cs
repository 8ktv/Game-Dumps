using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

public sealed class NamedPipeServerStream : PipeStream
{
	internal class ExecuteHelper
	{
		internal PipeStreamImpersonationWorker _userCode;

		internal SafePipeHandle _handle;

		internal bool _mustRevert;

		internal int _impersonateErrorCode;

		internal int _revertImpersonateErrorCode;

		internal ExecuteHelper(PipeStreamImpersonationWorker userCode, SafePipeHandle handle)
		{
			_userCode = userCode;
			_handle = handle;
		}
	}

	private static RuntimeHelpers.TryCode tryCode = ImpersonateAndTryCode;

	private static RuntimeHelpers.CleanupCode cleanupCode = RevertImpersonationOnBackout;

	public const int MaxAllowedServerInstances = -1;

	private void Create(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, HandleInheritability inheritability)
	{
		Create(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, null, inheritability, (PipeAccessRights)0);
	}

	private void Create(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability, PipeAccessRights additionalAccessRights)
	{
		string fullPath = Path.GetFullPath("\\\\.\\pipe\\" + pipeName);
		if (string.Equals(fullPath, "\\\\.\\pipe\\anonymous", StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentOutOfRangeException("pipeName", "The pipeName \\\"anonymous\\\" is reserved.");
		}
		if (base.IsCurrentUserOnly)
		{
			using (WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent())
			{
				SecurityIdentifier owner = windowsIdentity.Owner;
				PipeAccessRule rule = new PipeAccessRule(owner, PipeAccessRights.FullControl, AccessControlType.Allow);
				pipeSecurity = new PipeSecurity();
				pipeSecurity.AddAccessRule(rule);
				pipeSecurity.SetOwner(owner);
			}
			options &= ~PipeOptions.CurrentUserOnly;
		}
		int openMode = (int)((uint)direction | (uint)((maxNumberOfServerInstances == 1) ? 524288 : 0) | (uint)options) | (int)additionalAccessRights;
		int pipeMode = ((int)transmissionMode << 2) | ((int)transmissionMode << 1);
		if (maxNumberOfServerInstances == -1)
		{
			maxNumberOfServerInstances = 255;
		}
		GCHandle pinningHandle = default(GCHandle);
		try
		{
			global::Interop.Kernel32.SECURITY_ATTRIBUTES securityAttributes = PipeStream.GetSecAttrs(inheritability, pipeSecurity, ref pinningHandle);
			SafePipeHandle safePipeHandle = global::Interop.Kernel32.CreateNamedPipe(fullPath, openMode, pipeMode, maxNumberOfServerInstances, outBufferSize, inBufferSize, 0, ref securityAttributes);
			if (safePipeHandle.IsInvalid)
			{
				throw Win32Marshal.GetExceptionForLastWin32Error();
			}
			InitializeHandle(safePipeHandle, isExposed: false, (options & PipeOptions.Asynchronous) != 0);
		}
		finally
		{
			if (pinningHandle.IsAllocated)
			{
				pinningHandle.Free();
			}
		}
	}

	public void WaitForConnection()
	{
		CheckConnectOperationsServerWithHandle();
		if (base.IsAsync)
		{
			WaitForConnectionCoreAsync(CancellationToken.None).GetAwaiter().GetResult();
			return;
		}
		if (!global::Interop.Kernel32.ConnectNamedPipe(base.InternalHandle, IntPtr.Zero))
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 535)
			{
				throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error);
			}
			if (lastWin32Error == 535 && base.State == PipeState.Connected)
			{
				throw new InvalidOperationException("Already in a connected state.");
			}
		}
		base.State = PipeState.Connected;
	}

	public Task WaitForConnectionAsync(CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}
		if (!base.IsAsync)
		{
			return Task.Factory.StartNew(delegate(object s)
			{
				((NamedPipeServerStream)s).WaitForConnection();
			}, this, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}
		return WaitForConnectionCoreAsync(cancellationToken);
	}

	public void Disconnect()
	{
		CheckDisconnectOperations();
		if (!global::Interop.Kernel32.DisconnectNamedPipe(base.InternalHandle))
		{
			throw Win32Marshal.GetExceptionForLastWin32Error();
		}
		base.State = PipeState.Disconnected;
	}

	public string GetImpersonationUserName()
	{
		CheckWriteOperations();
		StringBuilder stringBuilder = new StringBuilder(514);
		if (!global::Interop.Kernel32.GetNamedPipeHandleState(base.InternalHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, stringBuilder, stringBuilder.Capacity))
		{
			throw WinIOError(Marshal.GetLastWin32Error());
		}
		return stringBuilder.ToString();
	}

	public void RunAsClient(PipeStreamImpersonationWorker impersonationWorker)
	{
		CheckWriteOperations();
		ExecuteHelper executeHelper = new ExecuteHelper(impersonationWorker, base.InternalHandle);
		RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(tryCode, cleanupCode, executeHelper);
		if (executeHelper._impersonateErrorCode != 0)
		{
			throw WinIOError(executeHelper._impersonateErrorCode);
		}
		if (executeHelper._revertImpersonateErrorCode != 0)
		{
			throw WinIOError(executeHelper._revertImpersonateErrorCode);
		}
	}

	private static void ImpersonateAndTryCode(object helper)
	{
		ExecuteHelper executeHelper = (ExecuteHelper)helper;
		RuntimeHelpers.PrepareConstrainedRegions();
		try
		{
		}
		finally
		{
			if (global::Interop.Advapi32.ImpersonateNamedPipeClient(executeHelper._handle))
			{
				executeHelper._mustRevert = true;
			}
			else
			{
				executeHelper._impersonateErrorCode = Marshal.GetLastWin32Error();
			}
		}
		if (executeHelper._mustRevert)
		{
			executeHelper._userCode();
		}
	}

	private static void RevertImpersonationOnBackout(object helper, bool exceptionThrown)
	{
		ExecuteHelper executeHelper = (ExecuteHelper)helper;
		if (executeHelper._mustRevert && !global::Interop.Advapi32.RevertToSelf())
		{
			executeHelper._revertImpersonateErrorCode = Marshal.GetLastWin32Error();
		}
	}

	private unsafe Task WaitForConnectionCoreAsync(CancellationToken cancellationToken)
	{
		CheckConnectOperationsServerWithHandle();
		if (!base.IsAsync)
		{
			throw new InvalidOperationException("Pipe is not opened in asynchronous mode.");
		}
		ConnectionCompletionSource connectionCompletionSource = new ConnectionCompletionSource(this);
		if (!global::Interop.Kernel32.ConnectNamedPipe(base.InternalHandle, connectionCompletionSource.Overlapped))
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			switch (lastWin32Error)
			{
			case 535:
				connectionCompletionSource.ReleaseResources();
				if (base.State == PipeState.Connected)
				{
					throw new InvalidOperationException("Already in a connected state.");
				}
				connectionCompletionSource.SetCompletedSynchronously();
				return Task.CompletedTask;
			default:
				connectionCompletionSource.ReleaseResources();
				throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error);
			case 997:
				break;
			}
		}
		connectionCompletionSource.RegisterForCancellation(cancellationToken);
		return connectionCompletionSource.Task;
	}

	private void CheckConnectOperationsServerWithHandle()
	{
		if (base.InternalHandle == null)
		{
			throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
		}
		CheckConnectOperationsServer();
	}

	public NamedPipeServerStream(string pipeName)
		: this(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, HandleInheritability.None)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction)
		: this(pipeName, direction, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, HandleInheritability.None)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances)
		: this(pipeName, direction, maxNumberOfServerInstances, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, HandleInheritability.None)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, PipeOptions.None, 0, 0, HandleInheritability.None)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, 0, 0, HandleInheritability.None)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, HandleInheritability.None)
	{
	}

	private NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, HandleInheritability inheritability)
		: base(direction, transmissionMode, outBufferSize)
	{
		if (pipeName == null)
		{
			throw new ArgumentNullException("pipeName");
		}
		if (pipeName.Length == 0)
		{
			throw new ArgumentException("pipeName cannot be an empty string.");
		}
		if ((options & (PipeOptions)536870911) != PipeOptions.None)
		{
			throw new ArgumentOutOfRangeException("options", "options contains an invalid flag.");
		}
		if (inBufferSize < 0)
		{
			throw new ArgumentOutOfRangeException("inBufferSize", "Non negative number is required.");
		}
		if ((maxNumberOfServerInstances < 1 || maxNumberOfServerInstances > 254) && maxNumberOfServerInstances != -1)
		{
			throw new ArgumentOutOfRangeException("maxNumberOfServerInstances", "maxNumberOfServerInstances must either be a value between 1 and 254, or NamedPipeServerStream.MaxAllowedServerInstances (to obtain the maximum number allowed by system resources).");
		}
		if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
		{
			throw new ArgumentOutOfRangeException("inheritability", "HandleInheritability.None or HandleInheritability.Inheritable required.");
		}
		if ((options & PipeOptions.CurrentUserOnly) != PipeOptions.None)
		{
			base.IsCurrentUserOnly = true;
		}
		Create(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, inheritability);
	}

	public NamedPipeServerStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
		: base(direction, PipeTransmissionMode.Byte, 0)
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

	~NamedPipeServerStream()
	{
		Dispose(disposing: false);
	}

	public Task WaitForConnectionAsync()
	{
		return WaitForConnectionAsync(CancellationToken.None);
	}

	public IAsyncResult BeginWaitForConnection(AsyncCallback callback, object state)
	{
		return TaskToApm.Begin(WaitForConnectionAsync(), callback, state);
	}

	public void EndWaitForConnection(IAsyncResult asyncResult)
	{
		TaskToApm.End(asyncResult);
	}

	private void CheckConnectOperationsServer()
	{
		if (base.State == PipeState.Closed)
		{
			throw Error.GetPipeNotOpen();
		}
		if (base.InternalHandle != null && base.InternalHandle.IsClosed)
		{
			throw Error.GetPipeNotOpen();
		}
		if (base.State == PipeState.Broken)
		{
			throw new IOException("Pipe is broken.");
		}
	}

	private void CheckDisconnectOperations()
	{
		if (base.State == PipeState.WaitingToConnect)
		{
			throw new InvalidOperationException("Pipe hasn't been connected yet.");
		}
		if (base.State == PipeState.Disconnected)
		{
			throw new InvalidOperationException("Already in a disconnected state.");
		}
		if (base.InternalHandle == null)
		{
			throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
		}
		if (base.State == PipeState.Closed || (base.InternalHandle != null && base.InternalHandle.IsClosed))
		{
			throw Error.GetPipeNotOpen();
		}
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, HandleInheritability.None)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, inheritability)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability, PipeAccessRights additionalAccessRights)
		: base(direction, transmissionMode, outBufferSize)
	{
		if (pipeName == null)
		{
			throw new ArgumentNullException("pipeName");
		}
		if (pipeName.Length == 0)
		{
			throw new ArgumentException("pipeName cannot be an empty string.");
		}
		if ((options & (PipeOptions)1073741823) != PipeOptions.None)
		{
			throw new ArgumentOutOfRangeException("options", "options contains an invalid flag.");
		}
		if (inBufferSize < 0)
		{
			throw new ArgumentOutOfRangeException("inBufferSize", "Non negative number is required.");
		}
		if ((maxNumberOfServerInstances < 1 || maxNumberOfServerInstances > 254) && maxNumberOfServerInstances != -1)
		{
			throw new ArgumentOutOfRangeException("maxNumberOfServerInstances", "maxNumberOfServerInstances must either be a value between 1 and 254, or NamedPipeServerStream.MaxAllowedServerInstances (to obtain the maximum number allowed by system resources).");
		}
		if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
		{
			throw new ArgumentOutOfRangeException("inheritability", "HandleInheritability.None or HandleInheritability.Inheritable required.");
		}
		if ((additionalAccessRights & ~(PipeAccessRights.ChangePermissions | PipeAccessRights.TakeOwnership | PipeAccessRights.AccessSystemSecurity)) != 0)
		{
			throw new ArgumentOutOfRangeException("additionalAccessRights", "additionalAccessRights is limited to the PipeAccessRights.ChangePermissions, PipeAccessRights.TakeOwnership, and PipeAccessRights.AccessSystemSecurity flags when creating NamedPipeServerStreams.");
		}
		Create(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, pipeSecurity, inheritability, additionalAccessRights);
	}
}
