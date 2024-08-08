using UnityEngine;

/// <summary>
/// An example class for conditional item events that supposed to be executed only one time, 
/// not intended for use in actual game logic.
/// </summary>
[CreateAssetMenu(menuName = "Item events/Empty Cinditional One Time Event")] 
public class EmptyConditionalOneTimeItemEvent : ItemEvent
{
    [SerializeField] private string output;
    [SerializeField] private float height = 10;

    public override void ExecuteItemEvent()
    {
        Debug.Log(output);
        EventExecuted = true;
    }

    public override bool Union(ItemEvent itemEvent)
    {
        itemEvent.ExecuteItemEvent();
        return true;
    }

    public override bool CheckCondition()
    {
        if (PlayerStatusInformer.PlayerGameObject != null)
        {
            return PlayerStatusInformer.PlayerGameObject.transform.position.y > height;
        }
        return false;
    }
}
