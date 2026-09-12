using UnityEngine;
using UnityEngine.InputSystem;

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
    public float fireInterval = 0.30f;

    private float nextFireTime;

    void Update()
    {
        // movement
        if (Keyboard.current.spaceKey.isPressed)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        // keep player inside screen
        Vector3 position = transform.position;
        position.y = Mathf.Clamp(position.y, minY, maxY);
        transform.position = position;

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
    }
}