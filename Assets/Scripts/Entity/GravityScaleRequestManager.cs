/// <summary>
/// A utility class for sending requests to <see cref="GravityScaleManager"/>.
/// </summary>
/// <remarks>
/// Requests can be activated or deactivated by changing the <c>RequestIsActive</c> property.
/// </remarks>
public class GravityScaleRequestManager
{
    private bool _requestIsActive;
    private int _requestId;
    private float _value;
    private int _priority;
    private GravityScaleManager _gravityScaleManager;

    public float Value { get => _value; set => _value = value; }
    public int Priority { get => _priority; set => _priority = value; }

    public bool RequestIsActive
    {
        get => _requestIsActive;

        set
        {
            if (_gravityScaleManager != null && _requestIsActive != value)
            {
                if (value)
                {
                    _requestIsActive = true;
                    _gravityScaleManager.AddGravityValue(_value, out _requestId, _priority);
                }
                else
                {
                    _requestIsActive = false;
                    _gravityScaleManager.RemoveGravityValue(_requestId);
                }
            }
        }
    }

    public GravityScaleRequestManager(GravityScaleManager gravityScaleManager, float value, int priority)
    {
        _gravityScaleManager = gravityScaleManager;
        Value = value;
        Priority = priority;
    }
}
