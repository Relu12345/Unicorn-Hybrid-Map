using Gtec.UnityInterface;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Gtec.UnityInterface.BCIManager;

public class ClassSelectionAvailableExample : MonoBehaviour
{
    private uint _selectedClass = 0;
    private bool _update = false;
    private StreetViewAPI _streetViewAPI;

    [SerializeField] ERPFlashController3D _flashController;
    [SerializeField] Dictionary<int, Renderer> _selectedObjects;
    void Start()
    {
        _streetViewAPI = FindAnyObjectByType<StreetViewAPI>();
        //attach to class selection available event
        BCIManager.Instance.ClassSelectionAvailable += OnClassSelectionAvailable;

        //get selected objects
        _selectedObjects = new Dictionary<int, Renderer>();
        List<ERPFlashObject3D> applicationObjects = _flashController.ApplicationObjects;
        foreach(ERPFlashObject3D applicationObject in applicationObjects)
        {
            Renderer[] renderers = applicationObject.GameObject.GetComponentsInChildren<Renderer>();
            foreach(Renderer renderer in renderers)
            {
                if (renderer.name.Equals("selected"))
                    _selectedObjects.Add(applicationObject.ClassId, renderer);
            }
        }

        // disable/hide selected objects by default
        foreach(KeyValuePair<int, Renderer> kvp in _selectedObjects)
            kvp.Value.gameObject.SetActive(false);
    }

    void OnApplicationQuit()
    {
        //detach from class selection available event
        BCIManager.Instance.ClassSelectionAvailable -= OnClassSelectionAvailable;
    }

    void Update()
    {
        //TODO ADD YOUR CODE HERE
        if(_update)
        {
            foreach (KeyValuePair<int, Renderer> kvp in _selectedObjects)
                kvp.Value.gameObject.SetActive(false);

            if (_selectedClass > 0)
            {
                _selectedObjects[(int)_selectedClass].gameObject.SetActive(true);
                switch(_selectedClass)
                {
                    // North
                    case 1:
                        _streetViewAPI.MoveNorth();
                        break;
                    // West
                    case 2:
                        _streetViewAPI.MoveEast();
                        break;
                    // East
                    case 3:
                        _streetViewAPI.MoveWest();
                        break;
                    // South
                    case 4:
                        _streetViewAPI.MoveSouth();
                        break;
                }
            }

            _update = false;
        } 
    }

    /// <summary>
    /// This event is called whenever a new class selection is available. Th
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClassSelectionAvailable(object sender, EventArgs e)
    {
        ClassSelectionAvailableEventArgs ea = (ClassSelectionAvailableEventArgs)e;
       _selectedClass = ea.Class;
        _update = true;
        Debug.Log(string.Format("Selected class: {0}", ea.Class));
    }
}
