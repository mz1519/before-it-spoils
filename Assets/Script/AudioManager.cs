using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip clickClip;
    public AudioClip cookClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    public void PlayClick()
    {
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }

    public void PlayCook()
    {
        if (sfxSource != null && cookClip != null)
        {
            sfxSource.PlayOneShot(cookClip);
        }
    }
}