// QuestionData.cs (Scripts/Data)
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Quiz/QuestionData")]
public class QuestionData : ScriptableObject
{
    [TextArea(2, 5)] public string questionText; // 問題文
    public string[] choices = new string[4];     // 4つの選択肢（レーン①〜④に対応）
    public int correctAnswerIndex;               // 正解のインデックス (0〜3)
}

// GenreData.cs (Scripts/Data)
[CreateAssetMenu(fileName = "NewGenre", menuName = "Quiz/GenreData")]
public class GenreData : ScriptableObject
{
    public string genreName;                     // ジャンル名
    public List<QuestionData> questionList;      // このジャンルに含まれる問題リスト
}