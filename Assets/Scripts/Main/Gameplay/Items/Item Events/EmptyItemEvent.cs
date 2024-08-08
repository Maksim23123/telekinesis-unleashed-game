using UnityEngine;

/// <summary>
/// An example class for item events, not intended for use in actual game logic.
/// </summary>
[CreateAssetMenu(menuName = "Item events/Empty Event")] 
public class EmptyItemEvent : ItemEvent
{
    [SerializeField] private string output;

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
