using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;

    private PlayerManager _player;

    public void SetUp(PlayerManager player) 
    {
        _player = player;    
    }

    private void Update()
    {
        if (_player.GetGameState() != GamePlayManager.GameState.Playing)
            return;

        RotateShields();
    }

    void RotateShields()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - _player.transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotación suavizada
        float smoothAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }
}
