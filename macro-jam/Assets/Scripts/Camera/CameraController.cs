using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Serializable]
    public struct Resolution
    {
        public float width;
        public float height;
    }

    [SerializeField]
    private float defaultFOV;

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
    }

    void AdjustCameraFOV()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        cam.fieldOfView = defaultFOV * (defaultAspect / currentAspect);
    }
}
