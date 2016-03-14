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
		
	private bool isBuildable;
	private GameObject fakeObstacle, fakeMob;
	private Material basicQuadMaterial;
    private List<Build> builds;
    
    void Start()
    {
        builds = new List<Build>();
        foreach (GameObject floor in GameObject.FindGameObjectsWithTag("floor"))
        {
            builds.Add(floor.GetComponent("Build") as Build);
        }

        hasTower = false;
		fakeObstacle = GameObject.Find("fakeObstacle");
		fakeMob = GameObject.Find("fakeMob");
		basicQuadMaterial = GetComponent<Renderer>().sharedMaterial;
    }

    void OnMouseUp()
    {
        // Display tower data
        GameManager.SelectUnselectTower(towerBuilt);
        if (isBuildable && towerToBuild && !towerBuilt)
        {
            towerBuilt = Instantiate(towerToBuild, transform.position, Quaternion.identity) as GameObject;
            Tower tower = towerBuilt.GetComponentInChildren<Tower>() as Tower;
            GameManager.gold -= tower.cost;
			GetComponent<Renderer>().sharedMaterial = basicQuadMaterial;
            RefreshBuildable(true, towerToBuild, true);
        }
    }

    /** *********************** REFRESH BUILDABLE  *************************  */
    public void RefreshBuildable(bool isOn, GameObject towerToBuild, bool isThereMoneyPb)
    {
        StartCoroutine(RefreshBuildableCoroutine(isOn, towerToBuild, isThereMoneyPb));
    }

    private IEnumerator RefreshBuildableCoroutine(bool isOn, GameObject towerToBuild, bool isThereMoneyPb)
    {
        foreach (Build build in builds)
        {
            if (!build.hasTower)
            {
                build.towerToBuild = isOn ? towerToBuild : null;
                build.NotifyTowerChanged(isOn, isThereMoneyPb);
                yield return new WaitForSeconds(0.0001F);
            }
        }
    }
    /** *********************** REFRESH BUILDABLE  *************************  */

    /** *********************** CHECK BUILDABLE  *************************  */
    public void NotifyTowerChanged(bool isOn, bool isThereMoneyPb)
    {
        StartCoroutine(CheckBuildableCoroutine(isOn, isThereMoneyPb));
    }

    private IEnumerator CheckBuildableCoroutine(bool isOn, bool isThereMoneyPb)
    {
		Material materialToSet = basicQuadMaterial;
        NavMeshPath path = null;

        if (towerBuilt == null)
        {
            GameObject obstacle = null;
            if (isOn && towerToBuild != null)
            {
                obstacle = Instantiate(fakeObstacle, transform.position, Quaternion.identity) as GameObject;
                path = new NavMeshPath();
            }

            // wait 0sec = wait for the next frame
            yield return new WaitForSeconds(0);
            if (path != null)
            {
                NavMeshAgent agent = fakeMob.GetComponent<NavMeshAgent>();
                agent.CalculatePath(GameObject.Find("home").transform.position, path);
                // the way can be blocked
                if (path.status != NavMeshPathStatus.PathComplete)
                {
                    isBuildable = false;
                }
                else
                {
                    isBuildable = true;
					materialToSet = selectedMaterial;
                }
            }
            Destroy(obstacle);
        }
        else
        {
            isBuildable = false;
        }
		GetComponent<Renderer>().sharedMaterial = materialToSet;
    }
    /** *********************** CHECK BUILDABLE  *************************  */

    public void NotifyDisable()
    {
		GetComponent<Renderer>().sharedMaterial = basicQuadMaterial;
    }
}
