using System.Net;
using System.Net.Sockets;
using System;

namespace GNetwork
{
	public abstract class Connection
	{
		public event Action ConnectionEstablished;
		public event Action ConnectionFailed;
		public event Action ConnectionLost;

		protected Socket socket;
		protected IPEndPoint ipEndPoint;

		public bool IsConnected { get; protected set; }

		public abstract void TryConnect( IPEndPoint ipEndPoint );
		public abstract void Disconnect();

		public virtual void Dispatch( byte[] data, SocketFlags flag = SocketFlags.None )
		{
			this.socket.SendTo( data, 0, data.Length, flag, this.ipEndPoint );
		}

		protected void Connected()
		{
			this.IsConnected = true;
			if ( this.ConnectionEstablished != null )
			{
				this.ConnectionEstablished.Invoke();
			}
		}

		protected void Disconnected()
		{
			this.IsConnected = false;
			if ( this.ConnectionLost != null )
			{
				this.ConnectionLost.Invoke();
			}
		}

		protected void FailToConnect()
		{
			if ( this.ConnectionFailed != null )
			{
				this.ConnectionFailed.Invoke();
			}
		}
	}
}