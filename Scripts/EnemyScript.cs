using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyScript : MonoBehaviour
{
  [SerializeField]private GameObject MaterialDrop, audioPrefab;
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
  private AudioSource SoundPlayer;
  private AudioClip ShootSound;
  private AudioClip GetHitSound;
  private AudioClip DeathSound;
  private ParticleSystem DeathParticles;
  private float MinRange = 5;
  private float MaxRange = 60;
  SpriteRenderer spr;

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
    DeathParticles = enemyData.DeathParticles;
    SoundPlayer = GetComponent<AudioSource>();
    ShootSound = enemyData.ShootSound;
    GetHitSound = enemyData.GetHitSound;
    DeathSound = enemyData.DeathSound;
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
    MinRange = enemyData.MinRange;
    MaxRange = enemyData.MaxRange;
    Ammo = enemyData.Ammo;
         
    spr = GetComponent<SpriteRenderer>();
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
        
        SoundPlayer.clip = ShootSound;
        SoundPlayer.Play();
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
      SoundPlayer.clip = GetHitSound;
      SoundPlayer.Play();
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
      SoundPlayer.clip = GetHitSound;
      SoundPlayer.Play();
      Gun killer = collision.gameObject.GetComponent<BulletScript>().FiredFrom;
      int dmg = collision.gameObject.GetComponent<BulletScript>().gunDamage;
      float force = collision.gameObject.GetComponent<BulletScript>().Force;
      Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
      rb.AddForce(collision.GetContact(0).normal * rb.mass * 2f *force);          
      ParticleSystem Shards = Instantiate( PSHitEmit, new Vector3( collision.GetContact(0).point.x , collision.GetContact(0).point.y , 1) , transform.rotation); 
      Shards.transform.localScale = new Vector3(.16f,.16f,1);         
      TakeDamage(dmg , killer);        
    }
               
  }

  void OnTriggerEnter2D(Collider2D collision)
  {          
    if (collision.gameObject.tag == "Projectile")
    {
      SoundPlayer.clip = GetHitSound;
      SoundPlayer.Play();
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
  IEnumerator Iexplosions(int amount, float spread)
  {
    for (int i=0;i<amount;i++)
    {
      SpawnExplosion(spread);
      yield return new WaitForSeconds(.08f);
    }
    Destroy(gameObject);  
  }
  public void Death()
  {
    Ammo=0;
    Speed=0;
    spr.sprite = null;     
    if (name == "Boss Drone")
      {     
        StartCoroutine(Iexplosions(5,1.2f));
      }
    else 
      {
        StartCoroutine(Iexplosions(1, 0f));
      }
    if (Random.Range(0,101)<=100)
      {
        GameObject lootdrop = Instantiate(MaterialDrop, transform.position, transform.rotation);        
        // Initialize(int amount )
        lootdrop.GetComponent<MaterialsScript>().Initialize( 1 ); //Different Amounts based on level??
      }
         
    
  }

  public void OnDisable()
  {
    EnemyManager.EnemiesList.Remove(gameObject);

  }

  public void Move()
  {
    Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
    rb.AddForce(Direction * Speed * 2f * rb.mass); 
  }

  public void CheckAttackRange()
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
    
  public void CheckNearest()
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


  void SpawnExplosion(float spread)
  {
      ParticleSystem Shards = Instantiate( PSHitEmit, transform.position , transform.rotation);
      Shards.transform.localScale =  new Vector3(Random.Range(.18f,.23f),Random.Range(.18f,.23f),1);    
      ParticleSystem explosion = Instantiate( DeathParticles, transform.position + new Vector3 (Random.Range(-spread,spread), Random.Range(-spread,spread),0), transform.rotation );
      GameObject clone = Instantiate(audioPrefab, transform.position, transform.rotation) as GameObject;
      AudioSource cloneAudio = clone.GetComponent<AudioSource>();
      cloneAudio.clip = DeathSound;
      cloneAudio.Play();
      Destroy(clone, DeathSound.length + 0.1f);


  }
}
