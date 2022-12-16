
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
//using UnityEngine.UI;
using TMPro;

public class PlayerMovementScript : MonoBehaviour
{
    
    [SerializeField] private TMP_Text MainGunCostText, MainGunStatsText, MainGunTitleText, SpeedCostText;
    [SerializeField] private GameObject[] Gun;
    [SerializeField] private float Speed;
    public GameObject MainGun, LeftGun ;
    private int spcost;
    [SerializeField] private Canvas CV;
    private PauseMenuScript Menu;
    [SerializeField] private TMP_Dropdown MainGunDropDown;

    
    // Start is called before the first frame update
    void Start()
    {   

        MainGunDropDown.onValueChanged.AddListener( delegate 
        {
            BuyMainGun();
        }
        );

        Menu = CV.GetComponentInChildren<PauseMenuScript>();
        Menu.PauseGame();
       
        SpeedCostText.text = "3";
        spcost = 3;
        
        PlayerStats.Instance.TakeDamage(-3);    //Set shield to 3
    
        Speed = 1.8f;   
        PlayerStats.Instance.IsAlive = true; 
    }

    // Update is called once per frame
    void Update()
    {      
             
    }

    void FixedUpdate()
    {
        //  Get input, check if its not too close to edge, then move player
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        if (( inputX > 0 & transform.position.x > 12.5 ) || ( inputX < 0 & transform.position.x < -12.5 ))
        {
            inputX = 0;
        }
        if (( inputY > 0 & transform.position.y > 6 ) || ( inputY < 0 & transform.position.y < -6))
        {
            inputY = 0;
        }
        // Add thruster particles on move

        Vector3 movement = new Vector3( Speed * inputX, Speed * inputY, 0);
        transform.Translate(movement * Time.fixedDeltaTime);
    }

    public void UpgradeMainGun()
    {
        Gun mg = MainGun.GetComponentInChildren<Gun>();
        if (PlayerStats.Instance.PlayerMaterials >= mg.Cost )
        {
        PlayerStats.Instance.PickUpMaterial(-mg.Cost);    
        mg.Upgrade();
        
        MainGunStatsText.text = "DMG: " + mg.Damage + "\nRPM: " + (mg.FireRate);
        MainGunTitleText.text = "Front Gunmount: " + mg.Name;
        MainGunCostText.text = "" + mg.Cost;
        }
    }

    public void UpgradeSpeed()
    {
        if (PlayerStats.Instance.PlayerMaterials >= spcost )
        {
        PlayerStats.Instance.PickUpMaterial(-spcost);    
        Speed += .35f;
        spcost = (int) ((double)spcost * 1.4); 
        SpeedCostText.text = "" + spcost;
        }
        //link to engine particle emission rate over time
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        
        if(other.gameObject.tag == "Enemy")
        {
            AsteroidScript es = other.gameObject.GetComponent<AsteroidScript>();
             PlayerStats.Instance.TakeDamage(1);  // es.damage ?           
        }
    }

void OnTriggerEnter2D(Collider2D other)
    {
        if( (other.gameObject.tag == "Loot") )
        {
            if (other.gameObject.name == "Material")
            {
                MaterialsScript ms = other.gameObject.GetComponent<MaterialsScript>();
                PlayerStats.Instance.PickUpMaterial(ms.amount);
            }
        }
    }


    public void BuyMainGun()  // return instance to button script?
    {
        if (MainGunDropDown.value != 0)
        {
        MainGun = Instantiate( Gun[MainGunDropDown.value-1], transform.Find("MainMount").transform) ;
        //Gun mg = MainGun.GetComponentInChildren<Gun>();

        
        MainGunDropDown.gameObject.SetActive(false); 
        Menu.ResumeGame();   
        
        }
        Gun mg = MainGun.GetComponentInChildren<Gun>();
        MainGunCostText.text = "" + mg.Cost ; 
        MainGunStatsText.text = "DMG: " + mg.Damage + "\nRPM: " + (mg.FireRate);
        MainGunTitleText.text = "Front Gunmount : " + mg.Name;
    } 
}

