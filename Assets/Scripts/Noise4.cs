using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Noise4
{
	#region Variables

	public enum Pattern
	{
		Wood2D,
		Wood3D,
		Marble2D,
		Marble3D
	}

	[Range( 1, 8 )]
	public int Octaves;
	[Range( 0.0f, 1.0f )]
	public float OctavesInfluence = 0.5f;
	public float OctaveScaling = 2.0f;
	public int Seed;
	public float Frquency;
	public Vector4 GradientsSize;

	#region LocalVariables

	private Vector4[,,,] gradients;
	private float[,,,] Q = new float[2, 2, 2, 2];
	private Vector4 gradientSize = new Vector4();
	private Vector4 v4 = new Vector4();
	private Vector4 d = new Vector4();
	private Vector4 n = new Vector4();
	private int X0, Y0, Z0, W0, X1, Y1, W1, Z1, SX, SY, SZ, SW, seed;
	private float noise, scale, value;

	//public bool Smooth;

	#endregion

	[Serializable]
	public class NoiseSettings
	{
		[Range( 1, 8 )]
		[SerializeField]
		public int Octaves = 6;
		[Range( 0.0f, 1.0f )]
		[SerializeField]
		public float OctavesInfluence = 0.5f;
		[SerializeField]
		public float OctaveScaling = 2.0f;
		[SerializeField]
		public int Seed = 0;
		[SerializeField]
		public float Frquency = 1f;
		[SerializeField]
		public Vector4 GradientsSize = new Vector4( 20f, 20f, 20f, 20f );
	}

	#endregion

	public void Initialize( Vector4 gradients, int octaves, float frequency, int seed, float octavesInfluence = 0.5f,
							float octaveScaling = 2.0f )
	{
		this.Octaves = octaves;
		this.GradientsSize = this.gradientSize = gradients;
		this.Frquency = frequency;
		this.Seed = this.seed = seed;
		this.OctavesInfluence = octavesInfluence;
		this.OctaveScaling = octaveScaling;
		this.GenerateGradients();
	}

	public void Initialize( NoiseSettings settings )
	{
		this.Octaves = settings.Octaves;
		this.GradientsSize = this.gradientSize = settings.GradientsSize;
		this.Frquency = settings.Frquency;
		this.Seed = this.seed = settings.Seed;
		this.OctavesInfluence = settings.OctavesInfluence;
		this.OctaveScaling = settings.OctaveScaling;
		this.GenerateGradients();
	}

	#region Methods

	private void GenerateGradients()
	{
		int x = (int)this.GradientsSize.x;
		int y = (int)this.GradientsSize.y;
		int z = (int)this.GradientsSize.z;
		int w = (int)this.GradientsSize.w;

		if ( x < 2 ) x = 2;
		if ( y < 2 ) y = 2;
		if ( z < 2 ) z = 2;
		if ( w < 2 ) w = 2;
		Random.seed = this.seed = this.Seed;
		this.gradientSize = this.GradientsSize = new Vector4( x, y, z, w );
		this.gradients = new Vector4[x, y, z, w];
		this.SX = x;
		this.SY = y;
		this.SZ = z;
		this.SW = w;
		float rr = 0.5f;
		for ( int i = 0; i < x; i++ )
		{
			for ( int j = 0; j < y; j++ )
			{
				for ( int k = 0; k < z; k++ )
				{
					for ( int l = 0; l < w; l++ )
					{
						this.gradients[i, j, k, l] = new Vector4( Random.Range( -rr, rr ), Random.Range( -rr, rr ),
																  Random.Range( -rr, rr ), Random.Range( -rr, rr ) );
					}
				}
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="x">Range(0-1)</param>
	/// <param name="y">Range(0-1)</param>
	/// <param name="z">Range(0-1)</param>
	/// <param name="w">Range(0-1)</param>
	/// <returns></returns>
	public float GetNoise( float x, float y, float z, float w )
	{
		if ( this.Seed != this.seed || this.GradientsSize != this.gradientSize )
		{
			this.GenerateGradients();
		}
		x *= this.Frquency;
		y *= this.Frquency;
		z *= this.Frquency;
		w *= this.Frquency;
		this.noise = 0f;
		this.scale = 1f;
		for ( int o = 0; o < this.Octaves; o++ )
		{
			this.noise += this.scale * this.Noise( x, y, z, w );
			x *= this.OctaveScaling;
			y *= this.OctaveScaling;
			z *= this.OctaveScaling;
			w *= this.OctaveScaling;
			this.scale *= this.OctavesInfluence;
		}
		return noise;
	}

	public float GetNoiseThreadSafe( float x, float y, float z, float w )
	{
		x *= this.Frquency;
		y *= this.Frquency;
		z *= this.Frquency;
		w *= this.Frquency;
		float tsnoise = 0f;
		float tsscale = 1f;
		for ( int o = 0; o < this.Octaves; o++ )
		{
			tsnoise += tsscale * this.NoiseThreadSafe( x, y, z, w );
			x *= this.OctaveScaling;
			y *= this.OctaveScaling;
			z *= this.OctaveScaling;
			w *= this.OctaveScaling;
			tsscale *= this.OctavesInfluence;
		}
		return tsnoise;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="pattern">type of pattern</param>
	/// <param name="x">Range(0-1)</param>
	/// <param name="y">Range(0-1)</param>
	/// <param name="z">Range(0-1)</param>
	/// <param name="noise"></param>
	/// <param name="scale">Pattern scale</param>
	/// <param name="offset">offset of the pattern: Range(0-1)</param>
	/// /// <param name="noiseInterference">Amount of noise interference on pattern</param>
	/// <returns></returns>
	public float ApplyPattern( Pattern pattern, float x, float y, float z, float noise, Vector3 scale, Vector3 offset,
							   float noiseInterference )
	{
		Vector4 pn = new Vector4();
		pn.x = ( x + offset.x ) * scale.x;
		pn.y = ( y + offset.y ) * scale.y;
		pn.z = ( z + offset.z ) * scale.z;
		pn.w = noise * noiseInterference;
		switch ( pattern )
		{
			case Pattern.Marble2D:
				return Mathf.Sin( pn.x + pn.y + pn.w );
			case Pattern.Marble3D:
				return Mathf.Sin( pn.x + pn.y + pn.z + pn.w );
			case Pattern.Wood2D:
				return Mathf.Sin( ( Mathf.Sqrt( pn.x * pn.x + pn.y * pn.y ) + pn.w ) * Mathf.PI );
			case Pattern.Wood3D:
				return Mathf.Sin( ( Mathf.Sqrt( pn.x * pn.x + pn.y * pn.y + pn.z * pn.z ) + pn.w ) * Mathf.PI );
		}
		return 0f;
	}

	private float Noise( float x, float y, float z, float w )
	{
		#region Range the values to gradients size

		x -= Mathf.Floor( x / this.gradientSize.x ) * this.gradientSize.x;
		y -= Mathf.Floor( y / this.gradientSize.y ) * this.gradientSize.y;
		z -= Mathf.Floor( z / this.gradientSize.z ) * this.gradientSize.z;
		w -= Mathf.Floor( w / this.gradientSize.w ) * this.gradientSize.w;

		this.X0 = (int)x;
		this.X1 = X0 + 1 == SX ? 0 : X0 + 1;

		this.Y0 = (int)y;
		this.Y1 = Y0 + 1 == SY ? 0 : Y0 + 1;

		this.Z0 = (int)z;
		this.Z1 = Z0 + 1 == SZ ? 0 : Z0 + 1;

		this.W0 = (int)w;
		this.W1 = W0 + 1 == SW ? 0 : W0 + 1;

		#endregion

		#region Noise Calculation

		#region Distances from points

		this.d.x = x - (float)X0;
		this.d.y = y - (float)Y0;
		this.d.z = z - (float)Z0;
		this.d.w = w - (float)W0;
		this.n.x = d.x - 1f;
		this.n.y = d.y - 1f;
		this.n.z = d.z - 1f;
		this.n.w = d.w - 1f;

		#endregion

		#region Values From Gradients

		this.v4.x = d.x;
		v4.y = d.y;
		v4.z = d.z;
		v4.w = d.w;
		this.Q[0, 0, 0, 0] = Vector4.Dot( v4, this.gradients[X0, Y0, Z0, W0] );

		this.v4.x = d.x;
		v4.y = d.y;
		v4.z = d.z;
		v4.w = n.w;
		this.Q[0, 0, 0, 1] = Vector4.Dot( v4, this.gradients[X0, Y0, Z0, W1] );

		this.v4.x = d.x;
		v4.y = d.y;
		v4.z = n.z;
		v4.w = d.w;
		this.Q[0, 0, 1, 0] = Vector4.Dot( v4, this.gradients[X0, Y0, Z1, W0] );

		this.v4.x = d.x;
		v4.y = d.y;
		v4.z = n.z;
		v4.w = n.w;
		this.Q[0, 0, 1, 1] = Vector4.Dot( v4, this.gradients[X0, Y0, Z1, W1] );

		this.v4.x = d.x;
		v4.y = n.y;
		v4.z = d.z;
		v4.w = d.w;
		this.Q[0, 1, 0, 0] = Vector4.Dot( v4, this.gradients[X0, Y1, Z0, W0] );

		this.v4.x = d.x;
		v4.y = n.y;
		v4.z = d.z;
		v4.w = n.w;
		this.Q[0, 1, 0, 1] = Vector4.Dot( v4, this.gradients[X0, Y1, Z0, W1] );

		this.v4.x = d.x;
		v4.y = n.y;
		v4.z = n.z;
		v4.w = d.w;
		this.Q[0, 1, 1, 0] = Vector4.Dot( v4, this.gradients[X0, Y1, Z1, W0] );

		this.v4.x = d.x;
		v4.y = n.y;
		v4.z = n.z;
		v4.w = n.w;
		this.Q[0, 1, 1, 1] = Vector4.Dot( v4, this.gradients[X0, Y1, Z1, W1] );

		this.v4.x = n.x;
		v4.y = d.y;
		v4.z = d.z;
		v4.w = d.w;
		this.Q[1, 0, 0, 0] = Vector4.Dot( v4, this.gradients[X1, Y0, Z0, W0] );

		this.v4.x = n.x;
		v4.y = d.y;
		v4.z = d.z;
		v4.w = n.w;
		this.Q[1, 0, 0, 1] = Vector4.Dot( v4, this.gradients[X1, Y0, Z0, W1] );

		this.v4.x = n.x;
		v4.y = d.y;
		v4.z = n.z;
		v4.w = d.w;
		this.Q[1, 0, 1, 0] = Vector4.Dot( v4, this.gradients[X1, Y0, Z1, W0] );

		this.v4.x = n.x;
		v4.y = d.y;
		v4.z = n.z;
		v4.w = n.w;
		this.Q[1, 0, 1, 1] = Vector4.Dot( v4, this.gradients[X1, Y0, Z1, W1] );

		this.v4.x = n.x;
		v4.y = n.y;
		v4.z = d.z;
		v4.w = d.w;
		this.Q[1, 1, 0, 0] = Vector4.Dot( v4, this.gradients[X1, Y1, Z0, W0] );

		this.v4.x = n.x;
		v4.y = n.y;
		v4.z = d.z;
		v4.w = n.w;
		this.Q[1, 1, 0, 1] = Vector4.Dot( v4, this.gradients[X1, Y1, Z0, W1] );

		this.v4.x = n.x;
		v4.y = n.y;
		v4.z = n.z;
		v4.w = d.w;
		this.Q[1, 1, 1, 0] = Vector4.Dot( v4, this.gradients[X1, Y1, Z1, W0] );

		this.v4.x = n.x;
		v4.y = n.y;
		v4.z = n.z;
		v4.w = n.w;
		this.Q[1, 1, 1, 1] = Vector4.Dot( v4, this.gradients[X1, Y1, Z1, W1] );

		#endregion

		#region Lerp Of the Values

		return
			SLerp(
				SLerp(
					SLerp(
						SLerp( Q[0, 0, 0, 0], Q[1, 0, 0, 0], d.x ),
						SLerp( Q[0, 1, 0, 0], Q[1, 1, 0, 0], d.x ),
						d.y ),
					SLerp(
						SLerp( Q[0, 0, 1, 0], Q[1, 0, 1, 0], d.x ),
						SLerp( Q[0, 1, 1, 0], Q[1, 1, 1, 0], d.x ),
						d.y ),
					d.z ),
				SLerp(
					SLerp(
						SLerp( Q[0, 0, 0, 1], Q[1, 0, 0, 1], d.x ),
						SLerp( Q[0, 1, 0, 1], Q[1, 1, 0, 1], d.x ),
						d.y ),
					SLerp(
						SLerp( Q[0, 0, 1, 1], Q[1, 0, 1, 1], d.x ),
						SLerp( Q[0, 1, 1, 1], Q[1, 1, 1, 1], d.x ),
						d.y ),
					d.z ),
				d.w );

		#endregion

		#endregion
	}

	private float SLerp( float a, float b, float x )
	{
		this.value = ( 1.0f + Mathf.Cos( x * Mathf.PI ) ) * 0.5f;
		return ( a * this.value ) + ( b - this.value * b );
	}

	private float NoiseThreadSafe( float x, float y, float z, float w )
	{
		int tsX0, tsX1, tsY0, tsY1, tsZ0, tsZ1, tsW0, tsW1;
		Vector4 tsd = new Vector4(), tsn = new Vector4(), tsv4 = new Vector4();
		float[,,,] tsQ = new float[2, 2, 2, 2];

		#region Range the values to gradients size

		x -= Mathf.Floor( x / this.gradientSize.x ) * this.gradientSize.x;
		y -= Mathf.Floor( y / this.gradientSize.y ) * this.gradientSize.y;
		z -= Mathf.Floor( z / this.gradientSize.z ) * this.gradientSize.z;
		w -= Mathf.Floor( w / this.gradientSize.w ) * this.gradientSize.w;

		tsX0 = (int)x;
		tsX1 = tsX0 + 1 == SX ? 0 : tsX0 + 1;

		tsY0 = (int)y;
		tsY1 = tsY0 + 1 == SY ? 0 : tsY0 + 1;

		tsZ0 = (int)z;
		tsZ1 = tsZ0 + 1 == SZ ? 0 : tsZ0 + 1;

		tsW0 = (int)w;
		tsW1 = tsW0 + 1 == SW ? 0 : tsW0 + 1;

		#endregion

		#region Noise Calculation

		#region Distances from points

		tsd.x = x - (float)tsX0;
		tsd.y = y - (float)tsY0;
		tsd.z = z - (float)tsZ0;
		tsd.w = w - (float)tsW0;
		tsn.x = tsd.x - 1f;
		tsn.y = tsd.y - 1f;
		tsn.z = tsd.z - 1f;
		tsn.w = tsd.w - 1f;

		#endregion

		#region Values From Gradients

		tsv4.x = tsd.x;
		tsv4.y = tsd.y;
		tsv4.z = tsd.z;
		tsv4.w = tsd.w;
		tsQ[0, 0, 0, 0] = Vector4.Dot( tsv4, this.gradients[tsX0, tsY0, tsZ0, tsW0] );

		tsv4.x = tsd.x;
		tsv4.y = tsd.y;
		tsv4.z = tsd.z;
		tsv4.w = tsn.w;
		tsQ[0, 0, 0, 1] = Vector4.Dot( tsv4, this.gradients[tsX0, tsY0, tsZ0, tsW1] );

		tsv4.x = tsd.x;
		tsv4.y = tsd.y;
		tsv4.z = tsn.z;
		tsv4.w = tsd.w;
		tsQ[0, 0, 1, 0] = Vector4.Dot( tsv4, this.gradients[tsX0, tsY0, tsZ1, tsW0] );

		tsv4.x = tsd.x;
		tsv4.y = tsd.y;
		tsv4.z = tsn.z;
		tsv4.w = tsn.w;
		tsQ[0, 0, 1, 1] = Vector4.Dot( tsv4, this.gradients[tsX0, tsY0, tsZ1, tsW1] );

		tsv4.x = tsd.x;
		tsv4.y = tsn.y;
		tsv4.z = tsd.z;
		tsv4.w = tsd.w;
		tsQ[0, 1, 0, 0] = Vector4.Dot( tsv4, this.gradients[tsX0, tsY1, tsZ0, tsW0] );

		tsv4.x = tsd.x;
		tsv4.y = tsn.y;
		tsv4.z = tsd.z;
		tsv4.w = tsn.w;
		tsQ[0, 1, 0, 1] = Vector4.Dot( tsv4, this.gradients[tsX0, tsY1, tsZ0, tsW1] );

		tsv4.x = tsd.x;
		tsv4.y = tsn.y;
		tsv4.z = tsn.z;
		tsv4.w = tsd.w;
		tsQ[0, 1, 1, 0] = Vector4.Dot( tsv4, this.gradients[tsX0, tsY1, tsZ1, tsW0] );

		tsv4.x = tsd.x;
		tsv4.y = tsn.y;
		tsv4.z = tsn.z;
		tsv4.w = tsn.w;
		tsQ[0, 1, 1, 1] = Vector4.Dot( tsv4, this.gradients[tsX0, tsY1, tsZ1, tsW1] );

		tsv4.x = tsn.x;
		tsv4.y = tsd.y;
		tsv4.z = tsd.z;
		tsv4.w = tsd.w;
		tsQ[1, 0, 0, 0] = Vector4.Dot( tsv4, this.gradients[tsX1, tsY0, tsZ0, tsW0] );

		tsv4.x = tsn.x;
		tsv4.y = tsd.y;
		tsv4.z = tsd.z;
		tsv4.w = tsn.w;
		tsQ[1, 0, 0, 1] = Vector4.Dot( tsv4, this.gradients[tsX1, tsY0, tsZ0, tsW1] );

		tsv4.x = tsn.x;
		tsv4.y = tsd.y;
		tsv4.z = tsn.z;
		tsv4.w = tsd.w;
		tsQ[1, 0, 1, 0] = Vector4.Dot( tsv4, this.gradients[tsX1, tsY0, tsZ1, tsW0] );

		tsv4.x = tsn.x;
		tsv4.y = tsd.y;
		tsv4.z = tsn.z;
		tsv4.w = tsn.w;
		tsQ[1, 0, 1, 1] = Vector4.Dot( tsv4, this.gradients[tsX1, tsY0, tsZ1, tsW1] );

		tsv4.x = tsn.x;
		tsv4.y = tsn.y;
		tsv4.z = tsd.z;
		tsv4.w = tsd.w;
		tsQ[1, 1, 0, 0] = Vector4.Dot( tsv4, this.gradients[tsX1, tsY1, tsZ0, tsW0] );

		tsv4.x = tsn.x;
		tsv4.y = tsn.y;
		tsv4.z = tsd.z;
		tsv4.w = tsn.w;
		tsQ[1, 1, 0, 1] = Vector4.Dot( tsv4, this.gradients[tsX1, tsY1, tsZ0, tsW1] );

		tsv4.x = tsn.x;
		tsv4.y = tsn.y;
		tsv4.z = tsn.z;
		tsv4.w = tsd.w;
		tsQ[1, 1, 1, 0] = Vector4.Dot( tsv4, this.gradients[tsX1, tsY1, tsZ1, tsW0] );

		tsv4.x = tsn.x;
		tsv4.y = tsn.y;
		tsv4.z = tsn.z;
		tsv4.w = tsn.w;
		tsQ[1, 1, 1, 1] = Vector4.Dot( tsv4, this.gradients[tsX1, tsY1, tsZ1, tsW1] );

		#endregion

		#region Lerp Of the Values

		return
			SLerpThreadSafe(
				SLerpThreadSafe(
					SLerpThreadSafe(
						SLerpThreadSafe( tsQ[0, 0, 0, 0], tsQ[1, 0, 0, 0], tsd.x ),
						SLerpThreadSafe( tsQ[0, 1, 0, 0], tsQ[1, 1, 0, 0], tsd.x ),
						tsd.y ),
					SLerpThreadSafe(
						SLerpThreadSafe( tsQ[0, 0, 1, 0], tsQ[1, 0, 1, 0], tsd.x ),
						SLerpThreadSafe( tsQ[0, 1, 1, 0], tsQ[1, 1, 1, 0], tsd.x ),
						tsd.y ),
					tsd.z ),
				SLerpThreadSafe(
					SLerpThreadSafe(
						SLerpThreadSafe( tsQ[0, 0, 0, 1], tsQ[1, 0, 0, 1], tsd.x ),
						SLerpThreadSafe( tsQ[0, 1, 0, 1], tsQ[1, 1, 0, 1], tsd.x ),
						tsd.y ),
					SLerpThreadSafe(
						SLerpThreadSafe( tsQ[0, 0, 1, 1], tsQ[1, 0, 1, 1], tsd.x ),
						SLerpThreadSafe( tsQ[0, 1, 1, 1], tsQ[1, 1, 1, 1], tsd.x ),
						tsd.y ),
					tsd.z ),
				tsd.w );

		#endregion

		#endregion
	}

	private float SLerpThreadSafe( float a, float b, float x )
	{
		float tsValue = ( 1.0f + Mathf.Cos( x * Mathf.PI ) ) * 0.5f;
		return ( a * tsValue ) + ( b - tsValue * b );
	}

	#endregion
}