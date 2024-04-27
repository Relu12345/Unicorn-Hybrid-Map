using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

public class StreetViewAPI : MonoBehaviour
{
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
        currentLatitude = -13.1650709f;
        currentLongitude = -72.5447154f;
        GenerateCubemap(currentLatitude.ToString(), currentLongitude.ToString());
        addressInputField.onEndEdit.AddListener(OnAddressEndEdit);
    }

    private void OnAddressEndEdit(string address)
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


    private void GenerateCubemap(string latitude, string longitude)
    {
        StartCoroutine(GetStreetViewImage(latitude, longitude, 0, 0, fov)); // Front
        StartCoroutine(GetStreetViewImage(latitude, longitude, 90, 0, fov)); // Right
        StartCoroutine(GetStreetViewImage(latitude, longitude, 180, 0, fov)); // Back
        StartCoroutine(GetStreetViewImage(latitude, longitude, 270, 0, fov)); // Left
        StartCoroutine(GetStreetViewImage(latitude, longitude, 0, 90, fov)); // Up
        StartCoroutine(GetStreetViewImage(latitude, longitude, 0, -90, fov)); // Down

        StartCoroutine(WaitTime());
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
                    frontTex = texture;
                else if (pitch == 90)
                    upTex = texture;
                else if (pitch == -90)
                    downTex = texture;
                break;
            case 90:
                leftTex = texture;
                break;
            case 180:
                backTex = texture;
                break;
            case 270:
                rightTex = texture;
                break;
        }
    }

    private IEnumerator WaitTime()
    {
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
    }

    // Call this function when the North button is pressed
    public void MoveNorth()
    {
        MoveInDirection(0, 5); // 0 degrees heading (North), 5 meters forward
    }

    // Call this function when the South button is pressed
    public void MoveSouth()
    {
        MoveInDirection(180, 5); // 180 degrees heading (South), 5 meters forward
    }

    // Call this function when the East button is pressed
    public void MoveEast()
    {
        MoveInDirection(90, 5); // 90 degrees heading (East), 5 meters forward
    }

    // Call this function when the West button is pressed
    public void MoveWest()
    {
        MoveInDirection(270, 5); // 270 degrees heading (West), 5 meters forward
    }

    // Function to move in a specific direction by a certain distance
    private void MoveInDirection(float heading, float distance)
    {
        // Convert latitude and longitude to float
        float lat = currentLatitude;
        float lon = currentLongitude;

        // Calculate new latitude and longitude based on the direction and distance
        float newLat = lat + (distance / 111111); // Approximately 1 degree latitude = 111111 meters
        float newLon = lon;

        if (heading == 0 || heading == 180)
        {
            // Moving North or South changes longitude
            newLon = lon + (distance / (111111 * Mathf.Cos(lat * Mathf.Deg2Rad)));
        }
        else if (heading == 90 || heading == 270)
        {
            // Moving East or West changes latitude
            newLon = lon + (distance / (111111 * Mathf.Cos(lat * Mathf.Deg2Rad)));
        }

        // Call GenerateCubemap with new latitude and longitude
        GenerateCubemap(newLat.ToString(), newLon.ToString());
    }
}
