using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GNetwork
{
	[StructLayout( LayoutKind.Explicit )]
	public struct IntUnion
	{
		[FieldOffset( 0 )]
		public int Value;
		[FieldOffset( 0 )]
		public byte Byte0;
		[FieldOffset( 1 )]
		public byte Byte1;
		[FieldOffset( 2 )]
		public byte Byte2;
		[FieldOffset( 3 )]
		public byte Byte3;

		public byte[] ToByteArray()
		{
			return new[] { this.Byte0, this.Byte1, this.Byte2, this.Byte3 };
		}

		public static byte[] FloatToBytes( int value )
		{
			return new IntUnion { Value = value }.ToByteArray();
		}

		public static int BytesToFloat( byte[] bytes )
		{
			if ( bytes.Length != 4 ) throw new ArgumentException( "You must provide four bytes." );
			return new IntUnion
			{
				Byte0 = bytes[0],
				Byte1 = bytes[1],
				Byte2 = bytes[2],
				Byte3 = bytes[3]
			}.Value;
		}

		public static int BytesToFloat( List<byte> bytes, int offset )
		{
			if ( bytes.Count - offset < 4 )
			{
				throw new ArgumentException( "You must provide four bytes." );
			}
			return new IntUnion
			{
				Byte0 = bytes[0 + offset],
				Byte1 = bytes[1 + offset],
				Byte2 = bytes[2 + offset],
				Byte3 = bytes[3 + offset]
			}.Value;
		}

		public static void AddBytes( List<byte> bytes, int value )
		{
			IntUnion floatUnion = new IntUnion { Value = value };
			bytes.Add( floatUnion.Byte0 );
			bytes.Add( floatUnion.Byte1 );
			bytes.Add( floatUnion.Byte2 );
			bytes.Add( floatUnion.Byte3 );
		}

		public void AddBytes( List<byte> bytes )
		{
			bytes.Add( this.Byte0 );
			bytes.Add( this.Byte1 );
			bytes.Add( this.Byte2 );
			bytes.Add( this.Byte3 );
		}

		public void SetFloat( List<byte> bytes, int offset )
		{
			if ( bytes.Count - offset < 4 )
			{
				throw new ArgumentException( "You must provide four bytes." );
			}
			this.Byte0 = bytes[1 + offset];
			this.Byte1 = bytes[1 + offset];
			this.Byte2 = bytes[2 + offset];
			this.Byte3 = bytes[3 + offset];
		}
	}
}