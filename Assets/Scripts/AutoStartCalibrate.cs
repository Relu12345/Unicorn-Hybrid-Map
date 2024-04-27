using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AutoStartCalibrate : MonoBehaviour
{
    public Button buttonToClick;

    void Start()
    {

        if (buttonToClick.name.Equals("btnContinue"))
        {
            // Check if the button to click is assigned
            if (buttonToClick != null)
            {
                // Invoke the button click after a short delay
                StartCoroutine(WaitForSeconds(15f));
                Invoke("ClickButton", 0.1f);
            }
            else
            {
                Debug.LogError("Button to click is not assigned!");
            }
        }
        else
        {

            StartCoroutine(WaitForSeconds(8f));
            // Check if the button to click is assigned
            if (buttonToClick != null)
            {
                // Invoke the button click after a short delay
                Invoke("ClickButton", 0.1f);
            }
            else
            {
                Debug.LogError("Button to click is not assigned!");
            }
        }
        Debug.Log("IM HERE");
    }


IEnumerator WaitForSeconds(float seconds)
    {
        // Wait for the specified amount of time
        yield return new WaitForSeconds(seconds);

        // After waiting, do something
        Debug.Log("Waited for " + seconds + " seconds.");

        // You can add your code here to execute after the waiting period
    }

    void ClickButton()
    {
        // Simulate a button click
        buttonToClick.onClick.Invoke();
    }
}

