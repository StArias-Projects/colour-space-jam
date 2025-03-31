using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class HealthPickup : Pickup
{
    [SerializeField]
    float healthGranted = 3;


    [SerializeField]
    TextMeshPro tutorial;


    static bool hasPlayerPickedThisUpYet = false;


    protected override void Start()
    {
        base.Start();
        if (!hasPlayerPickedThisUpYet)
        {
            tutorial.DOFade(1, 3);
        }
        else
        {
            Destroy(tutorial);
        }
    }

    protected override void Trigger(PlayerManager player)
    {
        base.Trigger(player);

        player.GainHealth(healthGranted);
       
        hasPlayerPickedThisUpYet = true;
        if (tutorial)
        {
            tutorial.DOFade(0, .15f);
        }
    }
}
