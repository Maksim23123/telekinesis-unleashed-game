/// <summary>
/// Specifies how an ItemEvent should be executed by ItemEventExecutor.
/// </summary>
public enum ItemEventType
{
    /// <summary>
    /// ItemEvent that is executed only once.
    /// </summary>
    OneTime,
    /// <summary>
    /// ItemEvent that is executed continuously under certain conditions.
    /// </summary>
    Conditional,
    /// <summary>
    /// ItemEvent that is executed only once under certain conditions.
    /// </summary>
    ConditionalOneTime
}
