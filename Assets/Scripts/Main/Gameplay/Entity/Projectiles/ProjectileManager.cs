using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private ProjectileProperties _projectileProperties;

    public ProjectileProperties ProjectileProperties { get => _projectileProperties; }

    private void Awake()
    {
        if (_projectileProperties != null)
        {
            Invoke(nameof(PerformSelfDestroy), _projectileProperties.LifeTime);
        }
    }

    public void PerformSelfDestroy()
    {
        Destroy(gameObject);
    }

    public void Launch(Vector2 direction)
    {
        if (TryGetComponent(out Rigidbody2D rigidbody) && _projectileProperties != null)
        {
            if (Vector2.Angle(Vector2.down, direction) > 90)
                transform.rotation = Quaternion.Euler(Vector2.Angle(Vector2.right, direction) * new Vector3(0, 0, 1));
            else
                transform.rotation = Quaternion.Euler(Vector2.Angle(Vector2.left, direction) * new Vector3(0, 0, 1));

            rigidbody.AddForce(direction * _projectileProperties.ImpulsPower, ForceMode2D.Impulse);
        }
    }
}
