using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Fusion;

public class ResultUIManager : MonoBehaviour
{
    
    // リザルト画面で使用するプレイヤー情報
    // 現在は動作確認用のクラスであり、今後プレイヤーデータの管理方法に合わせて変更予定
    [System.Serializable]
    private class ResultData
    {
        public string playerName;
        public int correctCount;

        public ResultData(string name, int score)
        {
            playerName = name;
            correctCount = score;
        }
    }


    [Header("UIパネルの参照")]
    [Tooltip("Result画面全体をまとめる親オブジェクト")]
    [SerializeField] private GameObject resultPanel;
    [Header("UIパーツの参照")]
    [Tooltip("プレイヤーの結果を表示するテキスト")]
    [SerializeField] private TextMeshProUGUI resultText;

    // プレイヤーのリザルト情報を保持するリスト
    private List<ResultData> results = new List<ResultData>();

    private void Start()
    {
        // 動作確認用のダミーデータ
        results.Add(new ResultData("プレイヤーA", 7));
        results.Add(new ResultData("プレイヤーB", 10));
        results.Add(new ResultData("プレイヤーC", 5));
        results.Add(new ResultData("プレイヤーD", 8));
        results.Add(new ResultData("プレイヤーE", 3));

        // 正解数の多い順に並べ替え
        SortRanking();

        // リザルト画面へ表示
        DisplayRanking();

        Debug.Log("Start");

        results.Add(new ResultData("プレイヤーA", 7));
        Debug.Log("A追加");

        results.Add(new ResultData("プレイヤーB", 10));
        Debug.Log("B追加");

        SortRanking();
        Debug.Log("Sort完了");

        DisplayRanking();
        Debug.Log("Display完了");

    }

    /// <summary>
    /// プレイヤーを正解数の降順で並べ替える
    /// </summary>
    private void SortRanking()
    {
        results.Sort((a, b) => b.correctCount.CompareTo(a.correctCount));
    }

    /// <summary>
    /// 並べ替えた結果を表示する
    /// （現在はConsole表示、後でUI表示に変更予定）
    /// </summary>
    private void DisplayRanking()
{
    // 表示を初期化
    resultText.text = "";

    // ランキングを表示
    for (int i = 0; i < results.Count; i++)
    {
        resultText.text +=
            $"{i + 1}位　{results[i].playerName}　{results[i].correctCount}点\n";
    }
    Debug.Log(resultText.text);
}
}