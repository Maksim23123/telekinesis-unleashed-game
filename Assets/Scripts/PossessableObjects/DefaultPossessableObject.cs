using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DefaultPossessableObject : PossessableObject
{
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void ProcessManipulation(Vector3 direction, float power)
    {
        if (TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(direction * power, ForceMode2D.Impulse);
        }
    }

    public override float GetContactDamage()
    {
        float aditionalMultiplier = 1;

        if (Random.value < StatsStorage.CriticalHitChance)
        {
            aditionalMultiplier = StatsStorage.CriticalMultiplier;
        }
        return Vector2.Distance(Vector2.zero, _rigidbody.velocity) * _rigidbody.mass * StatsStorage.DamageMultiplier * aditionalMultiplier;
    }
}
