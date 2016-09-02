using UnityEngine;
using System.Collections.Generic;

namespace GNetwork
{
	public class NetObject : MonoBehaviour
	{
		private static readonly Dictionary<int, NetObject> netObjectFromId = new Dictionary<int, NetObject>();

		public readonly List<NetData> NetData = new List<NetData>();
		private int? netObjectID = null;

		protected virtual void Awake()
		{
			GNetworkManager.RequireNetObjectID( this.SetID );
		}

		protected virtual void OnDestroy()
		{
			if ( this.netObjectID.HasValue )
			{
				NetObject.netObjectFromId.Remove( this.netObjectID.Value );
				GNetworkManager.RecycleNetObjectID( this.netObjectID.Value );
			}
		}

		private void SetID( int id )
		{
			this.netObjectID = id;
			NetObject.netObjectFromId.Add( this.netObjectID.Value, this );
		}

		protected virtual void InitializeNetObject( int id )
		{

		}
	}
}