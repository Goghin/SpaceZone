using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Loot", menuName="Loot" )]
public class LootData : ScriptableObject
{
    
    

    [Header("Info")]
    public string Name;

    [Header("Base stats")]
    public int Power;
    
    

    [Header("Data")]
    public AudioClip PickUpSound;
    public Sprite Model;
    

}
