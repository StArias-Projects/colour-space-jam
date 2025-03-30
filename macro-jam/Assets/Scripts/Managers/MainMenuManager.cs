using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private Button startGameButton;

    [SerializeField]
    private StudioEventEmitter startGameSFX;

    [SerializeField]
    private Button quitGameButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(() =>
        {
            GameManager.GetInstance().LoadGame();
            startGameSFX.Play();
        });

#if UNITY_WEBGL
        quitGameButton.gameObject.SetActive(false);
#else
        quitGameButton.onClick.AddListener(() =>
        {
            GameManager.GetInstance().QuitGame();
        });
#endif
    }
}
