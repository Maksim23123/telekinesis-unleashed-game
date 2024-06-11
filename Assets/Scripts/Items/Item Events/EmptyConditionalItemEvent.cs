using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item events/Empty conditional event")]
public class EmptyConditionalItemEvent : ItemEvent
{
    [SerializeField]
    string output;

    [SerializeField]
    float height = -9;

    public override void ExecuteItemEvent()
    {
        Debug.Log(output);
    }

    public override bool Union(ItemEvent itemEvent)
    {
        itemEvent.ExecuteItemEvent();
        return true;
    }

    public override bool CheckCondition()
    {
        return PlayerController.Instance.gameObject.transform.position.y < height;
    }
}
