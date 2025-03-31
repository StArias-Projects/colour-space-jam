using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _movementForce;
    [SerializeField] LayerMask boundaryLayer;
    [SerializeField] private float bounceForce;
    [SerializeField] private float velocityThreshold;

    private Vector2 _input;
    // In case we want to slow down the player, we need this
    private float currentMaxSpeed = 0;
    private PlayerManager _player;
    private bool isBounced = false;
    private Vector2 bounceDir = Vector2.zero;
    private Vector2 currentVelocity;
    private PlayerSFXController playerSFXController;
    private bool isMoving = false;
    private bool wasMoving = false;

    private void OnEnable()
    {
        GamePlayManager.OnGamePaused += OnGamePaused;
    }

    private void OnDisable()
    {
        GamePlayManager.OnGamePaused -= OnGamePaused;
    }

    public void SetUp(PlayerManager player, PlayerSFXController controller)
    {
        currentMaxSpeed = _maxSpeed;
        _player = player;
        playerSFXController = controller;
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

        if (isBounced)
        {
            _rb.AddForce(bounceDir * bounceForce, ForceMode2D.Impulse);
            isBounced = false;
            return;
        }

        currentVelocity = _rb.linearVelocity;

        _rb.AddForce(_input * _movementForce, ForceMode2D.Force);

        if (_rb.linearVelocityX > currentMaxSpeed)
            currentVelocity.x = currentMaxSpeed;
        if (_rb.linearVelocityY > currentMaxSpeed)
            currentVelocity.y = currentMaxSpeed;

        _rb.linearVelocity = currentVelocity;
    }

    private void Update()
    {
        isMoving = _input.sqrMagnitude > velocityThreshold * velocityThreshold;

        if (isMoving && !wasMoving)
            playerSFXController.PlayMovementSFX();
        else if (!isMoving && wasMoving)
            playerSFXController.StopMovementSFX();

        wasMoving = isMoving;
    }

    private void OnGamePaused()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int mask = 1 << collision.gameObject.layer;

        if ((mask & boundaryLayer.value) != 0)
        {
            Vector2 normal = collision.GetContact(0).normal;
            bounceDir = Vector2.Reflect(currentVelocity, normal).normalized;
            isBounced = true;
            playerSFXController.PlayWallBounce();
        }
    }
}
