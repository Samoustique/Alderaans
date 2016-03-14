using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public int speed;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.left * Time.deltaTime * speed, Space.World);
	}
}
