using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingBar;

    [SerializeField] private GameObject[] selectableCharacters;
    private int selectedCharacter;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    public void NextCharacter()
    {
        selectableCharacters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % selectableCharacters.Length;
        selectableCharacters[selectedCharacter].SetActive(true);
    }

    public void PreviousCharacter() 
    {
        selectableCharacters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if(selectedCharacter < 0)
        {
            selectedCharacter += selectableCharacters.Length;
        }
        selectableCharacters[selectedCharacter].SetActive(true);
    }

    public void StartGame()
    {
        ShowLoadingScreen();
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);

        scenesToLoad.Add(SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Single));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Level 1", LoadSceneMode.Additive));

        StartCoroutine(LoadingScreen());
    }

    IEnumerator LoadingScreen()
    {
        float loadingProgress = 0;
        for(int i= 0; i< scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                loadingProgress += scenesToLoad[i].progress;
                loadingBar.fillAmount = loadingProgress/scenesToLoad.Count;
                yield return null;
            }
            
        }
        HideLoadingScreen();
    }

    private void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
    }

    private void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }

}
