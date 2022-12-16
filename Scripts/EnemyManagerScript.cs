using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerScript : MonoBehaviour
{
    
    [SerializeField] private GameObject  Asteroid, Drone, AssaultDrone;
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

            int r = Random.Range(1,101);
            Debug.Log("Rolled "+r);
            if (r<5)
            {
                SpawnAssaultDrones(Random.Range(1,3));
                Debug.Log("Assault drone");
            }
            if ((r>5) & (r<25))
            {
                SpawnDrones(Random.Range(1,4));
                Debug.Log("Drone");
            }
            if ((r>25) & (r<100))
            {
            SpawnAsteroids(Random.Range(3,12));
            Debug.Log("Asteroids");
            }      
        }
    }



    void SpawnAsteroids(int amount)
    {
        for (int i = 0; i < amount; i++) 
            {            
                GameObject asteroid = Instantiate(Asteroid, new Vector3(Random.Range( -13f , 13f ), Random.Range( 8f , 12f ) ,0), Quaternion.Euler(0, 0, Random.Range(0,360))) ;
                
                int hp = 6 + (Random.Range(5,15)*AsteroidLevel) ;
                float speed = 50 + Random.Range( 20f, 100f );
                Vector2 direction = new Vector2(Random.Range( -1.6f , 1.6f ), Random.Range(-2f,-3f));
                float scale = Random.Range( .075f , .18f );

                asteroid.GetComponent<AsteroidScript>().Initialize(hp, speed, direction, scale);
                Collider2D coll = asteroid.GetComponent<Collider2D>();
                
                if (coll.IsTouching(new ContactFilter2D().NoFilter())) 
                    {
                        Destroy(asteroid);
                        SpawnAsteroids(1);
                        Debug.Log("Collision, retry");
                    }
            }                  
    }

     void SpawnDrones(int amount)
    {
        for (int i = 0; i < amount; i++) 
            {            
                GameObject enemy = Instantiate(Drone, new Vector3(Random.Range( -12f , 12f ), Random.Range( 4f , 6f ) ,0), Quaternion.Euler(0, 0, Random.Range(0,360))) ;
                
                //int hp = 8 + (7*AsteroidLevel) ;
                //float speed = 50 + Random.Range( 20f, 100f );
                //Vector2 direction = new Vector2(Random.Range( -1.6f , 1.6f ), Random.Range(-2f,-3f));
                //float scale = Random.Range( .075f , .18f );

                enemy.GetComponent<EnemyScript>().Initialize(1); // Level
                Collider2D coll = enemy.GetComponent<Collider2D>();
                
                if (coll.IsTouching(new ContactFilter2D().NoFilter())) 
                    {
                        Destroy(enemy);
                        SpawnDrones(1);
                        Debug.Log("Collision, retry");
                    }
            }                  
    }

    void SpawnAssaultDrones(int amount)
    {
        for (int i = 0; i < amount; i++) 
            {            
                GameObject enemy = Instantiate(AssaultDrone, new Vector3(Random.Range( -12f , 12f ), Random.Range( 4f , 6f ) ,0), Quaternion.Euler(0, 0, Random.Range(0,360))) ;
                
                //int hp = 8 + (7*AsteroidLevel) ;
                //float speed = 50 + Random.Range( 20f, 100f );
                //Vector2 direction = new Vector2(Random.Range( -1.6f , 1.6f ), Random.Range(-2f,-3f));
                //float scale = Random.Range( .075f , .18f );

                enemy.GetComponent<EnemyScript>().Initialize(1); // Level
                Collider2D coll = enemy.GetComponent<Collider2D>();
                
                if (coll.IsTouching(new ContactFilter2D().NoFilter())) 
                    {
                        Destroy(enemy);
                        SpawnAssaultDrones(1);
                        Debug.Log("Collision, retry");
                    }
            }                  
    }


}
