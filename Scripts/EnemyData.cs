using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Enemy", menuName="Enemy" )]
public class EnemyData : ScriptableObject
{   
    [Header("Info")]
    public string Name;

    [Header("Base stats")]
    public int HitPoints;
    public float Speed;
    public Sprite Model;

    [Header("Weapon Data")]
    public int Bursts;
    public int Damage;   
    public float FireRate;
    public float ProjectileSpeed;  
    public float Accuracy;
    public GameObject Projectile; 
    public Color color; 

    [Header("Sounds")]
    public AudioClip ShootSound;
    public AudioClip GetHitSound;
    public AudioClip DeathSound;


   
}
