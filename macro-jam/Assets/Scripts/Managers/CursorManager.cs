using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D UITexture;

    [SerializeField]
    private Texture2D gameTexture;

    private static CursorManager Instance;
    private GameState gameState = GameState.Opening;
    public CursorManager GetInstance()
    {
        if (Instance == null)
            Instance = this;
        return Instance;
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ChangeCursorTexture(GameState gameState)
    {
        this.gameState = gameState;
        Texture2D newCursor = null;

        switch (gameState)
        {
            case GameState.Opening:
            case GameState.Pause:
            case GameState.GameOver:
                newCursor = UITexture;
                break;
            case GameState.Playing:
                newCursor = gameTexture;
                break;
        }

        if (newCursor != null)
        {
            Vector2 hotspot = new Vector2(newCursor.width / 2f, newCursor.height / 2f);
            Cursor.SetCursor(newCursor, hotspot, CursorMode.Auto);
        }
        else
            Debug.LogWarning("CursorManager - Cursor texture is null! Keeping the current cursor.");
    }
}
