using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _movementForce;

    private Vector2 _input;
    // In case we want to slow down the player, we need this
    private float currentMaxSpeed = 0;
    private PlayerManager _player;

    private void OnEnable()
    {
        GamePlayManager.OnGamePaused += OnGamePaused;
    }

    private void OnDisable()
    {
        GamePlayManager.OnGamePaused -= OnGamePaused;
    }

    public void SetUp(PlayerManager player)
    {
        currentMaxSpeed = _maxSpeed;
        _player = player;
    }

    public void OnMove(CallbackContext value)
    {
        if (!_player || _player.GetGameState() != GameState.Playing)
            return;

        _input = value.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!_player || _player.GetGameState() != GameState.Playing)
            return;

        _rb.AddForce(_input * _movementForce, ForceMode2D.Force);

        Vector2 currentVelocity = _rb.linearVelocity;
        if (_rb.linearVelocityX > currentMaxSpeed)
            currentVelocity.x = currentMaxSpeed;
        if(_rb.linearVelocityY > currentMaxSpeed)
            currentVelocity.y = currentMaxSpeed;

        _rb.linearVelocity = currentVelocity;
    }

    private void OnGamePaused() 
    {
        _rb.linearVelocity = Vector2.zero;
    }
}
