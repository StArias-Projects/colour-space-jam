using UnityEngine;
using DG.Tweening;
using TMPro;

public class Pickup : MonoBehaviour
{
    

    [SerializeField]
    SpriteRenderer sprite;

    [SerializeField]
    SpriteRenderer spriteShadow;

    [SerializeField]
    float lifespan = 10;

    float timeAlive = 0;
    float spriteStartingScale;
    Rigidbody2D body;
    protected ProjectileManager projectileManager;
    protected bool triggered = false;

    Tween fadeInTween;

    public void SetUp(ProjectileManager projManagerRef)
    {
        projectileManager = projManagerRef;
    }


    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        fadeInTween = sprite.DOFade(0, 0).OnComplete(()=> sprite.DOFade(1, 1f));
        spriteStartingScale = sprite.transform.localScale.x;
        Invoke("OnLifespanOver", lifespan);
    }



    private void Update()
    {
        sprite.transform.Rotate(0,0,Time.deltaTime * 60);
        timeAlive += Time.deltaTime;
        if (!triggered)
        {
            sprite.transform.localScale = Vector3.one * (spriteStartingScale + Mathf.Sin(timeAlive * 3f)*.1f);
        }
        
    }

    protected virtual void Trigger(PlayerManager player)
    {
        triggered = true;
        body.simulated = false;

        fadeInTween.Kill();
        sprite.transform.DOScale(1, .15f);
        sprite.DOFade(0, .15f);
        spriteShadow.DOFade(0, .1f);
        Destroy(gameObject, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.TryGetComponent(out PlayerManager player))
        { 
            Trigger(player);
        }
    }


    void OnLifespanOver()
    {
        if (triggered) return;
        sprite.transform.DOScale(0, .15f);
        Destroy(gameObject, .15f);
    }

}
