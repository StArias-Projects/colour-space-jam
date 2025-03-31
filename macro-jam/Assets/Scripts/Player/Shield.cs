using UnityEngine;
using DG.Tweening;

public class Shield : MonoBehaviour
{

    public EnemyType color;

    private Vector3 startingLocalPosition;

    private float shieldHitPulseTime = .4f;

    private bool IsMainShield = false;
    private int shieldIndex;
    public int ShieldIndex { get { return shieldIndex; } }


    private PlayerSFXController playerSFXController;
    public void SetUp(PlayerSFXController player, int index)
    {
        playerSFXController = player;
        startingLocalPosition = transform.localPosition;
        shieldIndex = index;
    }

    public void PlayShieldBulletReflected()
    {
        playerSFXController.PlayBulletReflectedSFX(shieldIndex);
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
        if (IsMainShield)
        {
            transform.DOShakeRotation(shieldHitPulseTime * .2f, 5f).OnComplete(() => ToggleAsMainShield(true, shieldHitPulseTime * .8f));
            transform.DOLocalMove(startingLocalPosition * 1.3f, shieldHitPulseTime * .2f);
        }

        else
        {
            transform.DOShakeRotation(shieldHitPulseTime * .2f, 5f).OnComplete(() => ToggleAsMainShield(false, shieldHitPulseTime * .8f));
            transform.DOLocalMove(startingLocalPosition * .8f, shieldHitPulseTime * .2f);
        }
    }
}