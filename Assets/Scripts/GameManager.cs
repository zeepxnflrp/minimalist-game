using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scoring")]
    public float metersPerSecond = 10f;
    public TMP_Text distanceText;

    [Header("UI")]
    public TMP_Text pauseText;
    public TMP_Text gameOverText;


    [Header("Audio")]
    public AudioSource menuAudioSource;
    public AudioSource sfxAudioSource;

    public AudioClip menuMusic;
    public AudioClip deathSound;

    [Range(0f, 1f)]
    public float menuVolume = 0.25f;

    [Range(0f, 1f)]
    public float deathVolume = 0.4f;

    private float distance = 0f;

    private bool isPaused = false;
    private bool gameOver = false;

    private int bestDistance;
    private bool hasStarted;
    public GameObject startPanel;
    public bool IsPlaying { get { return hasStarted && !isPaused && !gameOver; } }

    void Awake()
    {
        Instance = this;

        Time.timeScale = 0f;
    }

    void Start()
    {
        bestDistance = PlayerPrefs.GetInt("BestDistance", 0);

        pauseText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        distanceText.gameObject.SetActive(false);
        startPanel.SetActive(true);


        UpdateDistanceText();

        PlayMenuMusic();
    }

    void Update()
    {
        if (!hasStarted)
        {
            if (!hasStarted)
            {
                if (Keyboard.current != null &&
                    Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    StartGame();
                }

                return;
            }
        }

        // game ovaaaa
        if (gameOver)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Time.timeScale = 1f;

                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().name
                );
            }

            return;
        }

        // pause
        if (isPaused)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ResumeGame();
            }

            return;
        }

        // pause 
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseGame();
            return;
        }

        // distance
        distance += metersPerSecond * Time.deltaTime;

        UpdateDistanceText();
    }

    public void StartGame()
    {
        if (hasStarted) return;
        hasStarted = true;

        if (menuAudioSource != null)
        {
            menuAudioSource.Stop();
        }

        startPanel.SetActive(false);
        distanceText.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }

    void UpdateDistanceText()
    {
        distanceText.text =
            $"distance  {Mathf.FloorToInt(distance)} m";
    }

    void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;
        pauseText.gameObject.SetActive(true);
    }

    void ResumeGame()
    {
        isPaused = false;

        pauseText.gameObject.SetActive(false);

        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        if (!hasStarted) return;
        if (gameOver)
            return;

        gameOver = true;

        int finalDistance = Mathf.FloorToInt(distance);

        if (finalDistance > bestDistance)
        {
            bestDistance = finalDistance;

            PlayerPrefs.SetInt(
                "BestDistance",
                bestDistance
            );

            PlayerPrefs.Save();
        }

        distanceText.gameObject.SetActive(false);
        pauseText.gameObject.SetActive(false);

        gameOverText.text =
            $"you lose :C\n\n" +
            $"score {finalDistance} m\n" +
            $"best {bestDistance} m";

        gameOverText.gameObject.SetActive(true);
        
        PlayerController player = FindAnyObjectByType<PlayerController>();

        if (player != null)
        {
            player.StopPlayerAudio();
        }
        
        if (sfxAudioSource != null && deathSound != null)
        {
            sfxAudioSource.PlayOneShot(
                deathSound,
                deathVolume
            );
        }

        Time.timeScale = 0f;
    }

    void PlayMenuMusic(){
        if (menuAudioSource == null || menuMusic == null)
        {
            return;
        }

        menuAudioSource.clip = menuMusic;
        menuAudioSource.volume = menuVolume;
        menuAudioSource.loop = true;
        menuAudioSource.Play();
    }
}