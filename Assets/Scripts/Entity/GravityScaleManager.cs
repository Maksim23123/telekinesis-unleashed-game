using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the gravity scale of a GameObject. Requires a 
/// <see cref="Rigidbody2D"/> component attached to the same GameObject.
/// </summary>
/// <remarks>
/// This class is a utility for external logic intending to change the GameObject's gravity scale.
/// The <see cref="GravityScaleRequestManager"/> class is designed to easily send and manage gravity value requests.
/// If multiple values are sent, the one with the highest <c>priority</c> is chosen.
/// </remarks>
[RequireComponent(typeof(Rigidbody2D))] 
public class GravityScaleManager : MonoBehaviour
{
    [SerializeField] private bool _clampFallingSpeed = false;
    [SerializeField] private float _maxFallingSpeed = -20;

    private Rigidbody2D _rigidbody;
    private float _defaultGravityScale;
    private float _currentValue;
    private SortedSet<GravityValueSlot> _gravityValueSlots = new SortedSet<GravityValueSlot>(new GravityValueSlot(0, 0, 0));

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _defaultGravityScale = _rigidbody.gravityScale;
    }

    private void FixedUpdate()
    {
        UpdateGravityScale();
        if (_clampFallingSpeed)
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x
                    , Mathf.Clamp(_rigidbody.velocity.y, _maxFallingSpeed, float.MaxValue));
    }

    private void UpdateGravityScale()
    {
        if (_gravityValueSlots.Count > 0)
        {
            GravityValueSlot mostPriorSlot = _gravityValueSlots.Max;
            if (mostPriorSlot.Value != _currentValue)
                _currentValue = mostPriorSlot.Value;
        }
        else
            _currentValue = _defaultGravityScale;
        _rigidbody.gravityScale = _currentValue;
    }

    /// <summary>
    /// Allows to apply value to gavity values list.
    /// </summary>
    /// <param name="value">Gravity scale value.</param>
    /// <param name="slotId">ID under which value was added to list.</param>
    /// <param name="priority">Priority of value.</param>
    public void AddGravityValue(float value, out int slotId, int priority = 0)
    {
        slotId = StaticTools.GetFreeId(_gravityValueSlots, x => x.SlotId);
        _gravityValueSlots.Add(new GravityValueSlot(slotId, priority, value));
    }

    /// <summary>
    /// Allows to remove value from gavity values list.
    /// </summary>
    /// <param name="slotId">ID under which value was sent to this class.</param>
    public void RemoveGravityValue(int slotId)
    {
        List<GravityValueSlot> slotsToRemove = _gravityValueSlots.Where(x => x.SlotId == slotId).ToList();
        foreach (var slot in slotsToRemove)
        {
            _gravityValueSlots.Remove(slot);
        }
    }
}
