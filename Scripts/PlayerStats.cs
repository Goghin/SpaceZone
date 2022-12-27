using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]public Engine engine;   
    public bool IsAlive = false;
    public static PlayerStats Instance { get; private set; }
    [SerializeField] public GameObject Player, GameOverCanvas;
    public int PlayerMaterials;
    [SerializeField] private TMP_Text MaterialsText, TravelSummary, KillSummary, ShieldsText;   
    [SerializeField]private Slider HpBar;
    public int PlayerHp, PlayerMaxHp;
    private int Shields;
    [SerializeField]private Image HpBarFill;
    public ShieldScript Shield;

    private void Awake() 
    {         
        PlayerHp=100;
        PlayerMaxHp=100;

        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }    
    }
    
   public void PickUpMaterial(int amount)
    {
        PlayerMaterials += amount;
        MaterialsText.text = "" + PlayerMaterials;
    }

    public void TakeDamage(int amount)
    {       
        PlayerHp -= amount;
        HpBar.value = (float)PlayerHp / (float)PlayerMaxHp;
        Color HealthBarColor = Color.Lerp(Color.red, Color.white, ((float)PlayerHp / (float)PlayerMaxHp));
        HpBarFill.color = HealthBarColor;
        if (PlayerHp > PlayerMaxHp)
        {
            PlayerHp = PlayerMaxHp;
        }      
        if (PlayerHp < 1) 
        {   
            if (Player.GetComponentInChildren<MountScript>().GunChosen == true)
            {                
                MountScript rprtkill = Player.GetComponentInChildren<MountScript>();            
                string killtext = rprtkill.ReportKillList(); // If dead without MainGun assigned
                KillSummary.text = killtext; 
            }           
            IsAlive = false;
            GameOverCanvas.gameObject.SetActive(true);
            PauseMenuScript PMS = GameObject.Find("PauseCanvas").GetComponent<PauseMenuScript>();
            PMS.PauseGame();
            PMS.gameObject.SetActive(false);
            
            // TravelSummary.text = "Distance travelled: " + (string.Format("{0:#,##0}", Distance)) + "m. \nFinal speed: " + Speed + "m/s.";  
            //***    call engine  *** 
        }
    }

    public void ShieldChange(int a)
    {
        Shields += a;
        ShieldsText.text = "" + Shields + "/" + Shield.GetMaxHits();

    }
    
}
