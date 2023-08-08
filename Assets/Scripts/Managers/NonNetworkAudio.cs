using UnityEngine;

public class NonNetworkAudio : MonoBehaviour
{
    public static NonNetworkAudio Instance;

    [SerializeField]
    private AudioClip mainMenuMusic;

    private AudioSource audioSource;
    public float volume = 0.1f;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing!");
            return;
        }
    }

    public void PlayMainMenuMusic()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing!");
            return;
        }

        audioSource.clip = mainMenuMusic;
        audioSource.loop = true;
        audioSource.Play();
        audioSource.volume = volume;
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
