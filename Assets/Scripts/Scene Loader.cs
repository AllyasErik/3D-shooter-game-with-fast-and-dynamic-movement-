using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private bool isAlreadyLoaded;
    [SerializeField] private int levelToLoadNumber;
    [SerializeField] private Transform nextSceneSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        LoadScene();
        other.gameObject.transform.parent.position = nextSceneSpawnPoint.position;
        //other.transform.position = nextSceneSpawnPoint.position;
    }

    private void OnTriggerExit(Collider other)
    {
        UnloadScene();
    }

    private void LoadScene()
    {
        if(!isAlreadyLoaded)
        {
            SceneManager.LoadSceneAsync("Level " + levelToLoadNumber, LoadSceneMode.Additive);
            isAlreadyLoaded = true;
        }
    }

    private void UnloadScene()
    {
        if(isAlreadyLoaded)
        {
            int levelToUnloadNumber = levelToLoadNumber - 1;
            SceneManager.UnloadSceneAsync("Level " + levelToUnloadNumber);
            isAlreadyLoaded = false;
        }
    }

}
