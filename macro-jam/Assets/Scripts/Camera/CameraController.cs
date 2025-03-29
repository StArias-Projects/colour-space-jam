using System;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections;

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

    private GamePlayManager gamePlayManager;
    private Camera cam;
    private float defaultAspect = 0;

    // Start is called before the first frame update
    public void SetUp(GamePlayManager gpManager)
    {
        gamePlayManager = gpManager;
        defaultAspect = defaultResolution.width / defaultResolution.height;
        cam = Camera.main;

        AdjustCameraFOV();
        PlayerManager.OnPlayerTakeDamage += OnPlayerTakeDamage;
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
        yield return new WaitForSecondsRealtime(.1f);
        Time.timeScale = 1;
        transform.DOShakePosition(.4f, withForce, 50);
    }
  
}
