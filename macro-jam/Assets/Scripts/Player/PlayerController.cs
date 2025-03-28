using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private Vector2 _input;
	[SerializeField] private Rigidbody2D _rb;
	[SerializeField] private float _speed = 0.5f;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	private void OnMove(InputValue value)
	{
		_input = value.Get<Vector2>();
		Debug.Log(_input);
	}

	private void FixedUpdate()
	{
		_rb.linearVelocity = _input * _speed;
	}
}
