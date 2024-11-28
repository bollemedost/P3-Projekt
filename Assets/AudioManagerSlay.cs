using UnityEngine;

public class AudioManagerSlay : MonoBehaviour
{
    public static AudioManagerSlay Instance;

    // References to audio sources for different types of sounds
    private AudioSource audioSource;
    public AudioClip playerAnimationClip;
    public AudioClip backgroundMusicClip;
    public AudioClip jumpSoundClip;

    // Ensure there is only one instance of the AudioManager
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevents this object from being destroyed on scene load
        }
        else
        {
            Destroy(gameObject); // If an instance already exists, destroy this one
        }

        // Create or get the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    // Play a sound when the player performs an animation (example)
    public void PlayAnimationSound()
    {
        if (playerAnimationClip != null)
        {
            audioSource.PlayOneShot(playerAnimationClip);
        }
    }

    // Play background music
    public void PlayBackgroundMusic()
    {
        if (backgroundMusicClip != null)
        {
            audioSource.clip = backgroundMusicClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Play a jump sound
    public void PlayJumpSound()
    {
        if (jumpSoundClip != null)
        {
            audioSource.PlayOneShot(jumpSoundClip);
        }
    }

    // Stop the current sound
    public void StopSound()
    {
        audioSource.Stop();
    }
}
