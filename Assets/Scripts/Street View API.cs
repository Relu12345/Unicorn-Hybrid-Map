using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using Meta.WitAi;

public class StreetViewAPI : MonoBehaviour
{
    /*
     * Blonda ,sexy, fundul mare, se intoarce, este mane.
     */
    [System.Serializable]
    private class GeocodeResponse
    {
        public string lat;
        public string lon;
    }

    // For deserializing json
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{\"array\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

    [SerializeField] TMP_InputField addressInputField;
    [SerializeField] Material skyboxMaterial;
    [SerializeField] string mapsApiKey;
    [SerializeField] string geocodeApiKey;

    private int width = 640;
    private int height = 640;
    private int fov = 90;
    private Texture2D frontTex, leftTex, rightTex, backTex, upTex, downTex;

    private float currentLatitude, currentLongitude;

    private void Start()
    {
        currentLatitude = 45.758276f;
        currentLongitude = 21.228940f;
        GenerateCubemap(currentLatitude.ToString(), currentLongitude.ToString());
        addressInputField.onEndEdit.AddListener(OnAddressEndEdit);
    }

    public void OnAddressEndEdit(string address)
    {
        StartCoroutine(GetGeocodeAndGenerateCubemap(address));
    }

    private IEnumerator GetGeocodeAndGenerateCubemap(string address)
    {
        string geocodeUrl = "https://geocode.maps.co/search?q=" + address + "&api_key=" + geocodeApiKey;

        UnityWebRequest geocodeRequest = UnityWebRequest.Get(geocodeUrl);
        yield return geocodeRequest.SendWebRequest();

        if (geocodeRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching geocode data: " + geocodeRequest.error);
            yield break;
        }

        // Deserialize the JSON response manually
        string jsonResponse = geocodeRequest.downloadHandler.text;
        GeocodeResponse[] geocodeDataArray = JsonHelper.FromJson<GeocodeResponse>(jsonResponse);

        if (geocodeDataArray.Length == 0)
        {
            Debug.LogError("No geocode data found for the address: " + address);
            yield break;
        }

        currentLatitude = float.Parse(geocodeDataArray[0].lat);
        currentLongitude = float.Parse(geocodeDataArray[0].lon);

        GenerateCubemap(currentLatitude.ToString(), currentLongitude.ToString());
    }


    IEnumerator Front;
    IEnumerator Right;
    IEnumerator Back;
    IEnumerator Left;
    IEnumerator Up;
    IEnumerator Down;
    IEnumerator Time;
    bool firstRun = true;
    private void GenerateCubemap(string latitude, string longitude)
    {
       
            Front = GetStreetViewImage(latitude, longitude, 0, 0, fov); // Front
            Right = GetStreetViewImage(latitude, longitude, 90, 0, fov); // Right
            Back = GetStreetViewImage(latitude, longitude, 180, 0, fov); // Back
            Left = GetStreetViewImage(latitude, longitude, 270, 0, fov); // Left
            Up = GetStreetViewImage(latitude, longitude, 0, 90, fov); // Up
            Down = GetStreetViewImage(latitude, longitude, 0, -90, fov); // Down
            Time = WaitTime();
         
           
        StartCoroutine(Front);
        StartCoroutine(Right);
        StartCoroutine(Back);
        StartCoroutine(Left);
        StartCoroutine(Up);
        StartCoroutine(Down);
        StartCoroutine(Time);


    }


    private IEnumerator GetStreetViewImage(string latitude, string longitude, int heading, int pitch, int fov)
    {
        string url = "https://maps.googleapis.com/maps/api/streetview?size=" + width +
            "x" + height + "&location=" + latitude + "," + longitude + "&heading=" +
            heading + "&pitch=" + pitch + "&fov=" + fov + "&key=" + mapsApiKey;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching street view image: " + www.error);
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        switch (heading)
        {
            case 0:
                if (pitch == 0)
                {
                    //frontTex.DestroySafely();
                    frontTex = texture;
                }
                else if (pitch == 90)
                {
                    //upTex.DestroySafely();
                    upTex = texture;
                }
                else if (pitch == -90)
                {
                    //downTex.DestroySafely();
                    downTex = texture; }
                break;
            case 90:
                {
                    //leftTex.DestroySafely();
                    leftTex = texture;
                    break;
                }
            case 180:
                {
                   //backTex.DestroySafely();
                    backTex = texture;
                    break;
                }
            case 270:
                {
                   //rightTex.DestroySafely();
                    rightTex = texture;
                    break;
                }
        }
        
        www.Dispose();
     
    }

    private IEnumerator WaitTime()
    {
        StopCoroutine(Right);
        StopCoroutine(Back);
        StopCoroutine(Left);
        StopCoroutine(Up);
        StopCoroutine(Down);
        System.GC.Collect();

        yield return new WaitForSeconds(1f);
        
       


        SetSkybox();
    }

    private void SetSkybox()
    {
        skyboxMaterial.SetTexture("_FrontTex", frontTex);
        skyboxMaterial.SetTexture("_BackTex", backTex);
        skyboxMaterial.SetTexture("_LeftTex", leftTex);
        skyboxMaterial.SetTexture("_RightTex", rightTex);
        skyboxMaterial.SetTexture("_UpTex", upTex);
        skyboxMaterial.SetTexture("_DownTex", downTex);

        RenderSettings.skybox = skyboxMaterial;

       //bool status = true;
    }

    // Call this function when the North button is pressed
    public void MoveNorth()
    {
        Debug.Log("[STREET] Selected North");
        MoveInDirection(0, 5); // 0 degrees heading (North), 5 meters forward
    }

    // Call this function when the South button is pressed
    public void MoveSouth()
    {
        Debug.Log("[STREET] Selected South");
        MoveInDirection(180, 5); // 180 degrees heading (South), 5 meters forward
    }

    // Call this function when the East button is pressed
    public void MoveEast()
    {
        Debug.Log("[STREET] Selected East");
        MoveInDirection(90, 5); // 90 degrees heading (East), 5 meters forward
    }

    // Call this function when the West button is pressed
    public void MoveWest()
    {
        Debug.Log("[STREET] Selected West");
        MoveInDirection(270, 5); // 270 degrees heading (West), 5 meters forward
    }

    private bool canGenerateCubemap = true; // Flag to check if generating cubemap is allowed

    private IEnumerator CubemapCooldown()
    {
        canGenerateCubemap = false;
        yield return new WaitForSeconds(3f);
        canGenerateCubemap = true;
    }

    private void MoveInDirection(float heading, float distance)
    {
        // Check if generating cubemap is allowed
        if (!canGenerateCubemap)
        {
            return;
        }

        // Convert latitude and longitude to float
        float lat = currentLatitude;
        float lon = currentLongitude;

        // Calculate the new latitude and longitude based on the heading
        switch (heading)
        {
            case 0: // North
                lat += 0.00005f * distance; // Move north by adding a small value to latitude
                break;
            case 90: // East
                lon += 0.00005f * distance; // Move east by adding a small value to longitude
                break;
            case 180: // South
                lat -= 0.00005f * distance; // Move south by subtracting a small value from latitude
                break;
            case 270: // West
                lon -= 0.00005f * distance; // Move west by subtracting a small value from longitude
                break;
            default:
                Debug.LogError("Invalid heading.");
                return;
        }

        Debug.Log($"lat: {lat}; lon: {lon}");

        // Generate cubemap for the new location
        GenerateCubemap(lat.ToString(), lon.ToString());

        // Start the cooldown coroutine
        StartCoroutine(CubemapCooldown());
    }
}