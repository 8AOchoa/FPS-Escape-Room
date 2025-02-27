using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 10f; // Adjust this as needed
    public GameObject destroyEffect; // Optional: Assign a particle prefab for destruction

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            DestroyTarget();
        }
    }

    private void DestroyTarget()
    {
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}