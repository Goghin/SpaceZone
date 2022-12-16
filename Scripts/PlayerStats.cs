using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerStats : MonoBehaviour
{
       
    public bool IsAlive = false;
    public static PlayerStats Instance { get; private set; }
    [SerializeField] public GameObject Player, GameOverCanvas;
    public int PlayerMaterials, PlayerShields;   
    [SerializeField] private TMP_Text MaterialsText, PlayerShieldsText, DistanceText, SpeedText, TravelSummary, KillSummary;
    public double Distance = 0, Speed = 0;
    


    private void Awake() 
    { 
    // If there is an instance, and it's not me, delete myself.
    
    if (Instance != null && Instance != this) 
    { 
        Destroy(this); 
    } 
    else 
    { 
        Instance = this; 
    }   
    Speed = 1;  
    UpdateSpeedText();
    
    }
    
   public void PickUpMaterial(int amount)
    {
        PlayerMaterials += amount;
        MaterialsText.text = "" + PlayerMaterials;
    }

    public void TakeDamage(int amount)
    {
        PlayerShields -= amount;
        PlayerShieldsText.text = "" + PlayerShields ;
        if (PlayerShields < 1) 
        {
            string killtext = Player.GetComponent<PlayerMovementScript>().MainGun.GetComponentInChildren<Gun>().ReportKillList(); // If dead without MainGun assigned
            IsAlive = false;
            GameOverCanvas.gameObject.SetActive(true);
             PauseMenuScript PMS = GameObject.Find("PauseCanvas").GetComponent<PauseMenuScript>();
            PMS.PauseGame();
            PMS.gameObject.SetActive(false);
            TravelSummary.text = "Distance travelled: " + (string.Format("{0:#,##0}", Distance)) + "m. \nFinal speed: " + Speed + "m/s."; 
            KillSummary.text = killtext; 


        }
    }


    void FixedUpdate()
    {
        UpdateDistance();
    }


    void UpdateDistance()
    {
        Distance += Speed * Time.deltaTime;        
        DistanceText.text = "" + (string.Format("{0:#,##0}", Distance)) + "m";
    }

    void UpdateSpeedText()
    {
        SpeedText.text = "" + Speed+ "m/s";
        //link to engine particle emission rate over time
    }
}
