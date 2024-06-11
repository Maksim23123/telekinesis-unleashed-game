using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item events/Empty Event")]
public class EmptyItemEvent : ItemEvent
{
    [SerializeField]
    string output;

    protected new bool _unionPermission = true;


    public override void ExecuteItemEvent()
    {
        Debug.Log(output);
    }

    public override bool Union(ItemEvent itemEvent)
    {
        itemEvent.ExecuteItemEvent();
        return true;
    }
}
