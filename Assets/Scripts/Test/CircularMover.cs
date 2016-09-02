using UnityEngine;

public class CircularMover : MonoBehaviour
{
	public Rigidbody Rigidbody;
	public float Force = 2f;
	public float AngularSpeed = 0.1f;
	private float angle;

	protected void FixedUpdate()
	{
		this.angle += this.AngularSpeed * Time.fixedDeltaTime;
		Vector3 force = new Vector3( Mathf.Cos( this.angle ), 0f, Mathf.Sin( this.angle ) ) * this.Force;
		this.Rigidbody.AddForce( force, ForceMode.Force );
	}
}