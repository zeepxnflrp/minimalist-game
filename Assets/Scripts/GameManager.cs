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

    private float distance = 0f;

    private bool isPaused = false;
    private bool gameOver = false;

    private int bestDistance;

    void Awake()
    {
        Instance = this;

        Time.timeScale = 1f;
    }

    void Start()
    {
        bestDistance = PlayerPrefs.GetInt("BestDistance", 0);

        pauseText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);

        UpdateDistanceText();
    }

    void Update()
    {
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

        Time.timeScale = 0f;
    }
}