using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{ 
    [SerializeField]private int Hits = 0, MaxHits;    
    private int HitsAbsorbed = 0;   
    private float Charge;
    private GameObject Shield;
    private SpriteRenderer shieldRenderer;
    private CapsuleCollider2D body;
    
    void Start()
    {
        shieldRenderer = gameObject.GetComponent<SpriteRenderer>();
        body = gameObject.GetComponent<CapsuleCollider2D>();
        StartCoroutine(ShieldEffect());  

    }

    void Update()
    {        
        if (Hits<1)
        {
            DeActivate();
        }    
    }

    void DeActivate()    
    {
        body.enabled = false;
        shieldRenderer.enabled = false;
        //StopCoroutine(ShieldEffect());
    }

    void Activate()
    {
        body.enabled = true;
        shieldRenderer.enabled = true;
        //StartCoroutine(ShieldEffect());
    }

    IEnumerator ShieldEffect()
    {   
        Vector3 scaleChange = new Vector3(0.02f, 0.02f, 1f);
        float aChange = 0.02f;       
        int times = 0, add = 1;
        for (;;)
        {           
            times += add;
            shieldRenderer.color = new Color(shieldRenderer.color.r, shieldRenderer.color.g, shieldRenderer.color.b, shieldRenderer.color.a - aChange) ;
            transform.localScale += scaleChange;            
            if ((times >=30) || (times <=0))
            {
                scaleChange = -scaleChange;
                aChange = -aChange;
                add = -add;               
            }
            yield return new WaitForSeconds(.05f);
        }

    }

    public void OnTriggerEnter2D(Collider2D collider)
    {

        if ((collider.gameObject.tag == "EnemyFire") && (Hits > 0))
        {
            HitsAbsorbed += 1;
            Hits -=1;
            PlayerStats.Instance.ShieldChange(-1);
            Color colr = Color.Lerp(Color.red, Color.white, (float)Hits/(float)10) ;
            shieldRenderer.color = new Color(colr.r, colr.g, colr.b, shieldRenderer.color.a );
            // Play sound, shield effect, 
            Destroy(collider.gameObject);
                     
        }

        if ((collider.gameObject.tag == "Enemy") && (Hits > 0))
        {
            HitsAbsorbed += 1;
            Hits -=1;
            PlayerStats.Instance.ShieldChange(-1);
            Color colr = Color.Lerp(Color.red, Color.white, (float)Hits/(float)10) ;
            shieldRenderer.color = new Color(colr.r, colr.g, colr.b, shieldRenderer.color.a );                       
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 boink =   collider.transform.position- transform.position;   
                rb.AddForce(boink * rb.mass * 150f); 
            }                                            
        }
    }

    public void RechargeShield(float a)
    {
        Charge+=a;
        if (Charge >= 100 && Hits < MaxHits)
        {
            Charge-=100;
            Activate();
            Hits+=1;
            PlayerStats.Instance.ShieldChange(1);
            Color colr = Color.Lerp(Color.red, Color.white, (float)Hits/(float)10) ;
            shieldRenderer.color = new Color(colr.r, colr.g, colr.b, shieldRenderer.color.a );
        }
        
          
    }

    public void AddMaxCharge(int a)
    {
        MaxHits += a;
    }
    public bool IsFullyCharged()
    {
        if (Hits<MaxHits)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int GetMaxHits()
    {
        return MaxHits;
    }
}


