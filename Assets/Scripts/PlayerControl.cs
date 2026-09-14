using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float riseSpeed = 5f;
    public float fallSpeed = 4f;

    public float minY = -4.3f;
    public float maxY = 4.3f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireInterval = 0.3f;

    private float nextFireTime;

    [Header("Gravity Shift")]
    public float gravityShiftDuration = 10f;
    public TMP_Text gravityShiftText;

    private bool gravityShifted = false;
    private float gravityShiftTimer = 0f;
    private bool wasTouchingCeiling = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip gravityShiftSound;

    [Range(0f, 1f)]
    public float shootVolume = 0.1f;

    [Range(0f, 1f)]
    public float gravityShiftVolume = 0.4f;

    void Start()
    {
        if (gravityShiftText != null)
        {
            gravityShiftText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying || Time.timeScale <= 0f) return;
        
        // gravity shift
        if (gravityShifted)
        {
            gravityShiftTimer -= Time.deltaTime;

            if (gravityShiftText != null)
            {
                gravityShiftText.text =
                    $"gravity reversed  {gravityShiftTimer:F1}s";
            }

            if (gravityShiftTimer <= 0f)
            {
                gravityShifted = false;
                gravityShiftTimer = 0f;

                if (gravityShiftText != null)
                {
                    gravityShiftText.gameObject.SetActive(false);
                }
            }
        }

        // movement
        if (!gravityShifted)
        {
            // normal gravity
            if (Keyboard.current.spaceKey.isPressed)
            {
                transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            }
        }
        else
        {
            // reversed gravity
            if (Keyboard.current.spaceKey.isPressed)
            {
                transform.position += Vector3.down * riseSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.up * fallSpeed * Time.deltaTime;
            }
        }

        // keep player inside screen
        Vector3 position = transform.position;
        bool touchingCeiling = position.y >= maxY;
        position.y = Mathf.Clamp(position.y, minY, maxY);
        transform.position = position;

        if (touchingCeiling && !wasTouchingCeiling)
        {
            ActivateGravityShift();
        }

        wasTouchingCeiling = touchingCeiling;

        // auto fire
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireInterval;
        }
    }

    void Fire()
    {
        Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
    }

    void ActivateGravityShift()
    {
        gravityShifted = true;
        gravityShiftTimer = gravityShiftDuration;

        if (audioSource != null && gravityShiftSound != null)
        {
            audioSource.PlayOneShot(gravityShiftSound, gravityShiftVolume);
        }

        if (gravityShiftText != null)
        {
            gravityShiftText.gameObject.SetActive(true);

            gravityShiftText.text =
                $"gravity reversed  {gravityShiftTimer:F1}s";
        }
    }


    public void StopPlayerAudio()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}