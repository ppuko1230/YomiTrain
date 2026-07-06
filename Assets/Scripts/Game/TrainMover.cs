using UnityEngine;

// 電車を移動させるためのクラス
public class TrainMover : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField, Tooltip("移動するスピード")]
    private float speed = 5.0f;

    [SerializeField, Tooltip("移動する方向（横移動ならX軸など）")]
    private Vector3 moveDirection = Vector3.right;

    private bool _isMoving = false;

    private void Update()
    {
        // _isMovingがtrueの間だけ、毎フレーム移動し続ける
        if (_isMoving)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// アニメーション（移動）を開始する関数
    /// </summary>
    public void StartTrainAnimation()
    {
        _isMoving = true;
        Debug.Log("電車のアニメーションを開始しました！");
    }

    /// <summary>
    /// アニメーション（移動）を停止する関数
    /// </summary>
    public void StopTrainAnimation()
    {
        _isMoving = false;
    }
}