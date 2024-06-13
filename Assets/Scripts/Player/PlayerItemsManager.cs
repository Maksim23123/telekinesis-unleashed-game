using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemsManager : MonoBehaviour
{
    private static PlayerItemsManager _instance;

    public static PlayerItemsManager Instance
    {
        get
        {
            return _instance;
        }
    }

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
}
