using FMODUnity;
using System.Collections;
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

    public static AudioManager GetInstance()
    {
        return Instance;
    }

    public void Awake()
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

    public static IEnumerator MusicFadeOut(StudioEventEmitter emitter, bool isPaused, float durationInSeconds)
    {
        yield return new WaitUntil(() => Instance);

        float startVolume = musicVolume;
        float elapsedTime = 0f;

        while (elapsedTime < durationInSeconds)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0f, elapsedTime / durationInSeconds);

            if (!emitter)
                elapsedTime = durationInSeconds + 1;

            emitter.SetParameter("volume", Mathf.Max(newVolume, 0));
            yield return null;
        }

        if (emitter)
        {
            emitter.SetParameter("volume", 0f);
            if (isPaused)
                emitter.EventInstance.setPaused(true);
            else
                emitter.Stop();
        }
    }

    public static IEnumerator MusicFadeIn(StudioEventEmitter emitter, bool isContinued, float durationInSeconds)
    {
        yield return new WaitUntil(() => Instance);

        if (isContinued)
            emitter.EventInstance.setPaused(false);
        else
            emitter.Play();

        emitter.SetParameter("volume", 0);
        float startVolume = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < durationInSeconds)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float newVolume = Mathf.Lerp(startVolume, musicVolume, elapsedTime / durationInSeconds);
            if (!emitter)
                elapsedTime = durationInSeconds + 1;

            emitter.SetParameter("volume", Mathf.Min(newVolume, musicVolume));
            yield return null;
        }

        if (emitter)
        {
            emitter.SetParameter("volume", musicVolume);
        }
    }
}
