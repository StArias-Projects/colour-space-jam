using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void OnMove(InputValue value)
	{
		if(_player == null || _player.GetGameState() != GamePlayManager.GameState.Playing)
            return;

        _input = value.Get<Vector2>();
		Debug.Log(_input);
	}

	private void FixedUpdate()
	{
		_rb.linearVelocity = _input * currentSpeed;
	}
}
