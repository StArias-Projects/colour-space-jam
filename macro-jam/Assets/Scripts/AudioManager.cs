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

    private static FMOD.Studio.Bus musicBus;
    private static FMOD.Studio.Bus sfxBus;

    private static AudioManager Instance;
    private static float musicVolume;
    private static float sfxVolume;

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

            musicVolume = sfxVolume = startingVolume;
            DontDestroyOnLoad(gameObject);
        }

        SetUp();
    }

    private void SetUp()
    {
        if (musicSlider)
        {
            musicSlider.maxValue = 1f;
            musicSlider.value = musicVolume;
            musicBus.setVolume(musicVolume);

            musicSlider.onValueChanged.AddListener((float value) =>
            {
                musicVolume = value;
                musicBus.setVolume(musicVolume);
            });
        }

        if (sfxSlider)
        {
            sfxSlider.maxValue = 1f;
            sfxSlider.value = sfxVolume;
            sfxBus.setVolume(sfxVolume);

            sfxSlider.onValueChanged.AddListener((float value) =>
            {
                sfxVolume = value;
                sfxBus.setVolume(sfxVolume);
            });
        }
    }
}
