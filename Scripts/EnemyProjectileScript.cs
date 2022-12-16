using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    
    
    
    public int Damage; 
    public float Force;
    
       
    public void Initialize(Vector3 target, int dmg, float speed, Color color)
    {
        
        
        Damage = dmg;
         
        Force = 10; // Add to init args

        float targetAngle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg-90;
        transform.rotation = Quaternion.Euler (0, 0, targetAngle);
 
        

        Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
        body.AddForce(target.normalized * speed , ForceMode2D.Impulse); //Pushes object   

        LineRenderer LR = GetComponentInChildren<LineRenderer>();
        if (LR != null)
        {
            LR.startColor = color;
            LR.endColor = color ;
        }            
        SpriteRenderer SR = GetComponentInChildren<SpriteRenderer>();
        if (SR != null)
        {
        SR.color = color;
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
