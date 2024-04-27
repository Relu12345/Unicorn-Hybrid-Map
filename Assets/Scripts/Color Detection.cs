using UnityEngine;

public class ColorDetection : MonoBehaviour
{
    public Texture2D image; // Assign the image in the Unity Editor
    public Color targetColor = Color.white; // Set the target color
    public float tolerance = 0.1f; // Color tolerance

    void Start()
    {
        float percentage = CalculateColorPercentage(image, targetColor, tolerance);
    }

    float CalculateColorPercentage(Texture2D texture, Color target, float tolerance)
    {
        Color[] pixels = texture.GetPixels();
        int matchCount = 0;

        foreach (Color pixel in pixels)
        {
            if (IsColorMatch(pixel, target, tolerance))
            {
                matchCount++;
            }
        }

        return (float)matchCount / pixels.Length * 100f;
    }

    bool IsColorMatch(Color pixel, Color target, float tolerance)
    {
        return (Mathf.Abs(pixel.r - target.r) < tolerance &&
                Mathf.Abs(pixel.g - target.g) < tolerance &&
                Mathf.Abs(pixel.b - target.b) < tolerance &&
                Mathf.Abs(pixel.a - target.a) < tolerance);
    }
}