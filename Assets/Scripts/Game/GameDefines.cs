public enum InGamePhase
{
    Setup, // 初期化
    WaitQuestion, // 問題出題待ち
    ParentAnswering, // 親が答えを回答中
    ChildrenAnswering, // 子が答えを回答中
    Calculate, // みんなの集計結果
    ResultAnim, // 集計結果をアニメーション
    RoundEnd // ラウンド終了
}