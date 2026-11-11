
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private List<GameObject> fishes;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private int gameDuration = 60;

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
    public Button closeButton;
    public Button quitButton;

    [Header("Hearts")]
    [SerializeField] private List<Heart> heartScripts = new List<Heart>();

    private int heartIndex = 0;

    [Header("Audio Settings")]
    public AudioSource heartDestroyAudio;
    public AudioSource backgroundMusic;

    private int score = 0;
    private int timeRemaining;

    private bool isGameOver = false;

    public bool isGameActive = false;

    private void Start()
    {
        InitializeGame();

        if (startButton != null)
            startButton.onClick.AddListener(SetDifficultyScreen);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(QuitGame);
    }

    private void InitializeGame()
    {
        if (menuScreen != null)
            menuScreen.SetActive(true);

        if (difficultyScreen != null)
            difficultyScreen.SetActive(false);

        if (gameScreen != null)
            gameScreen.SetActive(false);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        Cursor.visible = true;

        score = 0;
        timeRemaining = gameDuration;

        UpdateScoreText();
        UpdateTimerText();
    }

    // Opens the difficulty selection screen.
    public void SetDifficultyScreen()
    {
        if (menuScreen != null)
            menuScreen.SetActive(false);

        if (difficultyScreen != null)
            difficultyScreen.SetActive(true);
    }

    // Called by DifficultyButton.
    public void StartGame(float selectedDifficulty)
    {
        if (isGameActive)
            return;

        spawnRate = Mathf.Max(0.1f, selectedDifficulty);

        score = 0;
        timeRemaining = gameDuration;
        heartIndex = 0;

        isGameOver = false;

        UpdateScoreText();
        UpdateTimerText();

        // Find all hearts, including inactive ones.
        heartScripts = new List<Heart>(
            FindObjectsByType<Heart>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            )
        );

        heartScripts.Sort(
            (a, b) => string.CompareOrdinal(a.name, b.name)
        );

        // Make sure all hearts are visible.
        foreach (Heart heart in heartScripts)
        {
            if (heart != null)
                heart.gameObject.SetActive(true);
        }

        if (menuScreen != null)
            menuScreen.SetActive(false);

        if (difficultyScreen != null)
            difficultyScreen.SetActive(false);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (gameScreen != null)
            gameScreen.SetActive(true);

        Cursor.visible = false;

        isGameActive = true;

        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }

        StartCoroutine(SpawnFishes());
        StartCoroutine(CountdownTimer());
    }

    private IEnumerator SpawnFishes()
    {
        while (isGameActive)
        {
            if (fishes != null && fishes.Count > 0)
            {
                int randomIndex = Random.Range(0, fishes.Count);

                if (fishes[randomIndex] != null)
                {
                    Instantiate(fishes[randomIndex]);
                }
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    private IEnumerator CountdownTimer()
    {
        while (timeRemaining > 0 && isGameActive)
        {
            UpdateTimerText();

            yield return new WaitForSeconds(1f);

            timeRemaining--;
        }

        UpdateTimerText();

        if (isGameActive)
            GameOver();
    }

    public void UpdateScore(int scoreToAdd)
    {
        if (!isGameActive)
            return;

        score += scoreToAdd;

        UpdateScoreText();
    }

    public void UpdateHearts()
    {
        if (!isGameActive)
            return;

        if (heartIndex >= heartScripts.Count)
        {
            GameOver();
            return;
        }

        Heart currentHeart = heartScripts[heartIndex];

        if (currentHeart != null)
        {
            currentHeart.DestroyHearts();
        }

        if (heartDestroyAudio != null)
        {
            heartDestroyAudio.Play();
        }

        heartIndex++;

        // End the game when all hearts are lost.
        if (heartIndex >= heartScripts.Count)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        isGameActive = false;

        StopAllCoroutines();

        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }

        if (gameScreen != null)
            gameScreen.SetActive(false);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Cursor.visible = true;

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
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + timeRemaining;
        }
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }


    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
