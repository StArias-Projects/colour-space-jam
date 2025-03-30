using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicManager : MonoBehaviour
{

    [SerializeField]
    List<AudioClip> musicClips;

    AudioSource musicSource;
    int currentIndex;

    static MusicManager Instance;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        musicSource = GetComponent<AudioSource>();
        currentIndex = Random.Range(0, musicClips.Count);
        PlayNextClip();
    }

    void PlayNextClip()
    {
        currentIndex = (currentIndex + 1) % musicClips.Count;
        musicSource.clip = musicClips[currentIndex];
        musicSource.Play();
        Invoke("PlayNextClip", musicSource.clip.length);
    }
}
