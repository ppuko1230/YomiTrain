using DG.Tweening;
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    [Header("背景画像")]
    [SerializeField, Tooltip("ループさせる画像の配列")]
    private Transform[] backgrounds;

    [Header("速さ")]
    [SerializeField] private float speed = 2f;

    private float width;
    private float startX;

    void Start()
    {
        ArrangeBackgrounds();
    }

    void Update()
    {
        UpdateBackgroundScroll();
    }

    public void UpdateBackgroundScroll()
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

    [ContextMenu("背景を表示")]
    public void ShowBackgrounds()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].gameObject.SetActive(true);
        }
    }

    [ContextMenu("背景を非表示")]
    public void HideBackgrounds()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].gameObject.SetActive(false);
        }
    }

    public void SetBackgroundsVisible(bool visible)
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].gameObject.SetActive(visible);
        }
    }

    [ContextMenu("停止する(テスト)")]
    private void StopScrolling_Test()
    {
        ChangeSpeed(0f, 3f); // 現在の速さ→0まで、3秒かけて
    }

    //現在のスピードから、指定したスピードまで変化させる。
    public void ChangeSpeed(float toSpeed, float duration)
    {
        DOTween.To(
            () => speed,
            x => speed = x,
            toSpeed,
            duration
        ).SetEase(Ease.InQuad);
    }

    public void ArrangeBackgrounds()
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
}
