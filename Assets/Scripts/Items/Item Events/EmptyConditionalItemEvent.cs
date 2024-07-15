using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Item events/Empty conditional event")] 
public class EmptyConditionalItemEvent : ItemEvent
{
    [SerializeField] private string output;
    [SerializeField] private float height = -9;

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
        if (PlayerStatusInformer.PlayerGameObject != null && !PlayerStatusInformer.PlayerGameObject.IsDestroyed())
            return PlayerStatusInformer.PlayerGameObject.transform.position.y < height;
        else
            return false;
    }
}
