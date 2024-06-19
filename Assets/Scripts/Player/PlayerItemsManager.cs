using System.Collections.Generic;
using UnityEngine;

public class PlayerItemsManager : MonoBehaviour, IRecordable
{
    [SerializeField]
    private PlayerStatsHandler _playerStatsHandler;
    [SerializeField]
    private LayerMask _itemLayers;

    private readonly float _pickUpZoneRadius = 2.7f;
    private bool _pickUpAllowed = true;
    private List<ItemInfluenceReferenceSlot> _itemInfluenceReferenceSlots = new List<ItemInfluenceReferenceSlot>();
    private static PlayerItemsManager _instance;

    public int Priority => 0;

    public static PlayerItemsManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
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

    public void RequestPickUp()
    {
        if (_pickUpAllowed)
        {
            PickUpItem();
            _pickUpAllowed = false;
            Invoke(nameof(RestorPickUpAbility), 0.1f);
        }
    }

    public ObjectData GetObjectData()
    {
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
