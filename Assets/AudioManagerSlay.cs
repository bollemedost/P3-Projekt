using UnityEngine;

public class AudioManagerSlay : MonoBehaviour
{
    public static AudioManagerSlay Instance;

    // Audio clips for different actions
    public AudioClip jumpSoundClip;
    public AudioClip doubleJumpSoundClip;
    public AudioClip dashSoundClip;
    public AudioClip smashSoundClip;
    public AudioClip powerUpSoundClip;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keep the AudioManager persistent across scenes
        }
        else
        {
            Destroy(gameObject);  // If another instance exists, destroy this one
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the AudioManager!");
        }
    }

    // Method to play the jump sound
    public void PlayJumpSound()
    {
        if (jumpSoundClip != null)
        {
            audioSource.PlayOneShot(jumpSoundClip);
        }
    }

    // Method to play the double jump sound
    public void PlayDoubleJumpSound()
    {
        if (doubleJumpSoundClip != null)
        {
            audioSource.PlayOneShot(doubleJumpSoundClip);
        }
    }

    // Method to play the dash sound
    public void PlayDashSound()
    {
        if (dashSoundClip != null)
        {
            audioSource.PlayOneShot(dashSoundClip);
        }
    }

    // Method to play the smash sound
    public void PlaySmashSound()
    {
        if (smashSoundClip != null)
        {
            audioSource.PlayOneShot(smashSoundClip);
        }
    }

    // Method to play the power-up sound
    public void PlayPowerUpSound()
    {
        if (powerUpSoundClip != null)
        {
            audioSource.PlayOneShot(powerUpSoundClip);
        }
    }
}