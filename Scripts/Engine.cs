using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Engine : MonoBehaviour
{
  [SerializeField]private TMP_Text manCostText, trvlSpdCostText, accCostText, manLvlText, trvlSpdLvlText, accLvlText, TravelSpeedText, DistanceText;  
  private float Speed;
  private double travelSpeed, acceleration, Distance;

  private int manCost, manLvl;

  private int trvlSpdCost, trvlSpdLvl;
  
  private int accCost, accLvl;

  private float SpdBoostTimer;
  private bool SpdBoost;


    public double GetDistance()
    {
        return Distance;
    }
    public float GetSpeed()
    {
        return Speed;
    }
    
    void Start()
    {
        travelSpeed = 1;
        Speed = 1.8f;
        acceleration = 0;
        SetupText();
    }
    void Update()
    {
        
        UpdateDistance();
    }

    void UpdateDistance()
    {
        travelSpeed += acceleration * Time.deltaTime;
        Distance += travelSpeed * Time.deltaTime;        
        DistanceText.text = "" + (string.Format("{0:#,##0}", Distance)) + "m";
        TravelSpeedText.text = "" + (string.Format("{0:#,##0}", travelSpeed )) + "m/s";
    }

    public void UpgradeManeuverability()
    {
        if (PlayerStats.Instance.PlayerMaterials >= manCost)
        {
            PlayerStats.Instance.PickUpMaterial(-manCost);
            Speed += .3f;
            manLvl += 1;
            manCost += 1;
            manCostText.text =  manCost.ToString();
            manLvlText.text = "Level: "+ manLvl.ToString();
            // Add max lvl, disable button 
        }
    }

    public void UpgradeTravelSpeed()
    {
        if (PlayerStats.Instance.PlayerMaterials >= trvlSpdCost)
        {
            PlayerStats.Instance.PickUpMaterial(-trvlSpdCost);
            travelSpeed += 1;
            trvlSpdLvl += 1;
            trvlSpdCost += 1;
            trvlSpdCostText.text =  trvlSpdCost.ToString();
            trvlSpdLvlText.text = "Level: "+ trvlSpdLvl.ToString();
            // amount increasing with ?? more particles
        }
    }

    public void UpgradeAcceleration()
    {
        if (PlayerStats.Instance.PlayerMaterials >= accCost)
        {
            PlayerStats.Instance.PickUpMaterial(-accCost);
            acceleration += .02;
            accLvl += 1;
            accCost += 1;
            accCostText.text =  accCost.ToString();
            accLvlText.text = "Level: "+ accLvl.ToString();
            // particles? 
        }
    }

    public void ApplySpeedBoost(int a)
    {
        if (SpdBoost == true)
        {
            SpdBoostTimer += 2*a;
        }
        else
        {  
            StartCoroutine(SpeedBoost(a)) ;            
        }
    }

    IEnumerator SpeedBoost(int a)
    {       
        Speed   += 2f;
        SpdBoost = true;          
        SpdBoostTimer += a;
        for (int i = 0 ; i <= SpdBoostTimer; )
        {
            SpdBoostTimer -=1;
            yield return new WaitForSeconds(1);
        }
        Speed -=2;   
        SpdBoost = false;
        SpdBoostTimer = 0;       
    }


    public void SetupText()
    {
            manLvl += 1;
            manCost += 1;
            manCostText.text =  manCost.ToString();
            manLvlText.text = "Level: "+ manLvl.ToString();
            
            trvlSpdLvl += 1;
            trvlSpdCost += 1;
            trvlSpdCostText.text =  trvlSpdCost.ToString();
            trvlSpdLvlText.text = "Level: "+ trvlSpdLvl.ToString();

            accLvl += 1;
            accCost += 1;
            accCostText.text =  accCost.ToString();
            accLvlText.text = "Level: "+ accLvl.ToString();
    }
}
