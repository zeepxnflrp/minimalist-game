using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 4f;

    private Transform player;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direction =
                (player.position - transform.position).normalized;

            transform.position +=
                direction * speed * Time.deltaTime;
        }

        if (
            transform.position.x < -12f ||
            transform.position.x > 12f ||
            transform.position.y > 7f ||
            transform.position.y < -7f
        )
        {
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.EndGame();
        }
    }
}