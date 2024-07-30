using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int _itemID;
    [SerializeField] private PlayerStatsStorage _statsModifier;
    [SerializeField] private ItemEvent _itemEvent;

    public PlayerStatsStorage StatsModifier { get => _statsModifier; set => _statsModifier = value; }
    public ItemEvent ItemEvent { get => _itemEvent; }
    public int ItemID { get => _itemID; set => _itemID = value; }
}
