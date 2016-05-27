using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuButton : MonoBehaviour {

	private Animator animator;
	private GameObject mainCamera;
	private Transform menuPositionTransform;
	private GameObject refCam;

	public AudioClip audioClip;

	public void Start()
	{
		mainCamera = GameObject.Find ("Main Camera");
		refCam = GameObject.Find ("RefCam");
		animator = ((Animator)mainCamera.GetComponent<Animator>());
		menuPositionTransform = mainCamera.transform;
	}

	public void LoadLevel()
	{
		GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(audioClip);
		SceneManager.LoadScene ("towerDefense");
	}

	public void DisplayHistory()
	{		
		animator.Play("MenuCamHistoryAnim", -1, 0F);
	}

	public void DisplayMainMenu()
	{		
		animator.Play("MenuCamMainMenuAnim", -1, 0F);
	}
}
