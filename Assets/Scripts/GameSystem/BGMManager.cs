using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip defaultBGM;

    private AudioClip currentBGM;

    private void Start()
    {
        PlayDefaultBGM();
    }

    public void PlayDefaultBGM()
    {
        currentBGM = defaultBGM;
        PlayClip(defaultBGM);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        currentBGM = clip;
        PlayClip(clip);
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }

    private void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void RestoreToDefaultBGM()
    {
        if (currentBGM != defaultBGM)
        {
            PlayDefaultBGM();
        }
    }
}
