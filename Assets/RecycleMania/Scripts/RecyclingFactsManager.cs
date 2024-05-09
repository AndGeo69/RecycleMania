using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecyclingFactsManager : MonoBehaviour
{
    public HUD hud;

    public float triggerChance = 0.3f; // Adjust the chance of triggering as needed

    public void showRandomFact() {
        if (!hud.Info.gameObject.activeInHierarchy) {
            float randomValue = Random.value;
            if (randomValue <= triggerChance) {
                string randomFact = RecyclingFacts.Facts[Random.Range(0, RecyclingFacts.Facts.Length)];
                // Display the fact on the canvas
                
                Transform infoTransform = hud.transform.Find("Info");
                var infoContentText = infoTransform.Find("InfoContent").GetComponent<TextMeshProUGUI>();
                var infoContentTitleText = infoTransform.Find("InfoTitle").GetComponent<TextMeshProUGUI>();
                infoContentText.text = randomFact;
            
                hud.Info.gameObject.SetActive(true);

                StartCoroutine(RainbowTitleEffect(infoContentTitleText));
            }
        }
    }

    
    IEnumerator RainbowTitleEffect(TextMeshProUGUI titleText)
    {
        // Define rainbow colors
        Color[] rainbowColors = new Color[]
        {
            Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta
        };

        // Index to iterate through rainbow colors
        int colorIndex = 0;

        while (true)
        {
            // Change title text color
            titleText.color = rainbowColors[colorIndex];

            // Increment color index and wrap around if necessary
            colorIndex = (colorIndex + 1) % rainbowColors.Length;

            // Wait for a short duration before changing color again
            yield return new WaitForSeconds(0.2f);
        }
    }
}
