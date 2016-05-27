using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class GameManager : MonoBehaviour {
	public static int gold = 500;
	public static int life = 5;
	public static List<GameObject> mobsAlive = new List<GameObject>();
	public static Step step;

	public GameObject topCamObject;
	public AudioClip sellingSound, bigExplosion;
	public Text txtGold, txtLife;
	public GameObject panelTimerConstruction;
	public Button btnNextWave;
	public GameObject particles, home, gameOver;
	public int constructionLast;
	public enum Step
	{
		STARTING,
		CONSTRUCTION_TITLE,
		CONSTRUCTION,
		ROUND_TITLE,
		ROUND,
		GAME_OVER
	};

	private static Material notSelectedMaterial;
	private static int selectedCoordX, selectedCoordY;
	private static GameObject selectedTower;
	private static GameObject selectionAura;

	private Toggle[] toggles;
	private Dictionary<TglTower, Tower> choiceTowers;
	private GameObject title, smallTitle, timerConstruction, nextWave;
	private float startTimer, titleTimer, spawnTimer, constructionTimer, spawnLaps, shakeTimer, shakeAmount;
	private int nbRound = 1;
	private bool showTitleText;
	private Spawn spawner;
	private Animator animator;
	private Camera topCam;
	private AstarSingleton aStarSingleton;
	private Astar aStar;
	private GameObject btnSkip;
	private GameObject btnCamera;
	private List<GameObject> camReferences = new List<GameObject>();
	private int idCamRef = 0;

	void Start()
	{
		toggles = GameObject.Find("TowerChoice").GetComponentsInChildren<Toggle>();
		choiceTowers = new Dictionary<TglTower, Tower>();
		retrieveChoiceTowers();
		DisableChoices();

		title = GameObject.Find("txtTitle");
		title.SetActive(false);
		smallTitle = GameObject.Find("txtSmallTitle");
		smallTitle.SetActive(false);
		spawnLaps = 2;
		spawner = GameObject.Find("Spawn").GetComponentInChildren<Spawn>();
		step = Step.STARTING;
		shakeAmount = 0.1F;
		shakeTimer = 1F;

		HideTowerDetails();

		btnNextWave.onClick.AddListener(EndConstructionTime);
		nextWave = GameObject.Find("btnNextWave");
		panelTimerConstruction.SetActive(false);
		nextWave.SetActive(false);

		//((Button) GameObject.Find("btnRestart").GetComponent<Button>()).onClick.AddListener(Restart);
		//((Button) GameObject.Find("btnMainMenu").GetComponent<Button>()).onClick.AddListener(MainMenu);
		((Button) GameObject.Find("btnCamera").GetComponent<Button>()).onClick.AddListener(MoveCamera);
		((Button) GameObject.Find("btnSell").GetComponent<Button>()).onClick.AddListener(SellTower);
		((Button) GameObject.Find("btnUp").GetComponent<Button>()).onClick.AddListener(UpTower); 
		btnCamera = GameObject.Find ("btnCamera");
		btnSkip = GameObject.Find ("btnSkip");
		((Button) btnSkip.GetComponent<Button> ()).onClick.AddListener(SkipCinematic); 
		btnCamera.SetActive (false);
		gameOver.SetActive (false);

		animator = ((Animator)GetComponent<Animator>());
		topCam = (Camera) topCamObject.GetComponent<Camera> ();

		camReferences.Add(GameObject.Find("CameraRefBase"));
		camReferences.Add(GameObject.Find("CameraRefPos2"));
		camReferences.Add(GameObject.Find("CameraRefPos3"));

		animator.Play("CameraStartAnim", -1, 0F);
		startTimer = Time.time;

		/****A-STAR******/
		aStarSingleton = AstarSingleton.getInstance ();
		aStar = aStarSingleton.astar;

		foreach (GameObject floor in GameObject.FindGameObjectsWithTag("floor"))
		{
			Build build = floor.GetComponent("Build") as Build;
			build.InitiateAstar (aStarSingleton);
		}
	}

	void Update()
	{
		InputKeys();

		UpdateRound();

		UpdateGold();

		UpdateTowerSelection();

		// home life
		txtLife.text = "Life : " + life;
		if (life == 0)
		{
			if (shakeTimer >= 0)
			{
				animator.Stop ();
				Vector2 shakePos = UnityEngine.Random.insideUnitCircle * shakeAmount;
				transform.position = new Vector3 (transform.position.x + shakePos.x, transform.position.y + shakePos.y, transform.position.z);
				shakeTimer -= Time.deltaTime;
			}
			else if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("CameraGameOverAnim"))
			{
				animator.Rebind ();
				animator.Play("CameraGameOverAnim", -1, 0F);
			}
			if (step != Step.GAME_OVER)
			{
				EndOfGame ("Game Over");
				Instantiate (particles, home.transform.position, Quaternion.identity);
				GetComponent<AudioSource>().PlayOneShot(bigExplosion);
				Destroy (home);
			}
		}
	}

	void EndOfGame(String text)
	{
		StopNavMeshAgents ();
		StopTurretsShooting ();
		ShowGameOver (text);
		step = Step.GAME_OVER;
	}

	void ShowGameOver (String text)
	{
		foreach (GameObject ui in GameObject.FindGameObjectsWithTag("UI"))
		{
			string name = ui.transform.name;
			if (name != "Canvas")
			{
				ui.SetActive (false);
			}
		}

		gameOver.SetActive (true);
		Text t = gameOver.GetComponentInChildren<Text> ();
		t.text = text;
	}

	void StopTurretsShooting ()
	{
		foreach (GameObject floor in GameObject.FindGameObjectsWithTag("floor")) {
			Build build = floor.GetComponent("Build") as Build;
			GameObject goTower = build.towerBuilt;
			if (goTower != null)
			{
				Tower tower = goTower.GetComponentInChildren<Tower>() as Tower;
				if (tower != null)
				{
					tower.NotifyStopShooting ();
				}
			}
		}
	}

	void StopNavMeshAgents ()
	{
		foreach(GameObject mob in mobsAlive)
		{
			(mob.GetComponent<NavMeshAgent> () as NavMeshAgent).Stop();
		}
	}

	void InputKeys()
	{
		if (Input.GetKeyDown (KeyCode.Tab))
		{
			topCam.enabled = !topCam.enabled;
		}
	}

	void DisableChoices()
	{
		// disable towerChoice
		foreach (TglTower tglTower in choiceTowers.Keys)
		{
			tglTower.disable();
		}
	}

	void EnableChoices()
	{
		foreach (TglTower tglTower in choiceTowers.Keys)
		{
			tglTower.enable();
		}
	}

	private void StartingTime()
	{
		if (Time.time >= startTimer + 0.1F && animator.GetCurrentAnimatorStateInfo (0).IsName ("Stationnary"))
		{
			StartingConstructionTitle ();
			btnSkip.SetActive(false);
			btnCamera.SetActive(true);
		}
	}

	private void StartingConstructionTitle()
	{
		step = Step.CONSTRUCTION_TITLE;
		titleTimer = constructionTimer = Time.time;
	}

	private void ConstructionTitleTime()
	{
		(title.GetComponent<Text>()).text = "Construction";
		title.SetActive(true);
		smallTitle.SetActive(false);
		foreach(Toggle toggle in toggles)
		{
			toggle.enabled = false;
		}

		if (Time.time >= titleTimer + 2)
		{
			step = Step.CONSTRUCTION;
			constructionTimer = Time.time;
			panelTimerConstruction.SetActive (true);
			nextWave.SetActive(true);
			EnableChoices();
		}
	}

	private void ConstructionTime()
	{
		(smallTitle.GetComponent<Text>()).text = "Construction";
		title.SetActive(false);
		smallTitle.SetActive(true);

		// timer display
		float timeLeft = constructionTimer + constructionLast - Time.time;
		int seconds = (int)(timeLeft % 60F);
		(panelTimerConstruction.GetComponentInChildren<Text>()).text = seconds.ToString("00");

		if (seconds == 0)
		{
			EndConstructionTime();
		}
	}

	public void EndConstructionTime()
	{
		panelTimerConstruction.SetActive(false);
		nextWave.SetActive(false);
		step = Step.ROUND_TITLE;
		titleTimer = Time.time;
		DisableChoices();
	}

	private void RoundTitleTime()
	{
		(title.GetComponent<Text>()).text = "Wave " + nbRound;
		title.SetActive(true);
		smallTitle.SetActive(false);
		foreach (Toggle toggle in toggles)
		{
			toggle.enabled = false;
		}

		if (Time.time >= titleTimer + 2)
		{
			step = Step.ROUND;
			titleTimer = Time.time;
		}
	}

	private void RoundTime()
	{
		(smallTitle.GetComponent<Text>()).text = "Wave " + nbRound;
		title.SetActive(false);
		smallTitle.SetActive(true);

		// if we finished to spawn everybody
		if (Spawn.mobsPerRound[nbRound - 1].Count == 0)
		{
			if (GameManager.mobsAlive.Count == 0)
			{
				if (Spawn.mobsPerRound.Count == nbRound)
				{
					if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("CameraGameOverAnim"))
					{
						animator.Rebind ();
						animator.Play("CameraGameOverAnim", -1, 0F);
					}
					EndOfGame ("Victory !");
				}
				else
				{
					titleTimer = Time.time;
					nbRound++;
					step = Step.CONSTRUCTION_TITLE;
					//animator.Play("CameraReturnAnim", -1, 0F);
				}
			}
		}
		else
		{
			if (Time.time > spawnTimer + spawnLaps)
			{
				spawner.SpawnMob(nbRound);
				spawnTimer = Time.time;
				spawnLaps = UnityEngine.Random.Range(0.2F, 3F);
			}
		}
	}

	private void UpdateRound()
	{
		switch (step)
		{
		case Step.STARTING :
			StartingTime();
			break;
		case Step.CONSTRUCTION_TITLE :
			ConstructionTitleTime();
			break;
		case Step.CONSTRUCTION :
			ConstructionTime();
			break;
		case Step.ROUND_TITLE :
			RoundTitleTime();
			break;
		case Step.ROUND :
			RoundTime();
			break;
		}
	}

	private void LaunchRandomCamAnim()
	{
		String cameraCrossField = "CameraCrossField";
		String anim = "Anim";
		int animNumber = UnityEngine.Random.Range(1, 4);
		animator.Play(cameraCrossField + animNumber + anim, -1, 0F);
	}

	static public void ShowUpReward(Vector3 showPosition, GameObject textToInstantiate)
	{
		Vector3 worldToScreenPoint = Camera.main.WorldToScreenPoint(showPosition);
		GameObject toDelete = new GameObject();
		GameObject empty = Instantiate(toDelete, worldToScreenPoint, Quaternion.identity) as GameObject;
		empty.transform.name = "RewardWrapper";
		empty.transform.SetParent(GameObject.Find("Canvas").transform);
		GameObject temp = Instantiate(textToInstantiate, worldToScreenPoint, Quaternion.identity) as GameObject;
		temp.transform.SetParent(empty.transform);
		temp.GetComponent<Animator>().SetTrigger("Reward");
		Destroy(temp.gameObject, 1.0F);
		Destroy(empty.gameObject, 1.0F);
		Destroy(toDelete, 1.0F);
	}

	private void UpdateGold()
	{
		txtGold.text = "Credits :   " + gold;
	}

	private void UpdateTowerSelection()
	{
		if (step == Step.CONSTRUCTION)
		{
			foreach (TglTower tglTower in choiceTowers.Keys)
			{
				if (gold >= choiceTowers[tglTower].cost)
					tglTower.enable();
				else
					tglTower.disable();
			}
		}
	}

	private void retrieveChoiceTowers()
	{
		foreach (Toggle toggle in toggles)
		{
			TglTower tglTower = toggle.GetComponent<TglTower>() as TglTower;
			Tower tower = null;
			Transform sphere = FindSphereInChildren(tglTower.towerToBuild.transform);
			if (sphere != null)
			{
				tower = sphere.GetComponent<Tower>() as Tower;
				choiceTowers.Add(tglTower, tower);
			}
		}
	}

	static public void SelectUnselectTower(GameObject towerToSelect, int coordX, int coordY)
	{
		if (!towerToSelect)
		{
			UnselectTower();
		}
		else if (selectedTower != null && towerToSelect != selectedTower) // TODO autoriser cliquage dans zone d'infos de la tour
		{
			UnselectTower();
			SelectTower(towerToSelect, coordX, coordY);
		}
		else if (!selectedTower || towerToSelect != selectedTower) // TODO autoriser cliquage dans zone d'infos de la tour
		{
			SelectTower(towerToSelect, coordX, coordY);
		}
	}

	static private void SelectTower(GameObject towerToSelect, int coordX, int coordY)
	{
		selectedTower = towerToSelect;
		selectedCoordX = coordX;
		selectedCoordY = coordY;
		selectionAura = (GameObject) Instantiate(GameObject.Find("SelectionAura"), towerToSelect.transform.position, Quaternion.identity);
		selectionAura.transform.SetParent(selectedTower.transform);
		RevealTowerDetails(towerToSelect);
	}

	static private void RevealTowerDetails(GameObject towerToSelect)
	{
		Tower tower = towerToSelect.GetComponentInChildren<Tower>() as Tower;
		Bullet bullet = tower.bullet.GetComponent<Bullet>() as Bullet;
		GameObject towerSelectedDetails = GameObject.Find("TowerSelected");
		towerSelectedDetails.GetComponent<Image>().enabled = true;
		//SphereCollider sphereColl = towerToSelect.GetComponentInChildren<SphereCollider>() as SphereCollider;
		DisplayTowerCharacteristic("Name", tower.title);
		DisplayTowerCharacteristic("Lvl", "lvl" + tower.level);
		DisplayTowerCharacteristic("Dmg", "Dmg\n" + bullet.damage);
		DisplayTowerCharacteristic("Rng", "Rng\n" + tower.range);
		DisplayTowerCharacteristic("Rhy", "Spe\n" + (2 - tower.rhythm));
		DisplayTowerCharacteristic("Sell", "Sell\n+" + tower.sellingPrice);
		GameObject.Find("btnSell").GetComponent<Image>().enabled = true;

		Tower towerUp = null;
		if (tower.towerUp != null) {
			Transform sphere = FindSphereInChildren (tower.towerUp.transform);
			if (sphere != null) {
				towerUp = sphere.GetComponent<Tower> () as Tower;
			}

			if (towerUp) {
				DisplayTowerCharacteristic ("Up", "Up\n-" + towerUp.cost);

				GameObject gameObjectUp = GameObject.Find ("btnUp");
				gameObjectUp.GetComponent<Image> ().enabled = true;
				Button btnUp = gameObjectUp.GetComponent<Button> () as Button;
				btnUp.interactable = gold >= towerUp.cost;
			}
		}
		else
		{
			HideBtnUp ();
		}
	}

	static private Transform FindSphereInChildren(Transform objectToInspect)
	{
		Transform toReturn = null;
		foreach (Transform child in objectToInspect)
		{
			if (child.name == "Sphere") {
				return child;
			}
			else
			{
				toReturn = FindSphereInChildren(child);
				if(toReturn != null)
				{
					return toReturn;
				}
			}
		}
		return toReturn;
	}

	static private void DisplayTowerCharacteristic(string objectName, string textToSet)
	{
		Text text = GameObject.Find(objectName).GetComponent<Text>();
		text.text = textToSet;
		text.enabled = true;
	}	

	static public void UnselectTower()
	{
		Destroy(selectionAura);
		selectionAura = null;
		selectedTower = null;
		selectedCoordX = -1;
		selectedCoordY = -1;
		HideTowerDetails();
	}

	static private void HideTowerDetails()
	{
		// background image
		GameObject towerSelectedDetails = GameObject.Find("TowerSelected");
		towerSelectedDetails.GetComponent<Image>().enabled = false;

		// texts
		Text[] texts = towerSelectedDetails.GetComponentsInChildren<Text>();
		foreach (Text text in texts)
		{
			text.enabled = false;
		}

		// buttons
		Image[] images = towerSelectedDetails.GetComponentsInChildren<Image>();
		foreach (Image img in images)
		{
			img.enabled = false;
		}
		GameObject gameObjectUp = GameObject.Find("btnSell");
		gameObjectUp.GetComponent<Image>().enabled = false;
		gameObjectUp.GetComponentInChildren<Text>().enabled = false;
		HideBtnUp ();
	}

	static private void HideBtnUp()
	{
		GameObject gameObjectSell = GameObject.Find("btnUp");
		gameObjectSell.GetComponent<Image>().enabled = false;
		gameObjectSell.GetComponentInChildren<Text>().enabled = false;
	}

	private void SellTower()
	{
		Tower tower = selectedTower.GetComponentInChildren<Tower>() as Tower;
		gold += tower.sellingPrice;
		GetComponent<AudioSource>().PlayOneShot(sellingSound);
		aStar.SetAstarGrid (selectedCoordX, selectedCoordY, false);

		// Display green quads if construction mode is activated
		if (step == Step.CONSTRUCTION)
		{
			foreach (GameObject floor in GameObject.FindGameObjectsWithTag("floor")) {
				Build build = floor.GetComponent ("Build") as Build;
				build.NotifyTowerChanged ();
			}
		}

		Destroy(selectedTower);
		selectedCoordX = -1;
		selectedCoordY = -1;
		selectedTower = null;
		HideTowerDetails();
	}

	private void UpTower()
	{
		Tower tower = selectedTower.GetComponentInChildren<Tower>() as Tower;
		GameObject goTowerUp = Instantiate(tower.towerUp, selectedTower.transform.position, Quaternion.identity) as GameObject;
		Destroy (selectedTower);

		foreach (GameObject floor in GameObject.FindGameObjectsWithTag("floor")) {
			Build build = floor.GetComponent ("Build") as Build;
			if (build.aStarCoordX == selectedCoordX && build.aStarCoordY == selectedCoordY)
			{
				build.towerBuilt = goTowerUp;
				break;
			}
		}

		SelectTower (goTowerUp, selectedCoordX, selectedCoordY);
		Tower towerUp = goTowerUp.GetComponentInChildren<Tower>() as Tower;
		gold -= towerUp.cost;
	}

	private void SkipCinematic()
	{
		animator.Play("Stationnary", -1, 0F);
		transform.position = camReferences[idCamRef].transform.position;
		transform.rotation = camReferences[idCamRef].transform.rotation;
		StartingConstructionTitle ();
		btnSkip.SetActive(false);
		btnCamera.SetActive(true);
	}

	private void Restart()
	{
		SceneManager.LoadScene ("towerDefense");
	}

	private void MainMenu()
	{
		SceneManager.LoadScene ("Menu");
	}

	private void MoveCamera()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Stationnary"))
		{
			if (idCamRef == camReferences.Count - 1)
			{
				idCamRef = 0;
			}
			else
			{
				idCamRef++;
			}
			animator.Stop ();
			transform.position = camReferences[idCamRef].transform.position;
			transform.rotation = camReferences[idCamRef].transform.rotation;
			animator.Rebind ();
		}
	}
}
