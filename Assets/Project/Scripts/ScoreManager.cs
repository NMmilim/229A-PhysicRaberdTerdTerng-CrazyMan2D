using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private Coroutine sceneRoutine;
    private bool isRestarting = false;

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

    [Header("Scene")]
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private float delayBeforeLoad = 2f;

    [Header("Restart")]
    [SerializeField] private float restartDelay = 3f;
    private Coroutine restartRoutine;

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
    }
    void RestartGame()
    {
        if (isRestarting) return;

        isRestarting = true;

        if (restartRoutine != null)
            StopCoroutine(restartRoutine);

        restartRoutine = StartCoroutine(RestartCountdown());
    }
    IEnumerator RestartCountdown()
    {
        float t = restartDelay;

        while (t > 0f)
        {
            if (timerText != null)
                timerText.text = $"Restarting in {Mathf.Ceil(t)}...";

            yield return new WaitForSeconds(1f);
            t--;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = $"Time: {minutes:00}:{seconds:00}";

        if (currentTime <= 10f)
        {
            timerText.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 5f, 1f));
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            RestartGame();
        }

        if (isRestarting) return; // block UI override during countdown

        if (gameEnded) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            LoseGame();
        }

        UpdateTimerUI();
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
        if (isRestarting) return;

        gameEnded = true;

        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = "<size=150%><color=yellow>YOU WIN!</color></size>";
        }

        sceneRoutine = StartCoroutine(LoadSceneAfterDelay(winSceneName));
    }

    void LoseGame()
    {
        if (isRestarting) return;

        gameEnded = true;

        if (loseText != null)
        {
            loseText.gameObject.SetActive(true);
            loseText.text = "<size=150%><color=red>YOU LOSE!</color></size>";
        }

        sceneRoutine = StartCoroutine(LoadSceneAfterDelay(mainMenuScene));
    }

    IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        Time.timeScale = 1f; // safety reset

        SceneManager.LoadScene(sceneName);
    }

    // used by PauseManager
    public bool IsGameEnded()
    {
        return gameEnded;
    }
}