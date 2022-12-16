using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsScript : MonoBehaviour
{
    
    
    public int amount; 
        
    public void Initialize(int a ) 
          {
            name = "Material";
            amount = a;                           
          }    

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Finish" )
        {
            Destroy(gameObject);

        }
    }
}

