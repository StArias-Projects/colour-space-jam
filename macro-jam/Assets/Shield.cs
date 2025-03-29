using UnityEngine;

public class Shield : MonoBehaviour
{
    public EnemyType color;

    public Vector3 startingLocalPosition;

    private void Awake()
    {
        startingLocalPosition = transform.localPosition;
    }
}
