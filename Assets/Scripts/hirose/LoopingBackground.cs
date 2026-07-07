using DG.Tweening;
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    [SerializeField] private Transform[] backgrounds;
    [SerializeField] private float speed = 2f;

    private float width;
    private float startX;

    void Start()
    {
        width = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
        startX = backgrounds[0].position.x;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].position = new Vector3(
                startX + width * i,
                backgrounds[i].position.y,
                backgrounds[i].position.z
            );
        }
    }

    void Update()
    {
        foreach (Transform bg in backgrounds)
        {
            bg.position += Vector3.left * speed * Time.deltaTime;

            if (bg.position.x <= startX - width)
            {
                bg.position += Vector3.right * width * backgrounds.Length;
            }
        }
    }
}
