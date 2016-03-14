using UnityEngine;
using System.Collections;

public class Float : MonoBehaviour {
	public float speed;
	public float amplitude;
	private Vector3 tempPosition, initPosition;

	void Start () 
	{
		initPosition = tempPosition = transform.position;
	}

	void FixedUpdate () 
	{
		tempPosition.y = initPosition.y + Mathf.Sin(Time.realtimeSinceStartup * speed) * amplitude;
		transform.position = tempPosition;
	}
}
