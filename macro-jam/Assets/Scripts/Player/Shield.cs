using UnityEngine;
using DG.Tweening;

public class Shield : MonoBehaviour
{
    public EnemyType color;

    private Vector3 startingLocalPosition;

    [SerializeField]
    private float shieldHitPulseAmount = 2f;

    [SerializeField]
    private float shieldHitPulseTime = 1f;

    private bool IsMainShield = false;

    private void Awake()
    {
        startingLocalPosition = transform.localPosition;
    }


    public void ToggleAsMainShield(bool setThisToMainShield, float secondsToRotateShieldsOnClick)
    {
        IsMainShield = setThisToMainShield;
        if (setThisToMainShield)
        {
            transform.DOScale(Vector3.one, secondsToRotateShieldsOnClick);
            transform.DOLocalMove(startingLocalPosition * 1.5f, secondsToRotateShieldsOnClick);
        }

        else
        {
            transform.DOScale(Vector3.one * .5f, secondsToRotateShieldsOnClick);
            transform.DOLocalMove(startingLocalPosition, secondsToRotateShieldsOnClick);
        }
    }

    public void OnHit()
    {
        //if (IsMainShield)
        //{
        //    transform.DOScale(Vector3.one, shieldHitPulseTime);
        //    transform.DOLocalMove(startingLocalPosition * 1.5f, shieldHitPulseTime);
        //}

        //else
        //{
        //    transform.DOScale(Vector3.one * .5f, shieldHitPulseTime);
        //    transform.DOLocalMove(startingLocalPosition, shieldHitPulseTime);
        //}
    }

}