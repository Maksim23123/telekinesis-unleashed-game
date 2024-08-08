using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Projectile Properties")] 
public class ProjectileProperties : ScriptableObject
{
    [SerializeField] private float _damage;
    [SerializeField] private float _impulsPower;
    [SerializeField] private float _lifeTime;

    public float Damage { get => _damage; set => _damage = value; }
    public float ImpulsPower { get => _impulsPower; set => _impulsPower = value; }
    public float LifeTime { get => _lifeTime; set => _lifeTime = value; }
}
