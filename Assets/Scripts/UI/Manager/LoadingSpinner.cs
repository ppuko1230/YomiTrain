using UnityEngine;
using UnityEngine.UI;

public class LoadingSpinner : MonoBehaviour
{
    public Image[] dots;

    public float speed = 0.15f;

    private float timer;


    void Update()
    {
        timer += Time.deltaTime;


        int activeDot = Mathf.FloorToInt(timer / speed) % dots.Length;


        for (int i = 0; i < dots.Length; i++)
        {
            Color c = dots[i].color;

            if (i == activeDot)
            {
                c.a = 1.0f;   // 濃い
            }
            else
            {
                c.a = 0.25f;  // 薄い
            }

            dots[i].color = c;
        }
    }
}