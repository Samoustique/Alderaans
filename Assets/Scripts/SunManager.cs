using UnityEngine;
using System.Collections;

public class SunManager : MonoBehaviour {
	public Transform[] sun;
	public float dayCycleInMinutes = 1;
	public float sunRise;
	public float sunSet;
	public float skyboxBlendModifier;
	public enum TimeOfDay
	{
		IDLE,
		SUNRISE,
		SUNSET
	}

	private const float SECOND = 1;
	private const float MINUTE = 60 * SECOND;
	private const float HOUR = 60 * MINUTE;
	private const float DAY = 24 * HOUR;
	private const float DEGREES_PER_SECOND = 360 / DAY;

	private float dayCycleInSeconds;
	private float degreeRotation;
	private float timeOfDay;
	private Sun[] sunScript;
	private TimeOfDay today;
	private float noonTime;

	void Start () {
		today = TimeOfDay.IDLE;
		dayCycleInSeconds = dayCycleInMinutes * MINUTE;

		RenderSettings.skybox.SetFloat ("_Blend", 0);

		sunScript = new Sun[sun.Length];
		for(int i = 0 ; i < sun.Length ; ++i)
		{
			Sun temp = sun [i].GetComponent<Sun> ();
			if (temp == null)
			{
				sun [i].gameObject.AddComponent<Sun> ();
				temp = sun [i].GetComponent<Sun> ();
			}
			sunScript [i] = temp;
		}

		timeOfDay = 0;
		degreeRotation = DEGREES_PER_SECOND * DAY / dayCycleInSeconds;
		sunRise *= dayCycleInSeconds;
		sunSet *= dayCycleInSeconds;
		noonTime = dayCycleInSeconds / 2;

		SetupLighting ();
	}

	void Update () {
		for (int i = 0; i < sun.Length; ++i)
		{
			sun [i].Rotate (new Vector3 (degreeRotation, 0, 0) * Time.deltaTime);
		}

		timeOfDay += Time.deltaTime;

		if (timeOfDay > dayCycleInSeconds)
		{
			timeOfDay -= dayCycleInSeconds;
		}

		if (timeOfDay > sunRise && timeOfDay < sunSet && RenderSettings.skybox.GetFloat ("_Blend") < 1) {
			today = TimeOfDay.SUNRISE;
			BlendSkybox ();
		}
		else if (timeOfDay > sunSet && RenderSettings.skybox.GetFloat ("_Blend") > 0) {
			today = SunManager.TimeOfDay.SUNSET;
			BlendSkybox ();
		}
		else
		{
			today = SunManager.TimeOfDay.IDLE;
		}
	}

	private void BlendSkybox()
	{
		float tmp = 0;
		switch (today)
		{
		//case TimeOfDay.IDLE:
		case TimeOfDay.SUNRISE:
			tmp = (timeOfDay - sunRise) / dayCycleInSeconds * skyboxBlendModifier;
			break;
		case TimeOfDay.SUNSET:
			tmp = (timeOfDay - sunSet) / dayCycleInSeconds * skyboxBlendModifier;
			tmp = 1 - tmp;
			break;
		}

		RenderSettings.skybox.SetFloat ("_Blend", tmp);
	}

	private void SetupLighting()
	{
		for (int i = 0; i < sunScript.Length; i++)
		{
			if (sunScript [i].giveLight)
			{
				sunScript [i].GetComponent<Light> ().intensity = sunScript [i].minLightBrightness;
			}
		}
	}
}
