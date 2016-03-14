using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {

    void Start()
    {
        AudioListener.pause = false;
    }

    public void toggleSound()
    {
        AudioListener.pause = GetComponent<UnityEngine.UI.Toggle>().isOn;
    }
}
