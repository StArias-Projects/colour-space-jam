using UnityEngine;

public class SpriteShadow : MonoBehaviour
{
    static Vector3 offset = new Vector3(.15f, -.15f,0);
   

    void Start()
    {
       
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = transform.parent.GetComponent<SpriteRenderer>().sprite;
        print(transform.parent.GetComponent<SpriteRenderer>().sprite);
        Color color = Color.black;
        color.a = .5f;
        transform.localScale = Vector3.one;
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = -1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position + offset;
    }
}
