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

    public PlayerStatsStorage StatsModifier { get => _statsModifier; set => _statsModifier = value; }
}
