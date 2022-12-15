using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerScript : MonoBehaviour
{
    
    [SerializeField] private GameObject  Enemy;
    [SerializeField] private float SpawnCooldown;
    private int AsteroidLevel=1;
      
    void Start()
    {
        SpawnCooldown = 5f;

    }



    // Update is called once per frame
    void Update()
    {         
        SpawnCooldown -= Time.deltaTime;      
        if (SpawnCooldown < 0)
        {   
            SpawnCooldown += Random.Range(2f,5f);
            SpawnAsteroids(10);
        }
    }



    void SpawnAsteroids(int amount)
    {
        for (int i = 0; i < amount; i++) 
            {            
                GameObject enemy = Instantiate(Enemy, new Vector3(Random.Range( -13f , 13f ), Random.Range( 8f , 12f ) ,0), Quaternion.Euler(0, 0, Random.Range(0,360))) ;
                
                int hp = 8 + (7*AsteroidLevel) ;
                float speed = 50 + Random.Range( 20f, 100f );
                Vector2 direction = new Vector2(Random.Range( -1.6f , 1.6f ), Random.Range(-2f,-3f));
                float scale = Random.Range( .075f , .18f );

                enemy.GetComponent<AsteroidScript>().Initialize(hp, speed, direction, scale);
                Collider2D coll = enemy.GetComponent<Collider2D>();
                
                if (coll.IsTouching(new ContactFilter2D().NoFilter())) 
                    {
                        Destroy(enemy);
                        SpawnAsteroids(1);
                        Debug.Log("Collision, rety");
                    }
            }                  
    }



}
