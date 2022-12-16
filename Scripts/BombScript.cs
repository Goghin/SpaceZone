using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField]private ParticleSystem PSPrefab;

    void OnCollisionEnter2D(Collision2D collision)
    {               
        if (collision.gameObject.tag == "Enemy")
        {
            
            
            ParticleSystem Shards = Instantiate( PSPrefab, new Vector3( collision.GetContact(0).point.x , collision.GetContact(0).point.y , 1) , transform.rotation);
            //Shards.GetComponent<Renderer>().material.color = newColor; 
            //gameObject.GetComponent<ParticleSystem>().Play();   

        }

    }

}
