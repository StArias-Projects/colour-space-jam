using UnityEngine;
using TMPro;
using DG.Tweening;
using static UnityEngine.InputSystem.InputAction;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    TextMeshPro moveTutorial;

    [SerializeField]
    float timeUntilShowMoveTutorial = 1;


    [SerializeField]
    TextMeshPro shieldTutorial;

    [SerializeField]
    float timeUntilShowShieldTutorial = 5;

    [SerializeField]
    TextMeshPro reflectTutorial;

    [SerializeField]
    float timeUntilShowReflectTutorial = 5;

    bool hasMoved = false;
    bool hasRotated = false;
    bool hasReflected = false;

    private void Start()
    {
        StartCoroutine(ShowTextCoroutine());
        ProjectileController.OnEnemyKilled += OnEnemyDied;
    }

    private void OnDestroy()
    {
        ProjectileController.OnEnemyKilled -= OnEnemyDied;
    }
    IEnumerator ShowTextCoroutine()
    {
        yield return new WaitForSeconds(timeUntilShowMoveTutorial);
        hasRotated = false;
        if (!hasMoved)
        {
            moveTutorial.DOFade(1, 3);
        }
        yield return new WaitUntil(() => hasMoved);
        yield return new WaitForSeconds(timeUntilShowShieldTutorial);
        if (!hasRotated)
        {
            shieldTutorial.DOFade(1, 3);
        }
        yield return new WaitUntil(() => hasRotated);
        yield return new WaitForSeconds(timeUntilShowReflectTutorial);
        if (!hasReflected)
        {
            reflectTutorial.DOFade(1, 3);
        }
    }




    public void OnMoved(CallbackContext context)
    {
        if (hasMoved) return;

        hasMoved = true;
        moveTutorial.DOFade(0, 1).OnComplete(()=>Destroy(moveTutorial));
    }

    public void OnShieldRotated(CallbackContext context)
    {
        if (hasRotated) return;

        hasRotated = true;
        shieldTutorial.DOFade(0, 1).OnComplete(() => Destroy(shieldTutorial));
    }

    public void OnEnemyDied(EnemyType enemy)
    {
        if (hasReflected) return;

        hasReflected = true;
        reflectTutorial.DOFade(0, 1).OnComplete(() => Destroy(reflectTutorial));
    }
}
