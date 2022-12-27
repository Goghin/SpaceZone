using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrop : MonoBehaviour
{  [SerializeField]private GameObject audioPrefab;
   [SerializeField]private LootData[] Loot;
    private LootData lootData;
    public int Power;
    private AudioClip PickUpSound;   
    private SpriteRenderer spr;

public void Initialize() // add args (int level, int value)
{


    /*
        Loot[0] = Materials 1
        Loot[1] = Materials 3
        Loot[2] = Materials 5
        Loot[3] = Materials 10
        Loot[4] = hp
        Loot[5] = speed
        Loot[6] = weapon
        Loot[7] = shield
        


    */

    void DropCredits()
    {

    }

    void DropBoost()
    {
        
    }

    int r = Random.Range(0,101);
    switch (r)
    {
        case int when r is >= 0 and <= 50:  //Maneuverability boost
        lootData = Loot[0];
        break;

        case int when r is >= 51 and <= 75: //Maneuverability boost
        lootData = Loot[1];
        break;

        case int when r is >= 76 and <= 85:
        lootData = Loot[2];
        break;

        case int when r is >= 86 and <= 89:
        lootData = Loot[3];
        break;

        case int when r is >= 90 and <= 100:
        lootData = Loot[Random.Range(4,8)];
        if ( (PlayerStats.Instance.Shield == null) && (lootData == Loot[7]) )
        {
            lootData = Loot[0];
            Debug.Log("No shield");
        }
        break;

        default:
        lootData = Loot[0];
        break;
    }
    name = lootData.Name;
    Power = lootData.Power;
    PickUpSound = lootData.PickUpSound;
    spr = GetComponent<SpriteRenderer>();
    spr.sprite = lootData.Model; 
    PolygonCollider2D pgc = gameObject.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D;                
    pgc.isTrigger = true;
}



void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Finish")
        {
            Destroy(gameObject);
        }
        if(other.gameObject.tag == "Player" )
        {            
            GameObject clone = Instantiate(audioPrefab, transform.position, transform.rotation) as GameObject;
            AudioSource cloneAudio = clone.GetComponent<AudioSource>();
            cloneAudio.clip = PickUpSound;
            cloneAudio.Play();
            Destroy(clone, PickUpSound.length + 0.1f);
            
        }
    }
}
