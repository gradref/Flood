using UnityEngine;

public class PerlinMover : MonoBehaviour
{
	public Rigidbody Rigidbody;
	[SerializeField]
	public Noise4.NoiseSettings NoiseSettings = new Noise4.NoiseSettings();
	public float ForceMultiplier = 2f;

	public Vector3 NoiseX = new Vector3( 12405f, 0f, 3232f );
	public Vector3 NoiseY = new Vector3( -405f, 41242f, -3327f );
	public Vector3 NoiseZ = new Vector3( -42132, -43112f, 95783 );

	private Noise4 noise = new Noise4();

	protected void Start()
	{
		this.noise.Initialize( this.NoiseSettings );
	}

	protected void FixedUpdate()
	{
		float t = Time.realtimeSinceStartup;
		Vector3 f1 = this.NoiseX + this.NoiseX.normalized * t;
		Vector3 f2 = this.NoiseY + this.NoiseY.normalized * t;
		Vector3 f3 = this.NoiseZ + this.NoiseZ.normalized * t;

		Vector3 force;
		force.x = this.noise.GetNoise( f1.x, f1.y, f1.z, t );
		force.y = this.noise.GetNoise( f2.x, f2.y, f2.z, t );
		force.z = this.noise.GetNoise( f3.x, f3.y, f3.z, t );

		this.Rigidbody.AddForce( force * this.ForceMultiplier, ForceMode.Force );
	}
}