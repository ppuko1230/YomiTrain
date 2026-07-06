using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("UIパネルの参照")]
    [SerializeField] private GameObject inGameRoot;

    [Header("デバッグ用")]
    [Tooltip("押したら強制的に電車が走るテストボタン")]
    [SerializeField] private Button testRunTrainButton;

    [Header("選択肢ボタン")]
    [Tooltip("Option1〜4のボタンを順番にセットしてください")]
    [SerializeField] private Button[] optionButtons;

    private void Start()
    {
        if (AppManager.Instance != null)
            AppManager.Instance.OnStateChanged += HandleStateChanged;

        if (inGameRoot != null)
            inGameRoot.SetActive(false);

        // テストボタンのイベント登録
        if (testRunTrainButton != null)
        {
            testRunTrainButton.onClick.AddListener(OnClickTestRunTrain);
        }

        // 4つの選択肢ボタンにクリックイベントを登録する
        if (optionButtons != null)
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                int index = i;
                optionButtons[i].onClick.AddListener(() => OnClickOption(index));
            }
        }
    }

    private void OnDestroy()
    {
        if (AppManager.Instance != null)
            AppManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        bool isInGameState = (newState == GameState.InGame);
        if (inGameRoot != null) inGameRoot.SetActive(isInGameState);
        if (isInGameState)
        {
            SetupInGame();
        }
    }

    // ==========================================
    // 選択肢ボタンが押された時の処理
    // ==========================================
    private void OnClickOption(int choiceIndex)
    {
        Debug.Log($"選択肢 {choiceIndex + 1} が押されました！");

        if (PlayerManager.Instance != null)
        {
            // ※ここは後で引数(choiceIndex)を渡せるようにネットワーク側を改修します
            PlayerManager.Instance.RequestAnswerCreated();
        }

        // ▼▼▼ ここを追加 ▼▼▼
        // とりあえず見た目のテストとして、ViewManagerに直接「自分が選んだ番号」を伝えておく
        InGameViewManager viewManager = FindFirstObjectByType<InGameViewManager>();
        if (viewManager != null)
        {
            viewManager.SetMyChoice(choiceIndex);
        }
        // ▲▲▲ ここまで追加 ▲▲▲

        // 2回押せないように無効化
        foreach (Button btn in optionButtons)
        {
            if (btn != null) btn.interactable = false;
        }
    }
    private void SetupInGame()
    {
        Debug.Log("InGame画面を表示します。");
        Invoke(nameof(SendReady), 1.0f);

    }
    private void SendReady()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.RequestStartReady();
            Debug.Log("サーバーに準備完了を送信しました！");
        }
        else
        {
            Debug.LogWarning("PlayerManagerが見つかりません。");
        }
    }
    // ==========================================
    // 電車発車テスト用関数
    // ==========================================
    private void OnClickTestRunTrain()
    {
        Debug.Log("テスト出題ボタンが押されました！");
        NetworkBehavior networkBehavior = FindFirstObjectByType<NetworkBehavior>();
        if (networkBehavior != null)
        {
            // ★ランダムに出題するテスト関数を呼ぶ
            networkBehavior.TestStartQuestion();
        }
    }
}