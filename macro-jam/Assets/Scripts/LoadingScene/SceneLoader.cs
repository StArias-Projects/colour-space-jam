using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField]
    private Slider loadingBar;

    [SerializeField]
    private GameObject loadingCanvas;

    private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    private float totalProgress = 0f;

    private void Awake()
    {
        gameManager = GameManager.GetInstance();
        loadingCanvas.SetActive(false);
        loadingBar.maxValue = 100f;
    }

    public void LoadGame()
    {
        loadingCanvas.SetActive(true);

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneID.MainMenu));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneID.GamePlay, LoadSceneMode.Additive));

        StartCoroutine(UpdateProgress());
    }

    public void ReturnToMainMenu() 
    {
        loadingCanvas.SetActive(true);

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneID.GamePlay));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneID.MainMenu, LoadSceneMode.Additive));

        StartCoroutine(UpdateProgress());
    }

    public IEnumerator UpdateProgress()
    {
        bool allScenesDone = false;
        loadingBar.value = 0f;

        while (!allScenesDone)
        {
            totalProgress = 0f;
            allScenesDone = true;

            foreach (AsyncOperation scene in scenesLoading)
            {
                totalProgress += scene.progress;
                if (!scene.isDone)
                    allScenesDone = false;
            }

            totalProgress = (totalProgress / scenesLoading.Count) * 100f;
            loadingBar.value = Mathf.Round(totalProgress);

            yield return null;  // Wait for the next frame before checking again
        }

        loadingCanvas.SetActive(false);
        scenesLoading.Clear();
    }
}
