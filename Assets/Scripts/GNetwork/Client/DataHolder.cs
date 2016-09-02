using System.Collections.Generic;

public class DataHolder
{
	public readonly byte[] Data;
	public int Size { get; private set; }
	public DataHolder( int size )
	{
		this.Size = size;
		this.Data = new byte[size];
	}

	public void Set( List<byte> data )
	{
		for ( int i = 0; i < this.Size; i++ )
		{
			this.Data[i] = data[i];
		}
	}
}