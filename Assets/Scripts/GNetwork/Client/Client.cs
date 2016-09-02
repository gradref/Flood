using System.Net.Sockets;

namespace GNetwork
{
	using System.Threading;

	public class Client
	{
		public void SendPacket()
		{
			Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			byte[] myinfo = new byte[] { 210, 100 };
			socket.Send( myinfo, SocketFlags.None );
		}
	}

	public class NetworkManager
	{
		public void Start()
		{
			Thread thread = new Thread( this.NetListener );
			thread.Start();
		}

		private void NetListener()
		{
			//IPEndPoint ipEndPoint = new IPEndPoint( IPAddress.Parse("93.38.51.127"), 32 );
			//TcpClient tcpClient = new TcpClient( ipEndPoint );
			//tcpClient.
			//while ( true )
			//{
			//	Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			//	byte[] myinfo;
			//	socket.Receive( myinfo );
			//}
		}
	}
}