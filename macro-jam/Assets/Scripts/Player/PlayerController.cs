using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Rigidbody2D _rb;
	[SerializeField] private float _maxSpeed = 0.5f;

	private Vector2 _input;
	private float currentSpeed = 0;
	private PlayerManager _player;

    public void SetUp(PlayerManager player) 
	{
		currentSpeed = _maxSpeed;
		_player = player;
	}

    public void OnMove(CallbackContext value)
    {
        if (_player == null || _player.GetGameState() != GamePlayManager.GameState.Playing)
            return;

        _input = value.ReadValue<Vector2>();
        Debug.Log(_input);
    }

	private void FixedUpdate()
	{
		_rb.AddForce(_input * currentSpeed, ForceMode2D.Impulse);

    }
}
