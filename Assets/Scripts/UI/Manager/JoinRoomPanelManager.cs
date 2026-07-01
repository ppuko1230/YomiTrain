using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinRoomPanelManager : MonoBehaviour
{
    [Header("UIの参照")]
    [Tooltip("RoomIDを入力するテキストフィールド")]
    [SerializeField] private TMP_InputField roomIdInputField;

    [Tooltip("部屋に参加する（通信を開始する）ボタン")]
    [SerializeField] private Button joinButton;

    [Tooltip("エラーメッセージを表示するテキスト（あれば）")]
    [SerializeField] private TextMeshProUGUI errorText;

    [Tooltip("通信中に表示するロード画面のパネル")]
    [SerializeField] private GameObject loadingPanel;

    private void Start()
    {
        // 最初はロード画面とエラーテキストを隠しておく
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (errorText != null) errorText.text = "";

        // ボタンが押されたときの処理を登録
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(OnClickJoinRoom);
        }
    }

    /// <summary>
    /// 部屋に参加するボタンが押されたときに実行される処理
    /// </summary>
    public async void OnClickJoinRoom()
    {
        // 1. 入力されたテキストを取得（大文字小文字のズレを防ぐため、強制的に大文字にする）
        string inputRoomId = roomIdInputField.text.ToUpper().Trim();

        // 空白チェック
        if (string.IsNullOrEmpty(inputRoomId))
        {
            if (errorText != null) errorText.text = "Room IDを入力してください";
            return;
        }

        // 2. 準備段階：ボタンを押せなくし、エラーを消し、ロード画面を表示する
        joinButton.interactable = false;
        if (errorText != null) errorText.text = "";
        if (loadingPanel != null) loadingPanel.SetActive(true);

        // 3. 通信処理：NetworkManagerに「このIDの部屋に入れて！」と依頼し、結果を待つ
        bool isSuccess = await NetworkManager.Instance.JoinRoomClient(inputRoomId);

        // 4. 通信が終わったのでロード画面を隠す
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        // 5. 結果の処理
        if (isSuccess)
        {
            // 【成功した場合】Lobby画面へ遷移する！
            AppManager.Instance.ChangeState(GameState.RoomLobby);
        }
        else
        {
            // 【失敗した場合】エラーメッセージを表示して、もう一度ボタンを押せるようにする
            if (errorText != null) errorText.text = "部屋が見つからないか、満員です";
            joinButton.interactable = true;
        }
    }
}