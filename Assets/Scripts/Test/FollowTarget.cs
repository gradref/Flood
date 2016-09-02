using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour
{
	public Transform Target;

	private void Update()
	{
		this.transform.position = new Vector3( this.Target.position.x, this.transform.position.y, this.Target.position.z );
	}
}