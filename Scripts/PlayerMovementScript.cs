
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
//using UnityEngine.UI;
using TMPro;

public class PlayerMovementScript : MonoBehaviour
{
    
    [SerializeField] private Canvas CV;
    private PauseMenuScript Menu;
  
    
    // Start is called before the first frame update
    void Start()
    {   

        Menu = CV.GetComponentInChildren<PauseMenuScript>();
        Menu.PauseGame();       
        PlayerStats.Instance.TakeDamage(0);    // Updates HP bar
        PlayerStats.Instance.IsAlive = true; 
    }

    void FixedUpdate()
    {
        //  Get input, check if its not too close to edge, then move player
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        if (( inputX > 0 & transform.position.x > 12.6 ) || ( inputX < 0 & transform.position.x < -12.6 ))
        {
            inputX = 0;
        }
        if (( inputY > 0 & transform.position.y > 6.1 ) || ( inputY < 0 & transform.position.y < -6.1))
        {
            inputY = 0;
        }
        // Add side thruster particles on move
        float Speed = PlayerStats.Instance.engine.GetSpeed();
        Vector3 movement = new Vector3( Speed * inputX, Speed * inputY, 0);
        transform.Translate(movement * Time.fixedDeltaTime);
    }

   

    void OnCollisionEnter2D(Collision2D other)
    {       
        if(other.gameObject.tag == "Enemy")        
        {   
            if (other.gameObject.name == "Asteroid")           
            {
                AsteroidScript es = other.gameObject.GetComponent<AsteroidScript>();              
                PlayerStats.Instance.TakeDamage(es.HitPoints);
                es.TakeDamage(es.HitPoints);
            }
           else
           {
                EnemyScript es = other.gameObject.GetComponent<EnemyScript>();
                PlayerStats.Instance.TakeDamage(es.HitPoints);
                es.TakeDamage(es.HitPoints);
           }                                   
        }        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if( (other.gameObject.tag == "Loot") )
        {
            LootDrop loot = other.GetComponent<LootDrop>();
            if (other.gameObject.name == "Material")
            {
                PlayerStats.Instance.PickUpMaterial(loot.Power);
            }
            if (other.gameObject.name == "HP")
            {
                PlayerStats.Instance.TakeDamage(-loot.Power * 10); //Set to % of max ??
            }
            if (other.gameObject.name == "Speed")
            {   
                PlayerStats.Instance.engine.ApplySpeedBoost(loot.Power);
            }
            if (other.gameObject.name == "WeaponUp")
            {   
                Debug.Log("Weapon Up");
                //PlayerStats.Instance.engine.ApplySpeedBoost(loot.Power);
            }
            if (other.gameObject.name == "ShieldUp")
            {   
                PlayerStats.Instance.Shield .RechargeShield( loot.Power * 100); // 100 = one full charge
            }
            Destroy(other.gameObject);
        }
    }

    

    
}

