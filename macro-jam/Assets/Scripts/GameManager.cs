using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    [SerializeField]
    private MainMenuManager mainMenuManager;

    private static GameManager Instance;

    public static GameManager GetInstance()
    {
        return Instance;
    }

    public void Awake()
    {
        if (Instance)
        {
            Instance.gamePlayManager = gamePlayManager;
            Instance.mainMenuManager = mainMenuManager;
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
