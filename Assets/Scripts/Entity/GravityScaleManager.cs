using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void AddGravityValue(float value, out int slotId, int priority = 0)
    {
        slotId = StaticTools.GetFreeId(_gravityValueSlots, x => x.SlotId);
        _gravityValueSlots.Add(new GravityValueSlot(slotId, priority, value));
    }

    public void RemoveGravityValue(int slotId)
    {
        List<GravityValueSlot> slotsToRemove = _gravityValueSlots.Where(x => x.SlotId == slotId).ToList();
        foreach (var slot in slotsToRemove)
        {
            _gravityValueSlots.Remove(slot);
        }
    }
}
