using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private KeyCode pauseKey = KeyCode.P;
    private float normalTimeScale = 1;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject victoryScreenMenu;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingBar;

    private Score score;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI finishTimeText;
    public float timeSpent;
    private int minutes;
    private int seconds;

    public static bool isPaused;
    public static bool isGameOver;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private void Awake()
    {
        score = GetComponent<Score>();
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        victoryScreenMenu.SetActive(false);
        isPaused = false;
        isGameOver= false;
        timeSpent = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver)
        {
            timeSpent += Time.deltaTime;
            minutes = Mathf.FloorToInt(timeSpent / 60);
            seconds = Mathf.FloorToInt(timeSpent % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        

        if(Input.GetKeyDown(pauseKey) && !isGameOver)
        {
            TogglePauseGame();
        }
    }

    private void TogglePauseGame()
    {
        if(Time.timeScale > 0)
        {
            normalTimeScale = Time.timeScale;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if(Time.timeScale == 0)
        {
            Continue();
        }
    }

    public void Continue()
    {
        Time.timeScale = normalTimeScale;
        pauseMenu.SetActive(false);
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isGameOver = true;
    }

    public void Restart()
    {
        ShowLoadingScreen();

        scenesToLoad.Add(SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Single));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Level 1", LoadSceneMode.Additive));

        StartCoroutine(LoadingScreen());

        Continue();
    }

    public void VictoryScreen()
    {
        finalScoreText.text = score.GetScore().ToString();
        finishTimeText.text = timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        victoryScreenMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isGameOver = true;
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
        Time.timeScale = normalTimeScale;
        isPaused = false;
    }

    IEnumerator LoadingScreen()
    {
        float loadingProgress = 0;
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                loadingProgress += scenesToLoad[i].progress;
                loadingBar.fillAmount = loadingProgress / scenesToLoad.Count;
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
