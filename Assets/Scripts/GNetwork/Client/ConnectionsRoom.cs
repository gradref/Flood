using System.Collections.Generic;

namespace GNetwork
{
	public class ConnectionsRoom
	{
		public readonly List<Connection> Connections = new List<Connection>();

		public void Dispatch( List<byte> data )
		{
			if ( data != null )
			{
				for ( int i = 0; i < this.Connections.Count; i++ )
				{
					this.Connections[i].Dispatch( data.ToArray() );
				}
			}
		}
	}
}