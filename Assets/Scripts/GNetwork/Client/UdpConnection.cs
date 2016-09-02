using System.Net;
using System.Net.Sockets;
using GNetwork;

public class UdpConnection : Connection
{
	private UdpClient client;
	public override void TryConnect( IPEndPoint ipEndPoint )
	{
	
		if ( this.socket != null )
		{
			this.socket.Disconnect( false );
		}
		this.BindSocket( ipEndPoint );
	}

	public override void Disconnect()
	{
		throw new System.NotImplementedException();
	}

	private void BindSocket( IPEndPoint ipEndPoint )
	{
		this.ConfigureTcpSocket();
		this.socket.Bind( ipEndPoint );
		this.ipEndPoint = ipEndPoint;
	}

	private void ConfigureTcpSocket()
	{
		this.socket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );

		//Set socket block execution of thread
		this.socket.Blocking = false;

		// Don't allow another socket to bind to this port.
		this.socket.ExclusiveAddressUse = true;

		// The socket will linger for x seconds after 
		// Socket.Close is called.
		this.socket.LingerState = new LingerOption( false, 10 );

		// Disable the Nagle Algorithm for this tcp socket.
		this.socket.NoDelay = true;

		// Set the receive buffer size to 8k
		this.socket.ReceiveBufferSize = 8192;

		// Set the timeout for synchronous receive methods to 
		// 1 second (1000 milliseconds.)
		this.socket.ReceiveTimeout = 1000;

		// Set the send buffer size to 8k.
		this.socket.SendBufferSize = 8192;

		// Set the timeout for synchronous send methods
		// to 1 second (1000 milliseconds.)			
		this.socket.SendTimeout = 1000;

		// Set the Time To Live (TTL) to 42 router hops.
		this.socket.Ttl = 42;
	}
}
