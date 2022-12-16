using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : MonoBehaviour
{

    public int HitPoints;    
    [SerializeField]private GameObject MaterialDrop;
    [SerializeField]private ParticleSystem PSPrefab;
    [SerializeField]private Sprite[] SpriteArray;
    private Vector3 Scale;
    private Color newColor;
    private Rigidbody2D body;
    private EnemyManagerScript EnemyManager;
 
    public void Initialize(int Hp, float s, Vector2 dir, float scale, EnemyManagerScript em)
          {
            EnemyManager = em;
            SpriteRenderer spr = GetComponent<SpriteRenderer>();
            spr.sprite = SpriteArray[Random.Range(0, SpriteArray.Length)]; //Picks sprite from array                        
            newColor = new Color( Random.Range(.33f,.8f), Random.Range(.33f,.8f), Random.Range(.33f,.8f), 1.0f );
            spr.material.color = newColor;
            PolygonCollider2D pgc = gameObject.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D; // Adds collider           
            Scale = new Vector3(scale, scale, 1);
            transform.localScale += Scale; //Scales object         
            body = gameObject.GetComponent<Rigidbody2D>();
            body.AddForce(dir * s , ForceMode2D.Impulse); //Pushes object
            body.mass *= (1f+scale*3) * (1f+scale*3); //Scales mass            
            HitPoints = Hp;   
            gameObject.name = "Asteroid" ; 

          }  
  
    void OnCollisionEnter2D(Collision2D collision)
    {          
      if (collision.gameObject.tag == "Enemy")
        {
          ParticleSystem Shards = Instantiate( PSPrefab, new Vector3( collision.GetContact(0).point.x , collision.GetContact(0).point.y , 1) , transform.rotation);
          Shards.GetComponent<Renderer>().material.color = newColor;   
          Shards.transform.localScale = Scale; 
          Rigidbody2D other = collision.gameObject.GetComponent<Rigidbody2D>();
          transform.GetComponent<Rigidbody2D>().AddForce(collision.GetContact(0).normal * other.mass * 30f);     
          TakeDamage(1) ;                
        }

        if (collision.gameObject.tag == "Player")
        {                    
          Death();              
        }

        if (collision.gameObject.tag == "Projectile")
        {
          Gun killer = collision.gameObject.GetComponent<BulletScript>().FiredFrom;
          int dmg = collision.gameObject.GetComponent<BulletScript>().gunDamage;
          float force = collision.gameObject.GetComponent<BulletScript>().Force;
          transform.GetComponent<Rigidbody2D>().AddForce(collision.GetContact(0).normal * 200f*force);          
          ParticleSystem Shards = Instantiate( PSPrefab, new Vector3( collision.GetContact(0).point.x , collision.GetContact(0).point.y , 1) , transform.rotation);
          Shards.GetComponent<Renderer>().material.color = newColor;   
          Shards.transform.localScale = new Vector3(.13f,.13f,1); 
          TakeDamage(dmg , killer);        
        }
               
    }

    void OnTriggerEnter2D(Collider2D collision)
    {          
    if (collision.gameObject.tag == "Projectile")
        {
          Gun killer = collision.gameObject.GetComponent<BulletScript>().FiredFrom;
          int dmg = collision.gameObject.GetComponent<BulletScript>().gunDamage;                             
          ParticleSystem Shards = Instantiate( PSPrefab,  transform.position , transform.rotation);
          Shards.GetComponent<Renderer>().material.color = newColor;   
          Shards.transform.localScale = new Vector3(.13f,.13f,1); 
          TakeDamage(dmg , killer);        
        }
    }

    public void TakeDamage(int dmg)
    {      
      HitPoints -= dmg;     
      if (HitPoints < 1)
      {         
        Death();
      }
    }

    public void TakeDamage(int dmg, Gun killer)
    {      
      HitPoints -= dmg;     
      if (HitPoints < 1)
      {   
        killer.AddKill(this.name);
              
        Death();
      }
    }

     public void Death()
     {
        for (int i = 0; i < Random.Range(3,10); i++)
            {
            ParticleSystem Shards = Instantiate( PSPrefab, transform.position , transform.rotation);
            Shards.GetComponent<Renderer>().material.color = newColor;   
            Shards.transform.localScale = Scale * 2f; 
            }
        if (Random.Range(0,101)<=50)
          {
          GameObject lootdrop = Instantiate(MaterialDrop, transform.position, transform.rotation);        
          // Initialize(int amount )
          lootdrop.GetComponent<MaterialsScript>().Initialize( 1 ); //Different Amounts ??
          }

        
        Destroy(gameObject); 
     }

     public void OnDisable()
     {
      EnemyManager.EnemiesList.Remove(gameObject);
     }

}
