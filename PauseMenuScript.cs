using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{   
    
    public static bool GameIsPaused = false;
    [SerializeField]private GameObject PauseMenuUI;

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
    SceneManager.LoadScene(0);
    }
    
}
