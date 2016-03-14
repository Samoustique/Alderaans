using UnityEngine;
using System.Collections;
using System;

public class TowerBullet : Tower {

    public GameObject smoke;
    
    private int smokeTimer;
    private GameObject _smoke;

    protected override void PropulseBullet(GameObject _bullet, Vector3 target)
    {
        _bullet.GetComponent<Rigidbody>().AddForce(transform.forward * power);
    }

    protected override void ManageSmoke()
    {
        if (--smokeTimer == 0)
            Destroy(_smoke);
        if (_smoke)
            _smoke.transform.position = canon.transform.position;
    }

    protected override void SpreadSmoke()
    {
        smokeTimer = 200;
        if (!_smoke)
            _smoke = (GameObject)Instantiate(smoke, canon.transform.position, Quaternion.identity);
    }
}
