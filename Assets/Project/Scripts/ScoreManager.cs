using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score")]
    public int score;
    [SerializeField] private int winScore = 100;

    [Header("Timer")]
    [SerializeField] private float timeLimit = 60f;
    private float currentTime;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Buttons")]
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject menuButton;

    [Header("Scene")]
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private float delayBeforeLoad = 2f;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentTime = timeLimit;

        UpdateUI();

        if (winText != null) winText.gameObject.SetActive(false);
        if (loseText != null) loseText.gameObject.SetActive(false);

        if (restartButton != null) restartButton.SetActive(false);
        if (menuButton != null) menuButton.SetActive(false);
    }

    void Update()
    {
        if (gameEnded) return;

        // TIMER
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            LoseGame();
        }

        if (timerText != null)
            timerText.text = $"Time: {currentTime:F1}";
    }

    public void AddScore(int amount)
    {
        if (gameEnded) return;

        score += amount;
        UpdateUI();

        if (score >= winScore)
        {
            WinGame();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    void WinGame()
    {
        gameEnded = true;

        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = "<size=150%><color=yellow>YOU WIN!</color></size>";
        }

        StartCoroutine(LoadWinScene());
    }

    void LoseGame()
    {
        gameEnded = true;

        if (loseText != null)
        {
            loseText.gameObject.SetActive(true);
            loseText.text = "<size=150%><color=red>YOU LOSE!</color></size>";
        }

        ShowButtons();
    }

    IEnumerator LoadWinScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        Time.timeScale = 1f; // safety reset

        SceneManager.LoadScene(winSceneName);
    }

    void ShowButtons()
    {
        if (restartButton != null) restartButton.SetActive(true);
        if (menuButton != null) menuButton.SetActive(true);
    }

    // used by PauseManager
    public bool IsGameEnded()
    {
        return gameEnded;
    }

    // UI BUTTONS (optional reuse)
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}