using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

abstract public class Tower : MonoBehaviour {
    
    public GameObject bullet;
    public GameObject canon;
    public AudioClip canonSound;
    public float rhythm;
    public int power;
    public int cost;
	public string title;
	public int level;
	public int sellingPrice;
	public int range;
	public GameObject towerUp;
	
    private GameObject focusedMob;
    private List<GameObject> targetsStack;
    private float _rhythm;
	private bool isShootingAllowed = true;

    void Start()
    {
        init();
    }

    protected void init()
    {
        targetsStack = new List<GameObject>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Mob")
        {
            GameObject mobIn = col.gameObject.transform.gameObject;
            if(focusedMob == null)
            {
                focusedMob = mobIn;
            }
            else
            {
                targetsStack.Add(mobIn);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Mob")
        {
            GameObject mobOut = col.gameObject.transform.gameObject;
            if (mobOut != focusedMob)
            {
                targetsStack.Remove(mobOut);
            }
            nextTarget();
        }
    }

    private void nextTarget()
    {
        // get a new target
        if (targetsStack.Count > 0)
        {
            GameObject nextTarget = targetsStack[targetsStack.Count - 1];
            if (nextTarget != null)
            {
                focusedMob = nextTarget;
                _rhythm = rhythm;
            }
        }
    }

    protected void Fire(Vector3 target)
    {
        // Smoke
        SpreadSmoke();

        // Boom sound
        GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(canonSound);

        // Sending bullet
        GameObject _bullet = (GameObject) Instantiate(bullet, canon.transform.position, Quaternion.identity);
        PropulseBullet(_bullet, target);
    }

    virtual public void Update()
    {
		if (isShootingAllowed)
		{
			if (focusedMob != null) {
				// Smoke
				ManageSmoke ();

				// Canon
				Vector3 target = focusedMob.transform.position;
				transform.LookAt (target);

				// Fire timing
				_rhythm -= Time.deltaTime;
				if (_rhythm <= 0) {
					Fire (target);
					_rhythm = rhythm;
				}
			} else {
				nextTarget ();
			}
		}
    }

	public void NotifyStopShooting ()
	{
		isShootingAllowed = false;
	}

    abstract protected void PropulseBullet(GameObject _bullet, Vector3 target);
    abstract protected void ManageSmoke();
    abstract protected void SpreadSmoke();
}
