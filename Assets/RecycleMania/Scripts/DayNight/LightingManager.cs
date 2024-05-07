using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    [SerializeField, Range(0f, 10f)] private float timeScale = 0.14f; // Add a field to control the time scale


    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {

            //(Replace with a reference to the game time)
            TimeOfDay += Time.deltaTime * timeScale * TimeScaleFunction(); // Multiply by timeScale and the time scale function
            TimeOfDay %= 24; // Modulus to ensure always between 0-24
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }

    private float TimeScaleFunction()
    {
        // Use a sine function to create a smooth oscillation for time progression
        // Adjust the parameters of the sine function to control the time progression curve
        float period = 2f * Mathf.PI; // Full period of the sine function (in hours)
        float phaseShift = 0.5f * Mathf.PI; // Phase shift to align the peak with the middle of the day
        float amplitude = 0.5f; // Amplitude of the sine function
        float timeOfDayNormalized = TimeOfDay / 24f; // Normalize time to range [0, 1]
        float timeScaleFactor = amplitude * Mathf.Sin((timeOfDayNormalized * period) + phaseShift) + 1f; // Calculate time scale factor
        return timeScaleFactor;
    }


    private void UpdateLighting(float timePercent)
    {
        //Set ambient and fog
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

    }

    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}