using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullNanobotsScript : MonoBehaviour, IMiscUpgrade 
{
    
    public void BuyUpgrade(int nr)
    {   
        PlayerStats.Instance.PlayerMaxHp -= values[0];
        values[nr] += 1;
        upgradeCosts[nr] += 1;  // change to own variables per upgrade
        PlayerStats.Instance.PlayerMaxHp += values[0];
        PlayerStats.Instance.TakeDamage(-values[0]);  
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
    private int NrOfUpgrades = 3;

    private int[] upgradeCosts = new int[3];
    private int[] values = new int[3];
    private string[] names = new string[3];

    private float CoolDown = 0f;   

    void Awake()
    {   
        name = "Hull Nanobots";
        upgradeCosts[0] = 1; 
        upgradeCosts[1] = 1;
        upgradeCosts[2] = 1;

        values[0] = 20;
        values[1] = 25;
        values[2] = 1;

        names[0] = "Hull strength: ";
        names[1] = "Repair rate: ";
        names[2] = "Repair power: ";
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerStats.Instance.PlayerMaxHp += values[0];
        PlayerStats.Instance.TakeDamage(-values[0]);   
    }

    // Update is called once per frame
    void Update()
    {
        CoolDown += values[1]* Time.deltaTime;
        if (CoolDown >= 100)
        {
            CoolDown -= 100;
            PlayerStats.Instance.TakeDamage(-values[2]);
        }
    }
}
