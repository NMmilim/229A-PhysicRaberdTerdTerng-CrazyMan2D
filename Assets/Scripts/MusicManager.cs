using System.Collections;
using TMPro;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameMusic;

    [Header("Time Stages")]
    [SerializeField] private TextMeshProUGUI stageText;

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

    void Start()
    {
        ChangeMusic(mainMenuMusic);
    }

    // ---------------- PUBLIC CONTROL ----------------

    public void PlayMainMenu()
    {
        ChangeMusic(mainMenuMusic);
    }

    public void PlayGame()
    {
        ChangeMusic(gameMusic);
    }

    // ---------------- CORE SYSTEM ----------------

    public void ChangeMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (currentClip == clip) return;

        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(FadeMusic(clip));
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = audioSource.volume;

        // FADE OUT
        for (float t = 0; t < 1f; t += Time.unscaledDeltaTime * 2f)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.volume = 0f;

        audioSource.clip = newClip;
        audioSource.loop = true;
        audioSource.Play();

        currentClip = newClip;

        // FADE IN
        for (float t = 0; t < 1f; t += Time.unscaledDeltaTime * 2f)
        {
            audioSource.volume = Mathf.Lerp(0f, startVolume, t);
            yield return null;
        }

        audioSource.volume = startVolume;
    }
}