using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private Button startGameButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(() =>
        {
            GameManager.GetInstance().ChangeScene("GamePlay");
        });
    }
}
