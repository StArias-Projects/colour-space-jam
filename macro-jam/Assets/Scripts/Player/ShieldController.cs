using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;

public class ShieldController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private float secondsToRotateShieldsOnClick = .2f;

    [SerializeField] private List<Shield> shields;

    private int _timesShieldsRotated = 0;

    private Tween _rotateTween;

    [SerializeField]
    private Transform clickRotator;

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

    public void OnLeftClick(CallbackContext context)
    {
        if (context.started)
        {
            Rotate(false);
        }
    }

    public void OnRightClick(CallbackContext context)
    {
        if (context.started)
        {
            Rotate(true);
        }
    }

    private void Rotate(bool left)
    {
        _rotateTween.Kill();
        if (left)
        {
            _timesShieldsRotated += 1;
        }
        else
        {
            _timesShieldsRotated -= 1;
            if (_timesShieldsRotated < 0)
            {
                _timesShieldsRotated = 3;
            }
        }
        
        _timesShieldsRotated = _timesShieldsRotated % 4;
        _rotateTween = clickRotator.DOLocalRotate(_timesShieldsRotated * new Vector3(0, 0, 90), secondsToRotateShieldsOnClick);
        SetHighlightedShield();
    }

    private void SetHighlightedShield()
    {
        var highlighted_index = Mathf.Abs(_timesShieldsRotated);
        for (int i = 0; i < shields.Count; i++)
        {
            if (i == highlighted_index)
            {
                shields[i].transform.DOScale(Vector3.one, secondsToRotateShieldsOnClick);
                shields[i].transform.DOLocalMove(shields[i].startingLocalPosition * 1.5f, secondsToRotateShieldsOnClick);
            }
            else
            {
                shields[i].transform.DOScale(Vector3.one * .5f, secondsToRotateShieldsOnClick);
                shields[i].transform.DOLocalMove(shields[i].startingLocalPosition, secondsToRotateShieldsOnClick);
            }
        }

    }
}
