using UnityEngine;
using System.Collections;

public class Arena : MonoBehaviour {

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            Destroy(col.gameObject);
        }
    }
}
