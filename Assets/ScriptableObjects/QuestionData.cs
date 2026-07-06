using UnityEngine;

// 右クリックメニューから簡単に作成できるようにする属性
[CreateAssetMenu(fileName = "NewQuestion", menuName = "GameData/QuestionData")]
public class QuestionData : ScriptableObject
{
    [Header("お題の設定")]
    [TextArea(2, 5), Tooltip("お題の文章")]
    public string questionText;

    [Tooltip("4つの選択肢（上からレーン1, 2, 3, 4に対応）")]
    public string[] choices = new string[4];

}