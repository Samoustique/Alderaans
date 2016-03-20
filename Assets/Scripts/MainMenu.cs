﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

	private Animator animator;
	private GameObject mainCamera;
	private Transform menuPositionTransform;
	private GameObject refCam;

	public void Start()
	{
		mainCamera = GameObject.Find ("Main Camera");
		refCam = GameObject.Find ("RefCam");
		animator = ((Animator)mainCamera.GetComponent<Animator>());
		menuPositionTransform = mainCamera.transform;
	}

	public void LoadLevel()
	{
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
