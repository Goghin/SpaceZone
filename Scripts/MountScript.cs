using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MountScript : MonoBehaviour
{   
    [SerializeField]private GameObject[] GunChoices;
    [SerializeField]private GameObject FieldPanel, PickField, ButtonField, MountButton, PickButton;
    [SerializeField]private TMP_Text CostText, LvlText, StatsText;  

    public bool GunChosen = false;

    private GameObject PauseCanvas;
    private GameObject MountPanel;
    private GameObject PickerPanel;
    private GameObject[] PickButtons;
    GameObject temp;
    
    private float AtkBoostTimer;
    private bool AtkBoost;

    private int AmountOfGuns = 3, Level = 1;
    private List<Gun> Guns = new List<Gun>();
    private List<KillTracker> KillList = new List<KillTracker>();
    private TMP_Dropdown PickGunDropDown;  
    
    
    public void MergeKillLists()
    {   
        foreach (Gun g in Guns)
        {
            KillList.AddRange(g.GetKillList());
            
        }
        if (KillList.Count > 1)  
        {     
            for (int a = 0; a < KillList.Count; a++)
            {       
                for (int i = 1; i <= KillList.Count-a; i++)
                { 
                    if (KillList[a].GetTarget() == KillList[a+i].GetTarget())
                    {   
                        Debug.Log("Added " + KillList[a+i].GetKills() + " to " + KillList[a].GetTarget() + ". Removed " + KillList[a+i].GetTarget());
                        KillList[a].AddKills(KillList[a+i].GetKills());
                        KillList.Remove(KillList[a+i]);
                        if (a+i >= KillList.Count)
                        {
                            break;
                        }
                    }         
                }
            }
        }
    }
        
    public string ReportKillList()  
    {
        MergeKillLists();
        string result = Guns[0].Name + " kills:" ;
        for (int i = 0; i < KillList.Count; i++)
        {   
            string c = "\n"+ KillList[i].GetTarget() + ": " +  KillList[i].GetKills();
            result += c;            
        }
        return result;
    }    

    void Start()
    {
        InitializeUI();    
    }

    void InitializeUI()
    {   
        PickButtons = new GameObject[GunChoices.Length];
        Transform MountPos =  GameObject.Find("UIShipPic").transform.Find("FrontGunMount").transform;   // Find mounts
        Toggle mountButton = Instantiate(MountButton , MountPos).GetComponent<Toggle>();
        mountButton.onValueChanged.AddListener( delegate 
        {
            ToggleActive();
        }
        );

        PauseCanvas = GameObject.Find("PauseMenuUI");
        MountPanel = Instantiate(FieldPanel, PauseCanvas.transform) ;
        GameObject.Find("PauseCanvas").GetComponent<PauseMenuScript>().AddToPanelsList(MountPanel); 

        PickerPanel = Instantiate(PickField, MountPanel.transform) ;
        temp = Instantiate(PickButton, MountPanel.transform) ;
        
        for (int a=0;a<PickButtons.Length;a++)
        {
            PickButtons[a] = Instantiate(PickButton, PickerPanel.transform);
            int idx = a;
            PickButtons[a].GetComponent<Image>().sprite = GunChoices[a].GetComponent<DataScript>().data.Image;
            string desc = GunChoices[a].GetComponent<DataScript>().data.Name + ". " + GunChoices[a].GetComponent<DataScript>().data.Description;
            PickButtons[a].GetComponent<HoverScript>().Initialize(desc);
            PickButtons[a].GetComponent<Toggle>().onValueChanged.AddListener( delegate 
        {
            BuyGun(idx);
        }
        );
        }
    }

     public void UpgradeGun()
    {       
        if (PlayerStats.Instance.PlayerMaterials >= Guns[0].Cost * Guns.Count)
        {
            PlayerStats.Instance.PickUpMaterial(-Guns[0].Cost * Guns.Count);    
            foreach (Gun gun in Guns)
            {
               gun.Upgrade(); 
            }
            Level += 1;
            LvlText.text = "Level : "+ Level;
            CostText.text = (Guns[0].Cost * Guns.Count).ToString();  
        }
    }

    public void BuyGun(int a)  
    {
        {
            GunChosen = true;
            int dir = -1;
            int angle = 0;
            for (int i=0; i<AmountOfGuns;i++)
            {
                dir *= -1;
                angle += dir*i*12; // 12 degrees to each side per new gun
                GameObject newgun = Instantiate( GunChoices[a], transform.position, Quaternion.Euler(0,0,angle),transform) ;
                Gun gun = newgun.GetComponentInChildren<Gun>();
                Guns.Add(gun);
            }

        PickerPanel.gameObject.SetActive(false); 

        TMP_Text title = MountPanel.GetComponentInChildren<TMP_Text>();
        string gundesc = Guns.Count.ToString() + "X " + Guns[0].Name;
        if(Guns.Count>1)
        {
            gundesc +="s";
        }
        title.text  = gundesc; 

        GameObject UpgradeField = Instantiate(ButtonField, MountPanel.transform) ;
        TMP_Text Title = UpgradeField.transform.Find("Title").gameObject.GetComponent<TMP_Text>();
        
        LvlText  = UpgradeField.transform.Find("Level").GetComponent<TMP_Text>();
        CostText = UpgradeField.transform.Find("CostText").GetComponent<TMP_Text>();
        
        Button UpgradeButton = UpgradeField.transform.Find("Button").GetComponent<Button>();
        UpgradeButton.onClick.AddListener( delegate 
        {
            UpgradeGun();
        }
        );           
        Title.text = "Upgrade ";
        LvlText.text = "Level : "+ Level;
        CostText.text = (Guns[0].Cost * Guns.Count).ToString();
        //MountPanel.SetActive(false);    
        Destroy(temp);   
        //GameObject.Find("PauseCanvas").GetComponent<PauseMenuScript>().ResumeGame();          
        }        
    } 

    public void ToggleActive()
    {    
        if ( MountPanel.gameObject.activeSelf)
        {
            MountPanel.SetActive(false);
        }    
        else
        {
            GameObject.Find("PauseCanvas").GetComponent<PauseMenuScript>().CloseAllPanels();
            MountPanel.SetActive(true);
        }
    
    }
}



