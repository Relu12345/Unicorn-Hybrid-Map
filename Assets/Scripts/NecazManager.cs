using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NecazManager : MonoBehaviour
{
    public TMP_InputField necaz_de_input;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            necaz_de_input.text = "Macchu Picchu";
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Speech2Text.instance.StartRecording();
        }
    }
}
