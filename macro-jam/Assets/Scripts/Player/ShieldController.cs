using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private Transform _player;

    [SerializeField] private float rotationSpeed = 5f; // Velocidad de giro (editable desde el Inspector)

    private void Start()
    {
        _player = transform.parent; // El Player es el padre del ShieldParent
    }

    private void Update()
    {
        RotateShields();
    }

    void RotateShields()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - _player.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotación suavizada
        float smoothAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }
}
