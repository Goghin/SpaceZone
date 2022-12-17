using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyScript : MonoBehaviour
{
  [SerializeField]private GameObject MaterialDrop;
  [SerializeField]private ParticleSystem PSHitEmit;
  [SerializeField]private EnemyData enemyData;

  private Vector3 Direction;
  private ParticleSystem PsAttack;  
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
  private GameObject Player;
  private float accuracy;
  private EnemyManagerScript EnemyManager;
  private int Ammo = 10; 

  public enum EnemyState  
  {
    Disturbed,
    Attacking,
    Moving,
    Avoiding,
    Retreating
  }


  private void TakeAction()
  {
    switch(currentState)
    {
      case EnemyState.Disturbed:
      CoolDown += 2f;  // do nothing for 2 seconds, then reset state ?? Adds 2s weapon cooldown....
      currentState = EnemyState.Attacking;  
      break;

      case EnemyState.Attacking:
      if(CoolDown<0)
        {        
          CoolDown += 60 / FireRate;           
          Ammo -=1;
          StartCoroutine(Fire());            
        }                     
      break;

      case EnemyState.Moving:
      Move();
      break;

      case EnemyState.Avoiding:      
      for ( int i=0; i<Random.Range(1,4); i++)
      {
        Move();
      }     
      break;

      case EnemyState.Retreating:
      Vector3 target = (Player.transform.position - transform.position);
      Direction = -target.normalized;      
      Move();
      break;

      default:
      currentState = EnemyState.Attacking;  
      break;
    }

  }

  IEnumerator ActionStarter()
  {
    for(;;)
    {
      //5 checks & actions per second
      CheckAttackRange();
      CheckIfInsideView();
      CheckNearest();
      TakeAction();
      yield return new WaitForSeconds(.2f);
    }
  }

  public void Initialize(int lvl, EnemyManagerScript em)
  {
    EnemyManager = em;
    Player = GameObject.FindGameObjectWithTag("Player");
    gameObject.name = enemyData.Name;  
    HitPoints = enemyData.HitPoints;
    Speed= enemyData.Speed;
    Model = enemyData.Model;  
    Bursts = enemyData.Bursts;
    Damage = enemyData.Damage;    
    FireRate = enemyData.FireRate;
    ProjectileSpeed = enemyData.ProjectileSpeed;
    Projectile = enemyData.Projectile;
    ProjectileColor = enemyData.color;
    accuracy = enemyData.Accuracy;
         
    SpriteRenderer spr = GetComponent<SpriteRenderer>();
    spr.sprite = Model;      

    PolygonCollider2D pgc = gameObject.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D; // Adds collider  based on sprite alpha                    
    body = gameObject.GetComponent<Rigidbody2D>();

    PsAttack = gameObject.GetComponentInChildren<ParticleSystem>();
    ParticleSystem.MainModule settings = PsAttack.main;
    settings.startColor = new ParticleSystem.MinMaxGradient( ProjectileColor );

    StartCoroutine(ActionStarter());
                                          
  }  

  IEnumerator Fire()
  {     
    for (int a = 1; a <= Bursts; a++)
      {
        float spreadX = 1 + Random.Range( (-100+accuracy)/100, (100-accuracy)/100 );
        float spreadY = 1 + Random.Range( (-100+accuracy)/100, (100-accuracy)/100 ); 
        Vector3 target = (Player.transform.position - transform.position);    
        target.x *= spreadX;
        target.y *= spreadY;   
        GameObject gunfire = Instantiate (Projectile, transform.position, transform.rotation);         
        gunfire.GetComponent<EnemyProjectileScript>().Initialize(target, Damage, ProjectileSpeed, ProjectileColor);
        PsAttack.Play();
        yield return new WaitForSeconds(.15f);
      }     
  }


  void Update()
  { 
    if (CoolDown > 0 )
    {
      CoolDown -= Time.deltaTime;
    }
  }

  void OnCollisionEnter2D(Collision2D collision)
  {          
    if ((collision.gameObject.tag == "Enemy") & (collision.gameObject.name == "Asteroid"))
    {
      ParticleSystem Shards = Instantiate( PSHitEmit, new Vector3( collision.GetContact(0).point.x , collision.GetContact(0).point.y , 1) , transform.rotation);  
      Shards.transform.localScale = new Vector3(.16f,.16f,1);                  
      Rigidbody2D other = collision.gameObject.GetComponent<Rigidbody2D>();
      transform.GetComponent<Rigidbody2D>().AddForce(collision.GetContact(0).normal * other.mass * 50f);     
      TakeDamage(1) ;  
      currentState = EnemyState.Disturbed;              
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
      ParticleSystem Shards = Instantiate( PSHitEmit, new Vector3( collision.GetContact(0).point.x , collision.GetContact(0).point.y , 1) , transform.rotation); 
      Shards.transform.localScale = new Vector3(.16f,.16f,1);         
      TakeDamage(dmg , killer);        
    }
               
  }

  void OnTriggerEnter2D(Collider2D collision)
  {          
    if (collision.gameObject.tag == "Projectile")
    {
      Gun killer = collision.gameObject.GetComponent<BulletScript>().FiredFrom;
      int dmg = collision.gameObject.GetComponent<BulletScript>().gunDamage;                             
      ParticleSystem Shards = Instantiate( PSHitEmit,  transform.position , transform.rotation);
      //Shards.GetComponent<Renderer>().material.color = newColor;   
      Shards.transform.localScale = new Vector3(.16f,.16f,1); 
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
    for (int i = 0; i < 3; i++)
    {
      ParticleSystem Shards = Instantiate( PSHitEmit, transform.position , transform.rotation);
      Shards.transform.localScale =  new Vector3(Random.Range(.16f,.3f),Random.Range(.16f,.3f),1);     
    }
    if (Random.Range(0,101)<=100)
    {
      GameObject lootdrop = Instantiate(MaterialDrop, transform.position, transform.rotation);        
      // Initialize(int amount )
      lootdrop.GetComponent<MaterialsScript>().Initialize( 1 ); //Different Amounts based on level??
    }
    Destroy(gameObject); 
  }

  public void OnDisable()
  {
    EnemyManager.EnemiesList.Remove(gameObject);

  }

  public void Move()
  {
    transform.GetComponent<Rigidbody2D>().AddForce(Direction * Speed * 200f); 
  }

  public void CheckAttackRange()
  {
    Vector3 target = (Player.transform.position - transform.position);
    if (target.sqrMagnitude < 5)
    {
      Direction = -target.normalized;
      currentState = EnemyState.Avoiding;
    }
    else if (target.sqrMagnitude < 60)
    {
      currentState = EnemyState.Attacking;
    }
    else  
    {
      Direction = target.normalized;
      currentState = EnemyState.Moving;     
    }  
    if (Ammo < 1)
        {
          Direction = -target.normalized;
          currentState = EnemyState.Retreating;     
        }      
  }  
    
  public void CheckNearest()
  {    
    GameObject current = null;
    float distance = 9999f;
    foreach(GameObject g in EnemyManager.EnemiesList)
    {
        float dist =   (g.transform.position - transform.position).sqrMagnitude;
        if ((dist < distance) & (g != gameObject ))
        {
          current = g;
          distance = dist;
        }    
    }
    if(( current != null) && (distance < 5f))
    {
      Direction = -(current.transform.position - gameObject.transform.position).normalized ;
      currentState = EnemyState.Avoiding;
      Debug.Log("Entered avoiding, " + distance + " from " + current);
    }        
  }

  private void CheckIfInsideView()
  {
    float posX = transform.position.x;
    float posY = transform.position.y;

    if(posX > 12.5f)
    {
      Direction = new Vector3 (-1, Direction.y, Direction.z );
      currentState = EnemyState.Moving;
    }
    if(posX < -12.5f)
    {
      Direction = new Vector3 (1, Direction.y, Direction.z );
      currentState = EnemyState.Moving;
    }
    if(posY > 6.5f)
    {
      Direction = new Vector3 (Direction.x, -1, Direction.z );
      currentState = EnemyState.Moving;
    }
    if(posY < -5f)
    {
      Direction = new Vector3 (Direction.x, 1, Direction.z );
      currentState = EnemyState.Moving;
    }
  }

}
