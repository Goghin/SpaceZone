using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MiscSlot : MonoBehaviour
{   
    [SerializeField]private GameObject[] ItemChoices;
    [SerializeField]private GameObject FieldPanel, PickField, ButtonField, MountButton;    
    public bool ItemChosen = false;
    private GameObject PauseCanvas;
    private GameObject MountPanel;
    private GameObject PickerPanel;
    private IMiscUpgrade Item;
    private int[] Level;
    TMP_Text[] Title; 
    TMP_Text[] LvlText;  
    TMP_Text[] CostText; 
    Button[] UpgradeButton;  
    Toggle mountButton; 
    private TMP_Dropdown PickItemDropDown;  // Create panel with dropdown, upgrade buttons and stats on spawn
    
    public string ReportUseList()  
    {    
        string result = " placeholder" ;      
        return result;
    }    

    void Start()
    {
        InitializeUI();    
    }

    void InitializeUI()
    {   
        Transform MountPos =  GameObject.Find("UIShipPic").transform.Find("Mount").transform;   // Find mounts
        mountButton = Instantiate(MountButton , MountPos).GetComponent<Toggle>();
        MountPos.gameObject.name = "Full";
        mountButton.onValueChanged.AddListener( delegate 
        {
            ToggleActive();
        }
        );

        PauseCanvas = GameObject.Find("PauseMenuUI");
        MountPanel = Instantiate(FieldPanel, PauseCanvas.transform) ;
        GameObject.Find("PauseCanvas").GetComponent<PauseMenuScript>().AddToPanelsList(MountPanel); 
        TMP_Text title = MountPanel.GetComponentInChildren<TMP_Text>();
            title.text  = "Empty support slot:" ;          

        PickerPanel = Instantiate(PickField, MountPanel.transform) ;
        PickItemDropDown = PickerPanel.GetComponentInChildren<TMP_Dropdown>();
         title = PickerPanel.GetComponentInChildren<TMP_Text>();
          title.text  = "Pick new support item:" ;          

        PickItemDropDown.ClearOptions();        
        List<string> ItemList = new List<string>();
        ItemList.Add("Pick one:");
        for (int i=0; i<ItemChoices.Length; i++)
        {
            ItemList.Add(ItemChoices[i].name);
        }
        PickItemDropDown.AddOptions(ItemList);   
        PickItemDropDown.onValueChanged.AddListener( delegate 
        {
            BuyItem();
        }
        );
        MountPanel.SetActive(false);
    }

    public void UpgradeItem(int nr)
    {
        Debug.Log("slot "+ nr);
        if (PlayerStats.Instance.PlayerMaterials >= Item.UpgradeCost(nr))
        {
            PlayerStats.Instance.PickUpMaterial(-Item.UpgradeCost(nr));       
            Item.BuyUpgrade(nr); 
            Level[nr]++;
            LvlText[nr].text = "Level : "+ Level[nr];
            CostText[nr].text = Item.UpgradeCost(nr).ToString();   
            Title[nr].text = Item.UpgradeName(nr);
        }
    }

    public void BuyItem()  // return instance to button script?
    {
        if (PickItemDropDown.value != 0)
        {     
            PickerPanel.gameObject.SetActive(false);    
            GameObject newItem = Instantiate( ItemChoices[PickItemDropDown.value-1], transform.position, transform.rotation ,transform) ;
            Item = newItem.GetComponentInChildren<IMiscUpgrade>();  
            mountButton.gameObject.GetComponent<Image>().sprite = Item.GetGraphic(); 
            Level = new int[Item.AmountOfUpgrades()];
            for (int u = 0; u<Level.Length; u++)
            {
                Level[u] = 1;
            }  
            Title = new TMP_Text[Item.AmountOfUpgrades()]; 
            LvlText = new TMP_Text[Item.AmountOfUpgrades()];
            CostText = new TMP_Text[Item.AmountOfUpgrades()]; 
            UpgradeButton = new Button[Item.AmountOfUpgrades()];
            TMP_Text title = MountPanel.GetComponentInChildren<TMP_Text>();
            title.text  = Item.GetName() ;             
            for (int i = 0; i < Item.AmountOfUpgrades(); i++)
            {   int idx = i;
                GameObject UpgradeField = Instantiate(ButtonField, MountPanel.transform) ;             
                Title[i] = UpgradeField.transform.Find("Title").gameObject.GetComponent<TMP_Text>();  
                LvlText[i]  = UpgradeField.transform.Find("Level").GetComponent<TMP_Text>();
                CostText[i] = UpgradeField.transform.Find("CostText").GetComponent<TMP_Text>();                
                Title[i].text = Item.UpgradeName(i);
                LvlText[i].text = "Level : "+ Level[i];
                CostText[i].text = Item.UpgradeCost(i).ToString();
                UpgradeButton[i] = UpgradeField.transform.Find("Button").GetComponent<Button>();
                UpgradeButton[i].onClick.AddListener(() => UpgradeItem(idx));
            }               
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





