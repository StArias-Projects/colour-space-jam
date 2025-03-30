using UnityEngine;

public class SpritePulse : MonoBehaviour
{
    [SerializeField]
    PlayerManager playerManager;


    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one +( Vector3.one * Mathf.Sin(Time.time * (1.2f-playerManager.GetPercentHealth()) * 5) * .1f );
    }
}
