/// <summary>
/// This enum shows how ItemEvent should be executed by ItemIventExecutor.
/// </summary>
public enum ItemEventType
{
    /// <summary>
    /// Type of ItemEvent that suposed to be executed only one time.
    /// </summary>
    OneTime,
    /// <summary>
    /// Type of ItemEvent that supposed to be executed continuously under certain conditions.
    /// </summary>
    Conditional,
    /// <summary>
    /// Type of ItemEvent that supposed to be executed only one time under certain conditions.
    /// </summary>
    ConditionalOneTime
}
