using UnityEngine;

[CreateAssetMenu(fileName = "ApiConfig", menuName = "Configuration/API")]
public class ApiConfig : ScriptableObject
{
    public string apiKey;

    public string get() { return apiKey; }
}
