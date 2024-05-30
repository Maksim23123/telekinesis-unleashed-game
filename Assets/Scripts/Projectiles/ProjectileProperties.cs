using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Projectile Properties")]
public class ProjectileProperties : ScriptableObject
{
    public float _damage;

    public float _impulsPower;

    public float _lifeTime;
}
