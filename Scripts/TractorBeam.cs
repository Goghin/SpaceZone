using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeam : MonoBehaviour, IMiscUpgrade

// Upgrades to do Power Range MaxTargets
 
{
    public void BuyUpgrade(int nr)
    {   
        values[nr] += 1;
        upgradeCosts[nr] += 1;  // change to own variables per upgrade
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

    private float CoolDown = 5f, FireRate = 60f;   
    //private float Power = 12f, Range = 50f;

    //                               10?                      // Lock on cycle 12? = 5sec
    private List<GameObject> TargetList = new List<GameObject>();
    [SerializeField]private Material Beam;
    
    void Awake()
    {   
        name = "Tractor beam";
        upgradeCosts[0] = 5; 
        upgradeCosts[1] = 1;
        upgradeCosts[2] = 1;

        values[0] = 1;
        values[1] = 20;
        values[2] = 3;

        names[0] = "Targets: ";
        names[1] = "Range: ";
        names[2] = "Power: ";

    }
    void Update()
    {
        
        CoolDown -= Time.deltaTime;
        if (CoolDown <= 0)
        {
            CoolDown += 60/FireRate;
            UpDateTargetList();            
        }
        foreach(GameObject target in TargetList)
            {
            PullTarget(target);
            }           
    }

    private void UpDateTargetList()
    {           
        TargetList.Clear();  
        Vector3 myPos = transform.position; 
        List<GameObject> TempList = new List<GameObject>(); 
        GameObject[] loot = GameObject.FindGameObjectsWithTag("Loot"); 
        
        
        foreach (GameObject item in loot)     //Add all "Loot" in range to TempList   
            {
                Vector3 diff = item.transform.position - myPos;
                float currDistance = diff.sqrMagnitude;
                
                if (currDistance < (float)values[1])
                {
                    TempList.Add(item);
                   
                }            
            }                          
        for (int i=0; i<values[0]; i++)
        {
            GameObject closest = null;
            float distance = 10000f;                
            {
                if (TempList.Count !=0)
                for (int a=0; a<TempList.Count; a++)
                {
                    Vector3 diff = TempList[a].transform.position - myPos;
                    float currDistance = diff.sqrMagnitude;
                    if (currDistance < distance)
                    {
                        closest = TempList[a];
                        distance = currDistance;
                    }                             
                }
            } 
            TempList.Remove(closest);      
            TargetList.Add(closest);  
        }     
    }

    private void PullTarget(GameObject target)
    {   
        if (target != null)
        {
        DrawLine(transform.position, target.transform.position, Color.white );      
        Vector3 diff = new Vector3 (transform.position.x, transform.position.y , 0) - target.transform.position;            
        Rigidbody2D body = target.GetComponent<Rigidbody2D>();
        body.AddForce(diff.normalized *  (float)values[2]/100f, ForceMode2D.Impulse ); //Pulls object               
        } 
    }



void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.005f)
         {
             GameObject myLine = new GameObject();
             myLine.transform.position = start;
             myLine.AddComponent<LineRenderer>();
             LineRenderer lr = myLine.GetComponent<LineRenderer>();
             lr.material = Beam; 
             lr.startColor = color;
             lr.endColor = color;
             lr.startWidth = 0.06f;
             lr.endWidth = 0.14f;
             lr.SetPosition(0, start);
             lr.SetPosition(1, end);
             GameObject.Destroy(myLine, duration);
         }




}


interface IMiscUpgrade
{   
    void BuyUpgrade(int nr);
    int AmountOfUpgrades();
    int UpgradeCost(int nr);
    string UpgradeName(int nr);
    string GetName();
    Sprite GetGraphic();
}