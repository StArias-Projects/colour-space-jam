using System.Collections.Generic;
using UnityEngine;

public class AudioSourceRandomiser : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> clips;

    [SerializeField]
    float pitchVariation = .1f;


    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = 1 + Random.Range(-pitchVariation, pitchVariation);
        audioSource.clip = clips[Random.Range(0, clips.Count)];
        audioSource.Play();
    }
}
