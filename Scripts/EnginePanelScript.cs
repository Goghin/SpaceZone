using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnginePanelScript : MonoBehaviour
{   
    void Start()
    {
        GameObject.Find("PauseCanvas").GetComponent<PauseMenuScript>().AddToPanelsList(gameObject); 
    }
    
    public void ToggleActive()
    {
    
    if (gameObject.activeSelf)
    {
        gameObject.SetActive(false);
    }    
    else
    {
        GameObject.Find("PauseCanvas").GetComponent<PauseMenuScript>().CloseAllPanels(); 
        gameObject.SetActive(true);
    }
   
    }
}
