using System;
using UnityEngine;

public class DoorInteractable : InteractableObject
{
    [SerializeField] GameObject _closeState;
    [SerializeField] GameObject _openState;

    private bool _open = false;

    public bool Open
    { 
        get => _open; 

        set
        {
            _open = value;
            UpdateObject();
        }
    }

    public override void Activate()
    {
        Open = !Open;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateObject();
    }

    private void UpdateObject()
    {
        _closeState.SetActive(!Open);
        _openState.SetActive(Open);
    }
}
