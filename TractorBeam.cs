using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeam : MonoBehaviour

// Upgrades to do Power Range MaxTargets
 
{
    private int MaxTargets = 1; //1
    private float Power = 2f, Range = 10f, CoolDown = 5f, FireRate = 60f;   
    //                                10?                      // Lock on cycle 12? = 5sec
    private List<GameObject> TargetList = new List<GameObject>();
    [SerializeField]private Material Beam;
    

    void Update()
    {
        // DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + Range, 0), Color.white );  
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
        GameObject[] loot = GameObject.FindGameObjectsWithTag("Loot"); // GameObject.FindGameObjectsWithTag("Respawn");
        // Debug.Log("Loot in array " +loot.Length);
        
        foreach (GameObject item in loot)     //Add all "Loot" in range to TempList   
            {
                Vector3 diff = item.transform.position - myPos;
                float currDistance = diff.sqrMagnitude;
                // Debug.Log("Range " +currDistance );
                if (currDistance < Range)
                {
                    TempList.Add(item);
                   // Debug.Log("Added " +item.name );
                }            
            }                          
        for (int i=0; i<MaxTargets; i++)
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
        body.AddForce(diff.normalized * Power/1000f, ForceMode2D.Impulse ); //Pulls object               
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
