using UnityEngine;
using System.Collections;

public class DynamicSound : MonoBehaviour {

	private AudioSource audioSource;
	private Camera camera;

	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();
		camera = Camera.main;
		audioSource.volume = 0;
	}

	void Update ()
	{
		float distance = Vector3.Distance (transform.position, camera.transform.position);
		if (distance < 8)
		{
			audioSource.volume = 1 - (distance / 40);
		}
		else
		{
			audioSource.volume = 0;
		}
	}
}
