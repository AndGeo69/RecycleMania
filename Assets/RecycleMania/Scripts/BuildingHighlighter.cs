using UnityEngine;

public class BuildingHighlighter : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Color maxHighlightColor = Color.green;
    public Color minHighlightColor = Color.cyan;
    public float maxBeamHeight = 20f;
    public float minBeamHeight = 5f;
    public float maxBeamWidth = 2f;
    public float minBeamWidth = 2f;
    public float distanceThreshold = 50f; // Adjust this value as needed

    // Color HexToColor(string hex)
    // {
    //     // Remove any leading "#" characters
    //     hex = hex.TrimStart('#');

    //     // Parse the hexadecimal color value to get the individual RGB components
    //     byte red = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
    //     byte green = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
    //     byte blue = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

    //     // Normalize the RGB values to the range [0, 1]
    //     float r = red / 255f;
    //     float g = green / 255f;
    //     float b = blue / 255f;

    //     // Create and return the Color object
    //     return new Color(r, g, b);
    // }

    private void Start()
    {
        // Create a new LineRenderer component
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Set line renderer properties
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = maxHighlightColor;
        lineRenderer.endColor = minHighlightColor;
        lineRenderer.startWidth = minBeamWidth;
        lineRenderer.endWidth = minBeamWidth;

        
    }

    private void Update()
    {
        if (lineRenderer != null) {
            float distanceToPlayer = Vector3.Distance(transform.position, Camera.main.transform.position);

            // Calculate the ratio based on the distance and the threshold
            float ratio = distanceToPlayer / distanceThreshold + 30;

            // Interpolate between the min and max values based on the ratio
            float beamHeight = Mathf.Lerp(minBeamHeight, maxBeamHeight, ratio);
            float beamWidth = Mathf.Lerp(minBeamWidth, maxBeamWidth, ratio);
            Color highlightColor = Color.Lerp(lineRenderer.endColor, lineRenderer.startColor, ratio);

            // Update the position, color, and size of the beam
            Vector3 startPoint = transform.position;
            Vector3 endPoint = startPoint + Vector3.up * beamHeight;
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);
            // lineRenderer.startColor = highlightColor;
            // lineRenderer.endColor = highlightColor;
            lineRenderer.startWidth = beamWidth;
            lineRenderer.endWidth = beamWidth;
        } else {
            Debug.Log("linerenderer is null ?");
        }
        // Get the distance between the player and the building
        
    }
}
