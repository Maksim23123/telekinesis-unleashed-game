using UnityEngine;

/// <summary>
/// Class that contains information required for an item to be processed by those who will pick it up.
/// Should be attached to every item in the game.
/// </summary>
public class Item : MonoBehaviour
{
    [SerializeField] private int _itemID;
    [SerializeField] private PlayerStatsStorage _statsModifier;
    [SerializeField] private ItemEvent _itemEvent;

    /// <summary>
    /// Stats modification values for the player who picks up the item.
    /// </summary>
    public PlayerStatsStorage StatsModifier { get => _statsModifier; set => _statsModifier = value; }

    /// <summary>
    /// Item event to be sent to the ItemEventExecutor class.
    /// Used to add custom effects to the item.
    /// </summary>
    public ItemEvent ItemEvent { get => _itemEvent; }
    public int ItemID { get => _itemID; set => _itemID = value; }
}
