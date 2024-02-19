using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject instructionsMenu;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ShowInstructionsMenu()
    {
        this.gameObject.SetActive(false);
        instructionsMenu.SetActive(true);
    }

    public void BackToMainMenu()
    {
        instructionsMenu.SetActive(false);
        this.gameObject.SetActive(true);
       
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
