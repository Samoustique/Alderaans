using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public void LoadLevel()
	{
		SceneManager.LoadScene ("towerDefense");
	}
}
