using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NecazManager : MonoBehaviour
{
    public StreetViewAPI _streetViewAPI;
    public TMP_InputField necaz_de_input;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            necaz_de_input.text = "Bucharest Romania";
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            necaz_de_input.text = "Vienna";
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            necaz_de_input.text = "Macchu Picchu";
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Speech2Text.instance.StartRecording();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            _streetViewAPI.OnAddressEndEdit(necaz_de_input.text);

            GePeTo_Integration.instance.OnSubmitButtonClicked();
            Speech2Text.instance.SendRecording();
        }
    }
}
