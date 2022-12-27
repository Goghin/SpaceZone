using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGenerator : MonoBehaviour, IMiscUpgrade
{ 
    public void BuyUpgrade(int nr)
    {   
        Shield.AddMaxCharge(-values[0]);
        values[nr] += 1;
        upgradeCosts[nr] += 1; 
        Shield.AddMaxCharge(+values[0]);
        PlayerStats.Instance.ShieldChange(0);
         // change to own variables per upgrade
    }
    
    public int AmountOfUpgrades()
    {
        return NrOfUpgrades;
    }

    public int UpgradeCost(int nr)
    {
        return upgradeCosts[nr];
    }
 
    public string UpgradeName(int nr)
    {
        return names[nr] + values[nr].ToString();      
    }

    public string GetName()
    {
        return name;
    }
    public Sprite GetGraphic()
    {
        return graphic;
    }
    [SerializeField]Sprite graphic;

    private int NrOfUpgrades = 2;
    private int[] upgradeCosts = new int[2];
    private int[] values = new int[2];
    private string[] names = new string[2];
   
    ShieldScript Shield;   
    [SerializeField]private bool IsPrimary = false;
   
    void Awake()
    {   
        name = "Shield Generator";
        upgradeCosts[0] = 3; 
        upgradeCosts[1] = 1;
        
        values[0] = 1; // Max shields
        values[1] = 20; // recharge % rate per sec. 

        names[0] = "Max hits: ";
        names[1] = "Recharge rate %/s: ";
             
        if ( PlayerStats.Instance.Shield == null)
        {
            IsPrimary = true;
            Shield = transform.Find("shield").GetComponent<ShieldScript>();
            PlayerStats.Instance.Shield = Shield;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Shield.transform.position = player.transform.position;         
        }
        else
        {   
            Destroy(transform.Find("shield").gameObject);
            Shield = PlayerStats.Instance.Shield;
        }
        Shield.AddMaxCharge(values[0]);
        PlayerStats.Instance.ShieldChange(0);
    }
    
    void Update()
    {
        if (Shield.IsFullyCharged() == false)
        {    
            Shield.RechargeShield(values[1]*Time.deltaTime);        
        }
        
    }

     

    

    
}
