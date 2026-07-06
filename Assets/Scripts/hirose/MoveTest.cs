using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    [Header("背景画像")]
    [SerializeField]
    private Transform[] bgImages;

    [Header("背景画像1枚分の横幅")]
    [SerializeField]
    private float width;

    [Header("スクロールの速さ")]
    [SerializeField]
    private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // bg1は現在位置からスタート
        MoveBg(bgImages[0]);

        // bg2はbg1の右隣に配置してからスタート
        MoveBg(bgImages[1], true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveBg(Transform bg, bool offsetStart = false)
    {
        if (offsetStart)
        {
            // bg2をbg1のすぐ右に並べる（隙間なく接続）
            bg.position = new Vector2(width, bg.position.y);
        }

        /*
        bg.DOMoveX(bg.position.x - width, width / speed)
            .SetEase(Ease.Linear)
            .SetRelative()
            .SetLoops(-1, LoopType.Incremental);
        */
    }
}
