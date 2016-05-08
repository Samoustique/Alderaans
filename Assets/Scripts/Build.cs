using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class Build : MonoBehaviour {
    
    public bool hasTower;
    public GameObject towerToBuild, towerBuilt;
    public Text txtInfo;
	public Material selectedMaterial;
	public int aStarCoordX, aStarCoordY;
		
	private bool isBuildable;
	private Material basicQuadMaterial;
    private List<Build> builds;
	private Astar aStar;
    
    void Start()
    {
        builds = new List<Build>();
        foreach (GameObject floor in GameObject.FindGameObjectsWithTag("floor"))
        {
            builds.Add(floor.GetComponent("Build") as Build);
        }

        hasTower = false;
		basicQuadMaterial = GetComponent<Renderer>().sharedMaterial;
    }

	public void InitiateAstar(AstarSingleton aStarSingleton)
	{
		aStar = aStarSingleton.astar;
	}

    void OnMouseUp()
    {
		if (!EventSystem.current.IsPointerOverGameObject ())
		{
			// Display tower data
			GameManager.SelectUnselectTower (towerBuilt, aStarCoordX, aStarCoordY);
			if (isBuildable && towerToBuild && !towerBuilt) {
				towerBuilt = Instantiate (towerToBuild, transform.position, Quaternion.identity) as GameObject;
				Tower tower = towerBuilt.GetComponentInChildren<Tower> () as Tower;
				GameManager.gold -= tower.cost;
				aStar.SetAstarGrid (aStarCoordX, aStarCoordY, true);
				GetComponent<Renderer> ().sharedMaterial = basicQuadMaterial;
				RefreshBuildable (true, towerToBuild);
			}
		}
    }

   public void RefreshBuildable(bool isOn, GameObject towerToBuild)
    {
        foreach (Build build in builds)
        {
			build.towerToBuild = isOn ? towerToBuild : null;
			build.NotifyTowerChanged();
        }
    }

	public void NotifyTowerChanged()
    {
		isBuildable = aStar.IsFreeWayWithFakeTower (aStarCoordX, aStarCoordY);
		GetComponent<Renderer> ().sharedMaterial = isBuildable ? selectedMaterial : basicQuadMaterial;
    }

    public void NotifyDisable()
    {
		GetComponent<Renderer>().sharedMaterial = basicQuadMaterial;
    }
}
