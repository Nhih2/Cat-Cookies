using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [Header("Audio Clip")]
    [SerializeField] private AudioClip musicClip;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.clip = musicClip;
    }

    void Start()
    {
        if (musicClip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"{name}: No audio clip assigned!");
        }
    }
}
