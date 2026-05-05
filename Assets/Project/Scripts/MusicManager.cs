using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Music")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip mainMenuMusic;

    [SerializeField] private AudioClip earlyMusic;
    [SerializeField] private AudioClip midMusic;
    [SerializeField] private AudioClip lateMusic;
    [SerializeField] private AudioClip finalMusic;

    [Header("Time System")]
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private float timeLimit = 120f;

    private float currentTime;
    private bool gameEnded;
    public float CurrentTime => currentTime;
    public float TimeLimit => timeLimit;
    private enum TimeStage
    {
        Early,
        Mid,
        Late,
        Final
    }

    private TimeStage currentStage;

    private AudioClip currentClip;
    private Coroutine musicRoutine;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (scoreManager == null)
            scoreManager = ScoreManager.Instance;

        ChangeMusic(mainMenuMusic);
      
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Start")
        {
            PlayMainMenu();
            return;
        }

        if (scene.name == "EndCredits")
        {
            StopMusic();
            return;
        }

        // Any gameplay scene
        PlayGame();
    }
    public void StopMusic()
    {
        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        audioSource.Stop();
        currentClip = null;
    }

    public void PlayGame()
    {
        ChangeMusic(earlyMusic);
        currentStage = TimeStage.Early;
    }
    void Update()
    {
        if (gameEnded) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            LoseGame();
        }

        UpdateTimeStage();
    }

    void UpdateTimeStage()
    {
        if (scoreManager == null) return;

        float currentTime = scoreManager.CurrentTime;
        float timeLimit = scoreManager.TimeLimit;

        float t = currentTime / timeLimit; // 1 -> 0

        TimeStage newStage;

        if (t > 0.66f)
            newStage = TimeStage.Early;   // 90–60s
        else if (t > 0.33f)
            newStage = TimeStage.Mid;     // 60–30s
        else if (t > 0.10f)
            newStage = TimeStage.Late;    // 30–10s
        else
            newStage = TimeStage.Final;   // 10–0s

        if (newStage != currentStage)
        {
            currentStage = newStage;
            ApplyStage();
        }
    }

    void ApplyStage()
    {
        if (stageText == null) return;

        AudioClip targetMusic = null;

        switch (currentStage)
        {
            case TimeStage.Early:
                stageText.text = "EARLY GAME";
                stageText.color = Color.green;
                targetMusic = earlyMusic;
                break;

            case TimeStage.Mid:
                stageText.text = "MID GAME";
                stageText.color = Color.yellow;
                targetMusic = midMusic;
                break;

            case TimeStage.Late:
                stageText.text = "LATE GAME";
                stageText.color = new Color(1f, 0.5f, 0f);
                targetMusic = lateMusic;
                break;

            case TimeStage.Final:
                stageText.text = "FINAL RUSH!";
                stageText.color = Color.red;
                targetMusic = finalMusic;
                break;
        }

        // change music when stage changes
        if (targetMusic != null)
        {
            ChangeMusic(targetMusic);
        }
    }

    void LoseGame()
    {
        gameEnded = true;
        Debug.Log("Game Over");
    }

    // ---------------- MUSIC ----------------

    public void PlayMainMenu() => ChangeMusic(mainMenuMusic);

    public void ChangeMusic(AudioClip clip)
    {
        if (clip == null || currentClip == clip) return;

        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(FadeMusic(clip));
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < 1f; t += Time.unscaledDeltaTime * 2f)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.loop = true;
        audioSource.Play();
        currentClip = newClip;

        for (float t = 0; t < 1f; t += Time.unscaledDeltaTime * 2f)
        {
            audioSource.volume = Mathf.Lerp(0f, startVolume, t);
            yield return null;
        }
    }
}