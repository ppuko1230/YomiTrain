using UnityEngine;
using UnityEngine.UI;

public class RoomSelectionPanelManager : MonoBehaviour
{
    [Header("UIの参照")]
    [Tooltip("部屋を作る（通信を開始する）ボタン")]
    [SerializeField] private Button createRoomButton;

    [Tooltip("作成中に表示するロード画面のパネル")]
    [SerializeField] private GameObject loadingPanel;

    private void Start()
    {
        // 最初は念のためロード画面を非表示にしておく
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        // 部屋を作るボタンが押されたときの処理をスクリプトから登録
        if (createRoomButton != null)
        {
            createRoomButton.onClick.AddListener(OnClickCreateRoom);
        }
    }

    /// <summary>
    /// 部屋を作るボタンが押されたときに実行される処理
    /// </summary>
    public async void OnClickCreateRoom()
    {
        // 1. 準備段階：ボタンを押せなくし、ロード画面を表示する
        createRoomButton.interactable = false;
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        // 2. 通信処理：NetworkManagerに「部屋を作って！」と依頼し、結果を待つ
        string newSessionName = await NetworkManager.Instance.CreateRoomHost();

        // 3. 通信が終わったのでロード画面を隠す
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        // 4. 結果の処理
        if (!string.IsNullOrEmpty(newSessionName))
        {
            // 【成功した場合】
            // RoomIDの表示などはLobby画面に任せるので、ここは画面遷移を呼ぶだけ！
            AppManager.Instance.ChangeState(GameState.RoomLobby);
        }
        else
        {
            // 【失敗した場合】
            Debug.LogError("ルーム作成エラー");
            createRoomButton.interactable = true;
            // TODO: エラーダイアログなどを出す場合はここに記述
        }
    }
}