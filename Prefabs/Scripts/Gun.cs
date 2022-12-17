using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Gun : MonoBehaviour


{
    [SerializeField] WeaponData weaponData;
    private float CoolDown = 3f;
    
    private List<KillTracker> KillList = new List<KillTracker>();
  
        
    public int Cost = 3;
    public string Name = "Gun";
    public int Damage= 1;
    public float FireRate = 1;
    public int Hits = 1;
    public float LifeTime = 1 ; 
    public int Kills = 0;
    public float Force = 1;
    private ParticleSystem PS;

    void Awake()
    {
       Name = weaponData.Name; 
       Cost = weaponData.Basecost;
       Damage  = weaponData.Damage;
       FireRate = weaponData.FireRate;
       Hits = weaponData.Hits;
       LifeTime = weaponData.LifeTime;
       Force = weaponData.Force;
       PS = gameObject.GetComponentInChildren<ParticleSystem>();
       ParticleSystem.MainModule settings = PS.main;
       settings.startColor = new ParticleSystem.MinMaxGradient( weaponData.color );
    }   

    void Update()
    {
        CoolDown -= Time.deltaTime;
        
        if (CoolDown < 0)
        {
            CoolDown += 60/FireRate;
            Fire();
        }
           
    }
    public void Upgrade()
        {
            Cost += weaponData.Basecost;
            Damage += weaponData.dmgPerLVL;
            FireRate *= weaponData.FRperLVL;
            if (FireRate > weaponData.MaxRPM)
            {
                FireRate = weaponData.MaxRPM;
                Damage += weaponData.dmgPerLVL;
            }
        }


    public void Fire()
    {       
        GameObject bullet = Instantiate(weaponData.Bullet, transform.position, transform.rotation) ;           
        //  Initialize(int dmg, float speed, Color c, Vector3 direction, float lifeTime, int maxhits,, float force, Reference to Gun Script)
        bullet.GetComponent<BulletScript>().Initialize(Damage, weaponData.ProjectileSpeed, weaponData.color, new Vector3( 0, 1 ,0 ), LifeTime, Hits, Force, this );    // Needs to find a vector for gun rotation              
        
        PS.Play();    
        
    }

    public void AddKill(string targetname)
    {
        bool found = false;
        if (KillList.Count >= 1)
        { 
        for (int i = 0; i < KillList.Count; i++)
            { 
                if (targetname == KillList[i].GetTarget())
                {
                    KillList[i].AddKill();
                    found = true;
                }
            }
        }           
        if (!found)       
        {   
            KillTracker kt = new KillTracker(targetname);
            kt.AddKill();            
            KillList.Add(kt);           
        }
    }

    public string ReportKillList()  
    {
        string result = Name + " kills:" ;
        for (int i = 0; i < KillList.Count; i++)
        {   
            string c = "\n"+ KillList[i].GetTarget() + ": " +  KillList[i].GetKills();
            result += c;            
        }
        return result;
    }    
}

public class KillTracker
{
    private string Target;
    private int Kills;
    public KillTracker(string target)
    {
        Target = target;
        Kills = 0;
    }

    public string GetTarget()
    {
        return Target;
    }

    public void AddKill()
    {
        Kills +=1;
    }

    public int GetKills()
    {
        return Kills;
    }

}
