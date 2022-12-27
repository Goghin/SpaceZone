using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManagerScript : MonoBehaviour
{
    
    [SerializeField] private GameObject  Asteroid, Drone, AssaultDrone, SniperDrone, FighterDrone, BossDrone;
    public List<GameObject> EnemiesList = new List<GameObject>(); 
    [SerializeField] private float SpawnCooldown;
    
    //private GameObject CurrentSpawn;
      
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
            SpawnCooldown += Random.Range(3f,6f);
            int r = Random.Range(1,101);
            //Debug.Log("Rolled "+r);
            if (r<=1)
            {
               SpawnDrones(BossDrone, 1);
                //Debug.Log("Boss drone");
            }
            if ((r>1)&(r<=5))
            {
               SpawnDrones(SniperDrone, Random.Range(1,4));
                //Debug.Log("Sniper drone");
            }
            if ((r>5) & (r<=15))
            {
               SpawnDrones(FighterDrone, Random.Range(1,3));
                //Debug.Log("Kamikaze drone");
            }
            if ((r>15) & (r<=22))
            {
               SpawnDrones(AssaultDrone, Random.Range(1,4));
                //Debug.Log("Assault drone");
            }
            if ((r>22) & (r<=50))
            {
                SpawnDrones(Drone, Random.Range(1,5));
                // Debug.Log("Drone"); 
            }
            Debug.Log(ReportEnemyList()); 
            if ((r>50) & (r<=100))
            {
                SpawnAsteroids(Random.Range(3,9));
                //Debug.Log("Asteroids");
            }
            Debug.Log(ReportEnemyList());         
        }
    }



    void SpawnAsteroids(int amount)
    {
        for (int i = 0; i < amount; i++) 
            {            
                GameObject asteroid = Instantiate(Asteroid, new Vector3(Random.Range( -13f , 13f ), Random.Range( 8f , 12f ) ,0), Quaternion.Euler(0, 0, Random.Range(0,360))) ;
                
                int hp = 6 + (Random.Range(5,15)* LevelMulti()) ;
                float speed = 50 + Random.Range( 20f, 100f );
                Vector2 direction = new Vector2(Random.Range( -1.6f , 1.6f ), Random.Range(-2f,-3f));
                float scale = Random.Range( .075f , .18f );

                asteroid.GetComponent<AsteroidScript>().Initialize(hp, speed, direction, scale, this);
                Collider2D coll = asteroid.GetComponent<Collider2D>();
                
                if (coll.IsTouching(new ContactFilter2D().NoFilter())) 
                    {
                        
                        Destroy(asteroid);
                        SpawnAsteroids(1);
                        Debug.Log("Collision, retry");
                    }
                EnemiesList.Add(asteroid);
            }       
    }

    

     void SpawnDrones(GameObject enemyPrefab, int amount)
    {
        for (int i = 0; i < amount; i++) 
            {            
                GameObject enemy = Instantiate(enemyPrefab, new Vector3(Random.Range( -12f , 12f ), Random.Range( 8f , 9f ) ,0), Quaternion.Euler(0, 0, Random.Range(0,360))) ;
                
                
                enemy.GetComponent<EnemyScript>().Initialize(LevelMulti(), this); // Level
                Collider2D coll = enemy.GetComponent<Collider2D>();
                
                if (coll.IsTouching(new ContactFilter2D().NoFilter())) 
                    {
                        Destroy(enemy);
                        SpawnDrones(enemyPrefab, 1);
                        Debug.Log("Collision, retry");
                    }
                EnemiesList.Add(enemy);    
            }                              
    }

    public string ReportEnemyList()  
    {
        string result = "Enemies: " + EnemiesList.Count ;
        for (int i = 0; i < EnemiesList.Count; i++)
        {   
            string c = " "+ EnemiesList[i].name +",";
            result += c;            
        }
        return result;
    }    

    private int LevelMulti()
    {
        double x = PlayerStats.Instance.engine.GetDistance();
        x /= 1.115; 
        int result = Mathf.FloorToInt( Mathf.Log((float)x, 6.5f) );
        return result;

    }

}
