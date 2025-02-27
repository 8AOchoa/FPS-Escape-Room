using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static int destroyedTargets = 0;
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Destroy(collision.gameObject);
            destroyedTargets++;

            if (destroyedTargets >= 3)
            {
                GameObject.Find("Door").SetActive(false);
            }
        }
        Destroy(gameObject);
    }
}
