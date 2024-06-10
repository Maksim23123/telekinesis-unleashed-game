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
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerItemsManager>();
            }

            return _instance;
        }
    }

    [SerializeField]
    PlayerStatsHandler _playerStatsHandler;

    private readonly float _pickUpZoneRadius = 2.7f;

    [SerializeField]
    private LayerMask _itemLayers;

    bool _pickUpAllowed = true;

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
                _playerStatsHandler?.AddStatsModifier(itemScript.StatsModifier);
            }
            Destroy(firstAvailable);
        }
    }

    private void RestorPickUpAbility()
    {
        _pickUpAllowed = true;
    }
}
