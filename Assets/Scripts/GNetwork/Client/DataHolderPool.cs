using System.Collections.Generic;

namespace GNetwork
{
	public class DataHolderPool
	{
		private object poolLock = new object();
		private readonly Dictionary<int, Queue<DataHolder>> dataHoldersDictionary = new Dictionary<int, Queue<DataHolder>>();
		private int prealloc;

		public DataHolderPool( int prealloc = 8 )
		{
			this.prealloc = prealloc;
		}

		public DataHolder Get( int size )
		{
			lock( this.poolLock )
			{
				Queue<DataHolder> dataHolders;

				if ( !this.dataHoldersDictionary.TryGetValue( size, out dataHolders ) )
				{
					dataHolders = new Queue<DataHolder>();
					this.dataHoldersDictionary.Add( size, new Queue<DataHolder>( this.prealloc ) );
				}

				if ( dataHolders.Count == 0 )
				{
					return new DataHolder( size );
				}
				else
				{
					return dataHolders.Dequeue();
				}
			}
		}

		public void Recycle( DataHolder dataHolder )
		{
			lock( this.poolLock )
			{
				Queue<DataHolder> dataHolders;

				if ( !this.dataHoldersDictionary.TryGetValue( dataHolder.Size, out dataHolders ) )
				{
					dataHolders = new Queue<DataHolder>();
					this.dataHoldersDictionary.Add( dataHolder.Size, new Queue<DataHolder>( this.prealloc ) );
				}

				dataHolders.Enqueue( dataHolder );
			}
		}
	}
}