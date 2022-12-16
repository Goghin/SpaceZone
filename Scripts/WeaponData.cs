using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Gun", menuName="Weapon" )]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public string Name;

    [Header("Base stats")]
    public int Damage;   
    public float FireRate;
    public int Basecost;
    public float ProjectileSpeed;
    public int Hits;
    public float LifeTime;
    public float Force;
    

    [Header("Upgrade stats")]
    public int dmgPerLVL;   
    public float FRperLVL;
    public float MaxRPM;

    [Header("Projectile")]
    public GameObject Bullet; 
    public Color color; 
   
}
