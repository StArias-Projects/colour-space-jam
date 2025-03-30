using System;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    [Serializable]
    public struct Resolution
    {
        public float width;
        public float height;
    }

    [SerializeField]
    private float defaultSize;

    [SerializeField]
    private Resolution defaultResolution;

    [SerializeField]
    private Volume postProcessVolume;

    private GamePlayManager gamePlayManager;
    private Camera cam;
    private float defaultAspect = 0;

    private ChromaticAberration chromaticAberrationPostProcess;

    // Start is called before the first frame update
    public void SetUp(GamePlayManager gpManager)
    {
        gamePlayManager = gpManager;
        defaultAspect = defaultResolution.width / defaultResolution.height;
        cam = Camera.main;

        AdjustCameraFOV();
        PlayerManager.OnPlayerTakeDamage += OnPlayerTakeDamage;

        postProcessVolume.profile.TryGet(out chromaticAberrationPostProcess);
    }

    private void OnDestroy()
    {
        PlayerManager.OnPlayerTakeDamage -= OnPlayerTakeDamage;
    }

    void AdjustCameraFOV()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        // Adjust orthographic size instead of FOV
        cam.orthographicSize = defaultSize * (currentAspect / defaultAspect);
    }


    void OnPlayerTakeDamage(float amount)
    {
        StartCoroutine(ShakeCamera(amount / 5f));
    }

    IEnumerator ShakeCamera(float withForce)
    {
        Time.timeScale = 0;


        DOTween.To(() => chromaticAberrationPostProcess.intensity.value, (x) => chromaticAberrationPostProcess.intensity.value = x, .4f, .05f).SetEase(Ease.OutSine).SetUpdate(true);
        yield return new WaitForSecondsRealtime(.1f);
        Sequence sequence = DOTween.Sequence();
        
        DOTween.To(() => chromaticAberrationPostProcess.intensity.value, (x) => chromaticAberrationPostProcess.intensity.value = x, 0, 1.5f).SetEase(Ease.InSine);
        

        Time.timeScale = 1;
        transform.DOShakePosition(.5f, withForce, 50);
    }
  
}
