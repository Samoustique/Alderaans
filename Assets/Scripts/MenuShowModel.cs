using UnityEngine;
using System.Collections;

public class MenuShowModel : MonoBehaviour {
	void Update ()
	{
		Vector3 vec = new Vector3(UnityEngine.Random.Range(0, 2),UnityEngine.Random.Range(0, 2),UnityEngine.Random.Range(0, 2));
		transform.Rotate(vec * Time.deltaTime * 5);
	}
}
