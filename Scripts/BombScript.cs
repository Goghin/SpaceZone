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
            //BombExplosion(collision);
        }

    }

    public void BombExplosion(Collision2D collision)
    {
    Collider2D[] colliders = Physics2D.OverlapCircleAll(collision.contacts[0].point, 2f);

                    foreach (Collider2D hit in colliders)

                    {

                        Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                        float Force = 4f;

                        rb.AddForce(new Vector2(Force * (hit.transform.position.x - collision.contacts[0].point.x),Force * (hit.transform.position.y - collision.contacts[0].point.y)));

                    }
    }
}
