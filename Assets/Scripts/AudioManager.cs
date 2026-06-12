using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Música")]
    public AudioClip menuBGM;
    public AudioClip gameBGM;
    [Range(0f, 1f)] public float bgmVolume = 0.35f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 5f)] public float fadeDuration = 1.5f;

    [Header("Efectos de Sonido")]
    public AudioClip hitSound;
    public AudioClip painSound;
    public AudioClip explosionSound;
    public AudioClip pickupSound;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayBGMForCurrentScene();
    }

    private void PlayBGMForCurrentScene()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "MainMenu")
            PlayBGM(menuBGM);
        else if (scene == "MainScene")
            PlayBGM(gameBGM);
    }

    private void Initialize()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        musicSource.volume = bgmVolume;

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        sfxSource.volume = sfxVolume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
            PlayBGM(menuBGM);
        else if (scene.name == "MainScene")
            PlayBGM(gameBGM);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.isPlaying && musicSource.clip == clip) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Crossfade(clip));
    }

    private IEnumerator Crossfade(AudioClip newClip)
    {
        if (musicSource.isPlaying && musicSource.clip != null)
        {
            float startVol = musicSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
                yield return null;
            }
            musicSource.Stop();
        }

        musicSource.clip = newClip;
        musicSource.volume = 0f;
        musicSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, bgmVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = bgmVolume;
        fadeRoutine = null;
    }

    public void StopBGM()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        if (musicSource.isPlaying)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float startVol = musicSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = bgmVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.volume = sfxVolume;
        sfxSource.PlayOneShot(clip);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }
}
