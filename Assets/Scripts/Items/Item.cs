using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [SerializeField]
    int _itemID;

    [SerializeField]
    PlayerStatsStorage _statsModifier;

    [SerializeField]
    ItemEvent _itemEvent;

    public PlayerStatsStorage StatsModifier { get => _statsModifier; set => _statsModifier = value; }
    public ItemEvent ItemEvent { get => _itemEvent; }
    public int ItemID { get => _itemID; set => _itemID = value; }
}
