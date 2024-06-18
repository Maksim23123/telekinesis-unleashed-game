using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item events/Empty Event")]
public class EmptyItemEvent : ItemEvent
{
    [SerializeField]
    string output;

    public override int Priority => throw new NotImplementedException();

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
