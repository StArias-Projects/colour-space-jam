using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private Button startGameButton;

    [SerializeField]
    private StudioEventEmitter startGameSFX;

    private void Start()
    {
        startGameButton.onClick.AddListener(() =>
        {
            GameManager.GetInstance().LoadGame();
            startGameSFX.Play();
        });
    }
}
