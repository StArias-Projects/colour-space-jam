using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    private float startingVolume;

    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private Slider sfxSlider;

    FMOD.Studio.Bus musicBus;
    FMOD.Studio.Bus sfxBus;

    private static AudioManager Instance;

    public AudioManager GetInstance()
    {
        return Instance;
    }

    private void Awake()
    {
        if (Instance)
        {
            Instance.musicSlider = musicSlider;
            Instance.sfxSlider = sfxSlider;
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            musicBus = RuntimeManager.GetBus("bus:/Music");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");

            DontDestroyOnLoad(gameObject);
        }

        SetUp();
    }

    private void SetUp()
    {
        if (musicSlider)
        {
            musicSlider.maxValue = 1f;
            musicSlider.value = startingVolume;
            SetMusicVolume(startingVolume);
            musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(musicSlider.value); });
        }

        if (sfxSlider)
        {
            sfxSlider.maxValue = 1f;
            sfxSlider.value = startingVolume;
            SetSFXVolume(startingVolume);
            sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(sfxSlider.value); });
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicBus.setVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxBus.setVolume(volume);
    }
}
