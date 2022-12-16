using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private float Speed, LifeTime;
    private Vector3 Direction;
    public int gunDamage;
    private Color bulletColor;
    private int Hits;
    public Gun FiredFrom;
    public float Force;
    
       
    public void Initialize(int dmg, float s, Color c, Vector3 dir, float lifeTime, int hits, float force, Gun parent)
          {
            gunDamage = dmg;
            Speed = s;
            bulletColor = c;
            Direction = dir;
            LifeTime = lifeTime;
            Hits = hits;
            Force = force;
            FiredFrom = parent;
            
            LineRenderer LR = GetComponentInChildren<LineRenderer>();
              if (LR != null)
              {
                LR.startColor = bulletColor;
                LR.endColor = bulletColor ;
              }
            
            SpriteRenderer SR = GetComponentInChildren<SpriteRenderer>();
            if (SR != null)
              {
                SR.color = bulletColor;
              }

            Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
            body.AddForce(dir * s , ForceMode2D.Impulse); //Pushes object

            Destroy(gameObject, LifeTime);

          }  

    void OnCollisionEnter2D(Collision2D collision)
    {               
        if (collision.gameObject.tag == "Enemy")
        {            
          Hits -=1;
          if (Hits <= 0 )
          { 
            Destroy(gameObject);
          }       
        }
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {               
        if (collision.gameObject.tag == "Enemy")
        {            
          Hits -=1;
          if (Hits <= 0 )
          { 
            Destroy(gameObject);
          }       
        }
        
    }
    
}
