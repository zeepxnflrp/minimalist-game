using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x > 12f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Die();
            Destroy(gameObject);
        }
    }
}