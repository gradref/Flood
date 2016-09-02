using System.Collections.Generic;

namespace GNetwork
{
	public static class GNetworkManager
	{
		private static readonly List<ConnectionsRoom> rooms = new List<ConnectionsRoom>();
		public static readonly DataHolderPool DataHolderPool = new DataHolderPool( 8 );


		public delegate void IdRequestHandler( int id );
		private static readonly Connection remoteIDManager = new TcpConnection();
		private static readonly Queue<IdRequestHandler> idRequests = new Queue<IdRequestHandler>();

		public static void RequireNetObjectID( IdRequestHandler idRequest )
		{
			GNetworkManager.idRequests.Enqueue( idRequest );
			//TODO

		}

		public static void RecycleNetObjectID( int id )
		{
			//TODO
		}

		public static void AssignNetObjectID( int id )
		{
			if ( GNetworkManager.idRequests.Count > 0 )
			{
				IdRequestHandler idRequest = GNetworkManager.idRequests.Dequeue();
				if ( idRequest.Target != null )
				{
					idRequest( id );
				}
				else
				{
					GNetworkManager.RecycleNetObjectID( id );
				}
			}
		}
	}
}