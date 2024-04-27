using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCompleteBehaviour : MonoBehaviour
{
    [SerializeField] List<GameObject> objects;
    [SerializeField] GameObject trainObj;

    public void EnableObjs()
    {
        trainObj.SetActive(false);
        foreach (GameObject obj in objects)
        {
            obj.SetActive(true);
        }
    }
}
