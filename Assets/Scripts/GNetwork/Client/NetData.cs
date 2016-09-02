using UnityEngine;
using System;
using System.Collections.Generic;

namespace GNetwork
{
	public abstract class NetData
	{
		private static readonly Dictionary<Type, byte> dataIDs = new Dictionary<Type, byte>();

		private readonly List<byte> data = new List<byte>( 8 );
		public byte? DataID { get; private set; }
		private object dataLock = new object();

		protected NetData( int size )
		{
			byte id;
			if ( NetData.dataIDs.TryGetValue( this.GetType(), out id ) )
			{
				this.DataID = id;
			}
		}

		private void UpdateData( int netObjectID )
		{
			this.data.Clear();
			this.FillData();
			this.data.Add( this.DataID.Value );
			IntUnion.AddBytes( this.data, netObjectID );
		}

		public DataHolder GetData( int netObjectID )
		{
			lock( this.dataLock )
			{
				if ( this.DataID.HasValue )
				{
					this.UpdateData( netObjectID );

					DataHolder dataHolder = GNetworkManager.DataHolderPool.Get( this.data.Count );
					dataHolder.Set( this.data );
					return dataHolder;
				}
				else
				{
					Debug.LogError( "No dataID found for the type: " + this.GetType() );
				}
				return null;
			}
		}

		protected abstract void FillData();
	}
}