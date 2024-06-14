using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemsManager : MonoBehaviour, IRecordable
{
    private static PlayerItemsManager _instance;

    public static PlayerItemsManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public int Priority => 0;

    void Awake()
    {
        _instance = this;
    }

    [SerializeField]
    PlayerStatsHandler _playerStatsHandler;

    private readonly float _pickUpZoneRadius = 2.7f;

    [SerializeField]
    private LayerMask _itemLayers;

    bool _pickUpAllowed = true;

    List<ItemInfluenceReferenceSlot> _itemInfluenceReferenceSlots = new List<ItemInfluenceReferenceSlot>();

    public void RequestPickUp()
    {
        if (_pickUpAllowed)
        {
            PickUpItem();
            _pickUpAllowed = false;
            Invoke(nameof(RestorPickUpAbility), 0.1f);
        }
    }

    private void PickUpItem()
    {
        if (PlayerInteractionManager.Instance.TryGetAvailableInteractableObject(out GameObject firstAvailable, _itemLayers, _pickUpZoneRadius))
        {
            if (firstAvailable.TryGetComponent(out Item itemScript))
            {
                int statsModifierId = _playerStatsHandler.AddStatsModifier(itemScript.StatsModifier);
                int itemEventId = ItemEventsExecutor.Instance.AddItemEvent(itemScript.ItemEvent);
                _itemInfluenceReferenceSlots.Add(new ItemInfluenceReferenceSlot(StaticTools.GetFreeId(_itemInfluenceReferenceSlots, x => x.SlotId)
                    , itemScript.ItemID, statsModifierId, itemEventId));
            }
            Destroy(firstAvailable);
        }
    }

    private void RestorPickUpAbility()
    {
        _pickUpAllowed = true;
    }

    public ObjectData GetObjectData()
    {
        // CONTINUE HERE
        ObjectData objectData = new ObjectData();
        
        for (int i = 0; i < _itemInfluenceReferenceSlots.Count; i++)
        {
            objectData.objectDataUnits.Add(nameof(_itemInfluenceReferenceSlots) + i, _itemInfluenceReferenceSlots[i].GetObjectData());
        }

        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        _itemInfluenceReferenceSlots = new List<ItemInfluenceReferenceSlot>();
        int itemInfIndex = 0;
        while (objectData.objectDataUnits.TryGetValue(nameof(_itemInfluenceReferenceSlots) + itemInfIndex, out ObjectData slotData))
        {
            _itemInfluenceReferenceSlots.Add(ItemInfluenceReferenceSlot.RemakeItemInfluenceReferenceSlot(slotData));
            itemInfIndex++;
        }
    }
}
