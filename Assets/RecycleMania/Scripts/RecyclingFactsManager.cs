using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecyclingFactsManager : MonoBehaviour
{
    public HUD hud;

    public float triggerChance = 0.3f; // Adjust the chance of triggering as needed


    public void showRandomFact() {

        if (!hud.Info.gameObject.activeInHierarchy) {
            string randomFact = RecyclingFacts.Facts[Random.Range(0, RecyclingFacts.Facts.Length)];
            // Display the fact on the canvas
            
            hud.Info.transform.Find("InfoContent").GetComponent<Text>().text = randomFact;
            hud.Info.gameObject.SetActive(true);
        }
        
    }
}
