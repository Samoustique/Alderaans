using UnityEngine;
using System.Collections;

[AddComponentMenu("Environments/Sun")]
public class Sun : MonoBehaviour {
	public float minLightBrightness;
	public float maxLightBrightness;
	public float minFlareBrightness;
	public float maxFlareBrightness;
	public bool giveLight = false;

	void Start()
	{
		if (GetComponent<Light> () != null)
		{
			giveLight = true;
		}
	}
}
