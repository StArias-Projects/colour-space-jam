using UnityEngine;
using DG.Tweening;
using TMPro;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    float healthGranted = 3;

    [SerializeField]
    SpriteRenderer sprite;

    [SerializeField]
    SpriteRenderer spriteShadow;

    [SerializeField]
    TextMeshPro tutorial;

    [SerializeField]
    float lifespan = 10;

    static bool hasPlayerPickedThisUpYet = false;
    Rigidbody2D body;

    bool triggered = false;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite.DOFade(0, 0).OnComplete(()=> sprite.DOFade(1, 3f));
        if (!hasPlayerPickedThisUpYet)
        {
            tutorial.DOFade(1, 3);
        }
        else
        {
            Destroy(tutorial);
        }
        Invoke("OnLifespanOver", lifespan);
    }



    private void Update()
    {
        sprite.transform.Rotate(0,0,Time.deltaTime * 60);

        if (!triggered)
        {
            sprite.transform.localScale += Vector3.one * Mathf.Sin(Time.time *3f)*.0003f;
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.TryGetComponent(out PlayerManager player))
        {
            triggered = true;
            player.GainHealth(healthGranted);
            body.simulated = false;

            sprite.transform.DOScale(1, .15f);
            sprite.DOFade(0, .15f);
            spriteShadow.DOFade(0, .1f);
            Destroy(gameObject,1);
            hasPlayerPickedThisUpYet = true;
            if (tutorial)
            {
                tutorial.DOFade(0, .15f);
            }
        }
    }

    void OnLifespanOver()
    {
        if (triggered) return;
        sprite.transform.DOScale(0, .15f);
        Destroy(gameObject, .15f);
    }

}
