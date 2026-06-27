using UnityEngine;
using Fusion; // Photon Fusionを使うために必要
using System.Threading.Tasks;

// 通信に関する処理をひとまとめにした司令塔クラス
public class NetworkManager : MonoBehaviour
{
    // どこからでも NetworkManager.Instance でアクセスできるようにする（シングルトン）
    public static NetworkManager Instance { get; private set; }

    // 現在のルームIDを保持するプロパティ（外部からは読み取り専用）
    public string CurrentRoomId { get; private set; }

    // Fusionの通信本体となるコンポーネント
    private NetworkRunner _runner;

    private void Awake()
    {
        // シングルトンの初期化（シーンをまたいでも消えないようにする）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ホストとして部屋を作成するメソッド
    /// 成功したら生成したRoomIDを返し、失敗したらnullを返します。
    /// </summary>
    public async Task<string> CreateRoomHost()
    {
        // すでにRunnerが存在する場合は再利用、なければ追加
        _runner = gameObject.GetComponent<NetworkRunner>();
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
        }

        // ユーザー入力を受け付ける設定
        _runner.ProvideInput = true;

        // 6桁のランダムな英数字（RoomID）を生成
        string generatedRoomId = GenerateRandomRoomId(6);

        // Fusionの通信を開始（部屋を作る）
        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,           // ホスト（親）として起動
            SessionName = generatedRoomId,      // 生成した文字列を部屋の名前にする
            PlayerCount = 6                     // 一部屋の最大人数
        });

        // 接続結果の確認
        if (result.Ok)
        {
            CurrentRoomId = generatedRoomId; // ★追加：成功したら記憶しておく
            Debug.Log($"ルーム作成成功！ RoomID: {generatedRoomId}");
            return generatedRoomId;
        }
        else
        {
            Debug.LogError($"ルーム作成失敗: {result.ShutdownReason}");
            return null;
        }
    }

    /// <summary>
    /// クライアントとして既存の部屋に参加するメソッド
    /// 成功したらtrue、失敗したらfalseを返します。
    /// </summary>
    public async Task<bool> JoinRoomClient(string inputRoomId)
    {
        _runner = gameObject.GetComponent<NetworkRunner>();
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
        }

        _runner.ProvideInput = true;

        // Fusionの通信を開始（部屋に入る）
        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,         // クライアント（子）として参加
            SessionName = inputRoomId           // 検索画面で入力されたRoomID
        });

        if (result.Ok)
        {
            CurrentRoomId = inputRoomId; // ★追加：成功したら記憶しておく
            Debug.Log($"ルーム {inputRoomId} に参加成功！");
            return true;
        }
        else
        {
            Debug.LogError($"ルーム参加失敗: {result.ShutdownReason}");
            return false;
        }
    }

    // ランダムな英数字を生成するお助け関数
    private string GenerateRandomRoomId(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string result = "";
        for (int i = 0; i < length; i++)
        {
            result += chars[Random.Range(0, chars.Length)];
        }
        return result;
    }
}
