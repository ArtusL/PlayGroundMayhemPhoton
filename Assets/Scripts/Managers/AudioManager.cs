using UnityEngine;
using Photon.Pun;

public class AudioManager : MonoBehaviourPun
{
    [SerializeField]
    private AudioClip runSound;
    [SerializeField]
    private AudioClip sprintSound;
    [SerializeField]
    private AudioClip localrunSound;
    [SerializeField]
    private AudioClip localsprintSound;
    // You can uncomment other sounds as needed
    //[SerializeField]
    //private AudioClip jumpSound;
    //[SerializeField]
    //private AudioClip landSound;
    //[SerializeField]
    //private AudioClip powerupPickupSound;
    //[SerializeField]
    //private AudioClip powerupUseSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Make sure each player prefab has an AudioSource component.
        if (!audioSource)
        {
            Debug.LogError("Missing AudioSource component!");
            return;
        }

        audioSource.spatialBlend = 1.0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 50;
    }


    public void PlayLocalSprintSound()
    {
        if (photonView.IsMine)
        {
            PlaySound(localsprintSound);
            Debug.Log("Local sprint sound played.");
        }
    }

    public void PlayLocalRunSound()
    {
        if (photonView.IsMine)
        {
            PlaySound(localrunSound);
            Debug.Log("Local run sound played.");
        }
    }

    [PunRPC]
    public void PlayRunSound()
    {
        Debug.Log("Attempting to play run sound");
        PlaySound(runSound);
    }

    [PunRPC]
    public void PlaySprintSound()
    {
        Debug.Log("Attempting to play sprint sound");
        PlaySound(sprintSound);
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
            audioSource.loop = true;
            Debug.Log("Playing sound: " + clip.name);
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is missing");
        }
    }

    [PunRPC]
    public void StopRunningSound()
    {
        if (audioSource.clip == runSound)
        {
            audioSource.loop = false;
            //Debug.Log("Stopping run sound");
            audioSource.Stop();
        }
    }

    [PunRPC]
    public void StopSprintSound()
    {
        if (audioSource.clip == sprintSound)
        {
            audioSource.loop = false;
            //Debug.Log("Stopping sprint sound");
            audioSource.Stop();
        }
    }

    public void StopLocalRunningSound()
    {
        if (photonView.IsMine && audioSource.clip == localrunSound)
        {
            audioSource.loop = false;
            //Debug.Log("Stopping local run sound");
            audioSource.Stop();
        }
    }

    public void StopLocalSprintSound()
    {
        if (photonView.IsMine && audioSource.clip == localsprintSound)
        {
            audioSource.loop = false;
            Debug.Log("Stopping local sprint sound");
            audioSource.Stop();
        }
    }

    public AudioClip GetCurrentClip()
    {
        return audioSource.clip;
    }

    public bool IsPlayingLocalRunSound()
    {
        return audioSource.clip == localrunSound;
    }

    public bool IsPlayingLocalSprintSound()
    {
        return audioSource.clip == localsprintSound;
    }

    // You can uncomment other sound-playing methods as needed
    //[PunRPC]
    //public void PlayJumpSound()
    //{
    //    PlaySound(jumpSound);
    //}

    //[PunRPC]
    //public void PlayLandSound()
    //{
    //    PlaySound(landSound);
    //}

    //[PunRPC]
    //public void PlayPowerupPickupSound()
    //{
    //    PlaySound(powerupPickupSound);
    //}

    //[PunRPC]
    //public void PlayPowerupUseSound()
    //{
    //    PlaySound(powerupUseSound);
    //}
}
