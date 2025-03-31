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

    [SerializeField] private GameObject leftWall, rightWall, topWall, bottomWall;

    private GamePlayManager gamePlayManager;
    private Camera cam;
    private float defaultAspect = 0;
    private int lastScreenWidth, lastScreenHeight;

    private ChromaticAberration chromaticAberrationPostProcess;

    // Start is called before the first frame update
    public void SetUp(GamePlayManager gpManager)
    {
        gamePlayManager = gpManager;
        defaultAspect = defaultResolution.width / defaultResolution.height;
        cam = Camera.main;

        AdjustCameraAndWalls();
        PlayerManager.OnPlayerTakeDamage += OnPlayerTakeDamage;

        postProcessVolume.profile.TryGet(out chromaticAberrationPostProcess);
    }

    void Update()
    {
        // If the resolution changes (e.g., window resize), update the camera
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            AdjustCameraAndWalls();
        }
    }

    private void OnDestroy()
    {
        PlayerManager.OnPlayerTakeDamage -= OnPlayerTakeDamage;
    }

    void AdjustCameraSize()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float baseAspect = defaultResolution.width / defaultResolution.height;

        // Adjust the orthographic size to maintain the same width
        cam.orthographicSize = defaultSize * (baseAspect / screenAspect);
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    void AdjustCameraAndWalls()
    {
        // Calculate screen aspect ratio
        float screenAspect = (float)Screen.width / Screen.height;
        float baseAspect = defaultResolution.width / defaultResolution.height;

        // Adjust the camera's orthographic size based on the width
        cam.orthographicSize = defaultSize * (baseAspect / screenAspect);

        // Adjust the walls based on the camera's orthographic size
        float cameraHeight = cam.orthographicSize * 2;  // Camera height in world units (top to bottom)
        float cameraWidth = cameraHeight * screenAspect; // Camera width in world units (left to right)

        // Move the walls based on the camera's visible width and height in world units
        UpdateWallPositions(cameraWidth, cameraHeight);
    }

    void UpdateWallPositions(float cameraWidth, float cameraHeight)
    {
        // Place the walls at the correct positions based on the camera's size
        leftWall.transform.position = new Vector3(-cameraWidth / 2, 0, 0);
        rightWall.transform.position = new Vector3(cameraWidth / 2, 0, 0);
        topWall.transform.position = new Vector3(0, cameraHeight / 2, 0);
        bottomWall.transform.position = new Vector3(0, -cameraHeight / 2, 0);
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
