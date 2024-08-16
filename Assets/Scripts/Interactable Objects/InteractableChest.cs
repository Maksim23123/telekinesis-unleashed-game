using System;
using UnityEngine;

public class InteractableChest : InteractableObject
{
    [SerializeField] GameObject _closeState;
    [SerializeField] int _closeStateLayer;
    [SerializeField] GameObject _openState;
    [SerializeField] int _openStateLayer;

    [SerializeReference] DropTable _dropTable;

    private bool _used = false;

    public bool Used 
    { 
        get => _used; 

        set
        {
            _used = value;
            UpdateObject();
        }
    }

    public override void Activate()
    {
        if (!Used)
        {
            Used = true;
            DropItem();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateObject();
    }

    private void OnValidate()
    {
        const int MAX_LAYER_INDEX = 32;
        const int DEFAULT_LAYER = 0;
        if (_closeStateLayer > MAX_LAYER_INDEX)
        {

            _closeStateLayer = DEFAULT_LAYER;
        }

        if (_openStateLayer > MAX_LAYER_INDEX)
        {
            _openStateLayer = DEFAULT_LAYER;
        }
    }

    private void UpdateObject()
    {
        _closeState.SetActive(!Used);
        _openState.SetActive(Used);
        if (Used)
        {
            gameObject.layer = _openStateLayer;
        }
        else
        {
            gameObject.layer = _closeStateLayer;
        }
    }

    private void DropItem()
    {
        if (_dropTable != null)
        {
            GameObject dropPrefab = _dropTable.GetRandomDrop();
            Instantiate(dropPrefab, gameObject.transform.position, Quaternion.identity);
        }
    }
}
