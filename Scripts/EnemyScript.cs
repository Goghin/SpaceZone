using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyScript : MonoBehaviour
{
  [SerializeField]private GameObject lootDropPrefab;
  [SerializeField]private ParticleSystem PSHitEmit;
  [SerializeField]private EnemyData enemyData;

  public int HitPoints;

  private Vector3 Direction;
  private ParticleSystem PsAttack;  
  private int Damage, Bursts;    
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
  private ParticleSystem DeathParticles;
  private float MinRange = 5;
  private float MaxRange = 60;
  private SpriteRenderer spr;
  private PolygonCollider2D pgc;
  private bool IsDead = false;

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
    DeathParticles = enemyData.DeathParticles;
    Player = GameObject.FindGameObjectWithTag("Player");

    gameObject.name = enemyData.Name; 
    Model = enemyData.Model;  

    HitPoints = enemyData.HitPoints * lvl;
    Speed= enemyData.Speed + (float)lvl * .2f;
    ProjectileSpeed = enemyData.ProjectileSpeed + (float)lvl * .2f;
    Bursts = enemyData.Bursts + (lvl-1);
    Damage = (int) (enemyData.Damage * ((float)lvl * .5f));    
    FireRate = enemyData.FireRate + (float)lvl * .2f;
    
    Projectile = enemyData.Projectile;
    ProjectileColor = enemyData.color;
    
    accuracy = enemyData.Accuracy;
    MinRange = enemyData.MinRange;
    MaxRange = enemyData.MaxRange;
    Ammo = enemyData.Ammo;
         
    spr = GetComponent<SpriteRenderer>();
    spr.sprite = Model;      

    pgc = gameObject.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D; // Adds collider  based on sprite alpha                    
    body = gameObject.GetComponent<Rigidbody2D>();

    PsAttack = gameObject.GetComponentInChildren<ParticleSystem>();
    ParticleSystem.MainModule settings = PsAttack.main;
    settings.startColor = new ParticleSystem.MinMaxGradient( ProjectileColor );

    Debug.Log("Im a " + lvl + " " + name );

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
      Shards.transform.localScale = new Vector3(.25f,.25f,1);                  
      if( (body != null) && (collision.gameObject.GetComponent<Rigidbody2D>() != null))
      {
        body.AddForce(collision.GetContact(0).normal * collision.gameObject.GetComponent<Rigidbody2D>().mass * 50f); 
      }     
      TakeDamage(1) ;  
      currentState = EnemyState.Disturbed;              
    }

    if (collision.gameObject.tag == "Player")
    {                    
      //Death();              
    }

    if (collision.gameObject.tag == "Projectile")
    {
      Gun killer = collision.gameObject.GetComponent<BulletScript>().FiredFrom;
      int dmg = collision.gameObject.GetComponent<BulletScript>().gunDamage;
      float force = collision.gameObject.GetComponent<BulletScript>().Force;     
      if( body!=null)
      {
        body.AddForce(collision.GetContact(0).normal * body.mass * 2f *force);    
      }      
      ParticleSystem Shards = Instantiate( PSHitEmit, new Vector3( collision.GetContact(0).point.x , collision.GetContact(0).point.y , 1) , transform.rotation); 
      Shards.transform.localScale = new Vector3(.16f,.16f,1)* (1+(float)dmg/100);         
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
      Shards.transform.localScale = new Vector3(.16f,.16f,1)* (1+(float)dmg/100); 
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

  IEnumerator Iexplosions(int amount, float spread)
  {
    for (int i=0;i<amount;i++)
    {
      SpawnExplosion(spread);
      yield return new WaitForSeconds(.08f);
    }
    Destroy(gameObject);  
  }
  private void Death()
  {
    //Destroy(body);
    pgc.enabled = false;
    Ammo=0;
    Speed=0;
    spr.sprite = null;     
    if (name == "Boss Drone")
      {     
        StartCoroutine(Iexplosions(5, 1.2f));
        DropLoot(3, 1f);
      }
    else 
      {
        StartCoroutine(Iexplosions(1, 0f));
        if (Random.Range(0,101)<=100)
        {       
          DropLoot(1, .2f);
        }
      } 
  }

  void DropLoot(int amount, float spread)
  {
    if(IsDead != true)
    {
      IsDead = true;
      for (int  i=0; i<amount; i++)
      { 
          GameObject lootdrop = Instantiate(lootDropPrefab, transform.position + new Vector3 (Random.Range(-spread,spread), Random.Range(-spread,spread),0), Quaternion.identity);                
          lootdrop.GetComponent<LootDrop>().Initialize(  ); //Different Amounts based on level??     
      }
    }     
  }

  private void OnDisable()
  {
    EnemyManager.EnemiesList.Remove(gameObject);

  }

  private void Move()
  {
    if( body!=null)
    {
      body.AddForce(Direction * Speed * 2f * body.mass);
    }
     
  }

  private void CheckAttackRange()
  {
    Vector3 target = (Player.transform.position - transform.position);
    if (target.sqrMagnitude < MinRange)
    {
      Direction = -target.normalized;
      currentState = EnemyState.Avoiding;
    }
    else if (target.sqrMagnitude < MaxRange)
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
    
  private void CheckNearest()
  { 
    if(name!="Kamikaze Drone")   
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
      if(( current != null) && (distance < 5f))  // float = Avoid distance
      {
        Direction = -(current.transform.position - gameObject.transform.position).normalized ;
        currentState = EnemyState.Avoiding;        
      }  
    }      
  }

  private void CheckIfInsideView()
  {
    float posX = transform.position.x;
    float posY = transform.position.y;
    if (Ammo > 0)
    {
      if(posX > 12.7f)
      {
        Direction = new Vector3 (-1, Direction.y, Direction.z );
        currentState = EnemyState.Moving;
      }
      if(posX < -12.7f)
      {
        Direction = new Vector3 (1, Direction.y, Direction.z );
        currentState = EnemyState.Moving;
      }
      if(posY > 6.7f)
      {
        Direction = new Vector3 (Direction.x, -1, Direction.z );
        currentState = EnemyState.Moving;
      }
      if(posY < -6.5f)
      {
        Direction = new Vector3 (Direction.x, 1, Direction.z );
        currentState = EnemyState.Moving;
      }
    }
  }

  void SpawnExplosion(float spread)
  {
      ParticleSystem Shards = Instantiate( PSHitEmit, transform.position , transform.rotation);
      Shards.transform.localScale =  new Vector3(Random.Range(.25f,.4f),Random.Range(.25f,.4f),1);    
      ParticleSystem explosion = Instantiate( DeathParticles, transform.position + new Vector3 (Random.Range(-spread,spread), Random.Range(-spread,spread),0), transform.rotation );
      
      


  }
}
