using System.Collections;
using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    public float shakeDuration = 0.5f; // Duration of the shake effect
    public float shakeAmount = 10f; // Intensity of the shake
    public Transform objectToShake;

    private Vector3 originalPosition;

    void Start()
    {
        // Store the original position of the object
        originalPosition = objectToShake.localPosition;
    }

    public void TriggerShake()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Calculate a random position offset within the specified magnitude
            Vector3 randomPos = originalPosition + Random.insideUnitSphere * shakeAmount;

            // Apply the random position offset to the object
            objectToShake.localPosition = randomPos;

            // Increment the elapsed time
            elapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Reset the object to its original position
        objectToShake.localPosition = originalPosition;
    }
}
