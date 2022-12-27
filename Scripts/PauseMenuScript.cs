using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{   
    
    public static bool GameIsPaused = false;
    [SerializeField]private GameObject PauseMenuUI;
    private List<GameObject> PanelsList = new List<GameObject>();

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape)) && (PlayerStats.Instance.IsAlive = true))
        {
            if (GameIsPaused)
            {
                ResumeGame();
            } else
            {
                PauseGame();
            }

        }
    }

    public void ResumeGame()
    {
        PauseMenuUI.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        GameIsPaused = true;
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
     SceneManager.LoadScene("MainMenu");
    }


    public void AddToPanelsList(GameObject item)
    {
        PanelsList.Add(item);
    }

    public void CloseAllPanels()
    {
        if (PanelsList.Count != 0)
        {
            foreach (GameObject item in PanelsList)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
    
    
}



    

   

   