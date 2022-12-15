using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    
    private float Speed;
    private Vector3 Target, pos;
    public int Damage;
    private Color bulletColor;   
    public float Force;
    
       
    public void Initialize(Vector3 target, int dmg, float speed, Color color)
    {
        pos = transform.position;
        Damage = dmg;
        Speed = speed;
        bulletColor = color;
        Target = target;
        Force = 10; // Add to init args
        Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
        body.AddForce((Target-pos).normalized * speed , ForceMode2D.Impulse); //Pushes object              
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
        Destroy(gameObject, 10);

    }

    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {               
        if (collision.gameObject.tag == "Player")
        {            
          
            Destroy(gameObject);
                
        }
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {               
        if  (collision.gameObject.tag == "Asteroid")
        {            
          
            Destroy(gameObject);
               
        }
        
    }
}
