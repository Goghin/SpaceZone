using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
   
    public int gunDamage;
    private int Hits;
    public Gun FiredFrom;
    public float Force;
    
       
    public void Initialize(int dmg, float s, Color color, float lifeTime, int hits, float force, Gun parent)
          {
            gunDamage = dmg;
            Hits = hits;
            Force = force;
            FiredFrom = parent;
            
            LineRenderer LR = GetComponentInChildren<LineRenderer>();
              if (LR != null)
              {
                LR.startColor = Color.white; // color;
                LR.endColor = color ;
              }
            
            SpriteRenderer SR = GetComponentInChildren<SpriteRenderer>();
            if (SR != null)
              {
                SR.color = color;
              }

            Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
            body.AddForce(transform.up * s , ForceMode2D.Impulse); //Pushes object
            
            Destroy(gameObject, lifeTime);
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
