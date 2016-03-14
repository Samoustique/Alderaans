using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerLaser : Tower {

    private Dictionary<GameObject, float> laserToProceed;

    void Start()
    {
        laserToProceed = new Dictionary<GameObject, float>();
        init();
    }

    protected override void PropulseBullet(GameObject laser, Vector3 target)
    {
        laser.transform.LookAt(target);
        laserToProceed.Add(laser, Time.time);
    }
    
    public override void Update()
    {
        base.Update();

        if (laserToProceed.Count > 0)
        {
            List<GameObject> itemsToRemove = new List<GameObject>();

            foreach (KeyValuePair<GameObject, float> entry in laserToProceed)
            {
                GameObject laser = entry.Key;

                if (entry.Value + 1.0F > Time.time) // proceed laser
                    laser.transform.position += laser.transform.forward * power * Time.deltaTime;
                else // destroy laser
                    itemsToRemove.Add(laser);
            }            

            // Remove items
            foreach (GameObject laser in itemsToRemove)
            {
                Destroy(laser);
                laserToProceed.Remove(laser);
            }
        }
    }

    protected override void ManageSmoke()
    {
        // No Smoke with laser
    }

    protected override void SpreadSmoke()
    {
        // No Smoke with laser
    }
}
