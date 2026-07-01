using UnityEngine;
using UnityEngine.UI; // Buttonをスクリプトから制御するために必要

public class TitleUIManager : MonoBehaviour
{
    [Header("UIパネルの参照")]
    [SerializeField] private GameObject titleRoot;     // Title画面全体をまとめる親オブジェクト（キャンバス等）
    [SerializeField] private GameObject titlePanel;    // タイトルのメイン画面
    [SerializeField] private GameObject settingsPanel; // 設定画面
    // ※ roomSelectionPanel は RoomSelectUIManager に移動します

    [Header("ボタンの参照")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button returnButton;

    private void Start()
    {
        // 1. AppManagerのイベント「OnStateChanged」に自分のメソッドを登録する
        // （これでAppManagerがChangeStateを呼んだ時に、自動でHandleStateChangedが実行されます）
        AppManager.Instance.OnStateChanged += HandleStateChanged;

        // 2. 各ボタンが押された時の処理をスクリプトから登録する
        // （InspectorのOnClickで手動設定する手間が省け、設定忘れのバグを防げます）
        startButton.onClick.AddListener(OnStartButtonClicked);
        settingsButton.onClick.AddListener(ShowSettings);
        returnButton.onClick.AddListener(ReturnToTitle);
    }

    private void OnDestroy()
    {
        // 【重要】このオブジェクトが破棄される時は、s
        // （これを忘れると、存在しないオブジェクトに通知が送られてエラーになります）
        if (AppManager.Instance != null)
        {
            AppManager.Instance.OnStateChanged -= HandleStateChanged;
        }
    }

    // ==========================================
    // AppManagerから状態変化の通知を受け取った時の処理
    // ==========================================
    private void HandleStateChanged(GameState newState)
    {
        // 現在のStateが「Title」なら表示(true)、「それ以外」なら非表示(false)にする
        bool isTitleState = (newState == GameState.Title);
        titleRoot.SetActive(isTitleState);

        // もしTitle状態になったなら、内部のパネルを初期状態（設定画面などを閉じた状態）にリセットする
        if (isTitleState)
        {
            ReturnToTitle();
        }
    }

    // ==========================================
    // ボタンが押された時の処理
    // ==========================================
    private void OnStartButtonClicked()
    {
        // ❌ NG: roomSelectionPanel.SetActive(true);
        // ⭕ OK: AppManagerに「RoomSelectへ進めて！」とお願いするだけ
        AppManager.Instance.ChangeState(GameState.RoomSelect);
    }

    private void ShowSettings()
    {
        // 設定画面は「Title状態の中でのサブ画面」なので、AppManagerには頼まずここで完結させてOK
        titlePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void ReturnToTitle()
    {
        titlePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // 音量調整用バー
    public void OnVolumeChanged(float volume)
    {
        // 後で実装
        Debug.Log("音量：" + volume);
    }

}