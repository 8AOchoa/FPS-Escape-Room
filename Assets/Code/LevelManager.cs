using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Target[] targets; // Assign all 3 targets in Inspector
    public GameObject door; // Assign your door GameObject
    public Vector3 openPositionOffset = new Vector3(0, 5, 0); // Door movement
    public AudioClip doorOpenMusic; // Your "You Win" clip

    private Vector3 closedPosition;
    private bool isDoorOpen = false;
    private AudioSource audioSource;

    private void Start()
    {
        closedPosition = door.transform.position;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = doorOpenMusic;
        audioSource.loop = false; // No looping
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (!isDoorOpen && AreAllTargetsDestroyed())
        {
            OpenDoor();
        }
    }

    private bool AreAllTargetsDestroyed()
    {
        foreach (Target target in targets)
        {
            if (target != null) // If any target still exists
                return false;
        }
        return true;
    }

    private void OpenDoor()
    {
        door.transform.position = closedPosition + openPositionOffset;
        isDoorOpen = true;
        
        // Play "You Win" audio once
        if (audioSource != null && doorOpenMusic != null)
        {
            audioSource.PlayOneShot(doorOpenMusic); // Plays once, no looping
        }
        else
        {
            Debug.LogWarning("AudioSource or doorOpenMusic not set up!");
        }
    }
}