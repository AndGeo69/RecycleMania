using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRatelimiter : MonoBehaviour
{
    public int targetFrameRate = 20;

    // Start is called before the first frame update
    void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }

    // Update is called once per frame
    void Update() {

        if (Application.targetFrameRate != targetFrameRate) {
            Application.targetFrameRate = targetFrameRate;
            Debug.Log("framerate updated");
        }

    }
}
