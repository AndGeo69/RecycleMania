using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;                         

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Dont destroy on reloading the scene
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        DestroyFallingObjects();
    }

    private void DestroyFallingObjects() {
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

            // Iterate through each Rigidbody
        foreach (Rigidbody rb in rigidbodies)
        {
            // Check if the object's Y position is below the threshold
            if (rb.transform.position.y < -20f)
            {
                // Remove the object
                Destroy(rb.gameObject);
            }
        }
    }

    public PlayerController Player;
    
}
