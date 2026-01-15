using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public List<GameObject> fishes;
    private float spawnRate = 2.0f;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI finalScoreText;
    public GameObject menuScreen;
    public GameObject gameScreen;
    public GameObject gameOverScreen;
    public GameObject difficultyScreen;
    public Button startButton;
    public Button restartButton;
    public Button quitButton;
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    [Header("Hearts")]
    public List<Heart> heartScripts;
    private Heart heartScript;
    private int heartIndex = 0;

    [Header("Timer")]
    private int timeRemaining = 60;

    private int score = 0;
    public bool isGameActive = false;

    [Header("Audio Settings")]
    public AudioSource heartDestroyAudio;
    public AudioSource backgroundMusic;

    private float difficulty = 2.0f;

    void Start()
    {
        menuScreen.SetActive(true);
        difficultyScreen.SetActive(false);
        gameScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        
        Cursor.visible = true;

        score = 0;
        scoreText.text = "Score: " + score;
        timerText.text = "Time: " + timeRemaining;

        startButton.onClick.AddListener(SetDifficultyScreen);
        quitButton.onClick.AddListener(Application.Quit);
    }

    private void GameRunning()
    {
        difficultyScreen.SetActive(false);
        gameScreen.SetActive(true);

        Cursor.visible = false;

        heartScripts = new List<Heart>(FindObjectsByType<Heart>(FindObjectsSortMode.None));
        heartScripts.Sort((a, b) => a.name.CompareTo(b.name));

        isGameActive = true;

        /*
        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true; 
            backgroundMusic.Play();
        }
        */

        StartCoroutine(SpawnFishes());
        StartCoroutine(CountdownTimer());
    }

    IEnumerator SpawnFishes()
    {
        while (isGameActive)
        {
            if (fishes.Count > 0)
            {
                int randomIndex = Random.Range(0, fishes.Count);
                Instantiate(fishes[randomIndex]);
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    IEnumerator CountdownTimer()
    {
        timeRemaining = 60;
        while (timeRemaining > 0 && isGameActive)
        {
            timerText.text = "Time: " + timeRemaining;
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        timerText.text = "Time: 0";
        if (isGameActive)
            GameOver();
    }

    public void UpdateScore(int scoreToAdd)
    {
        if (!isGameActive) return;

        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void UpdateHearts()
    {
        if (heartIndex < heartScripts.Count)
        {
            heartScript = heartScripts[heartIndex];
            heartScript.DestroyHearts();

            if (heartDestroyAudio != null)
            {
                heartDestroyAudio.Play();
            }

            heartIndex++;

            if (heartIndex >= heartScripts.Count)
            {
                isGameActive = false;
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        isGameActive = false;

        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }

        gameScreen.SetActive(false);
        gameOverScreen.SetActive(true);

        Cursor.visible = true;

        StopAllCoroutines();

        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            finalScoreText.text = "New High Score: " + score;
        }
        else
        {
            finalScoreText.text = "Score: " + score;
        }

        PlayerPrefs.Save();

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(Application.Quit);
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void SetDifficultyScreen()
    {
        spawnRate = difficulty;

        menuScreen.SetActive(false);
        difficultyScreen.SetActive(true);

        easyButton.onClick.AddListener(GameRunning);
        mediumButton.onClick.AddListener(GameRunning);
        hardButton.onClick.AddListener(GameRunning);
    }
}
