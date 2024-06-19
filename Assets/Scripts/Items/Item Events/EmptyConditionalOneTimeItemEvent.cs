using UnityEngine;

[CreateAssetMenu(menuName = "Item events/Empty Cinditional One Time Event")]
public class EmptyConditionalOneTimeItemEvent : ItemEvent
{
    [SerializeField]
    private string output;

    [SerializeField]
    private float height = 10;

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
        return PlayerController.Instance.gameObject.transform.position.y > height;
    }
}
