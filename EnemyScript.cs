using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
  [SerializeField]private GameObject MaterialDrop;
  [SerializeField]private ParticleSystem PSPrefab;
  [SerializeField]private EnemyData enemyData;
  public string Name;   
  private int HitPoints, Damage, Bursts;    
  private float Speed, FireRate, ProjectileSpeed, CoolDown = 3; 
  private Sprite Model;
  private GameObject Projectile; 
  private Color ProjectileColor; 
  private Vector3 Scale;
  private Color newColor;
  private Rigidbody2D body;
  private int Level;
  private EnemyState currentState;
  private Vector3 PlayerPosition;
  public enum EnemyState 
  {
    Idle,
    Attack,
    Move,
    Avoid
  }

  private void TakeAction()
  {
    switch(currentState)
    {
      case EnemyState.Idle:
      break;

      case EnemyState.Attack:
      break;

      case EnemyState.Move:
      break;

      case EnemyState.Avoid:
      break;

      default:
      break;
    }

  }

  public void Initialize(int lvl)
  {
    PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    Name = enemyData.Name;  
    HitPoints = enemyData.HitPoints;
    Speed= enemyData.Speed;
    Model = enemyData.Model;  
    Bursts = enemyData.Bursts;
    Damage = enemyData.Damage;    
    FireRate = enemyData.FireRate;
    ProjectileSpeed = enemyData.ProjectileSpeed;
    Projectile = enemyData.Projectile;
    ProjectileColor = enemyData.color;
         
    SpriteRenderer spr = GetComponent<SpriteRenderer>();
    spr.sprite = Model; //Picks sprite from array                        
    PolygonCollider2D pgc = gameObject.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D; // Adds collider                      
    body = gameObject.GetComponent<Rigidbody2D>();
                                          
  }  

  private void Attack()
  {        
    GameObject gunfire = Instantiate (Projectile, transform.position, transform.rotation);          
    gunfire.GetComponent<EnemyProjectileScript>().Initialize(PlayerPosition, Damage, ProjectileSpeed, ProjectileColor);
    CoolDown += 60/FireRate;
  }

  void Awake()
  {
        
  }

  void Update()
  {
        
  }

  void OnCollisionEnter2D(Collision2D collision)
  {          
    if ((collision.gameObject.tag == "Enemy") & (collision.gameObject.name == "Asteroid"))
    {
      ParticleSystem Shards = Instantiate( PSPrefab, new Vector3( collision.GetContact(0).point.x , collision.GetContact(0).point.y , 1) , transform.rotation);                    
      Rigidbody2D other = collision.gameObject.GetComponent<Rigidbody2D>();
      transform.GetComponent<Rigidbody2D>().AddForce(collision.GetContact(0).normal * other.mass * 50f);     
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

}
